using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWin : WinRoot
{
    public Transform taskItemContent;
    public Transform rightPanelTrans;
    public Transform rewardGroup;

    public ToggleGroup taskGroup;

    public Toggle togAvaliable;
    public Toggle togInProgess;
    public Toggle toggleFinish;

    public Button btnTake;
    public Button btnFinish;
    public Button btnCancel;

    public Text txtDes;
    public Text txtCoin;
    public Text txtExp;
    public Text txtTaskName;
    public Text txtSubmit;

    public Text txtWeaponName;
    public Text txtShieldName;
    public Text txtPotionCount;

    public GameObject goWeapon;
    public GameObject goShield;

    private List<TaskItem> taskList;
    private List<TaskItem> ownerTasks_InComplete = new List<TaskItem>();
    private List<TaskItem> ownerTasks_Result = new List<TaskItem>();

    private int currentNpcId = -1;
    private NpcTaskStatus currentStatus = NpcTaskStatus.Available;
    private bool isOwner = false;
    private TaskItem currentTask;

    private bool isSetToggle = false;
    private bool firstInitToggle = false;

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
            if(!isSetToggle)
            {
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

                isSetToggle = true;
            }
            
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

        SetActive(btnCancel, false);
        SetActive(btnTake, false);
        SetActive(btnFinish, false);
        SetActive(rightPanelTrans, false);
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

        TaskUIItem firstItem = null;

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
        Debug.Log(taskList.Count);
        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject taskGo = resSvc.LoadPrefab(PathDefine.TogTaskItem);
            taskGo.transform.SetParent(taskItemContent);
            TaskUIItem item = taskGo.GetComponent<TaskUIItem>();
            TaskItem taskData = taskList[i];
            item.InitItem(taskData, taskGroup);

            if (i == 0)
            {
                firstItem = item;
            }

            item.togTask.onValueChanged.AddListener((isOn) =>
            {
                if (!isOn) return;

                if (!firstInitToggle)
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                }
                else
                {
                    firstInitToggle = false;
                }


                currentTask = taskData;
                SetRightPanelInfo(taskData);
                switch (taskData.npcInfo.taskState)
                {
                    case TaskStatus.None:
                        break;
                    case TaskStatus.InProgress:
                        SetActive(btnFinish, false);
                        SetActive(btnCancel);
                        break;
                    case TaskStatus.Complated:
                        SetActive(btnCancel, false);
                        SetActive(btnFinish);
                        break;
                }
                SetActive(rightPanelTrans);
            });
        }

        if (taskList.Count > 0)
        {
            SetRightPanelOnRefresh(firstItem);
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

        TaskUIItem firstItem = null;

        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject taskGo = resSvc.LoadPrefab(PathDefine.TogTaskItem);
            taskGo.transform.SetParent(taskItemContent);
            TaskUIItem item = taskGo.GetComponent<TaskUIItem>();
            TaskItem taskData = taskList[i];

            if(i == 0)
            {
                firstItem = item;
            }

            item.InitItem(taskData, taskGroup);
            item.togTask.onValueChanged.AddListener((isOn) =>
            {
                if (!isOn) return;

                if(!firstInitToggle)
                {
                    audioSvc.PlayUIAudio(Message.UIClickBtn);
                }
                else
                {
                    firstInitToggle = false;
                }

                currentTask = taskData;
                SetRightPanelInfo(taskData);
                SetActive(rightPanelTrans);
                
            });

            if (currentStatus == NpcTaskStatus.Available)
            {
                SetActive(item.txtPrg, false);
                SetActive(item.imgPrg, false);
                SetActive(item.imgPrgBg, false);
                SetActive(btnTake);
            }
            else if (currentStatus == NpcTaskStatus.Complete)
            {
                SetActive(btnCancel, false);
                SetActive(btnFinish);
            }
            else if (currentStatus == NpcTaskStatus.Incomplete)
            {
                SetActive(btnFinish, false);
                SetActive(btnCancel);
            }
        }

        if(taskList.Count > 0)
        {
            SetRightPanelOnRefresh(firstItem);
        }
    }

    private void SetRightPanelOnRefresh(TaskUIItem firstItem)
    {
        currentTask = taskList[0];
        firstInitToggle = true;
        firstItem.togTask.isOn = true;

        if (taskList.Count == 1)
        {
            firstItem.togTask.onValueChanged.Invoke(true);
        }
    }

    private void SetRightPanelInfo(TaskItem taskData)
    {
        SetText(txtTaskName, taskData.data.taskName);
        SetText(txtSubmit, resSvc.GetNPCData(taskData.data.submitNpcID).name);
        SetText(txtCoin, taskData.data.coin.ToString());
        SetText(txtExp, taskData.data.exp.ToString());
        SetText(txtDes, taskData.data.description);

        var item = taskData.data.item;
        SetActive(goShield, false);
        SetActive(goWeapon, false);

        for(int i = 0; i < item.Length; i++)
        {
            var arr = item[i].Split('#');
            int itemID = int.Parse(arr[0]);
            int count = int.Parse(arr[1]);

            var itemData = resSvc.GetGameItemCfg(itemID);
            var type = itemData.equipmentType;
            if (type == EquipmentType.Shield)
            {
                SetActive(goShield);
                SetText(txtShieldName, itemData.name);
            }
            else if(type == EquipmentType.Weapon)
            {
                SetActive(goWeapon);
                SetText(txtWeaponName, itemData.name);
            }
            else
            {
                SetText(txtPotionCount, count);
            }

        }
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        isOwner = false;
        this.SetWinState(false);
        
    }

    public void OnClickTakeBtn()
    {
        if(currentTask != null)
        {
            TaskSystem.Instance.ChangeTaskStatus(currentTask, TaskStatus.InProgress);
            audioSvc.PlayUIAudio(Message.UIClickBtn);
        }
    }

    public void OnClickFinishBtn()
    {
        if(currentTask != null)
        {
            TaskSystem.Instance.ChangeTaskStatus(currentTask, TaskStatus.Finished);
            audioSvc.PlayUIAudio(Message.UIClickBtn);
        }
    }

    public void OnClickCancelBtn()
    {
        if(currentTask != null)
        {
            TaskSystem.Instance.ChangeTaskStatus(currentTask, TaskStatus.Failed);
            audioSvc.PlayUIAudio(Message.UIClickBtn);
        }
    }
}
