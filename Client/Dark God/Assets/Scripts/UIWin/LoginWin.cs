using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWin : WinRoot
{
    public Text accInput;
    public Text pasInput;

    public Button loginButton;

    protected override void InitWin()
    {
        if(PlayerPrefs.GetString("acc") != null && PlayerPrefs.GetString("pas") != null)
        {
            SetText(accInput, PlayerPrefs.GetString("acc"));
            SetText(pasInput, PlayerPrefs.GetString("pas"));
        }
        else
        {
            SetText(accInput);
            SetText(pasInput);
        }
    }
}
