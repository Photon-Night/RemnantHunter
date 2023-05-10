using Game.Event;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager
{
    ResService resSvc;

    //所有有效任务
    private List<NTaskInfo> taskInfos;
    private Dictionary<int, TaskItem> allTaskDic = new Dictionary<int, TaskItem>();
    /// <summary>
    /// npc任务状态
    /// </summary>
    private Dictionary<int, Dictionary<NpcTaskStatus, List<TaskItem>>> npcTaskDic = new Dictionary<int, Dictionary<NpcTaskStatus, List<TaskItem>>>();
    private Dictionary<TaskType, Dictionary<int, TaskItem>> ownerTaskDic = new Dictionary<TaskType, Dictionary<int, TaskItem>>();
    private List<TaskItem> ownerTaskList = new List<TaskItem>();
    public System.Action<TaskItem> onTaskStatusChanged;


    public List<TaskItem> GetNpcTaskList(int id, NpcTaskStatus status)
    {
        if(npcTaskDic.ContainsKey(id))
        {
            if(npcTaskDic[id].ContainsKey(status))
            {
                return npcTaskDic[id][status];
            }
        }

        return null;
    }

    public List<TaskItem> GetOwnerTaskLst()
    {
        return ownerTaskList;
    }

    public TaskItem GetPlayerTask(int taskId, TaskType type)
    {
        if(ownerTaskDic.ContainsKey(type))
        {
            TaskItem item = null;
            if(ownerTaskDic[type].TryGetValue(taskId, out item))
            {
                return item;
            }
        }

        return null;
    }

    public TaskItem GetTaskItem(int taskId)
    {
        TaskItem task = null;
        if(allTaskDic.TryGetValue(taskId, out task))
        {
            return task;
        }
        return null;
    }

    public void InitManager(List<NTaskInfo> infos)
    {
        resSvc = ResService.Instance;
        allTaskDic.Clear();
        this.npcTaskDic.Clear();
        taskInfos = infos;
        InitTasks();
    }

    private void InitTasks()
    {
        //初始化已有任务
        if(taskInfos != null)
        foreach (var info in taskInfos)
        {
            TaskItem task = new TaskItem(info);
            this.allTaskDic[task.data.ID] = task;
            AddPlayerTaskData(task);
        }

        CheckAvailableTask();

        foreach (var item in this.allTaskDic)
        {
            this.AddNpcTask(item.Value.data.acceptNpcID, item.Value);
            this.AddNpcTask(item.Value.data.submitNpcID, item.Value);
        }
    }

    public void AddPlayerTaskData(TaskItem task)
    {
        if(!ownerTaskDic.ContainsKey(task.data.taskType))
        {
            ownerTaskDic[task.data.taskType] = new Dictionary<int, TaskItem>();
        }

        if(!ownerTaskDic[task.data.taskType].ContainsKey(task.data.ID))
        {
            ownerTaskDic[task.data.taskType][task.data.targetID] = task;
            ownerTaskList.Add(task);
        }
    }
    /// <summary>
    /// 初始化有效任务
    /// </summary>
    private void CheckAvailableTask()
    {
        foreach (var item in resSvc.GetTaskDic())
        {
            if (item.Value.limitLevel > GameRoot.Instance.PlayerData.lv)
                continue;
            if (this.allTaskDic.ContainsKey(item.Key))
                continue;
            if (item.Value.preTaskID != -1)
            {
                TaskItem _preTask = null;
                int preID = item.Value.preTaskID;
                if (this.allTaskDic.TryGetValue(preID, out _preTask))
                {
                    if (_preTask.npcInfo == null)//未获取
                        continue;
                    if (_preTask.npcInfo.taskState != TaskStatus.Finished)//未完成
                        continue;
                }
                else//未接取
                    continue;
            }

            TaskItem task = new TaskItem(item.Value);
            this.allTaskDic[task.data.ID] = task;
        }
    }

    private void AddNpcTask(int npcID, TaskItem task)
    {
        if (!this.npcTaskDic.ContainsKey(npcID))
        {
            this.npcTaskDic[npcID] = new Dictionary<NpcTaskStatus, List<TaskItem>>();
        }

        List<TaskItem> availables;
        List<TaskItem> complates;
        List<TaskItem> incomplates;

        if (!this.npcTaskDic[npcID].TryGetValue(NpcTaskStatus.Available, out availables))
        {
            availables = new List<TaskItem>();
            this.npcTaskDic[npcID][NpcTaskStatus.Available] = availables;
        }

        if (!this.npcTaskDic[npcID].TryGetValue(NpcTaskStatus.Complete, out complates))
        {
            complates = new List<TaskItem>();
            this.npcTaskDic[npcID][NpcTaskStatus.Complete] = complates;
        }

        if (!this.npcTaskDic[npcID].TryGetValue(NpcTaskStatus.Incomplete, out incomplates))
        {
            incomplates = new List<TaskItem>();
            this.npcTaskDic[npcID][NpcTaskStatus.Incomplete] = incomplates;
        }

        if (task.npcInfo == null)
        {
            if (npcID == task.data.acceptNpcID && !this.npcTaskDic[npcID][NpcTaskStatus.Available].Contains(task))
            {
                this.npcTaskDic[npcID][NpcTaskStatus.Available].Add(task);
            }
        }
        else
        {
            if (task.data.submitNpcID == npcID && task.npcInfo.taskState == TaskStatus.Complated)
            {
                if (!this.npcTaskDic[npcID][NpcTaskStatus.Complete].Contains(task))
                {
                    this.npcTaskDic[npcID][NpcTaskStatus.Complete].Add(task);
                }
            }
            if (task.data.submitNpcID == npcID && task.npcInfo.taskState == TaskStatus.InProgress)
            {
                if (!this.npcTaskDic[npcID][NpcTaskStatus.Incomplete].Contains(task))
                {
                    this.npcTaskDic[npcID][NpcTaskStatus.Incomplete].Add(task);
                }
            }

            if(task.data.acceptNpcID == npcID && task.npcInfo.taskState == TaskStatus.Failed)
            {
                if (npcID == task.data.acceptNpcID && !this.npcTaskDic[npcID][NpcTaskStatus.Available].Contains(task))
                {
                    this.npcTaskDic[npcID][NpcTaskStatus.Available].Add(task);
                }
            }
        }
    }
    /// <summary>
    /// 获取npc任务状态，优先级为 有完成任务，有新可接任务，有未完成任务
    /// </summary>
    /// <param name="npcID"></param>
    /// <returns></returns>
    public NpcTaskStatus GetTaskStatusByNpc(int npcID)
    {
        Dictionary<NpcTaskStatus, List<TaskItem>> status = null;
        //获取任务
        if (this.npcTaskDic.TryGetValue(npcID, out status))
        {
            if (status[NpcTaskStatus.Complete].Count > 0)
                return NpcTaskStatus.Complete;
            if (status[NpcTaskStatus.Available].Count > 0)
                return NpcTaskStatus.Available;
            if (status[NpcTaskStatus.Incomplete].Count > 0)
                return NpcTaskStatus.Incomplete;
        }

        return NpcTaskStatus.None;
    }

    public void RefreshTaskState(NTaskInfo info)
    {
        this.npcTaskDic.Clear();
        TaskItem result = null;
        if(this.allTaskDic.ContainsKey(info.taskID))
        {
            this.allTaskDic[info.taskID].npcInfo = info;
            result = this.allTaskDic[info.taskID];
        }
        else
        {
            result = new TaskItem(info);
            this.allTaskDic[info.taskID] = result;
        }

        if(info.taskState == TaskStatus.InProgress)
        {
            AddPlayerTaskData(result);
        }
        else if(info.taskState == TaskStatus.Finished)
        {
            ownerTaskDic[result.data.taskType].Remove(info.taskID);
            ownerTaskList.Remove(result);
        }
        else if(info.taskState == TaskStatus.Failed)
        {
            ownerTaskDic[result.data.taskType].Remove(info.taskID);
            ownerTaskList.Remove(result);
        }
        CheckAvailableTask();

        foreach (var item in allTaskDic)
        {
            this.AddNpcTask(item.Value.data.acceptNpcID, item.Value);
            this.AddNpcTask(item.Value.data.submitNpcID, item.Value);
        }

    }

    public void ChangeTaskPrg(int taskId, int prg)
    {
        if(allTaskDic.ContainsKey(taskId))
        {
            allTaskDic[taskId].npcInfo.prg = prg;
        }
    }
    
 }

