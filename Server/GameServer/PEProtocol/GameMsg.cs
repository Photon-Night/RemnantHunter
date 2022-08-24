using PENet;


namespace PEProtocol
{
    [System.Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
    }

    public class ServerCfg
    {
        public const string srvIP = "127.0.0.1";
        public const int srvPort = 10086;
    }

    [System.Serializable]
    public class ReqLogin
    {
        public string acc;
        public string pas;
    }

    public enum CMD
    {
        None = 0,
        ReqLogin = 101,
        RspLogin = 102,
    }

}
