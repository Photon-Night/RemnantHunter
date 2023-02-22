using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWin : WinRoot
{
    public Transform taskItemContent;
    public Toggle togAvaliable;
    public Toggle togInProgess;
    public Toggle toggleFinish;

    private List<TaskRewardData> taskLst = new List<TaskRewardData>();
    private List<TaskItem> taskList;
    private PlayerData pd = null;
    private int currentNpcId = -1;
    private NpcTaskStatus currentStatus = NpcTaskStatus.Available;

    public void SetCurrentNpcId(int id)
    {
        currentNpcId = id;
    }
    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.Instance.PlayerData;
        togAvaliable.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                ChangeNpcTaskType(1);
            }
        });

        togInProgess.onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
            ChangeNpcTaskType(2);
        });

        toggleFinish.onValueChanged.AddListener((isOn) =>
        {
            if(isOn)
            ChangeNpcTaskType(3);
        });
        Refresh();
    }

    protected override void ClearWin()
    {
        base.ClearWin();

    }

    public void OpenWinByNpc(int id)
    {
        currentNpcId = id;
        SetWinState();
    }
    private void ChangeNpcTaskType(int value)
    {
        if (value == 1)
            currentStatus = NpcTaskStatus.Available;
        else if (value == 2)
            currentStatus = NpcTaskStatus.Incomplete;
        else if (value == 3)
            currentStatus = NpcTaskStatus.Complete;

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < taskItemContent.childCount; i++)
        {
            Destroy(taskItemContent.GetChild(i).gameObject);
        }

        if (currentNpcId != -1 && currentStatus != NpcTaskStatus.None)
            taskList = TaskSystem.Instance.GetTaskList(currentNpcId, currentStatus);
        else
            return;
        if (taskList == null)
            return;
        
        for(int i = 0; i < taskList.Count; i++)
        {
            GameObject taskGo = resSvc.LoadPrefab(PathDefine.TaskItem);
            taskGo.transform.SetParent(taskItemContent);

            TaskUIItem item = taskGo.GetComponent<TaskUIItem>();
            item.InitItem(taskList[i]);

            TaskItem task = taskList[i];
            if(currentStatus == NpcTaskStatus.Available)
            {
                SetActive(item.txtPrg, false);
                SetActive(item.imgPrg, false);
                SetActive(item.imgPrgBg, false);
                SetActive(item.btnTake.gameObject);
                item.btnTake.onClick.AddListener(() =>
                {       
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                    TaskSystem.Instance.ChangeTaskStatus(task, TaskStatus.InProgress);
                });
            }
            else if(currentStatus == NpcTaskStatus.Complete)
            {
                SetActive(item.btnFinish.gameObject);
                item.btnFinish.onClick.AddListener(() =>
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                    TaskSystem.Instance.ChangeTaskStatus(task, TaskStatus.Finished);
                });
            }
            else if(currentStatus == NpcTaskStatus.Incomplete)
            {
                SetActive(item.btnAbondon.gameObject);
                item.btnAbondon.onClick.AddListener(() =>
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                    TaskSystem.Instance.ChangeTaskStatus(task, TaskStatus.Failed);
                });
            }
        }
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
            SetText(GetTransform(trans, "txtExp"), "经验 " + data.exp);
            SetText(GetTransform(trans, "txtCoin"), "金币 " + data.coin);
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

        GameRoot.AddTips("获得奖励");
        GameRoot.AddTips(Message.Color("金币 + " + data.coin, Message.ColorBlue));
        GameRoot.AddTips(Message.Color("经验 + " + data.exp, Message.ColorBlue));
    }
}
