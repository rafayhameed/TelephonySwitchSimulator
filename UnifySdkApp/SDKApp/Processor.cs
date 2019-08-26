using HiPathProCenterLibrary;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using trg.satmap.logging;

namespace SDKApp
{
    public delegate void CallDetailGridHandler(string[] row, string callID);
    public delegate void CallDetailGridUpdater(bool remove, string callID, params string[] list);

    class Processor
    {
        #region private members

        private static ISMLogger _logger;
        private static Hashtable AgentsListFromServer;
        private static ListView listViewAgentsList = null;
        private static Hashtable agentsTimer;
        private Hashtable agentsRingingTime;
        private ManualResetEvent agentsQueueWait = new ManualResetEvent(false);
        private Dictionary<string, string> activeCallsDict;
        private BlockingCollection<Object> eventQueue;
        private Thread agentRingQueueProcessor;
        private Thread eventProcessorThread;
        #endregion

        #region public members
        public event CallDetailGridHandler callDetailGridEvent;
        public event CallDetailGridUpdater callDetailGridUpdateEvent;
        public Dictionary<string, Agent> agentsQueue;
        public static Hashtable callsTable;
        #endregion

        public Processor(Hashtable AgentsListFromServerObj, ListView listViewAgentsListObj, Hashtable AgentsTimerObj)
        {
            _logger = SMLogManager.GetLogger("trg.sm.si.unify.UnifySimulator.processor");
            AgentsListFromServer = AgentsListFromServerObj;
            listViewAgentsList = listViewAgentsListObj;
            agentsTimer = AgentsTimerObj;
            InitializeComponents();
        }

        public void EnqueueEventQueue(Object mediaEvent)
        {
            eventQueue.Add(mediaEvent);
        }

        private void ProcessEvent()
        {
            while (true)
            {
                Object baseEvent = eventQueue.Take();

                switch (baseEvent)
                {
                    case MediaEvent mediaEvent:
                        _logger.Info("Unhandled Event from Switch");
                        ProcessMediaEvent(mediaEvent);
                        break;
                    case RoutingEvent routingEvent:
                        ProcessRoutingEvent(routingEvent);
                        break;
                    default:
                        _logger.Info("Unhandled Event from Switch ");
                        break;
                }
            }
        }

        private void ProcessMediaEvent(MediaEvent mediaEvent)
        {
            switch (mediaEvent.ObjectType)
            {

                case enMediaEventObjectTypes.MediaEventObjectType_AgentStatus:
                    IAgentStatusEvent statusEvent = (IAgentStatusEvent)mediaEvent;
                    ProcessAgentStatusEvent(statusEvent);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_Delivered:
                    var objDeliveredEvent = mediaEvent as DeliveredEvent;
                    _logger.Info("MediaEventObjectType_Delivered callID: " + objDeliveredEvent.CallID);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_Established:
                    var objEstablishedEvent = mediaEvent as EstablishedEvent;
                    _logger.Info("MediaEventObjectType_Established " + objEstablishedEvent.CallID);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_Queued:
                    var objQueuedEvent = mediaEvent as QueuedEvent;
                    _logger.Info("MediaEventObjectType_Queued " + objQueuedEvent.CallID);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_Diverted:
                    var objDivertedEvent = mediaEvent as DivertedEvent;
                    _logger.Info("MediaEventObjectType_Diverted " + objDivertedEvent.CallID);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_Disconnected:
                    var objDisconnectEvent = mediaEvent as DisconnectedEvent;
                    _logger.Info("MediaEventObjectType_Disconnected CallID: " + objDisconnectEvent.CallID + " Reason " + objDisconnectEvent.Reason);
                    callDetailGridUpdateEvent?.Invoke(true, objDisconnectEvent.CallID);
                    lock (activeCallsDict)
                        activeCallsDict.Remove(objDisconnectEvent.CallID);
                    break;
                case enMediaEventObjectTypes.MediaEventObjectType_ManagerStateChanged:
                    IManagerStateChangedEvent stateEvent = (IManagerStateChangedEvent)mediaEvent;
                    _logger.Info("MediaEvent ManagerStateChanged ## " + stateEvent.State);
                    break;
            }
        }
        private void ProcessAgentStatusEvent(IAgentStatusEvent statusEvent)
        {
            int index;
            Agent agent;
            try
            {
                if (AgentsListFromServer.Contains(statusEvent.AgentKey.ToString()))
                {
                    agent = (Agent)AgentsListFromServer[statusEvent.AgentKey.ToString()];
                    index = listViewAgentsList.Items.IndexOfKey(agent.Name);

                    switch (statusEvent.State)
                    {

                        case enAgentStates.AgentState_Unavailable:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is now UnAvailable on " + " Extension = " + statusEvent.Extension + "  " + statusEvent.AgentID);
                            UpdateAgentList(agent, "UnAvailable", Color.DarkGray, index);
                            break;
                        case enAgentStates.AgentState_Working:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is now on work on " + " Extension = " + statusEvent.Extension + "  " + statusEvent.AgentID);
                            UpdateAgentList(agent, "work", Color.Yellow, index);
                            break;
                        case enAgentStates.AgentState_LoggedOff:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is now on LoggedOFF " + " Extension = " + statusEvent.Extension + "  " + statusEvent.AgentID);
                            UpdateAgentList(agent, "LoggedOUT", Color.AliceBlue, index);
                            break;
                        case enAgentStates.AgentState_Ringing:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is now Ringing " + " Extension = " + statusEvent.Extension + "  " + statusEvent.AgentID);
                            ProcessRingingEvent(statusEvent);
                            break;
                        case enAgentStates.AgentState_Connected:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is on callID: " + statusEvent.CallID);
                            UpdateAgentList(agent, "On Call", Color.Orchid, index);
                            break;
                        case enAgentStates.AgentState_Available:
                            _logger.Info("AgentStatusEvent " + statusEvent.AgentID + " is now available on " + " Extension = " + statusEvent.Extension + "  " + statusEvent.AgentID);
                            ProcessAvailableAgentEvent(agent.AgentID);
                            UpdateAgentList(agent, "Available", Color.Aqua, index);
                            listViewAgentsList.Items[index].SubItems[5].Text = "";
                            break;
                        case enAgentStates.AgentState_Pending:
                            _logger.Info("AgentStatusEvent  Pending State " + statusEvent.ACDState);
                            break;
                        default:
                            _logger.Info("AgentStatusEvent  Unhandeled Event " + statusEvent.State);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ProcessAgentStatusEvent " + ex);
            }
        }



        private void UpdateAgentList(Agent agent, string status, Color colr, int index)
        {
            try
            {
                agent.Status = status;
                listViewAgentsList.Items[index].BackColor = colr;
                listViewAgentsList.Items[index].SubItems[2].Text = agent.Status;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in UpdateAgentList " + ex);
            }

        }

        private void ProcessAvailableAgentEvent(string agentID)
        {
            if (agentsQueue.ContainsKey(agentID))
            {
                lock (agentsQueue)
                {
                    agentsQueue.Remove(agentID);
                    lock (agentsRingingTime)
                        agentsRingingTime.Remove(agentID);
                }
            }
        }

        private void ProcessRingingEvent(IAgentStatusEvent objRoutingEventAssignedEvent)
        {
            int index;
            Agent agent;
            string callID;
            try
            {
                if (objRoutingEventAssignedEvent != null)
                {
                    callID = objRoutingEventAssignedEvent.CallID;
                    _logger.Info("RoutingEvent Assigned ## CallID: " + callID + " AgentKey: " + objRoutingEventAssignedEvent.AgentKey + " AgentStation: " + objRoutingEventAssignedEvent.Extension);
                    callDetailGridUpdateEvent?.Invoke(false, callID, "assigned", "ringing");

                    if (AgentsListFromServer.Contains(objRoutingEventAssignedEvent.AgentKey.ToString()))
                    {
                        agent = (Agent)AgentsListFromServer[objRoutingEventAssignedEvent.AgentKey.ToString()];
                        agent.Status = "Ringing";
                        agent.CallID = callID;
                        index = listViewAgentsList.Items.IndexOfKey(agent.Name);
                        listViewAgentsList.Items[index].BackColor = Color.OrangeRed;
                        listViewAgentsList.Items[index].SubItems[2].Text = agent.Status;
                        listViewAgentsList.Items[index].SubItems[5].Text = callID;

                        if (!agentsQueue.ContainsKey(agent.AgentID))
                        {
                            _logger.Info("Adding Time for Agent: " + agent.AgentID);
                            lock (agentsQueue)
                                agentsQueue.Add(agent.AgentID, agent);
                            if (agentsRingingTime.Contains(agent.AgentID))
                            {
                                lock (agentsRingingTime)
                                    agentsRingingTime[agent.AgentID] = DateTime.Now;
                            }
                            else
                            {
                                lock (agentsRingingTime)
                                    agentsRingingTime.Add(agent.AgentID, DateTime.Now);
                            }
                            agentsQueueWait.Set();
                        }
                        else
                        {
                            _logger.Info("Agent already present in Ringing Queue " + agent.AgentID);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ProcessRingingEvent " + ex);
            }

        }



        private void ProcessRoutingEvent(RoutingEvent objRoutingEvent)
        {

            try
            {
                switch (objRoutingEvent.Code)
                {

                    case enRoutingEventCodes.RoutingEventCode_Enqueued:

                        var objRoutingEventEnqueuedEvent = objRoutingEvent as EnqueuedEvent;
                        if (objRoutingEventEnqueuedEvent != null)
                        {
                            _logger.Info("RoutingEvent Enqueued ## CallID: " + objRoutingEventEnqueuedEvent.CallID + " Description: " + objRoutingEventEnqueuedEvent.Description + " Priority: " + objRoutingEventEnqueuedEvent.Priority);
                            string[] row = { objRoutingEventEnqueuedEvent.CallID, "Not Assigned", "Wait" };
                            callDetailGridEvent?.Invoke(row, objRoutingEventEnqueuedEvent.CallID);
                            lock (activeCallsDict)
                                activeCallsDict.Add(objRoutingEventEnqueuedEvent.CallID, "Enqued");
                        }
                        break;
                    case enRoutingEventCodes.RoutingEventCode_Dequeued:
                        var objRoutingEventDequeuedEvent = objRoutingEvent as DequeuedEvent;
                        _logger.Info("RoutingEvent Dequeued  ## ContactId: " + objRoutingEventDequeuedEvent.CallID + " Description: " + objRoutingEventDequeuedEvent.Description + " Priority: " + objRoutingEventDequeuedEvent.Priority + " DequeueReason: " + objRoutingEventDequeuedEvent.DequeueReason);
                        break;
                    case enRoutingEventCodes.RoutingEventCode_Assigned:
                        var objRoutingEventAssignedEvent = objRoutingEvent as AssignedEvent;
                        //   processRingingEvent(objRoutingEventAssignedEvent);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in ProcessRoutingEvent " + ex);
            }

        }


        public void ProcessStatsEvent(StatisticsEvent statsEvent)
        {
            //_logger.Info("Stats Event : " + statsEvent.EventType);
        }

        private void ProcessAgentsQueue()
        {
            IEnumerable<Agent> agentsList;
            try
            {
                while (true)
                {
                    agentsQueueWait.WaitOne();
                    lock (agentsQueue)
                    {
                        agentsList = from agent in agentsQueue.Values where /*agent.Status.Equals("Ringing") && */agentsRingingTime.Contains(agent.AgentID) && agent.WaitTime.Equals((DateTime.Now - (DateTime)agentsRingingTime[agent.AgentID]).Seconds.ToString()) select agent;

                        if (agentsList != null && agentsList.Count() > 0)
                        {
                            foreach (Agent agent in agentsList.ToList())
                            {
                                DropCall(agent);
                            }
                        }

                    }
                    if (agentsQueue.Count == 0)
                    {
                        agentsQueueWait.Reset();
                    }
                    else
                    {
                        agentsQueueWait.Set();
                    }
                    agentsList = null;
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EXCeption " + ex);
            }


        }

        private void DropCall(Agent agent)
        {
            string callID;
            VoiceCall oVoiceCall;
            try
            {
                if (agentsQueue.ContainsKey(agent.AgentID) && activeCallsDict.ContainsKey(agent.CallID))
                {
                    callID = agent.CallID;
                    oVoiceCall = (VoiceCall)MainPage.mediaObj.NewVoiceCall();
                    _logger.Info(" Ending MSG " + callID + " Ext: " + agent.Extenssion);
                    try
                    {
                        oVoiceCall.Disconnect(agent.Extenssion);
                    }
                    catch (System.Runtime.InteropServices.COMException)
                    {
                        _logger.Info("Call removed from server");
                    }

                    lock (agentsRingingTime)
                    {
                        agentsRingingTime.Remove(agent.AgentID);
                        lock (agentsQueue)
                            agentsQueue.Remove(agent.AgentID);
                    }


                }
                else
                {
                    _logger.Info("Call has already been Terminated");
                }

                agent.CallID = "";

            }
            catch (Exception ex)
            {
                _logger.Error("Exception " + ex);
            }

        }

        private void InitializeComponents()
        {
            agentsQueue = new Dictionary<string, Agent>();
            agentsRingingTime = new Hashtable();
            eventQueue = new BlockingCollection<Object>();
            callsTable = new Hashtable();
            activeCallsDict = new Dictionary<string, string>();
            agentRingQueueProcessor = new Thread(ProcessAgentsQueue)
            {
                IsBackground = true
            };
            agentRingQueueProcessor.Start();

            eventProcessorThread = new Thread(ProcessEvent)
            {
                IsBackground = true
            };
            eventProcessorThread.Start();
        }


    }
}
