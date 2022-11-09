using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : MonoBehaviour
{
    #region UI Define
    public Image imgHpGray;
    public Image imgHpRed;

    public Animation criticalAni;
    public Text txtCritical;

    public Animation hpAni;
    public Text txtHp;

    public Animation dodgeAni;
    public Text txtDodge;
    #endregion
    private int hp;
    private Transform rootTrans;
    private RectTransform rect;
    private float scaleRate = 1f * Message.ScreenStandardHeight / Screen.height;

    public void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.anchoredPosition = screenPos * scaleRate;
    }
    public void SetItemInfo(int hp)
    {
        this.hp = hp;

    }

    public void InitItemInfo(int hp, Transform trans)
    {
        this.hp = hp;
        rootTrans = trans;
        imgHpGray.fillAmount = 1;
        imgHpRed.fillAmount = 1;
    }

    public void SetCritical()
    {
        criticalAni.Stop();
        txtCritical.text = "±©»÷";
        criticalAni.Play();
    }

    public void SetHurt(int hurt)
    {
        hpAni.Stop();
        txtHp.text = "-" + hurt;
        hpAni.Play();
    }

    public void SetDodge()
    {
        dodgeAni.Stop();
        txtDodge.text = "ÉÁ±Ü";
        dodgeAni.Play();
    }

    public void SetHpVal(int hp, int value)
    {

    }
}
