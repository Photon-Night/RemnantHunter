using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIRoot : MonoBehaviour
{
    protected ResService resSvc = null;
    protected AudioService audioSvc = null;
    protected NetService netSvc = null;
    protected TimerService timerSvc = null;

    protected void InitUI()
    {
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;
        netSvc = NetService.Instance;
        timerSvc = TimerService.Instance;
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

    protected void SetText(Text text, int num = 0)
    {
        text.text = num.ToString();
    }
    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }

    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num.ToString());
    }

    protected T GetOrAddComponect<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }

    protected void OnClick(GameObject go, object args, Action<object> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }

    protected void OnClickDown(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickDown = cb;

    }

    protected void OnClickUp(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onClickUp = cb;

    }

    protected void OnDrag(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onDrag = cb;

    }

    protected void OnEnter(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onEnter = cb;
    }

    protected void OnExit(GameObject go, Action<PointerEventData> cb)
    {
        PEListener listener = GetOrAddComponect<PEListener>(go);
        listener.onExit = cb;
    }

    protected void SetSprite(Image img, string path)
    {
        Sprite sp = resSvc.LoadSprite(path);
        if (sp != null)
        {
            img.sprite = sp;
        }
    }

    protected Transform GetTransform(Transform parent, string name)
    {
        if (parent != null)
        {
            return parent.Find(name);
        }
        else
        {
            return transform.Find(name);
        }
    }
}
