using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class MissionSys:Singleton<MissionSys>
    {
        private CacheSvc cacheSvc = null;
        private CfgSvc cfgSvc = null;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;

            PECommon.Log("MissionSystem Loading");
        }


        public void ReqMissionEnter(MsgPack pack)
        {
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            ReqMissionEnter data = pack.msg.reqMissionEnter;
            int power = cfgSvc.GetMapCfg(data.mid).power;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspMissionEnter,
            };

            if(pd.power - power < 0)
            {
                msg.err = (int)ErrorCode.LackPower;
            }
            else if(pd.mission < data.mid)
            {
                msg.err = (int)ErrorCode.ClientDataError;
            }
            else 
            {
                pd.power -= power;

                if (cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.rspMissionEnter = new RspMissionEnter
                    {
                        mid = data.mid,
                        power = pd.power,
                    };
                }
                else
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }


            }
            pack.session.SendMsg(msg);
        }
    }
}
