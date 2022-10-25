using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleWin : WinRoot
{
    public Image imgDirPoint;
    public Image imgDirBg;
    public Image imgTouch;

    public Text txtLevel;
    public Text txtExpPrg;

    public Transform expPrgTrans;

    private float pointDis;
    private Vector2 startPos;
    private Vector2 defaultPos;

    private Vector2 currentDir;


    protected override void InitWin()
    {
        base.InitWin();
        pointDis = Screen.height * 1f / Message.ScreenStandardHeight * Message.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        RegisterTouchEvts();
        RefreshUI();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            BattleSystem.Instance.bm.ReqReleaseSkill(1);
        }


    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtLevel, pd.lv);


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
            currentDir = Vector2.zero;
            BattleSystem.Instance.SetMoveDir(currentDir);

        });

        OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
        {
            Vector2 _dir = evt.position - startPos;
            float len = _dir.magnitude;

            if (len > pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(_dir, Message.ScreenOPDis);
                imgDirPoint.transform.position = startPos + clampDir;
            }
            else
            {
                imgDirPoint.transform.position = evt.position;
            }

            currentDir = _dir.normalized;

            BattleSystem.Instance.SetMoveDir(currentDir);
        });
    }

    public void OnlClickSkillBtn(int index)
    {
        BattleSystem.Instance.ReqReleaseSkill(index);
    }

    public void OnClickResetSkillDataBtn()
    {
        ResService.Instance.ReSetSkillCfgData();
    }

    public Vector2 GetCurrentDir()
    {
        return currentDir;
    }
}
