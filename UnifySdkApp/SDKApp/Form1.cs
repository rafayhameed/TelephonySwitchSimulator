using HiPathProCenterLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using trg.satmap.logging;

namespace SDKApp
{
    public partial class MainPage : Form
    {
        private static ISMLogger _logger;
        private HiPathProCenterManager managerObj;
        private AdministrationManager adminMngrObj;
        private Users usersObj;
        private RoutingManager routingObj;
        private StatisticsManager statsManager;
        private Hashtable AgentsListFromServer = new Hashtable();
        private Hashtable AgentsTimer = new Hashtable();
        private List<Agent> AgentsListFromConfig;
        private Hashtable loginObjectTable;
        private Processor processor;

        public static MediaManager mediaObj;

        public MainPage()
        {
            InitializeComponent();
            Thread startupThread = new Thread(Start);
            startupThread.IsBackground = true;
            startupThread.Start();
        }

        private void Start()
        {

            SMLogManager.Initialize();
            _logger = SMLogManager.GetLogger("trg.sm.si.unify.UnifySimulator.main");
            _logger.Info("StartUp Thread started");
            dataGridCallDetail.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dataGridCallDetail_RowPostPaint);
            loginObjectTable = new Hashtable();
            AgentsTimer = new Hashtable();
            ConnectToServer();
            _logger.Info("Server Connected successfully Connected");
            AdminLogon();
            _logger.Info("Admin logged in successfully");
            _logger.Info("Exitting StartUp Thread");
            loadBox.Hide();
        }

        private void dataGridCallDetail_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(dataGridCallDetail.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(), e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 10, e.RowBounds.Location.Y + 4);
            }
        }

        private void ConnectToServer()
        {
            string serverAddrs;
            try
            {
                managerObj = new HiPathProCenterManager()
                {
                    DiagnosticFileName = "SwitchLogs"
                };
                managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_Application);
                //   managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_HiPathProCenterFunctionEntryExit);
                //  managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_HiPathProCenterInformation);
                //  managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_HiPathProCenterSevere);
                managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_HiPathProCenterWarning);
                //   managerObj.EnableDiagnosticFilter(enDiagnosticFilters.DiagnosticFilter_Toolkit);

                serverAddrs = "6000@AFIOSCCV8.satmapinc.com";
                managerObj.Initialize(serverAddrs, enEventModes.EventMode_FireEvents);
                //    labelConnStatus.Text = "Connected";
                WriteMessage("Connected to Server " + serverAddrs);
                adminMngrObj = (AdministrationManager)managerObj.HireAdministrationManager(enEventModes.EventMode_FireEvents);
                WriteMessage("Admin Manager Object hired");
                usersObj = adminMngrObj.QueryUsers();
                WriteMessage("Received Users. Total users count = " + usersObj.Count);
            }
            catch (Exception ex)
            {
                WriteMessage("Exception " + ex);
            }
        }



        private void AdminLogon()
        {
            int key;
            string password;

            try
            {
                key = 1;
                password = "password";
                managerObj.Logon(key, password);

                CheckForIllegalCrossThreadCalls = false;
                processor = new Processor(AgentsListFromServer, listViewAgentsList, AgentsTimer);
                processor.callDetailGridEvent += new CallDetailGridHandler(AddInCallDetailGrid);
                processor.callDetailGridUpdateEvent += new CallDetailGridUpdater(UpdateCallDetailGrid);

                mediaObj = (MediaManager)managerObj.HireMediaManager(enEventModes.EventMode_FireEvents);
                routingObj = managerObj.HireRoutingManager(enEventModes.EventMode_FireEvents);


                mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_AgentStatusEvents);
                mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_VoiceEvents);
                mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_VoiceDetailsUpdates);
                mediaObj.EventOccurred += processor.EnqueueEventQueue;
                //mediaObj.EventOccurred += processor.ProcessMediaEvent;

                routingObj.ListenForEvents(enRoutingEventTypes.RoutingEventType_VoiceEvents);
                routingObj.EventOccurred += processor.EnqueueEventQueue;
                // routingObj.EventOccurred += processor.ProcessRoutingEvent;

                //adminMngrObj.ListenForEvents(enAdministrationEventTypes.AdministrationEventType_AdministrationDatabaseUpdates);
                //adminMngrObj.ListenForEvents(enAdministrationEventTypes.AdministrationEventType_ConfigurationSynchronizationEvents);
                //adminMngrObj.EventOccurred += _objAdmin_EventOccurred;


                statsManager = (StatisticsManager)managerObj.HireStatisticsManager(enEventModes.EventMode_FireEvents);
                statsManager.EventOccurred += new _IStatisticsManagerEvents_EventOccurredEventHandler(processor.ProcessStatsEvent);


                WriteMessage("Admin Looged on Successfuly");
                FillUsersList();
            }
            catch (Exception ex)
            {
                _logger.Error("Error in AdminLogon " + ex);
            }
        }

        private void FillUsersList()
        {
            // AgentsListFromServer = new Hashtable();
            ListViewItem item;
            Hashtable agentExtenssion;
            try
            {
                agentExtenssion = new Hashtable();
                GetAgentsListFromConfig();
                foreach (Agent agent in AgentsListFromConfig)
                {
                    agentExtenssion.Add(agent.AgentKey, agent);
                }



                foreach (User user in usersObj)
                {
                    // AgentNameKey agent = new AgentNameKey(user.FirstName + "," + user.LastName + "," + user.ID, user.Key);
                    Agent agenta = new Agent();
                    agenta.Name = user.FirstName + " " + user.LastName;
                    agenta.AgentID = user.ID;
                    agenta.AgentKey = user.Key.ToString();

                    if (agentExtenssion.Contains(agenta.AgentKey))
                    {
                        Agent agentConfig = (Agent)(agentExtenssion[agenta.AgentKey]);
                        agenta.WaitTime = agentConfig.WaitTime;
                        agenta.Extenssion = agentConfig.Extenssion;
                        agenta.CallTime = agentConfig.CallTime;
                    }

                    item = new ListViewItem(new[] { agenta.Name, agenta.Extenssion, agenta.Status, agenta.WaitTime, agenta.CallTime, "" })
                    {
                        Tag = agenta
                    };
                    item.Name = agenta.Name;
                    //    comboBox1.Items.Add(agent);
                    listViewAgentsList.Invoke(new MethodInvoker(delegate
                    {
                        listViewAgentsList.Items.Add(item);
                    }));
                    // listViewAgentsList.Items.Add(item);
                    AgentsListFromServer.Add(agenta.AgentKey, agenta);
                    WriteMessage("  [ Agent ] : " + agenta.Name + "," + agenta.AgentID + " Extenssion=  " + agenta.Extenssion + " WaitTime= " + agenta.WaitTime);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception in FillUsersList " + ex);
            }

        }

        private void AddInAgentsList(ListViewItem item)
        {
            listViewAgentsList.Items.Add(item);
        }

        private void buttonLoginAgent_Click(object sender, EventArgs e)
        {
            IAgent loginAgent;
            int counter = 0;
            try
            {
                string agentName, extenssion;
                keyList key = new keyList();
                foreach (ListViewItem a in listViewAgentsList.SelectedItems)
                {
                    Agent agent = (Agent)a.Tag;
                    int selectedIndex = listViewAgentsList.SelectedIndices[counter];
                    extenssion = (listViewAgentsList.SelectedItems[counter++].SubItems[1].Text);

                    agentName = agent.Name;

                    //mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_AgentStatusEvents, agent.AgentKey);
                    //mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_VoiceEvents, agent.AgentKey);
                    //mediaObj.ListenForEvents(enMediaEventTypes.MediaEventType_VoiceDetailsUpdates, agent.AgentKey);


                    key.Add(Int32.Parse(agent.AgentKey));



                    loginAgent = mediaObj.NewAgent();
                    loginAgent.Key = Int32.Parse(agent.AgentKey);

                    if (String.IsNullOrEmpty(extenssion))
                    {
                        loginAgent.Logon(enMediaTypes.MediaType_WebCollaboration);
                    }
                    else
                    {
                        loginAgent.Logon(enMediaTypes.MediaType_Voice, agent.Extenssion);
                        //  routingObj.ListenForEvents(enRoutingEventTypes.RoutingEventType_VoiceEvents, agent.AgentKey);
                    }


                    ////   UserRealtimeElement;
                    //StatisticsManager StatsManager = new StatisticsManager();


                    //agent.LoginOject = loginAgent;
                    agent.Status = "LoggedIN";
                    loginObjectTable.Add(loginAgent.Key.ToString(), loginAgent);
                    agent.LoggedIn = true;

                    listViewAgentsList.Items[selectedIndex].BackColor = Color.LightGreen;
                    listViewAgentsList.Items[selectedIndex].SubItems[2].Text = agent.Status;
                    // listViewAgentsList.Items[selectedIndex].SubItems[3].Text = DateTime.Now.TimeOfDay.ToString();


                }


            }
            catch (Exception ex)
            {
                WriteMessage("Exception " + ex.HResult + "  " + ex);
            }

        }





        private void GetAgentsListFromConfig()
        {
            string path = "D:\\VS Projects\\UnifySdkApp\\SDKApp\\Config.xml";
            try
            {
                AgentsListFromConfig = new List<Agent>();
                XmlSerializer serializer = new XmlSerializer(typeof(List<Agent>), new XmlRootAttribute("AgentList"));
                XmlReader reader = XmlReader.Create(path);
                AgentsListFromConfig = (List<Agent>)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                WriteMessage("Exception while getting Agents List from Config File" + ex.HResult + "  " + ex);
            }
        }



        private void buttonLogOut_Click(object sender, EventArgs e)
        {
            IAgent logoutAgent = null;
            int counter = 0;
            int selectedIndex = 0;
            try
            {
                foreach (ListViewItem item in listViewAgentsList.SelectedItems)
                {
                    selectedIndex = listViewAgentsList.SelectedIndices[counter++];
                    Agent agent = (Agent)item.Tag;
                    if (loginObjectTable.Contains(agent.AgentKey))
                    {
                        logoutAgent = (IAgent)loginObjectTable[agent.AgentKey];
                        logoutAgent.Logoff(enMediaTypes.MediaType_Voice);
                        agent.Status = "LoggedOUT";
                        listViewAgentsList.Items[selectedIndex].BackColor = Color.AliceBlue;
                        listViewAgentsList.Items[selectedIndex].SubItems[2].Text = agent.Status;
                        loginObjectTable.Remove(agent.AgentKey);
                        _logger.Info("Logging off agent " + agent.Name);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in logout" + ex);
            }

        }

        private void WriteMessage(string message)
        {
            // managerObj.WriteToDiagnosticFile(message);
            _logger.Info(message);
        }

        private class AgentNameKey
        {
            public String Name;
            public int Key;

            public AgentNameKey(String UserName, int UserKey)
            {
                Name = UserName;
                Key = UserKey;
            }
            public override String ToString()
            {
                return Name;
            }
        }

        private void buttonAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                int counter = 0;
                int selectedIndex = 0;
                IAgent logoutAgent;
                foreach (ListViewItem item in listViewAgentsList.SelectedItems)
                {
                    selectedIndex = listViewAgentsList.SelectedIndices[counter++];
                    Agent agent = (Agent)item.Tag;
                    if (loginObjectTable.Contains(agent.AgentKey))
                    {
                        logoutAgent = (IAgent)loginObjectTable[agent.AgentKey];
                        logoutAgent.Available();
                        agent.Status = "Available";
                        listViewAgentsList.Items[selectedIndex].BackColor = Color.Aqua;
                        listViewAgentsList.Items[selectedIndex].SubItems[2].Text = agent.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteMessage("Exception " + ex);
            }
        }


        private void buttonUnAvailable_Click(object sender, EventArgs e)
        {
            try
            {
                int counter = 0;
                int selectedIndex = 0;
                IAgent unAvailableAgent;
                foreach (ListViewItem item in listViewAgentsList.SelectedItems)
                {
                    selectedIndex = listViewAgentsList.SelectedIndices[counter++];
                    Agent agent = (Agent)item.Tag;
                    if (loginObjectTable.Contains(agent.AgentKey))
                    {
                        unAvailableAgent = (IAgent)loginObjectTable[agent.AgentKey];
                        unAvailableAgent.Unavailable();
                        agent.Status = "UnAvailable";
                        listViewAgentsList.Items[selectedIndex].BackColor = Color.DarkGray;
                        listViewAgentsList.Items[selectedIndex].SubItems[2].Text = agent.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteMessage("Exception " + ex);
            }
        }

        private void buttonWork_Click(object sender, EventArgs e)
        {
            IAgent workAgent;
            int counter = 0;
            int selectedIndex = 0;
            try
            {
                foreach (ListViewItem item in listViewAgentsList.SelectedItems)
                {
                    selectedIndex = listViewAgentsList.SelectedIndices[counter++];
                    Agent agent = (Agent)item.Tag;
                    if (loginObjectTable.Contains(agent.AgentKey))
                    {
                        workAgent = (IAgent)loginObjectTable[agent.AgentKey];
                        workAgent.Work();
                        agent.Status = "Work";
                        listViewAgentsList.Items[selectedIndex].BackColor = Color.Yellow;
                        listViewAgentsList.Items[selectedIndex].SubItems[2].Text = agent.Status;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteMessage("Exception " + ex);
            }
        }
        public void AddInCallDetailGrid(string[] row, string callID)
        {
            int index;
            try
            {
                if (dataGridCallDetail.InvokeRequired)
                {
                    CallDetailGridHandler d = new CallDetailGridHandler(AddInCallDetailGrid);
                    Invoke(d, new object[] { row, callID });
                }
                else
                {
                    lock (dataGridCallDetail)
                    {
                        index = dataGridCallDetail.Rows.Add(row);
                        Processor.callsTable.Add(callID, index);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in AddInCallDetailGrid " + ex);
            }


        }


        public void UpdateCallDetailGrid(bool removeRequest, string callID, params string[] list)
        {
            int index = -1;
            DataGridViewRow row;
            try
            {
                if (dataGridCallDetail.InvokeRequired)
                {
                    CallDetailGridUpdater d = new CallDetailGridUpdater(UpdateCallDetailGrid);
                    Invoke(d, new object[] { removeRequest, callID, list });
                }
                else
                {
                    lock (dataGridCallDetail)
                    {
                        row = dataGridCallDetail.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => r.Cells["CallID"].Value.ToString().Equals(callID))
                        .FirstOrDefault();
                    }

                    if (row != null)
                    {
                        index = row.Index;
                        if (removeRequest)
                        {
                            lock (dataGridCallDetail)
                            {
                                dataGridCallDetail.Rows.RemoveAt(index);
                            }
                        }
                        else
                        {
                            lock (dataGridCallDetail)
                            {
                                for (int i = 0; i < list.Length; i++)
                                {
                                    dataGridCallDetail.Rows[index].Cells[i + 1].Value = list[i];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Exception in UpdateCallDetailGrid " + ex);
            }
        }


        //void _objAdmin_EventOccurred(AdministrationEvent administrationEvent)
        //{
        //    _logger.Info("AdministrationEvent|" + administrationEvent.Code + "|" + administrationEvent.EventType + "|" + administrationEvent.ObjectType + "|" + administrationEvent.Resource);
        //    switch (administrationEvent.ObjectType)
        //    {
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Base:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_CallType:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_DeleteList:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_UnavailableReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_User:

        //            if (administrationEvent.EventType == enAdministrationEventTypes.AdministrationEventType_AdministrationDatabaseUpdates)
        //            {
        //                WriteMessage("Found Event");
        //            }
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Queue:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_WorkReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_CallbackRetryReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_CallbackDeleteReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Aggregate:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Group:
        //            if (administrationEvent.EventType == enAdministrationEventTypes.AdministrationEventType_AdministrationDatabaseUpdates)
        //            {
        //                WriteMessage("Found Event");
        //            }
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Department:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_WrapupReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_EmailDiscardReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_Language:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_EmailMessageTemplate:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_WebCollaborationTemplate:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_RoutingUnavailableReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_RoutingWorkReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_PostProcessingReason:
        //            break;
        //        case enAdministrationEventObjectTypes.AdministrationEventObjectType_ManagerStateChanged:
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}


    }
}
