﻿using PENet;
using PEProtocol;
using System.Collections.Generic;

namespace GameServer
{
    public class CacheSvc : Singleton<CacheSvc>
    {
        private Dictionary<string, ServerSession> onlineAccDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, PlayerData> onlineSession = new Dictionary<ServerSession, PlayerData>();
        public void Init()
        {
            PECommon.Log("CacheService Loading");
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
            return null;
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
    }
}