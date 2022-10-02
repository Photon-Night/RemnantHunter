using PEProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ChatSys : Singleton<ChatSys>
    {
        public CacheSvc cacheSvc;
        public void Init()
        {
            cacheSvc = CacheSvc.Instance;
            PECommon.Log("ChatSystem Loading");
        }

        public void SendChat(MsgPack pack)
        {
            SendChat data = pack.msg.sendChat;
            PlayerData pd = cacheSvc.GetPlayerDataBySession(pack.session);

            TaskSys.Instance.CalcTaskPrgs(pd, 6);

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.PushChat
            };

            msg.pushCHat = new PushChat
            {
                name = pd.name,
                chat = data.chat,
            };

            List<ServerSession> sessions = cacheSvc.GetOnlineSession();
            byte[] bytes = PENet.PETool.PackNetMsg(msg);

            for (int i = 0; i < sessions.Count; i++)
            {
                sessions[i].SendMsg(bytes);
            }
        }
    }
}
