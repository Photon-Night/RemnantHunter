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
                case CMD.ReqCheckConnection:
                    HeartbeatPacketSys.Instance.ReqCheckConnection(pack);
                    break;

                case CMD.ReqLogin:
                    LoginSys.Instance.ReqLogin(pack);
                    
                    break;

                case CMD.ReqRename:
                    LoginSys.Instance.ReqRename(pack);
                    break;

                //case CMD.ReqGuide:
                //    GuideSys.Instance.ReqGuide(pack);
                //    break;

                case CMD.ReqStrong:
                    StrongSys.Instance.ReqStrong(pack);
                    break;

                case CMD.SendChat:
                    ChatSys.Instance.SendChat(pack);
                    break;

                case CMD.ReqBuy:
                    BuySys.Instance.ReqBuy(pack);
                    break;

                //case CMD.ReqTakeTaskReward:
                //    TaskSys.Instance.ReqTakeTaskReward(pack);
                //    break;

                case CMD.ReqMissionEnter:
                    MissionSys.Instance.ReqMissionEnter(pack);
                    break;

                case CMD.ReqFBFightEnd:
                    MissionSys.Instance.ReqFBFightEnd(pack);
                    break;
                case CMD.ReqUpdateTaskPrg:
                    TaskSys.Instance.ReqUpdateTaskPrg(pack);
                    break;
                case CMD.ReqUpdateTaskInfo:
                    TaskSys.Instance.ReqUpdateTaskInfo(pack);
                    break;
                case CMD.ReqUseProp:
                    BagSys.Instance.ReqUseProp(pack);
                    break;
                case CMD.ReqChangeEquipment:
                    BagSys.Instance.ReqChangeEquipent(pack);
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
