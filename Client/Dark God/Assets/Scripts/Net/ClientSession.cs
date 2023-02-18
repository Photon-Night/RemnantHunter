using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PENet;
using PEProtocol;

public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {
        GameRoot.AddTips("连接服务器成功");
        PECommon.Log("Connect To Server");
    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        PECommon.Log("RcvPack CMD:" + ((CMD)msg.cmd).ToString());
        NetService.Instance.AddNetPkg(msg);
    }

    protected override void OnDisConnected()
    {
        GameRoot.AddTips("服务器断开连接");
        PECommon.Log("DisConnect To Server");
    }
}
