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

    private void Start()
    {
        
    }

    public void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    Debug.Log("ssss");
        //    SetCritical();
        //    SetHurt(336);
        //}
        //
        //if (Input.GetKeyUp(KeyCode.A))
        //{
        //    SetDodge();
        //}
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.anchoredPosition = screenPos * scaleRate;
        UpdateGrayHp();
    }
    public void SetItemInfo(int hp)
    {
        this.hp = hp;

    }

    public void InitItemInfo(int hp, Transform trans)
    {
        this.hp = hp;
        rootTrans = trans;
        rect = transform.GetComponent<RectTransform>();
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


    private float currentPrg;
    private float targetPrg;
    public void SetHpVal(int oldHp, int newHp)
    {
        currentPrg = oldHp * 1f / hp;
        targetPrg = newHp * 1f / hp;
        imgHpRed.fillAmount = targetPrg;
    }

    private void UpdateGrayHp()
    {
        UpdateHpBlend();
        imgHpGray.fillAmount = currentPrg;
    }
    private void UpdateHpBlend()
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
