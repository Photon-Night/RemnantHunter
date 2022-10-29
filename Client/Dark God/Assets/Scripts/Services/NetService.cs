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
        PECommon.Log("NetService Loading", PEProtocol.LogType.log);
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
                    GameRoot.AddTips("��ǰ�˺�������");
                    break;

                case ErrorCode.WrongPass:
                    GameRoot.AddTips("�������");
                    break;

                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("��ǰ�����Ѵ���");
                    break;
                case ErrorCode.UpdateDBError:
                    GameRoot.AddTips("���粻�ȶ�");
                    PECommon.Log("���ݿ�����쳣");
                    break;

                case ErrorCode.ServerDataError:
                    GameRoot.AddTips("�ͻ��������쳣");
                    break;

                case ErrorCode.LackCoin:
                    GameRoot.AddTips("��Ҳ���");
                    break;

                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("ˮ������");
                    break;

                case ErrorCode.LackLevel:
                    GameRoot.AddTips("�ȼ�����");
                    break;

                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("��ʯ����");
                    break;

                case ErrorCode.ClientDataError:
                    GameRoot.AddTips("�ͻ��������쳣");
                    break;

                case ErrorCode.LackPower:
                    GameRoot.AddTips("��������");
                    break;
            }
            return;
        }

        switch ((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSystem.Instance.OnLoginRsp(msg);
                break;

            case CMD.RspRename:
                LoginSystem.Instance.OnRenameRsp(msg);
                break;

            case CMD.RspGuide:
                MainCitySystem.Instance.RspGuide(msg);
                break;

            case CMD.RspStrong:
                MainCitySystem.Instance.RspStrong(msg);
                break;

            case CMD.PushChat:
                MainCitySystem.Instance.PushChat(msg);
                break;

            case CMD.RspBuy:
                MainCitySystem.Instance.RspBuy(msg);
                break;

            case CMD.PushPower:
                MainCitySystem.Instance.PushPower(msg);
                break;

            case CMD.RspTakeTaskReward:
                MainCitySystem.Instance.RspTakeTaskReward(msg);
                break;

            case CMD.PushTaskPrgs:
                MainCitySystem.Instance.PushTaskPrgs(msg);
                break;

            case CMD.RspMissionEnter:
                MissionSystem.Instance.RspMissionEnter(msg);
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
