using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWin : WinRoot
{
    public Animation tipsAnim;
    public Text txtTips;

    protected override void InitWin()
    {
        base.InitWin();

        SetActive(txtTips, false);
    }

    public void SetTips(string tips)
    {
        SetActive(txtTips);
        SetText(txtTips, tips);

        AnimationClip clip = tipsAnim.GetClip("TipEnter");
        tipsAnim.Play();
        //ÑÓÊ±¹Ø±Õ
        StartCoroutine(AnimPlayDone(clip.length, () =>
        {
            SetActive(txtTips, false);
        }));
    }

    private IEnumerator AnimPlayDone(float sec, System.Action cb)
    {
        yield return new WaitForSeconds(sec);
        if(cb != null)
        {
            cb();
        }
    }
}
