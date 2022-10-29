using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWin : WinRoot
{
    public Transform taskItemContent;

    private List<TaskRewardData> taskLst = new List<TaskRewardData>();
    private PlayerData pd = null;


    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.Instance.PlayerData;

        RefreshUI();
    }
    public void RefreshUI()
    {
        taskLst.Clear();

        List<TaskRewardData> todoLst = new List<TaskRewardData>();
        List<TaskRewardData> doneLst = new List<TaskRewardData>();

        for (int i = 0; i < pd.task.Length; i++)
        {
            string[] taskInfo = pd.task[i].Split('|');

            TaskRewardData data = new TaskRewardData
            {
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[1]),
                taked = taskInfo[2].Equals("1"),
            };
            if (data.taked)
            {
                doneLst.Add(data);
            }
            else
            {
                todoLst.Add(data);
            }
        }

        for (int i = 0; i < taskItemContent.childCount; i++)
        {
            Destroy(taskItemContent.GetChild(i).gameObject);
        }

        taskLst.AddRange(todoLst);
        taskLst.AddRange(doneLst);

        for (int i = 0; i < taskLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItem);
            go.transform.SetParent(taskItemContent);

            TaskCfg data = resSvc.GetTaskCfgData(taskLst[i].ID);
            Transform trans = go.transform;

            SetText(GetTransform(trans, "txtName"), data.taskName);
            SetText(GetTransform(trans, "txtExp"), "���� " + data.exp);
            SetText(GetTransform(trans, "txtCoin"), "��� " + data.coin);
            SetText(GetTransform(trans, "txtPrg"), taskLst[i].prgs + "/" + data.count);

            Image prg = GetTransform(trans, "imgPrg").GetComponent<Image>();
            prg.fillAmount = taskLst[i].prgs * 1f / data.count * 1f;

            Button btnTake = GetTransform(trans, "btnTake").GetComponent<Button>();
            btnTake.onClick.AddListener(() =>
            {
                OnClickTakeBtn(data.ID);
            });

            if(taskLst[i].taked)
            {
                btnTake.interactable = false;
                SetActive(GetTransform(trans, "imgTaked"));
            }
            else
            {
                btnTake.enabled = true;
                SetActive(GetTransform(trans, "imgTaked"), false);

                if(taskLst[i].prgs == data.count)
                {
                    btnTake.interactable = true;
                }
                else
                {
                    btnTake.interactable = false;
                }
            }
        }

    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
       
        this.SetWinState(false);
    }

    private void OnClickTakeBtn(int id)
    {
        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqTakeTaskReward
        };

        msg.reqTakeTaskReward = new ReqTakeTaskReward
        {
            rid = id,
        };

        netSvc.SendMessage(msg);

        TaskCfg data = resSvc.GetTaskCfgData(id);

        GameRoot.AddTips("�������");
        GameRoot.AddTips(Message.Color("��� + " + data.coin, Message.ColorBlue));
        GameRoot.AddTips(Message.Color("���� + " + data.exp, Message.ColorBlue));
    }
}
