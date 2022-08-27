using PENet;
using PEProtocol;
using System.Collections.Generic;

namespace GameServer
{
    public class CacheSvc : Singleton<CacheSvc>
    {
        private Dictionary<string, ServerSession> onlineAccDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, PlayerData> onlineSession = new Dictionary<ServerSession, PlayerData>();

        private DBMgr dbMgr = null;
        public void Init()
        {
            PECommon.Log("CacheService Loading");
            dbMgr = DBMgr.Instance;
        }

        public bool IsAccOnLine(string acc)
        {
            return onlineAccDic.ContainsKey(acc);
        }
        /// <summary>
        /// 数据库查找账号，存在返回信息， 否则返回null
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        public PlayerData GetPlayerData(string acc, string pas)
        {
            //TODO 
            //数据库查找
            return dbMgr.QueryPlayerData(acc, pas);
        }
        /// <summary>
        /// 账号上线，数据缓存
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="session"></param>
        /// <param name="playerData"></param>
        public void AccOnLine(string acc, ServerSession session, PlayerData playerData)
        {
            onlineAccDic.Add(acc, session);
            onlineSession.Add(session, playerData);
        }

        public bool IsNameExist(string name)
        {
            return dbMgr.QueryNameData(name);
        }

        public PlayerData GetPlayerDataBySession(ServerSession session)
        {
            if (onlineSession.TryGetValue(session, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                return null;
            }
        }

        public bool UpdatePlayerData(int id, PlayerData playerData)
        {           
            return dbMgr.UpdatePlayerData(id, playerData);
        }

        public void ReleaseCache(ServerSession session)
        {
            
        }
    }
}
