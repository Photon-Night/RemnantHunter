using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskWin : WinRoot
{
    protected override void InitWin()
    {
        base.InitWin();

        RefreshUI();
    }
    public void RefreshUI()
    {

    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        this.SetWinState(false);
    }
}
