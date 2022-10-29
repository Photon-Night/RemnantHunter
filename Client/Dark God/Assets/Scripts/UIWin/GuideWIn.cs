using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideWin : WinRoot
{
    public Text txtName;
    public Text txtTalk;
    public Image imgIcon;

    private PlayerData pd;
    private GuideCfg currentTaskData;
    private string[] dialogArr;
    private int index;

    protected override void InitWin()   
    {
        base.InitWin();
        pd = GameRoot.Instance.PlayerData;
        currentTaskData = MainCitySystem.Instance.GetCurrentTaskData();
        dialogArr = currentTaskData.dilogArr.Split('#');
        index = 1;
        SetTalk();
    }

    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if(talkArr[0] == "0")
        {
            SetSprite(imgIcon, PathDefine.SelfIcon);
            SetText(txtName, pd.name);
        }
        else
        {
            switch (currentTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    SetText(txtName, "����");
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, "����");
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, "����");
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, "����");
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, "Сܿ");
                    break;

            }
        }
        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name));
    }

    public void OnClickNextTalkBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        index += 1;
        if(index == dialogArr.Length)
        {
            GameMsg msg = new GameMsg
            { 
                cmd = (int)CMD.ReqGuide 
            };

            msg.reqGuide = new ReqGuide { guideid = currentTaskData.ID };

            netSvc.SendMessage(msg);
            SetWinState(false);

        }
        else
        SetTalk();

    }
}
