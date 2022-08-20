 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWin : WinRoot
{
    public InputField iptName;
    public Button rdNameBtn;
    protected override void InitWin()
    {
        base.InitWin();
        iptName.text = resSvc.GetRDNameData();
        rdNameBtn.onClick.AddListener(ClickRDNameBtn);
    }

    public void ClickRDNameBtn()
    {
        iptName.text = resSvc.GetRDNameData();
        audioSvc.PlayUIAudio(Message.UIClickBtn);
    }
}
