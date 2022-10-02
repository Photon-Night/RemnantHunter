using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class BuySys : Singleton<BuySys>
    {
        public CacheSvc cacheSvc;
        public void Init()
        {
            PECommon.Log("BuySystem Loading");
            cacheSvc = CacheSvc.Instance;
        }

        public void ReqBuy(MsgPack pack)
        {
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);
            ReqBuy data = pack.msg.reqBuy;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspBuy
            };

            if(pd.diamond < data.diamond)
            {
                msg.err = (int)ErrorCode.LackDiamond;
            }
            else
            {
                pd.diamond -= data.diamond;
                switch (data.buyType)
                {
                    case 0:
                        pd.coin += 1000;
                        TaskSys.Instance.CalcTaskPrgs(pd, 5);
                        break;
                    case 1:
                        pd.power += 100;
                        TaskSys.Instance.CalcTaskPrgs(pd, 4);
                        break;
                }

                RspBuy rspBuy = new RspBuy
                {
                    power = pd.power,
                    coin = pd.coin,
                    buyType = data.buyType,
                    diamond = pd.diamond
                };

                msg.rspBuy = rspBuy;
                PECommon.Log("Client" + pack.session.sessionID + "Cost " + data.diamond + "/" + pd.diamond);

                cacheSvc.UpdatePlayerData(pd.id, pd);
            }

           

            pack.session.SendMsg(msg);
        }

    }
}
