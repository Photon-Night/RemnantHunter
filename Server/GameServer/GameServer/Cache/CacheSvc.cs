using PENet;
using PEProtocol;
using System.Collections.Generic;

namespace GameServer
{
    public class CacheSvc : Singleton<CacheSvc>
    {
        private Dictionary<string, ServerSession> onlineAccDic = new Dictionary<string, ServerSession>();

        public void Init()
        {
            PECommon.Log("CacheService Loading");
        }

        public bool IsAccOnLine(string acc)
        {
            return onlineAccDic.ContainsKey(acc);
        }
    }
}
