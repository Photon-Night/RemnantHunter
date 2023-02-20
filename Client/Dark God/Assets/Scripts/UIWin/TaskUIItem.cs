using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIItem : MonoBehaviour
{
    public Image imgItemBg;
    public Text txtName;
    public Text txtPrg;
    public Image imgPrg;
    public Text txtCoin;
    public Text txtExp;

    private Color bgColor;
    
    public void InitItem(TaskItem task)
    {
        bgColor = imgItemBg.color;
        txtName.text = task.data.taskName;
        txtPrg.text = task.npcInfo.prg + "/" + task.data.targetCount;
        imgPrg.fillAmount = task.npcInfo.prg / task.data.targetCount;
        txtCoin.text = task.data.coin.ToString();
        txtExp.text = task.data.exp.ToString();
    }

    public void OnMouseDrag()
    {
        AudioService.Instance.PlayUIAudio(Message.UIClickBtn);
        //打开详情界面
    }

    public void OnMouseEnter()
    {
        bgColor.a = 190 / 255;
    }

    public void OnMouseExit()
    {
        bgColor.a = 1f;
    }
}
