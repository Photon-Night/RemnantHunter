﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ServerRoot : Singleton<ServerRoot>
    {
        public void Init()
        {
            DBMgr.Instance.Init();
            //服务层
            NetSvc.Instance.Init();
            CacheSvc.Instance.Init();
            CfgSvc.Instance.Init();
            TimerSvc.Instance.Init();
            //业务层
            LoginSys.Instance.Init();
            //GuideSys.Instance.Init();
            StrongSys.Instance.Init();
            ChatSys.Instance.Init();
            BuySys.Instance.Init();
            PowerSys.Instance.Init();
            TaskSys.Instance.Init();
            MissionSys.Instance.Init();
            BagSys.Instance.Init();
            HeartbeatPacketSys.Instance.Init();
         
        }


        public void Update()
        {
            NetSvc.Instance.Update();
            TimerSvc.Instance.Update();
        }

        private int SessionID = 0;
        public int GetSessionID()
        {
            if (SessionID == int.MaxValue)
                SessionID = 0;

            return SessionID += 1;
        }
    }
}
