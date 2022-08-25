using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PENet;
using PEProtocol;

public class NetService : MonoSingleton<NetService>
{

    PESocket<ClientSession, GameMsg> client = null;
    private Queue<GameMsg> msgQue = null;
    private static readonly string obj = "lock";
    public void ServiceInit()
    {
        PECommon.Log("NetService Loading");
        client = new PESocket<ClientSession, GameMsg>();
        msgQue = new Queue<GameMsg>();
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

    public void AddNetPkg(GameMsg msg)
    {
        lock(obj)
        {
            msgQue.Enqueue(msg);
        }
    }

    private void ProcessMsg(GameMsg msg)
    {
        if(msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.AccountIsOnline:
                    GameRoot.AddTips("当前账号已上线");
                    break;

                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
            }
            return;
        }

        switch ((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSystem.Instance.OnLoginRsp(msg);
                break;
        }

    }

    private void Update()
    {
        if(msgQue.Count > 0)
        {
            GameMsg msg = msgQue.Dequeue();
            ProcessMsg(msg);
        }
    }
}
