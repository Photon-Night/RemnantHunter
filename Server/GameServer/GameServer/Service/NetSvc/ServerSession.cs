using PENet;
using PEProtocol;


namespace GameServer
{
    public class ServerSession : PESession<GameMsg>
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
            PECommon.Log("Client Req:"  + ((CMD)msg.cmd).ToString());
            NetSvc.Instance.AddMsgQue(this, msg); 
        }
    }
}
