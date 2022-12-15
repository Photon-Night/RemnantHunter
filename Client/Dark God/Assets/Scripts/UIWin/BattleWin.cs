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

    public Text txtSkill1CD;
    public Image imgSkill1CD;
    private bool isSkill1CD;
    private int skill1CDNum;
    private float skill1FillCount;
    private float skill1NumCount;
    private float skill1CDTime;

    public Text txtSkill2CD;
    public Image imgSkill2CD;
    private bool isSkill2CD;
    private int skill2CDNum;
    private float skill2CDTime;
    private float skill2FillCount;
    private float skill2NumCount;

    public Text txtSkill3CD;
    public Image imgSkill3CD;
    private bool isSkill3CD;
    private int skill3CDNum;
    private float skill3CDTime;
    private float skill3FillCount;
    private float skill3NumCount;

    public Transform expPrgTrans;

    public Image imgHP;
    public Text txtHP;


    private float pointDis;
    private Vector2 startPos;
    private Vector2 defaultPos;

    private Vector2 currentDir;
    private int hpNum;

    protected override void InitWin()
    {
        base.InitWin();
        imgHP.fillAmount = 1;
        hpNum = GameRoot.Instance.PlayerData.hp;
        SetText(txtHP, hpNum + "/" + hpNum);
        pointDis = Screen.height * 1f / Message.ScreenStandardHeight * Message.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        RegisterTouchEvts();

        skill1CDTime = resSvc.GetSkillData(101).cdTime/1000;
        skill2CDTime = resSvc.GetSkillData(102).cdTime / 1000;
        skill3CDTime = resSvc.GetSkillData(103).cdTime / 1000;


        RefreshUI();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            BattleSystem.Instance.bm.ReqReleaseSkill(1);
        }

        SkillCD();
    }

    public void SkillCD()
    {
        if (isSkill1CD)
        {
            skill1FillCount += Time.deltaTime;
            if (skill1FillCount >= skill1CDTime)
            {
                skill1FillCount = 0;
                SetActive(imgSkill1CD, false);
                isSkill1CD = false;
            }
            else
            {
                imgSkill1CD.fillAmount = 1 - skill1FillCount / skill1CDTime;
            }

            skill1NumCount += Time.deltaTime;
            if (skill1NumCount >= 1)
            {
                skill1NumCount -= 1;
                skill1CDNum -= 1;
                SetText(txtSkill1CD, skill1CDNum);
            }
        }
        if (isSkill2CD)
        {
            skill2FillCount += Time.deltaTime;
            if (skill2FillCount >= skill2CDTime)
            {
                skill2FillCount = 0;
                SetActive(imgSkill2CD, false);
                isSkill2CD = false;
            }
            else
            {
                imgSkill2CD.fillAmount = 1 - skill2FillCount / skill2CDTime;
            }

            skill2NumCount += Time.deltaTime;
            if (skill2NumCount >= 1)
            {
                skill2NumCount -= 1;
                skill2CDNum -= 1;
                SetText(txtSkill2CD, skill2CDNum);
            }
        }
        if (isSkill3CD)
        {
            skill3FillCount += Time.deltaTime;
            if (skill3FillCount >= skill3CDTime)
            {
                skill3FillCount = 0;
                SetActive(imgSkill3CD, false);
                isSkill3CD = false;
            }
            else
            {
                imgSkill3CD.fillAmount = 1 - skill3FillCount / skill3CDTime;
            }

            skill3NumCount += Time.deltaTime;
            if (skill3NumCount >= 1)
            {
                skill3NumCount -= 1;
                skill3CDNum -= 1;
                SetText(txtSkill3CD, skill3CDNum);
            }
        }
    }
    

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtLevel, pd.lv);


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

    public void OnlClickSkill1Btn()
    {
        if (!isSkill1CD && !BattleSystem.Instance.isPlayerAttack())
        {
            isSkill1CD = true;
            SetActive(imgSkill1CD);
            imgSkill1CD.fillAmount = 1;
            skill1CDNum = (int)skill1CDTime;
            SetText(txtSkill1CD, skill1CDNum);
            BattleSystem.Instance.ReqReleaseSkill(1);
        }
    }

    public void OnClickSkill2Btn()
    {
        if (!isSkill2CD && !BattleSystem.Instance.isPlayerAttack())
        {
            isSkill2CD = true;
            SetActive(imgSkill2CD);
            imgSkill2CD.fillAmount = 1;
            skill2CDNum = (int)skill2CDTime;
            SetText(txtSkill2CD, skill2CDNum);
            BattleSystem.Instance.ReqReleaseSkill(2);
        }
    }

    public void OnClickSkill3Btn()
    {
        if (!isSkill3CD && !BattleSystem.Instance.isPlayerAttack())
        {
            isSkill3CD = true;
            SetActive(imgSkill3CD);
            imgSkill3CD.fillAmount = 1;
            skill3CDNum = (int)skill3CDTime;
            SetText(txtSkill3CD, skill3CDNum);
            BattleSystem.Instance.ReqReleaseSkill(3);
        }
    }

    public void OnClickNormalAtk()
    {
        BattleSystem.Instance.ReqReleaseSkill(0);
    }

    public void OnClickResetSkillDataBtn()
    {
        ResService.Instance.ReSetSkillCfgData();
    }

    public Vector2 GetCurrentDir()
    {
        return currentDir;
    }

    public void SetHPUI(int hp)
    {
        SetText(txtHP, hp + "/" + hpNum);
        imgHP.fillAmount = (hp * 1f) / (hpNum * 1f);
    }
}
