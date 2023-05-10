using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Event;

public abstract class WinRoot : UIRoot
{
    protected bool isTriggerEvent = true;
    public void SetWinState(bool isActive = true)
    {
        if(this.gameObject.activeSelf != isActive)
        this.gameObject.SetActive(isActive);


        if (isActive)
        {
            InitWin();
        }
        else
        {
            ClearWin();
        }

        if (isTriggerEvent)
        {
            GameEventManager.TriggerEvent<bool>(EventNode.Event_OnSetUIWinState, isActive);

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
