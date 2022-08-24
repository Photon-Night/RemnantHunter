using PENet;
using PEProtocol;

namespace GameServer
{
    class LoginSys : Singleton<LoginSys>
    {
        public void Init()
        {
            PECommon.Log("LoginSystem Loading");
        }
    }
}
