using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSystem : SystemRoot
{
    public static LoginSystem Instance = null;

   public void SystemInit()
    {
        Debug.Log("LoginSystem Loading");
        Instance = this;
        base.InitSys();
    }

    public void OnLoginEnter()
    {
        resSvc.LoadSceneAsync(Message.SceneLogin, () =>
        {
            GameRoot.Instance.loginWin.SetWinState();
            audioSvc.PlayerBGMusic(Message.BGLogin);
        });


    }


}
