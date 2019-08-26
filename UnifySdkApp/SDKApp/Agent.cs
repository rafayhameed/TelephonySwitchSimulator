

namespace SDKApp
{
    public class Agent
    {
        private string name;
        private string agentKey;
        private string agentID;
        private string extenssion = "";
        private string waitTime;
        private string callTime;
        private bool loggedIn = false;
        private string status = "LoggedOUT";
        private string callID;

        public string Name { get => name; set => name = value; }
        public string AgentKey { get => agentKey; set => agentKey = value; }
        public string AgentID { get => agentID; set => agentID = value; }
        public string Extenssion { get => extenssion; set => extenssion = value; }
        public string WaitTime { get => waitTime; set => waitTime = value; }
        public string CallTime { get => callTime; set => callTime = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }
        public string Status { get => status; set => status = value; }
        public string CallID { get => callID; set => callID = value; }
    }
}
