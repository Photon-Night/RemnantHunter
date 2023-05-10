using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTaskStatusItem : UIRoot
{
    private Transform rootTrans;
    private RectTransform rect;
    private NpcTaskStatus currentStatus;
    public Image[] imgStatus;
    private int currentIndex = 0;
    private float scaleRate = 1f * Message.ScreenStandardHeight / Screen.height;
    Vector3 screenPos;
    Vector3 uiPos;
    private void Update()
    {
        if (rootTrans == null) return;

        screenPos = Camera.main.WorldToViewportPoint(rootTrans.position);
        if (screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1 && screenPos.z > 0)
        {
            // 物体在相机的可视区域内，进行坐标转换
            uiPos = Camera.main.WorldToScreenPoint(rootTrans.position);
            uiPos.z = 0; // 将z坐标设为0，使UI在屏幕上显示在最前面
            rect.anchoredPosition = uiPos * scaleRate;
            if(currentStatus != NpcTaskStatus.None && currentStatus != NpcTaskStatus.Incomplete)
            {
                SetActive(imgStatus[currentIndex], false);
                currentIndex = (int)currentStatus - 1;
                SetActive(imgStatus[currentIndex]);
            }
            else
            {
                SetActive(imgStatus[currentIndex], false);
            }
        }
        else
        {
            // 物体不在相机的可视区域内，隐藏UI
            SetActive(imgStatus[currentIndex], false);
        }
    }

    public void InitItem(NpcTaskStatus status, Transform trans)
    {
        rect = GetComponent<RectTransform>();
        rootTrans = trans;
        SetItemState(status);
    }

    public void SetItemState(NpcTaskStatus status)
    {      
        currentStatus = status;
    }
}
