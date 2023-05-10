using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWin : WinRoot
{
    public Text txtPrg;
    public Image imgFG;
    public Image imgPoint;

    private float fgWidth;

    protected override void InitWin()
    {
        base.InitWin();
        isTriggerEvent = false;
        fgWidth = imgFG.GetComponent<RectTransform>().sizeDelta.x;

        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0;
        imgPoint.transform.localPosition = new Vector3(-860, 0, 0);
    }

    public void SetProgress(float prg)
    {
        SetText(txtPrg, ((int)prg * 100) + "%");
        imgFG.fillAmount = prg;
        float posX = (fgWidth * prg) - 860;
        imgPoint.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0);
    }

 
}
