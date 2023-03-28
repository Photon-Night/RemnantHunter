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

    private List<TaskItem> taskList;
    private List<TaskItem> ownerTasks_InComplete = new List<TaskItem>();
    private List<TaskItem> ownerTasks_Result = new List<TaskItem>();

    private int currentNpcId = -1;
    private NpcTaskStatus currentStatus = NpcTaskStatus.Available;
    private bool isOwner = false;

    public void SetCurrentNpcId(int id)
    {
        currentNpcId = id;
    }

    public void OpenOwnerTaskWin()
    {
        isOwner = true;
        SetWinState();
    }
    protected override void InitWin()
    {
        base.InitWin();
        if(!isOwner)
        {
            SetActive(togAvaliable.gameObject);
            SetActive(togInProgess.gameObject);
            SetActive(toggleFinish.gameObject);

            togAvaliable.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    audioSvc.PlayUIAudio(Message.UIOpenPage);
                    ChangeNpcTaskType(1);
                }
            });

            togInProgess.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    audioSvc.PlayUIAudio(Message.UIOpenPage);
                    ChangeNpcTaskType(2);
                }
            });

            toggleFinish.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    audioSvc.PlayUIAudio(Message.UIOpenPage);
                    ChangeNpcTaskType(3);
                }
            });
        }
        else
        {
            SetActive(togAvaliable.gameObject, false);
            SetActive(togInProgess.gameObject, false);
            SetActive(toggleFinish.gameObject, false);
        }


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

        if (isOwner)
        {
            ShowByOwner();
        }
        else
        {
            ShowByNpc();
        }
        
    }

    private void ShowByOwner()
    {
        var list = TaskSystem.Instance.GetOwnerTask();
        if (list == null)
            return;

        ownerTasks_Result.Clear();
        ownerTasks_InComplete.Clear();

        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].npcInfo.taskState == TaskStatus.Complated)
            {
                ownerTasks_Result.Add(list[i]);
            }
            else if(list[i].npcInfo.taskState == TaskStatus.InProgress)
            {
                ownerTasks_InComplete.Add(list[i]);
            }
        }
        ownerTasks_Result.AddRange(ownerTasks_InComplete);
        taskList = ownerTasks_Result;
       
        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject taskGo = resSvc.LoadPrefab(PathDefine.TaskItem);
            taskGo.transform.SetParent(taskItemContent);

            TaskUIItem item = taskGo.GetComponent<TaskUIItem>();
            item.InitItem(taskList[i]);

            TaskItem task = taskList[i];

            if (task.npcInfo.taskState == TaskStatus.Complated)
            {               
                item.btnFinish.onClick.AddListener(() =>
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                    TaskSystem.Instance.ChangeTaskStatus(task, TaskStatus.Finished);
                });
            }
            else if (task.npcInfo.taskState == TaskStatus.InProgress)
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

    private void ShowByNpc()
    {
        if (currentNpcId != -1 && currentStatus != NpcTaskStatus.None)
            taskList = TaskSystem.Instance.GetTaskList(currentNpcId, currentStatus);
        else
            return;
        if (taskList == null)
            return;

        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject taskGo = resSvc.LoadPrefab(PathDefine.TaskItem);
            taskGo.transform.SetParent(taskItemContent);

            TaskUIItem item = taskGo.GetComponent<TaskUIItem>();
            item.InitItem(taskList[i]);

            TaskItem task = taskList[i];
            if (currentStatus == NpcTaskStatus.Available)
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
            else if (currentStatus == NpcTaskStatus.Complete)
            {
                SetActive(item.btnFinish.gameObject);
                item.btnFinish.onClick.AddListener(() =>
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                    TaskSystem.Instance.ChangeTaskStatus(task, TaskStatus.Finished);
                });
            }
            else if (currentStatus == NpcTaskStatus.Incomplete)
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

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        isOwner = false;
        this.SetWinState(false);
        MainCitySystem.Instance.EnableCam();
    }

}
