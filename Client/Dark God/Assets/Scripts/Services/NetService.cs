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
                    GameRoot.AddTips("用户已在线");
                    break;

                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;

                case ErrorCode.NameIsExist:
                    GameRoot.AddTips("名字已存在");
                    break;
                case ErrorCode.UpdateDBError:
                    GameRoot.AddTips("网络不稳定");
                    PECommon.Log("数据库更行异常", PEProtocol.LogType.Error);
                    break;

                case ErrorCode.ServerDataError:
                    GameRoot.AddTips("客户端数据异常");
                    PECommon.Log("数据库数据异常", PEProtocol.LogType.Error);
                    break;

                case ErrorCode.LackCoin:
                    GameRoot.AddTips("金币不足");
                    break;

                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("水晶不足");
                    break;

                case ErrorCode.LackLevel:
                    GameRoot.AddTips("等级不足");
                    break;

                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("钻石不足");
                    break;

                case ErrorCode.ClientDataError:
                    GameRoot.AddTips("客户端数据异常");
                    break;

                case ErrorCode.LackPower:
                    GameRoot.AddTips("体力不足");
                    break;

                case ErrorCode.LackPreTask:
                    GameRoot.AddTips("前置任务未完成");
                    break;

                case ErrorCode.LackTargetCount:
                    GameRoot.AddTips("未达到任务要求");
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

            //case CMD.RspGuide:
            //    MainCitySystem.Instance.RspGuide(msg);
            //    break;

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

            //case CMD.RspTakeTaskReward:
            //    MainCitySystem.Instance.RspTakeTaskReward(msg);
            //    break;
            //
            //case CMD.PushTaskPrgs:
            //    MainCitySystem.Instance.PushTaskPrgs(msg);
            //    break;

            case CMD.RspMissionEnter:
                MissionSystem.Instance.RspMissionEnter(msg);
                break;

            case CMD.RspCheckConnection:
                RspCheckConnection(msg);
                break;

            case CMD.RspFBFightEnd:
                BattleSystem.Instance.RspFBFightEnd(msg);
                break;

            case CMD.RspUpdateTaskInfo:
                TaskSystem.Instance.RspUpdateTaskInfo(msg);
                break;

            case CMD.RspUpdateTaskPrg:
                TaskSystem.Instance.RspUpdateTaskPrg(msg);
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

    private void RspCheckConnection(GameMsg msg)
    {
        int sessionID = msg.rspCheckConnection.id;
        GameMsg newMsg = new GameMsg
        {
            cmd = (int)CMD.ReqCheckConnection,
            reqCheckConnection = new ReqCheckConnection
            {
                id = sessionID
            }
        };

        SendMessage(newMsg);
    }
}
