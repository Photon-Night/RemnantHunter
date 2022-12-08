using PENet;
using PEProtocol;
using System.Collections.Generic;

namespace GameServer
{
    public class CacheSvc : Singleton<CacheSvc>
    {
        private Dictionary<string, ServerSession> onlineAccDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, PlayerData> onlineSession = new Dictionary<ServerSession, PlayerData>();
        private Dictionary<int, ServerSession> onlineSessionConn = new Dictionary<int, ServerSession>();

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
            onlineSessionConn.Add(session.sessionID, session);
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
            var ge = onlineAccDic.GetEnumerator();
            string _acc = "";
            while (ge.MoveNext())
            {
                if (ge.Current.Value == session)
                {
                    _acc = ge.Current.Key;
                    break;
                }
            }
            bool succ = onlineAccDic.Remove(_acc);
            bool succ2 = onlineSession.Remove(session);
            bool succ3 = onlineSessionConn.Remove(session.sessionID);

            PECommon.Log("OffLine Result: SessionID:" + session.sessionID + "___Account Release:" + succ + "___Session Release:" + succ2 + "___Session Conn Release" + succ3);
            ge.Dispose();
        }

        public List<ServerSession> GetOnlineSession()
        {
            List<ServerSession> sessions = new List<ServerSession>();
            foreach (var item in onlineSession)
            {
                sessions.Add(item.Key);
            }
            return sessions;
        }
        public Dictionary<ServerSession, PlayerData> GetOnlineCache()
        {
            return onlineSession;
        }

        public Dictionary<int, ServerSession> GetConnSessionCache()
        {
            return onlineSessionConn;
        }

        public void SetConnCheckFlagBySessionID(int sessionID, bool flag = false)
        {
            onlineSessionConn[sessionID].connCheckFlag = flag;
        }

        public ServerSession GetOnlineSessionByID(int id)
        {
            var e = onlineSession.GetEnumerator();
            ServerSession session = null;
            while(e.MoveNext())
            {
                if(e.Current.Value.id == id)
                {
                    session = e.Current.Key;
                    break;
                }
            }

            e.Dispose();
            return session;
        }
    }
}
