using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionWin : WinRoot
{
    protected override void InitWin()
    {
        base.InitWin();
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        this.SetWinState(false);
    }
}
