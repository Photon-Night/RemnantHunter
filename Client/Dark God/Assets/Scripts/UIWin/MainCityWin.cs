using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public Button btnGuide;

    public Transform expPrgTrans;
    public Animation menuRootAnim;

    private bool menuRootState = true;

    private float pointDis;
    private Vector2 startPos;
    private Vector2 defaultPos;

    private GuideCfg currentTaskData;
    protected override void InitWin()
    {
        base.InitWin();
        pointDis = Screen.height * 1f / Message.ScreenStandardHeight * Message.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        RefreshUI();
        RegisterTouchEvts();
    }

    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtFight, PECommon.GetFightByProps(pd));
        SetText(txtPower, "体力" + pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1f / PECommon.GetPowerLimit(pd.lv);
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

        
        int expPrgVal = (int)(pd.exp * 100f / PECommon.GetExpUpValByLv(pd.lv));
        SetText(txtExpPrg, expPrgVal + "%");

        //设置经验条显示。计算经验格排列自适应，计算经验显示
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

        currentTaskData = resSvc.GetGuideCfgData(pd.guideID);
        if(currentTaskData != null)
        {
            SetGuideBtnIcon(currentTaskData.npcID);
        }
        else
        {
            SetGuideBtnIcon(-1);
        }
    }

    private void SetGuideBtnIcon(int npcID)
    {
        string spPath = "";
        Image img = btnGuide.GetComponent<Image>();
        switch (npcID)
        {
            case Message.NPCArtisan:
                spPath = PathDefine.ArtisanHead;
                break;
            case Message.NPCGeneral:
                spPath = PathDefine.GeneralHead;
                break;
            case Message.NPCTrader:
                spPath = PathDefine.TraderHead;
                break;
            case Message.NPCWiseMan:
                spPath = PathDefine.WiseManHead;
                break;
        }
        SetSprite(img, spPath);

    }

    public void OnClickMenuRoot()
    {
        menuRootState = !menuRootState;
        AnimationClip clip;
        if(menuRootState)
        {
            clip = menuRootAnim.GetClip("MenuOpen");
        }
        else
        {
            clip = menuRootAnim.GetClip("MenuClose");
        }
        audioSvc.PlayUIAudio(Message.UIExtenBtn);
        menuRootAnim.Play(clip.name);
    }

    public void OnClickHeadBtn()
    {
        audioSvc.PlayUIAudio(Message.UIOpenPage);
        MainCitySystem.Instance.OpenInfoWin();
    }

    public void RegisterTouchEvts()
    {
        OnClickDown(imgTouch.gameObject, (PointerEventData evt) => 
        {
            startPos = evt.position;
            SetActive(imgDirPoint);
            imgDirBg.transform.position = evt.position;
        });

        OnClickUp(imgTouch.gameObject, (PointerEventData evt) => 
        {
            imgDirBg.transform.position = defaultPos;
            SetActive(imgDirPoint, false);
            imgDirPoint.transform.localPosition = Vector2.zero;
            MainCitySystem.Instance.SetMoveDir(Vector2.zero);

        });

        OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
        {
            Vector2 _dir = evt.position - startPos;
            float len = _dir.magnitude;
            if(len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(_dir, Message.ScreenOPDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }
            MainCitySystem.Instance.SetMoveDir(_dir.normalized);
        });
    }

    public void OnCLickGuideBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);

        if(currentTaskData != null)
        {
            MainCitySystem.Instance.RunTask(currentTaskData);
        }
        else
        {
            GameRoot.AddTips("无更多任务");
        }
    }
}
