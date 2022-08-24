using PENet;
using PEProtocol;


namespace GameServer
{
    class ServerSession : PESession<GameMsg>
    {
        protected override void OnConnected()
        {
            PECommon.Log("Client Connect");
            
        } 

        protected override void OnDisConnected()
        {
            PECommon.Log("Client DisConnect");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PECommon.Log("Client Req:" );
           
        }
    }
}
