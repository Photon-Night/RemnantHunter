using PENet;


namespace PEProtocol
{
    [System.Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;
        public ReqRename reqRename;
        public RspRename rspRename;
        public ReqGuide reqGuide;
        public RspGuide rspGuide;
        public ReqStrong reqStrong;
        public RspStrong rspStrong;
        public SendChat sendChat;
        public PushChat pushCHat;
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
    [System.Serializable]
    public class RspLogin
    {
        //TODO
        public PlayerData playerData;
    }

    [System.Serializable]
    public class ReqRename
    {
        public string name;
    }

    [System.Serializable]
    public class RspRename
    {
        public string name;
    }

    [System.Serializable]

    public class ReqGuide
    {
        public int guideid;
    }

    [System.Serializable]
    public class RspGuide
    {
        public int guideid;
        public int coin;
        public int lv;
        public int exp;
    }

    [System.Serializable]
    public class ReqStrong
    {
        public int pos;
    }

    [System.Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int[] strong;
    }


    [System.Serializable]
    public class SendChat
    {
        public string chat;
    }

    [System.Serializable]
    public class PushChat
    {
        public string name;
        public string chat;
    }


    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;//闪避概率
        public int pierce;//穿透比率
        public int critical;//暴击概率

        public int guideid;
        public int[] strong;
        public int crystal;
    }

    public enum CMD
    {
        None = 0,

        ReqLogin = 101,
        RspLogin = 102,
        ReqRename = 103,
        RspRename = 104,

        ReqGuide = 200,
        RspGuide = 201,
        ReqStrong = 202,
        RspStrong = 203,

        SendChat = 204,
        PushChat = 205,
    }

    public enum ErrorCode
    {
        None = 0,
        AccountIsOnline,
        WrongPass,
        NameIsExist,

        UpdateDBError,
        ServerDataError,

        LackLevel,
        LackCoin,
        LackCrystal,
    }


}
