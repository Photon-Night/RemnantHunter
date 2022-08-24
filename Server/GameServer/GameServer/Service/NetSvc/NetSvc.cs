using PENet;
using PEProtocol;

namespace GameServer
{
    public class NetSvc:Singleton<NetSvc>
    {
        public void Init()
        {
            PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
            server.StartAsServer(ServerCfg.srvIP, ServerCfg.srvPort);

            PECommon.Log("NetServer Loading");
        }
    }
}
