using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StrongWin : WinRoot
{
    #region UIDefine
    public Image imgCurPos;
    public Text txtStartLv;
    public Transform starTransGrp;
    public Text propHP1;
    public Text propHurt1;
    public Text propDef1;
    public Text propHP2;
    public Text propHurt2;
    public Text propDef2;
    public Image propArr1;
    public Image propArr2;
    public Image propArr3;

    public Text txtNeedLv;
    public Text txtCostCoin;
    public Text txtCostCrystal;

    public Text txtCoin;

    public Transform costInfoRoot;
    #endregion
    public Transform imgGrp;
    private List<Image> imgs = new List<Image>();
    private int currentIndex;
    private PlayerData pd;
    protected override void InitWin()
    {
        base.InitWin();

        RegisterClickEvt();
        pd = GameRoot.Instance.PlayerData;
        ClickPosItem(0);
    }
    private void RegisterClickEvt()
    {
        for (int i = 0; i < imgGrp.childCount; i++)
        {
            var img = imgGrp.GetChild(i);
            OnClick(img.gameObject, i, (object args) =>
            {
                audioSvc.PlayUIAudio(Message.UIClickBtn);
                ClickPosItem((int)args);
            });
            imgs.Add(img.GetComponent<Image>());
        }
    }

    private void ClickPosItem(int index)
    {
        currentIndex = index;
        RefreshItem();
        for (int i = 0; i < imgs.Count; i++)
        {
            Transform trans = imgs[i].transform;
            if(currentIndex == i)
            {               
                SetSprite(imgs[i], PathDefine.ItemArrorBG);
                trans.localPosition = new Vector3(8.6f, trans.localPosition.y, trans.localPosition.z);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(309f, 131.3f);

            }

            else
            {
                SetSprite(imgs[i], PathDefine.ItemPlatBG);
                trans.localPosition = new Vector3(-1.8f, trans.localPosition.y, trans.localPosition.z);
                trans.GetComponent<RectTransform>().sizeDelta = new Vector2(276.4f, 119f);
            }
        }
    }

    private void RefreshItem()
    {
        SetText(txtCoin, pd.coin);
        SetText(txtStartLv, pd.strong[currentIndex]);
        switch(currentIndex)
        {
            case 0:
                SetSprite(imgCurPos, PathDefine.ItemTouKui);
                break;
            case 1:
                SetSprite(imgCurPos, PathDefine.ItemBody);
                break;
            case 2:
                SetSprite(imgCurPos, PathDefine.ItemYaobu);
                break;
            case 3:
                SetSprite(imgCurPos, PathDefine.ItemHand);
                break;
            case 4:
                SetSprite(imgCurPos, PathDefine.ItemLeg);
                break;
            case 5:
                SetSprite(imgCurPos, PathDefine.ItemFoot);
                break;
        }

        SetText(txtStartLv, pd.strong[currentIndex]);
        var currentLv = pd.strong[currentIndex];
        for(int i = 0; i < starTransGrp.childCount; i++)
        {
            var img = starTransGrp.GetChild(i).GetComponent<Image>();
            if(i < currentLv)
            {            
                SetSprite(img, PathDefine.SpStar2);
            }
            else
            {
                SetSprite(img, PathDefine.SpStar1);
            }
        }

        int sumAddHp = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 1);
        int sumAddHurt = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 2);
        int sumAddDef = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 3);

        SetText(propHP1, "+" + sumAddHp);
        SetText(propHurt1, "+" + sumAddHurt);
        SetText(propDef1, "+" + sumAddDef);

        int nextLv = currentLv + 1;
        StrongCfg sd = resSvc.GetStrongCfgData(currentIndex, nextLv);
        if (sd != null)
        {
            SetActive(costInfoRoot);

            SetActive(propHP2);
            SetActive(propHurt2);
            SetActive(propDef2);
            SetActive(propArr1);
            SetActive(propArr2);
            SetActive(propArr3);

            SetText(propHP2, "+" + sd.addHp);
            SetText(propHurt2, "+" + sd.addHurt);
            SetText(propDef2, "+" + sd.addDef);

            SetText(txtNeedLv, sd.minLv);
            SetText(txtCostCoin, sd.coin);
            SetText(txtCostCrystal, sd.crystal + "/" + );

        }
        else
        {
            SetActive(costInfoRoot, false);
            SetActive(propHP2, false);
            SetActive(propHurt2, false);
            SetActive(propDef2, false);
            SetActive(propArr1, false);
            SetActive(propArr2, false);
            SetActive(propArr3, false);
        }
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }
}
