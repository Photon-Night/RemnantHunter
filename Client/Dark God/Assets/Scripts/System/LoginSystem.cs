using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSystem : MonoSingleton<LoginSystem>
{
   public void SystemInit()
    {
        Debug.Log("LoginSystem Loading");
    }

    public void OnLoginEnter()
    {
        ResService.Instance.LoadSceneAsync(Message.SceneLogin, () =>
        {
            GameRoot.Instance.loginWin.SetWinState();
            AudioService.Instance.PlayerBGMusic(Message.BGLogin);
        });


    }


}
