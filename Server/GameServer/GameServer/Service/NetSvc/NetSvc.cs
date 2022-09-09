using PENet;
using PEProtocol;
using System.Collections.Generic;

namespace GameServer
{
    public class NetSvc : Singleton<NetSvc>
    {

        private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();
        public static readonly string obj = "lock";
        public void Init()
        {
            PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
            server.StartAsServer(ServerCfg.srvIP, ServerCfg.srvPort);

            PECommon.Log("NetService Loading");
        }


        public void AddMsgQue(ServerSession session, GameMsg msg)
        {
             lock(obj)
            {
                msgPackQue.Enqueue(new MsgPack(session, msg));
            }
        }

        public void Update()
        {
            if(msgPackQue.Count != 0)
            {
                PECommon.Log("PackCount:" + msgPackQue.Count);
                lock (obj)
                {
                    MsgPack pack = msgPackQue.Dequeue();
                    HandOutMsg(pack);
                }
            }
        }

        private void HandOutMsg(MsgPack pack)
        {
            switch ((CMD)pack.msg.cmd)
            {
                case CMD.ReqLogin:
                    LoginSys.Instance.ReqLogin(pack);
                    break;

                case CMD.ReqRename:
                    LoginSys.Instance.ReqRename(pack);
                    break;

                case CMD.ReqGuide:
                    GuideSys.Instance.ResqGuide(pack);
                    break;
            }

        }

    }

    public class MsgPack
    {
        public ServerSession session;
        public GameMsg msg;

        public MsgPack(ServerSession session, GameMsg msg)
        {
            this.session = session;
            this.msg = msg;
        }
    }


}
