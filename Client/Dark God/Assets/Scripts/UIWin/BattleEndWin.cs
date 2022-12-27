using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndWin : WinRoot
{
    public Transform transReward;
    public Button btnClose;
    public Button btnExit;
    public Button btnSure;

    public Text txtTime;
    public Text txtRestHP;
    public Text txtReward;

    private FBEndType endType = FBEndType.None;

    protected override void InitWin()
    {
        base.InitWin();

    }

    public void RefreshUI()
    {
        switch(this.endType)
        {
            case FBEndType.Stop:
                SetActive(btnClose.gameObject, true);
                SetActive(btnExit.gameObject, true);
                SetActive(transReward, false);
                break;
            case FBEndType.Win:
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);
                SetActive(transReward, true);
                break;
            case FBEndType.Lose:
                break;
        }
    }

    public void SetEndType(FBEndType endType)
    {
        this.endType = endType;
    }

    
}
public enum FBEndType
{
    None = 0,
    Win = 1,
    Stop = 2,
    Lose = 3,
}
