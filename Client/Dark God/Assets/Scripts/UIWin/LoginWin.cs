using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWin : WinRoot
{
    public InputField accInput;
    public InputField pasInput;

    public Button loginButton;

    protected override void InitWin()
    {
        base.InitWin();
        
        if(PlayerPrefs.GetString("acc") != null && PlayerPrefs.GetString("pas") != null)
        {
            accInput.text = PlayerPrefs.GetString("acc");
            pasInput.text = PlayerPrefs.GetString("pas");
        }
        else
        {
            accInput.text = "";
            pasInput.text = "";
        }
    }

    public void ClickEnterBtn()
    {
        audioSvc.PlayUIAudio(Message.UILoginBtn);
        if(CheckInput())
        {
            LoginSystem.Instance.OnLoginRsp();
        }
    }

    private bool CheckInput()
    {
        string acc = accInput.text;
        string pas = pasInput.text;

        if (acc != "" && pas != "")
        {
            PlayerPrefs.SetString("acc", acc);
            PlayerPrefs.SetString("pas", pas);
            return true;
        }

        else
            GameRoot.AddTips("«Î ‰»Î’À∫≈√‹¬Î");
            return false;
    }
}
