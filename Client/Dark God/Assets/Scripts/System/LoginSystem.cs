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

    public void OnLoginRsp()
    {
        GameRoot.AddTips("µÇÂ¼³É¹¦");
        GameRoot.Instance.loginWin.SetWinState(false);
        GameRoot.Instance.createWin.SetWinState();
    }

}
