using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StrongWin : WinRoot
{

    public Transform imgGrp;

    protected override void InitWin()
    {
        base.InitWin();

        RegisterClickEvt();
    }
    private void RegisterClickEvt()
    {
        for (int i = 0; i < imgGrp.childCount; i++)
        {
            var img = imgGrp.GetChild(i);
            OnClick(img.gameObject, i, (object args) =>
            {
                audioSvc.PlayUIAudio(Message.UIClickBtn);
                ClickPosItem((int)args);
            });
        }
    }

    private void ClickPosItem(int index)
    {
        PECommon.Log(index.ToString());
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }
}
