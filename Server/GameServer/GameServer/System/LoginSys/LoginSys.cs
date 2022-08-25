using PENet;
using PEProtocol;

namespace GameServer
{
    class LoginSys : Singleton<LoginSys>
    {

        private CacheSvc cache;
        public void Init()
        {
            PECommon.Log("LoginSystem Loading");
            cache = CacheSvc.Instance;
        }

        public void ReqLogin(MsgPack pack)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspLogin,
                rspLogin = new RspLogin 
                {

                }
            };
            ReqLogin _data = pack.msg.reqLogin;
            if(cache.IsAccOnLine(_data.acc))
            {
                msg.err = (int)ErrorCode.AccountIsOnline;
            }
            else
            {

            }





            pack.session.SendMsg(msg);
        }
    }
}
