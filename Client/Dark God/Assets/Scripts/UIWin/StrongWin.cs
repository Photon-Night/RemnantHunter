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
    public RectTransform imgFrame;
    #endregion
    public Transform imgGrp;
    private List<Image> imgs = new List<Image>();
    private int currentIndex;
    private PlayerData pd;

    private StrongCfg sd;
    private bool isRegister;
    private Vector3 imgFrameStartPos;
    protected override void InitWin()
    {
        base.InitWin();

        pd = GameRoot.Instance.PlayerData;
        if (!isRegister)
        {
            RegisterClickEvt();
            isRegister = true;
            imgFrameStartPos = imgFrame.localPosition;
        }

        ClickPosItem(0);
        imgFrame.localPosition = imgFrameStartPos;
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
        imgFrame.position = imgs[index].rectTransform.position;
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
                SetActive(img);
            }
            else
            {
                SetActive(img, false);
            }
        }

        int sumAddHp = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 1);
        int sumAddHurt = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 2);
        int sumAddDef = resSvc.GetPropAddValPreLv(currentIndex, currentLv, 3);

        SetText(propHP1, "+" + sumAddHp);
        SetText(propHurt1, "+" + sumAddHurt);
        SetText(propDef1, "+" + sumAddDef);

        int nextLv = currentLv + 1;
        sd = resSvc.GetStrongCfgData(currentIndex, nextLv);
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
            SetText(txtCostCrystal, sd.crystal + "/" + pd.crystal);

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

    public void OnClickStrongBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        if(pd.strong[currentIndex] < 10)
        {
            if(pd.lv < sd.minLv)
            {
                GameRoot.AddTips("等级不足");
            }
            else if(pd.coin < sd.coin)
            {
                GameRoot.AddTips("金币不足");
            }
            else if(pd.crystal < sd.crystal)
            {
                GameRoot.AddTips("ˮ水晶不足");
            }
            else
            {
                GameMsg msg = new GameMsg
                {
                    cmd = (int)CMD.ReqStrong
                };
                ReqStrong rs = new ReqStrong
                {
                    pos = currentIndex
                };
                msg.reqStrong = rs;

                netSvc.SendMessage(msg);
            }          
        }
        else
        {
            GameRoot.AddTips("强化已达上限");
        }
    }

    public void RefreshUI()
    {
        audioSvc.PlayUIAudio(Message.FBItem);
        ClickPosItem(currentIndex);
    }
}
