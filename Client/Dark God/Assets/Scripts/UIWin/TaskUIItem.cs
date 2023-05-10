using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskUIItem : UIRoot
{
    public Toggle togTask;

    public Text txtName;
    public Text txtPrg;
    public Text txtAccepter;

    public Image imgPrg;
    public Image imgPrgBg;
    public Image imgItemBg;

    public void InitItem(TaskItem task, ToggleGroup root)
    {
        base.InitUI();
        SetText(txtName, task.data.taskName);   
        SetText(txtAccepter, resSvc.GetNPCData(task.data.acceptNpcID).name);
        togTask.group = root;
        if (task.npcInfo != null)
        {
            SetText(txtPrg, $"{task.npcInfo.prg}/{task.data.targetCount}");
            imgPrg.fillAmount = (float)task.npcInfo.prg / (float)task.data.targetCount;
        }
    }
}
