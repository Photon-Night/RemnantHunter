using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;

public class LoginWin : WinRoot
{
    public InputField accInput;
    public InputField pasInput;
    public Image cover;

    protected override void InitWin()
    {
        base.InitWin();
        if (PlayerPrefs.GetString("acc") != null && PlayerPrefs.GetString("pas") != null)
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

    protected override void ClearWin()
    {
        base.ClearWin();
        SetActive(cover, false);
    }
    public void ClickEnterBtn()
    {
        audioSvc.PlayUIAudio(Message.UILoginBtn);
        //SetActive(cover);
        string _acc = accInput.text;
        string _pas = pasInput.text;

        if (_acc != "" && _pas != "")
        {
            PlayerPrefs.SetString("acc", _acc);
            PlayerPrefs.SetString("pas", _pas);
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin 
                {
                    acc = _acc,
                    pas = _pas
                }
            };

            netSvc.SendMessage(msg);
        }

        else
            GameRoot.AddTips("请输入完整账号信息");
    }

    public void SetCover(bool state = true)
    {
        SetActive(cover, state);
    }

    public void OnClickCheckOutBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
