using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class StrongSys : Singleton<StrongSys>
    {
        public CacheSvc cacheSvc;
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            PECommon.Log("StrongSystem Loading");
        }

        public void ReqStrong(MsgPack pack)
        {
            ReqStrong data = pack.msg.reqStrong;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspStrong
            };

            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            int currentLv = pd.strong[data.pos];
            StrongCfg nextSd = CfgSvc.Instance.GetStrongCfgData(data.pos, currentLv + 1);

            if(pd.lv < nextSd.minLv)
            {
                msg.err = (int)ErrorCode.LackLevel;
            }
            else if(pd.coin < nextSd.coin)
            {
                msg.err = (int)ErrorCode.LackCoin;
            }
            else if(pd.crystal < nextSd.crystal)
            {
                msg.err = (int)ErrorCode.LackCrystal;
            }
            else
            {
                pd.coin -= nextSd.coin;
                pd.crystal -= nextSd.crystal;

                pd.strong[data.pos] += 1;

                pd.hp += nextSd.addHp;
                pd.ad += nextSd.addHurt;
                pd.ap += nextSd.addHurt;
                pd.addef += nextSd.addDef;
                pd.apdef += nextSd.addDef;
            }

            if(!cacheSvc.UpdatePlayerData(pd.id, pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
            }
            else
            {
                msg.rspStrong = new RspStrong
                {
                    coin = pd.coin,
                    crystal = pd.crystal,
                    hp = pd.hp,
                    ad = pd.ad,
                    ap = pd.ap,
                    addef = pd.addef,
                    apdef = pd.apdef,

                    strong = pd.strong,
                };

                TaskSys.Instance.CalcTaskPrgs(pd, 3);
            }

            pack.session.SendMsg(msg);
        }

        
    }
}
