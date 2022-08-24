using System;
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
            NetSvc.Instance.Init();

            LoginSys.Instance.Init();
        }

    }
}
