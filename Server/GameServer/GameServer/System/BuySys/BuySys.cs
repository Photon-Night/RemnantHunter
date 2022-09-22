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
        public void Init()
        {
            PECommon.Log("BuySystem Loading");
        }

        public void ReqBuy(MsgPack pack)
        {

        }

    }
}
