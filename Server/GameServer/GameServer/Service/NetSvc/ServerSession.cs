using PENet;
using PEProtocol;


namespace GameServer
{
    public class ServerSession : PESession<GameMsg>
    {
        public int sessionID = 0;
        protected override void OnConnected()
        {
            sessionID = ServerRoot.Instance.GetSessionID();
            PECommon.Log("SesionID:" + sessionID + "Client Connect");
            
        } 

        protected override void OnDisConnected()
        {
            LoginSys.Instance.ClearOffLine(this);
            PECommon.Log("SesionID:" + sessionID + "Client DisConnect");
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PECommon.Log("SesionID:" + sessionID + "Client Req:"  + ((CMD)msg.cmd).ToString());
            NetSvc.Instance.AddMsgQue(this, msg); 
        }
    }
}
