using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskUIItem : UIRoot
{
    public Image imgItemBg;
    public Text txtName;
    public Text txtPrg;
    public Text txtCoin;
    public Text txtExp;

    public Button btnTake;
    public Button btnFinish;
    public Button btnAbondon;

    public Image imgPrg;
    public Image imgPrgBg;
    public void InitItem(TaskItem task)
    {
        base.InitUI();
        txtName.text = task.data.taskName;
        txtCoin.text = task.data.coin.ToString();
        txtExp.text = task.data.exp.ToString();
        if (task.npcInfo != null)
        {
            txtPrg.text = task.npcInfo.prg + "/" + task.data.targetCount;
            imgPrg.fillAmount = (float)task.npcInfo.prg / (float)task.data.targetCount;
        }

        RegisterTouchEvts();
    }

    private void RegisterTouchEvts()
    {
        OnEnter(this.gameObject, (PointerEventData evt) => 
        {
            imgItemBg.color = Color.gray;
        });

        OnExit(this.gameObject, (PointerEventData evt) =>
        {
            imgItemBg.color = Color.white;
        });

        OnClickDown(this.gameObject, (PointerEventData evt) =>
        {
            audioSvc.PlayUIAudio(Message.UIClickBtn);
        });
    }
}
