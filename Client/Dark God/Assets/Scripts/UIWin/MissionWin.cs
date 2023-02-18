using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWin : WinRoot
{

    private PlayerData pd;
    public Transform[] btnMissionArr;
    protected override void InitWin()
    {
        pd = GameRoot.Instance.PlayerData;
        base.InitWin();
        RefreshUI();
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        this.SetWinState(false);
    }

    public void RefreshUI()
    {
        int index = (pd.mission % 10000) - 1;
        for (int i = 0; i < btnMissionArr.Length; i++)
        {
            if (i <= index)
            {
                SetActive(btnMissionArr[i].gameObject);
            }
            else
            {
                SetActive(btnMissionArr[i].gameObject, false);
            }
        }
    }

    public void OnClickMissionBtn(int index)
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);

        MapCfg data = resSvc.GetMapCfgData(index);
        if(pd.power - data.power < 0)
        {
            PECommon.Log("体力不足");
        }
        else
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqMissionEnter,
            };

            msg.reqMissionEnter = new ReqMissionEnter
            {
                mid = index,
            };

            netSvc.SendMessage(msg);
        }
        
    }
}
