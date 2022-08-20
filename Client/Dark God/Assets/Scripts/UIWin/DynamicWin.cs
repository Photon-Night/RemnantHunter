using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWin : WinRoot
{
    public Animation tipsAnim;
    public Text txtTips;

    private bool isTipsShow;

    private Queue<string> tipsQue = new Queue<string>();
    protected override void InitWin()
    {
        base.InitWin();

        SetActive(txtTips, false);
    }

    public void AddTips(string tips)
    {
        lock(tipsQue)
        {
            tipsQue.Enqueue(tips);
        }
    }

    private void Update()
    {
         if(tipsQue.Count > 0 && !isTipsShow)
        {
                
            lock(tipsQue)
            {
                string tip = tipsQue.Dequeue();
                isTipsShow = true;
                SetTips(tip);
            }
        }
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
            isTipsShow = false;
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
