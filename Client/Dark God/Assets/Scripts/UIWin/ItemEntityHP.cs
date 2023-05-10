using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHP : UIRoot
{
    #region UI Define
    private bool active = false;
    public bool Active
    {
        get
        {
            return active;
        }
        set
        {
            active = value;
            if(!value)
            {
                SetActive(imgRoot, false);
            }
        }
    }

    public Image imgHpGray;
    public Image imgHpRed;
    public GameObject imgRoot;

    public Animation criticalAni;
    public Text txtCritical;

    public Animation hpAni;
    public Text txtHp;

    public Animation dodgeAni;
    public Text txtDodge;
    #endregion
    private int hp;
    public Transform rootTrans;
    private RectTransform rect;
    private float scaleRate = 1f * Message.ScreenStandardHeight / Screen.height;

    Vector3 screenPos;
    Vector3 uiPos;


    public void Update()
    {
        if (rootTrans == null || !Active) return;

        

        screenPos = Camera.main.WorldToViewportPoint(rootTrans.position);
        if (screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1 && screenPos.z > 0)
        {
            SetActive(imgRoot);
            uiPos = Camera.main.WorldToScreenPoint(rootTrans.position);
            uiPos.z = 0; // 将z坐标设为0，使UI在屏幕上显示在最前面
            rect.anchoredPosition = uiPos * scaleRate;
            UpdateGrayHp();
        }
        else
        {
            // 物体不在相机的可视区域内，隐藏UI
            SetActive(imgRoot, false);
        }

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
        txtCritical.text = "暴击";
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
        txtDodge.text = "闪避";
        dodgeAni.Play();
    }


    [SerializeField]private float currentPrg;
    [SerializeField]private float targetPrg;
    public void SetHpVal(int oldHp, int newHp)
    {
        //Debug.Log(oldHp + " " + newHp + " " + hp);
        currentPrg = oldHp * 1f / hp;
        targetPrg = newHp * 1f / hp;
        imgHpRed.fillAmount = targetPrg;

        Debug.Log($"hurt {oldHp.ToString()} {newHp.ToString()}");
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
