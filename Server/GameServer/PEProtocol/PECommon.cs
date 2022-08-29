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
    }

   public enum LogType
    {
        log = 0,
        Warn = 1,
        Error = 2,
        Info = 3,
    }
}
