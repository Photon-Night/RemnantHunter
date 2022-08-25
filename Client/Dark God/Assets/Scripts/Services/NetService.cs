using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PENet;
using PEProtocol;

public class NetService : MonoSingleton<NetService>
{

    PESocket<ClientSession, GameMsg> client = null;
    public void ServiceInit()
    {
        PECommon.Log("NetService Loading");
        client = new PESocket<ClientSession, GameMsg>();
        client.SetLog(true, (string msg, int lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "Warn:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
        });
        client.StartAsClient(ServerCfg.srvIP, ServerCfg.srvPort);
    }

    public void SendMessage(GameMsg msg)
    {
        client.session.SendMsg(msg);
    }
}
