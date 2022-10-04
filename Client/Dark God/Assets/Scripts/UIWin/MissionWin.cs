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
}
