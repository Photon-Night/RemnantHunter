 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocol;
using PENet;

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

    public void ClickCreateBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);

        if(iptName.text != "")
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = iptName.text
                }
            };

            netSvc.SendMessage(msg);
        }
        else
        {
            GameRoot.AddTips("��ǰ���ֲ��淶");
        }
    }
}
