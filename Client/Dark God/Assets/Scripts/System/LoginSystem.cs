using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;

public class LoginSystem : SystemRoot
{
    public static LoginSystem Instance = null;

    public LoginWin loginWin;
    public CreateWin createWin;
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
            loginWin.SetWinState();
            audioSvc.PlayerBGMusic(Message.BGLogin);
        });


    }

    public void OnLoginRsp(GameMsg msg)
    {
        GameRoot.AddTips("µÇÂ¼³É¹¦");

        GameRoot.Instance.SetPlayerData(msg.rspLogin);
        if (msg.rspLogin.playerData.name == "")
        {
            createWin.SetWinState();
        }
        else
        {
            MainCitySystem.Instance.EnterMainCity();
        }
        loginWin.SetWinState(false);

    }

    public void OnRenameRsp(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        MainCitySystem.Instance.EnterMainCity();
        createWin.SetWinState(false);
    }
}
