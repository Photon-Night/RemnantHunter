 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using PENet;

public class CreateWin : WinRoot
{
    public InputField iptName;
    public Button btnRdName;

    protected override void InitWin()
    {
        base.InitWin();
        iptName.text = resSvc.GetRDNameData();
        btnRdName.onClick.AddListener(OnClickbtnRdName);
    }

    public void OnClickbtnRdName()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        iptName.text = resSvc.GetRDNameData();
    }

    public void OnClickChangeRole()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.ChangeRole();
    }

    public void OnClickChangeBody()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.ChangeBody();
    }

    public void OnClickChangeWeapon()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
    }

    public void OnClickChangeShield()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
    }

    public void OnClickCreateBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);

        if(iptName.text != "")
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = iptName.text,
                    modle = MainCitySystem.Instance.GetPlayerModleIndex(),
                }
            };

            netSvc.SendMessage(msg);
        }
        else
        {
            GameRoot.AddTips("请输入名称");
        }
    }
}
