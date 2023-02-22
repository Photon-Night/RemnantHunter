using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WinRoot : UIRoot
{
    public void SetWinState(bool isActive = true)
    {
        if(this.gameObject.activeSelf != isActive)
        this.gameObject.SetActive(isActive);

        if(isActive)
        {
            InitWin();
        }
        else
        {
            ClearWin();
        }
    }

    protected virtual void InitWin()
    {
        base.InitUI();
    }

    protected virtual void ClearWin()
    {
        resSvc = null;
        audioSvc = null;
        netSvc = null;
    }

    public bool GetWinState()
    {
        return gameObject.activeSelf;
    }
}
