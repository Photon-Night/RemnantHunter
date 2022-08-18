using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinRoot : MonoBehaviour
{
    protected ResService resSvc = null;
    protected AudioService audioSvc = null;
    public void SetWinState(bool isActive = true)
    {
        if(this.gameObject.activeSelf != isActive)
        this.gameObject.SetActive(isActive);

        if(isActive)
        {
            InitWin();
        }
        else
        {
            ClearWin();
        }
    }

    protected virtual void InitWin()
    {
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;
    }

    protected virtual void ClearWin()
    {
        resSvc = null;
        audioSvc = null;
    }

    protected void SetActive(GameObject go, bool isActive = true)
    {
        go.SetActive(isActive);
    }

    protected void SetActive(Transform trans, bool state = true)
    {
        trans.gameObject.SetActive(state);
    }

    protected void SetActive(RectTransform rectTrans, bool state = true)
    {
        rectTrans.gameObject.SetActive(state);
    }

    protected void SetActive(Image image, bool state = true)
    {
        image.transform.gameObject.SetActive(state);
    }

    protected void SetActive(Text text, bool state = true)
    {
        text.gameObject.SetActive(state);
    }

    protected void SetText(Text text, string context = "")
    {
        text.text = context;
    }

    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }

    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num.ToString());
    }
}
