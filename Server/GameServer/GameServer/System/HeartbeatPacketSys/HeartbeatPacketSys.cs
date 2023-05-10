using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

namespace GameServer
{
    class HeartbeatPacketSys : Singleton<HeartbeatPacketSys>
    {
        TimerSvc timerSvc;
        CacheSvc cacheSvc;

        Dictionary<int, ServerSession> sessionDic;
        

        public void Init()
        {
            timerSvc = TimerSvc.Instance;
            cacheSvc = CacheSvc.Instance;

            sessionDic = cacheSvc.GetConnSessionCache();

            //timerSvc.AddTimeTask((tid) => { Heartbeat(); }, PECommon.HeartbeatSpace, PETimeUnit.Second, 0);
            //timerSvc.AddTimeTask((tid) => { CheckConnection(); }, PECommon.CheckSpace + PECommon.HeartbeatSpace, PETimeUnit.Second, 0);
        }

        private void Heartbeat()
        {           
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspCheckConnection,
                rspCheckConnection = new RspCheckConnection(),
            };
            var e = sessionDic.GetEnumerator();
            
            while(e.MoveNext())
            {
                msg.rspCheckConnection.id = e.Current.Key;
                PECommon.Log("Session_ID: " + e.Current.Value.sessionID + " Check_Connection");
                e.Current.Value.SendMsg(msg);
            }
            e.Dispose();
        }

        private void CheckConnection()
        {
            List<ServerSession> session = sessionDic.Values.ToList();
            for(int i = 0; i < session.Count; i++)
            {
                if(session[i].connCheckFlag)
                {
                    sessionDic[session[i].sessionID].connCheckFlag = false;
                }
                else
                {
                    cacheSvc.ReleaseCache(session[i]);

                }
            }
        }

        public void ReqCheckConnection(MsgPack pack)
        {
            if (sessionDic.ContainsKey(pack.session.sessionID))
            {

                sessionDic[pack.session.sessionID].connCheckFlag = true;
                PECommon.Log("Session_ID: " + pack.session.sessionID + " is online");
            }
        }
    }
}
