using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StrongWin : WinRoot
{

    public Transform imgGrp;
    private List<Image> imgs = new List<Image>();
    private int currentIndex;

    protected override void InitWin()
    {
        base.InitWin();

        RegisterClickEvt();
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

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        SetWinState(false);
    }
}
