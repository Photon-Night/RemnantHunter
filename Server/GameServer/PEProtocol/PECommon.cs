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

    }

   public enum LogType
    {
        log = 0,
        Warn = 1,
        Error = 2,
        Info = 3,
    }
}
