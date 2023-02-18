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

    public Animation aniWin;

    private FBEndType endType = FBEndType.None;

    private int fid;
    private int costtime;
    private int restHP;

    protected override void InitWin()
    {
        base.InitWin();
        RefreshUI();
    }

    public void RefreshUI()
    {
        switch (this.endType)
        {
            case FBEndType.Stop:
                SetActive(btnClose.gameObject, true);
                SetActive(btnExit.gameObject, true);
                SetActive(transReward, false);
                break;
            case FBEndType.Win:
                SetActive(btnExit.gameObject, false);
                SetActive(btnClose.gameObject, false);
                SetActive(transReward, false);

                MapCfg data = resSvc.GetMapCfgData(fid);
                int min = costtime / 60;
                int sec = costtime % 60;

                SetText(txtTime, min + ":" + sec);
                SetText(txtRestHP, restHP);
                SetText(txtReward, "金币: " + data.coin
                    + " " + "经验: " + data.exp
                    + " " + "钻石: " + data.crystal);

                timerSvc.AddTimeTask((int tid) => {
                    SetActive(transReward);
                    aniWin.Play();
                    timerSvc.AddTimeTask((int tid1) => {
                        audioSvc.PlayUIAudio(Message.FBItem);
                        timerSvc.AddTimeTask((int tid2) => {
                            audioSvc.PlayUIAudio(Message.FBItem);
                            timerSvc.AddTimeTask((int tid3) => {
                                audioSvc.PlayUIAudio(Message.FBItem);
                                timerSvc.AddTimeTask((int tid5) => {
                                    audioSvc.PlayUIAudio(Message.FBWin);
                                }, 300);
                            }, 270);
                        }, 270);
                    }, 325);
                }, 1000);

                break;
            case FBEndType.Lose:
                SetActive(btnClose.gameObject, false);
                SetActive(btnExit.gameObject);
                SetActive(transReward, false);
                audioSvc.PlayUIAudio(Message.FBLose);
                break;
        }
    }

    public void SetEndType(FBEndType endType)
    {
        this.endType = endType;
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        timerSvc.SetTimeScale(1f);
        SetWinState(false);
    }

    public void OnClickExitBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        BattleSystem.Instance.DestroyBattle();
        MainCitySystem.Instance.EnterMainCity();
    }

    public void OnClickSureBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.EnterMainCity();
        BattleSystem.Instance.DestroyBattle();
        MissionSystem.Instance.OpenMissionWin();
    }


    public void SetBattleEndData(int fid, int costtime, int restHP)
    {
        this.fid = fid;
        this.costtime = costtime;
        this.restHP = restHP;
    }
}
public enum FBEndType
{
    None = 0,
    Win = 1,
    Stop = 2,
    Lose = 3,
}
