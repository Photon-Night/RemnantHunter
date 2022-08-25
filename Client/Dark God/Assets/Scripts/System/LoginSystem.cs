using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class LoginSystem : SystemRoot
{
    public static LoginSystem Instance = null;

     public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("LoginSystem Loading");
        Instance = this;
    }

    public void OnLoginEnter()
    {
        resSvc.LoadSceneAsync(Message.SceneLogin, () =>
        {
            GameRoot.Instance.loginWin.SetWinState();
            audioSvc.PlayerBGMusic(Message.BGLogin);
        });


    }

    public void OnLoginRsp(GameMsg msg)
    {
        GameRoot.AddTips("µÇÂ¼³É¹¦");

        GameRoot.Instance.SetPlayerData(msg.rspLogin);
        if (msg.rspLogin.playerData.name == "")
        {
            GameRoot.Instance.createWin.SetWinState();
        }
        else
        {
            //TODO
        }
        GameRoot.Instance.loginWin.SetWinState(false);

    }

}
