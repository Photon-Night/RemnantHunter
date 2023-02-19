using PENet;

namespace PEProtocol
{
    public class PECommon
    {
        public static void Log(string msg = "", LogType tp = LogType.log)
        {
            LogLevel lv = (LogLevel)tp;
            PETool.LogMsg(msg, lv);
        }

        public static int GetFightByProps(PlayerData pd)
        {
            return pd.lv * 100 + pd.ad + pd.ap + pd.addef + pd.apdef;
        }

        public static int GetPowerLimit(int lv)
        {
            return ((lv - 1) / 10) * 510 + 150;
        }
         

        public static int GetExpUpValByLv(int lv)
        {
            return 100 * lv * lv;
        }

        public static void CalcExp(PlayerData pd, int addExp)
        {
            int currentLv = pd.lv;
            int currentExp = pd.exp;
            int addRestExp = addExp;

            while (true)
            {
                int upNeedExp = GetExpUpValByLv(currentLv) - currentExp;
                if (addRestExp >= upNeedExp)
                {
                    currentLv += 1;
                    currentExp = 0;
                    addRestExp -= upNeedExp;
                }
                else
                {
                    pd.lv = currentLv;
                    pd.exp = currentExp + addRestExp;
                    break;
                }
            }
        }

        public const int PowerAddSpace = 2;
        public const int PowerAddCount = 2;

        public const int HeartbeatSpace = 5;
        public const int CheckSpace = 5;
    }

   public enum LogType
    {
        log = 0,
        Warn = 1,
        Error = 2,
        Info = 3,
    }
    public enum TaskStatus
    {
        None = -1,
        InProgress = 0,
        Complated = 1,
        Finished = 2,
        Failed = 3,
    }
}
