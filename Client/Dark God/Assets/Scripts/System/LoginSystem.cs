using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.Playables;
using Game.Service;
using Game.Bag;

public class LoginSystem : SystemRoot<LoginSystem>
{
    public LoginWin loginWin;  

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
        loginWin.SetCover();
        GameRoot.Instance.SetPlayerData(msg.rspLogin);
        

        action = (director) =>
        {
            loginWin.SetWinState(false);
            MainCitySystem.Instance.EnterMainCity();
            dreSvc.UnRegisterTimelineEvent(TimelineEventType.Stopped, action);
        };

        dreSvc.PlayTimeLine(Message.TimelineLogin);
        dreSvc.RegisterTimelineEvent(TimelineEventType.Stopped, action);
    }

    
}
