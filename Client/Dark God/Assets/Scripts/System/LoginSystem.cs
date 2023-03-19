using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.Playables;
using Game.Service;

public class LoginSystem : SystemRoot<LoginSystem>
{
    public LoginWin loginWin;
    public CreateWin createWin;

    private System.Action<PlayableDirector> action = null;
    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("LoginSystem Loading");
    }

    public void OnLoginEnter()
    {
        audioSvc.PlayBGMusic(Message.BGLogin);
        loginWin.SetWinState();
    }

    public void OnLoginRsp(GameMsg msg)
    {
        GameRoot.AddTips("登入成功");

        GameRoot.Instance.SetPlayerData(msg.rspLogin);
        action = (director) =>
        {
            if (msg.rspLogin.playerData.name == "")
            {
                createWin.SetWinState();
            }
            else
            {
                MainCitySystem.Instance.EnterMainCity();
            }
            loginWin.SetWinState(false);
            dreSvc.UnRegisterTimelineEvent(TimelineEventType.Stopped, action);
        };
        dreSvc.PlayTimeLine(Message.TimelineLogin);
        dreSvc.RegisterTimelineEvent(TimelineEventType.Stopped, action);
    }

    private void ChangeScene(PlayableDirector director)
    {

    }

    public void OnRenameRsp(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        MainCitySystem.Instance.EnterMainCity();
        createWin.SetWinState(false);
    }
}
