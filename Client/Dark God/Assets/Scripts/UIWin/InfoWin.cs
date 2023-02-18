using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoWin : WinRoot
{
    #region UIDefine
    public Text txtInfo;
    public Text txtExp;
    public Image imgExpPrg;
    public Text txtPower;
    public Image imgPowerPrg;

    public Text txtJob;
    public Text txtFight;
    public Text txtHP;
    public Text txtHurt;
    public Text txtDef;

    public RawImage imgChar;

    public Transform detailTrans;

    public Text dtxHP;
    public Text dtxAD;
    public Text dtxDodge;
    public Text dtxAP;
    public Text dtxADDef;
    public Text dtxAPDef;
    public Text dtxPierce;
    public Text dtxCritical;

    #endregion


    private Vector2 startPos; 
    protected override void InitWin()
    {
        base.InitWin();
        RegTouchEvts();
        RefreshUI();
    }

    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtInfo, pd.name + " LV." + pd.lv);
        SetText(txtExp, pd.exp + "/" + PECommon.GetExpUpValByLv(pd.lv));
        imgExpPrg.fillAmount = pd.exp * 1f / PECommon.GetExpUpValByLv(pd.lv);
        SetText(txtPower, pd.power + "/" + PECommon.GetPowerLimit(pd.lv));
        imgPowerPrg.fillAmount = pd.power * 1f / PECommon.GetPowerLimit(pd.lv);

        SetText(txtJob, "职业        " + "暗夜刺客");
        SetText(txtFight, "战力        " + PECommon.GetFightByProps(pd));
        SetText(txtHP, "血量        " + pd.hp);
        SetText(txtHurt, "伤害        " + (pd.ad + pd.ap));
        SetText(txtDef, "防御        " + (pd.apdef + pd.apdef));

        SetText(dtxHP, pd.hp);
        SetText(dtxAD, pd.ad);
        SetText(dtxAP, pd.ap);
        SetText(dtxADDef, pd.addef);
        SetText(dtxAPDef, pd.apdef);
        SetText(dtxPierce, pd.pierce + "%");
        SetText(dtxDodge, pd.dodge + "%");
        SetText(dtxCritical, pd.critical + "%");

    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        MainCitySystem.Instance.CloseInfoWin();
    }

    private void RegTouchEvts()
    {
        OnClickDown(imgChar.gameObject, (PointerEventData evt) =>
        {
            startPos = evt.position;
            MainCitySystem.Instance.SetStartRoate();
        });

        OnDrag(imgChar.gameObject, (PointerEventData evt) =>
        {
            float roate = (startPos.x - evt.position.x) * 0.4f;
            MainCitySystem.Instance.SetPlayerRoate(roate);
        });
    }

    public void OnClickOpenDetail()
    {
        audioSvc.PlayUIAudio(Message.UIOpenPage);
        detailTrans.gameObject.SetActive(true);
    }

    public void OnClickCloseDetail()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        detailTrans.gameObject.SetActive(false);
    }
}
