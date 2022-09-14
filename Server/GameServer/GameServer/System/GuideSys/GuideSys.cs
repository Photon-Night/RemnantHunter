using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PEProtocol;

namespace GameServer
{
    class GuideSys : Singleton<GuideSys>
    {
        private CacheSvc cacheSvc;
        private CfgSvc cfgSvc;

        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            cfgSvc = CfgSvc.Instance;
            PECommon.Log("GuideSystem Loading");
        }

        public void ResqGuide(MsgPack pack)
        {
            ReqGuide data = pack.msg.reqGuide;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspGuide
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            GuideCfg gc = cfgSvc.GetGuideCfgData(data.guideid);

            if (pd.guideid == data.guideid)
            {
                pd.guideid += 1;

                pd.coin += gc.coin;
                CalcExp(pd, gc.exp);

                if(!cacheSvc.UpdatePlayerData(pd.id, pd))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    RspGuide rspGuide = new RspGuide
                    {
                        guideid = pd.guideid,
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp
                    };

                    msg.rspGuide = rspGuide;
                }
            }
            else
            {
                msg.err = (int)ErrorCode.ServerDataError;
            }

            pack.session.SendMsg(msg);

        }

        private void CalcExp(PlayerData pd, int addExp)
        {
            int currentLv = pd.lv;
            int currentExp = pd.exp;
            int addRestExp = addExp;

            while(true)
            {
                int upNeedExp = PECommon.GetExpUpValByLv(currentLv) - currentExp;
                if(addRestExp >= upNeedExp)
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
    }

}
