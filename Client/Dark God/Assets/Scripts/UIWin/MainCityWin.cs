using Game.Bag;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainCityWin : WinRoot
{

    public Image imgDirBg;
    public Image imgDirPoint;
    public Image imgTouch;


    public Text txtFight;
    public Text txtPower;
    public Image imgPowerPrg;
    public Text txtLevel;
    public Text txtName;
    public Text txtExpPrg;
    public Text txtNPCName;

    public Button btnGuide;
    public Button btnChat;
    public Button btnTalk;

    public Transform expPrgTrans;
    public Animation menuRootAnim;

    public RectTransform imgFrame;
    public Vector2[] menuFramePos;


    public bool MenuRootState { get; private set; }

    //private float pointDis;
    //private Vector2 startPos;
    //private Vector2 defaultPos;
    //private GuideCfg currentTaskData;
    private int menuIndex = 0;
    public int MenuIndex
    {
        get
        {
            return menuIndex;
        }
        private set
        {
            if(menuIndex != value)
            {
                menuIndex = value;
                //ChangeMenuIndex(0);
            }
        }
    }
    protected override void InitWin()
    {
        base.InitWin();
        isTriggerEvent = false;
        //pointDis = Screen.height * 1f / Message.ScreenStandardHeight * Message.ScreenOPDis;
        //defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        

        RefreshUI();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, $"{pd.power}/{PECommon.GetPowerLimit(pd.lv)}");
        imgPowerPrg.fillAmount = pd.power * 1f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

        
        int expPrgVal = (int)(pd.exp * 100f / PECommon.GetExpUpValByLv(pd.lv));
        SetText(txtExpPrg, expPrgVal + "%");

        
        int index = expPrgVal / 10;

        GridLayoutGroup gird = expPrgTrans.GetComponent<GridLayoutGroup>();

        float globalRate = 1f * Message.ScreenStandardHeight / Screen.height;
        float screenWidth = Screen.width * globalRate;
        float Width = (screenWidth - 223) / 10;

        gird.cellSize = new Vector2(Width, 19);

        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image img = expPrgTrans.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                img.fillAmount = 1;
            }
            else if (i == index)
            {
                img.fillAmount = expPrgVal % 10 * 1f / 10;
            }
            else
                img.fillAmount = 0;
        }
    }

    public void ChangeMenuIndex(int change)
    {
        if (!MenuRootState)
            return;

        MenuIndex += change;
        if(MenuIndex < 0)
        {
            MenuIndex = menuFramePos.Length - 1;
        }
        else if(MenuIndex >= menuFramePos.Length)
        {
            MenuIndex = 0;
        }

        imgFrame.anchoredPosition = menuFramePos[MenuIndex];
        audioSvc.PlayUIAudio(Message.UIClickBtn);
    }
   

    public void SetBtnTalkActive(bool active, string npcName = null)
    {
        if(active)
        {
            SetText(txtNPCName, npcName);
        }
        SetActive(btnTalk.gameObject, active);
    }

    public void OnClickTalkBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.StartTalk();
        SetActive(btnTalk.gameObject, false);
    }
    public void OnClickMenuRoot()
    {
        MenuRootState = !MenuRootState;
        AnimationClip clip;
        if(MenuRootState)
        {
            MenuIndex = 0;
            clip = menuRootAnim.GetClip("OpenMenu");
        }
        else
        {
            clip = menuRootAnim.GetClip("CloseMenu");
        }
        audioSvc.PlayUIAudio(Message.UIExtenBtn);
        menuRootAnim.Play(clip.name);
    }

    public void OnClickHeadBtn()
    {
        audioSvc.PlayUIAudio(Message.UIOpenPage);
        MainCitySystem.Instance.OpenInfoWin();
    }

    //public void RegisterTouchEvts()
    //{
    //    OnClickDown(imgTouch.gameObject, (PointerEventData evt) => 
    //    {
    //        startPos = evt.position;
    //        SetActive(imgDirPoint);
    //        imgDirBg.transform.position = evt.position;
    //    });
    //
    //    OnClickUp(imgTouch.gameObject, (PointerEventData evt) => 
    //    {
    //        imgDirBg.transform.position = defaultPos;
    //        SetActive(imgDirPoint, false);
    //        imgDirPoint.transform.localPosition = Vector2.zero;
    //        MainCitySystem.Instance.SetMoveDir(Vector2.zero);
    //
    //    });
    //
    //    OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
    //    {
    //        Vector2 _dir = evt.position - startPos;
    //        float len = _dir.magnitude;
    //        if(len > pointDis)
    //        {
    //            Vector2 clampDir = Vector2.ClampMagnitude(_dir, Message.ScreenOPDis);
    //            imgDirPoint.transform.position = startPos + clampDir;
    //        }
    //        else
    //        {
    //            imgDirPoint.transform.position = evt.position;
    //        }
    //        MainCitySystem.Instance.SetMoveDir(_dir.normalized);
    //    });
    //}

    //public void OnCLickGuideBtn()
    //{
    //    audioSvc.PlayUIAudio(Message.UIClickBtn);
    //
    //    if(currentTaskData != null)
    //    {
    //        //MainCitySystem.Instance.RunTask(currentTaskData);
    //    }
    //    else
    //    {
    //        GameRoot.AddTips("无更多任务");
    //    }
    //}

    public void OnClickStrongBtn()
    {
        MenuIndex = (int)MenuFunction.Strong - 1;
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenStrongWin();
    }

    public void OnClickChatBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.chatWin.SetWinState();
    }

    public void OnClickBagBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenBagWin();
    }

    public void OnClickMakeBtn()
    {
        MenuIndex = (int)MenuFunction.Make - 1;
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenBuyWin(Message.BuyCoin);
    }

    public void OnClickBuyPowerBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenBuyWin(Message.BuyPower);
    }

    public void OnClickMissionBtn()
    {
        MenuIndex = (int)MenuFunction.Mission - 1;
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenMissionWin();
    }

    public void OnClickTaskBtn()
    {
        MenuIndex = (int)MenuFunction.Task - 1;
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.OpenTaskWin();
    }
}
