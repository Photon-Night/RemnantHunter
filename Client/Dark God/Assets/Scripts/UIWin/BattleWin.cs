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

    public Image[] imgSkillPoint;

    //public Text txtSkill1CD;
    //public Image imgSkill1CD;
    //private bool isSkill1CD;
    //private int skill1CDNum;
    //private float skill1FillCount;
    //private float skill1NumCount;
    //private float skill1CDTime;
    //
    //public Text txtSkill2CD;
    //public Image imgSkill2CD;
    //private bool isSkill2CD;
    //private int skill2CDNum;
    //private float skill2CDTime;
    //private float skill2FillCount;
    //private float skill2NumCount;
    //
    //public Text txtSkill3CD;
    //public Image imgSkill3CD;
    //private bool isSkill3CD;
    //private int skill3CDNum;
    //private float skill3CDTime;
    //private float skill3FillCount;
    //private float skill3NumCount;

    public Transform expPrgTrans;

    public Image imgHP;
    public Text txtHP;

    public Image imgPower;
    public Text txtPower;

    public Transform bossHPBar;
    public Image imgYellow;
    public Image imgRed;
    public Text txtBossHP;

    private float pointDis;
    private Vector2 startPos;
    private Vector2 defaultPos;

    private Vector2 currentDir;
    private int hpNum;
    private float powerNum;

    private int currentSkillPointIndex;
    private bool canRecover;
    private float currentFill = 0f;

    [SerializeField]private float currentPrg = 0f;
    [SerializeField]private float targetPrg = 0f;

    protected override void InitWin()
    {
        base.InitWin();
        isTriggerEvent = false;

        imgHP.fillAmount = 1;
        hpNum = GameRoot.Instance.PlayerData.hp;
        powerNum = GameRoot.Instance.PlayerData.lv - 1 + 100;
        SetText(txtHP, hpNum + "/" + hpNum);
        pointDis = Screen.height * 1f / Message.ScreenStandardHeight * Message.ScreenOPDis;
        defaultPos = imgDirBg.transform.position;
        SetActive(imgDirPoint, false);
        currentSkillPointIndex = Message.SkillPointCount - 1;
        //RegisterTouchEvts();

        //skill1CDTime = resSvc.GetSkillData(101).cdTime/1000;
        //skill2CDTime = resSvc.GetSkillData(102).cdTime / 1000;
        //skill3CDTime = resSvc.GetSkillData(103).cdTime / 1000;
        //
        //isSkill1CD = false;
        //isSkill2CD = false;
        //isSkill3CD = false;

        RefreshUI();
    }

    public void Update()
    {      
        //SkillCD();

        if(bossHPBar.gameObject.activeSelf)
        {
            UpdateHPBlend();
            imgYellow.fillAmount = currentPrg;
        }

        
    }

    private void FixedUpdate()
    {
        if (canRecover && imgSkillPoint[currentSkillPointIndex].fillAmount < 1)
        {
            imgSkillPoint[currentSkillPointIndex].fillAmount += Time.fixedDeltaTime / Message.SkillPointRecoverTime;
            currentFill = imgSkillPoint[currentSkillPointIndex].fillAmount;         
        }
    }
    //public void SkillCD()
    //{
    //    if (isSkill1CD)
    //    {
    //        skill1FillCount += Time.deltaTime;
    //        if (skill1FillCount >= skill1CDTime)
    //        {
    //            skill1FillCount = 0;
    //            SetActive(imgSkill1CD, false);
    //            isSkill1CD = false;
    //        }
    //        else
    //        {
    //            imgSkill1CD.fillAmount = 1 - skill1FillCount / skill1CDTime;
    //        }
    //
    //        skill1NumCount += Time.deltaTime;
    //        if (skill1NumCount >= 1)
    //        {
    //            skill1NumCount -= 1;
    //            skill1CDNum -= 1;
    //            SetText(txtSkill1CD, skill1CDNum);
    //        }
    //    }
    //    if (isSkill2CD)
    //    {
    //        skill2FillCount += Time.deltaTime;
    //        if (skill2FillCount >= skill2CDTime)
    //        {
    //            skill2FillCount = 0;
    //            SetActive(imgSkill2CD, false);
    //            isSkill2CD = false;
    //        }
    //        else
    //        {
    //            imgSkill2CD.fillAmount = 1 - skill2FillCount / skill2CDTime;
    //        }
    //
    //        skill2NumCount += Time.deltaTime;
    //        if (skill2NumCount >= 1)
    //        {
    //            skill2NumCount -= 1;
    //            skill2CDNum -= 1;
    //            SetText(txtSkill2CD, skill2CDNum);
    //        }
    //    }
    //    if (isSkill3CD)
    //    {
    //        skill3FillCount += Time.deltaTime;
    //        if (skill3FillCount >= skill3CDTime)
    //        {
    //            skill3FillCount = 0;
    //            SetActive(imgSkill3CD, false);
    //            isSkill3CD = false;
    //        }
    //        else
    //        {
    //            imgSkill3CD.fillAmount = 1 - skill3FillCount / skill3CDTime;
    //        }
    //
    //        skill3NumCount += Time.deltaTime;
    //        if (skill3NumCount >= 1)
    //        {
    //            skill3NumCount -= 1;
    //            skill3CDNum -= 1;
    //            SetText(txtSkill3CD, skill3CDNum);
    //        }
    //    }
    //}


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
    //        currentDir = Vector2.zero;
    //        BattleSystem.Instance.SetMoveDir(currentDir);
    //
    //    });
    //
    //    OnDrag(imgTouch.gameObject, (PointerEventData evt) =>
    //    {
    //        Vector2 _dir = evt.position - startPos;
    //        float len = _dir.magnitude;
    //
    //        if (len > pointDis)
    //        {
    //            Vector2 clampDir = Vector2.ClampMagnitude(_dir, Message.ScreenOPDis);
    //            imgDirPoint.transform.position = startPos + clampDir;
    //        }
    //        else
    //        {
    //            imgDirPoint.transform.position = evt.position;
    //        }
    //
    //        currentDir = _dir.normalized;
    //
    //        BattleSystem.Instance.SetMoveDir(currentDir);
    //    });
    //}

    public void OnClickResetSkillDataBtn()
    {
        ResService.Instance.ReSetSkillCfgData();
    }

    public void OnClickHeadBtn()
    {
        BattleSystem.Instance.SetBattleEndWinState(FBEndType.Stop);
        timerSvc.SetTimeScale(0);
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

    public void SetPowerUI(float power)
    {
        SetText(txtPower, $"{(int)power}/{powerNum}");
        imgPower.fillAmount = power / powerNum;
    }

    public void SetSkillPointUI(int count)
    {
        if (count == Message.SkillPointCount)
        {
            canRecover = false;
        }
        else
        {
            canRecover = true;
        }

        for (int i = 0; i < imgSkillPoint.Length; i++)
        {
            if (i < count)
            {
                imgSkillPoint[i].fillAmount = 1;
            }
            else if (i == count)
            {
                currentSkillPointIndex = count;
                imgSkillPoint[i].fillAmount = currentFill >= 1 ? 0 : currentFill;
            }
            else
            {
                imgSkillPoint[i].fillAmount = 0;
            }

        }

    }
    public void SetMonsterHPState(bool state, float prg = 1)
    {
        SetActive(bossHPBar, state);
        imgRed.fillAmount = prg;
        imgYellow.fillAmount = prg;
    }

    public void SetBossHPVal(int oldVal, int newVal, int sumVal)
    {
        currentPrg = oldVal * 1f / sumVal;
        targetPrg = newVal * 1f / sumVal;
        imgRed.fillAmount = targetPrg;
        SetText(txtBossHP, newVal + "/" + sumVal);
    }

    private void UpdateHPBlend()
    {
        if (Mathf.Abs(currentPrg - targetPrg) < Message.AccelerHpSpeed * Time.deltaTime)
        {
            currentPrg = targetPrg;
        }
        else if (currentPrg > targetPrg)
        {
            currentPrg -= Message.AccelerHpSpeed * Time.deltaTime;
        }
        else
        {
            currentPrg += Message.AccelerHpSpeed * Time.deltaTime;
        }
    }

    
}
