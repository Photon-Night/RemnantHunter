using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOutWin : WinRoot
{
    public void OnClickCheckOutBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        

        SetWinState(false);
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }

}
