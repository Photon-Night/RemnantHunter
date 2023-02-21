using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : SystemRoot<TaskSystem>
{
    private TaskManager taskMgr = null;
    public TaskWin taskWin;

    public override void InitSystem()
    {
        base.InitSystem();
        taskMgr = new TaskManager();
        List<NTaskInfo> infos = null;
        if(GameRoot.Instance.PlayerData.taskDatas != null)
        {
            infos = new List<NTaskInfo>(GameRoot.Instance.PlayerData.taskDatas);
        }
        taskMgr.InitManager(infos);
    }

    public void SetRegisterEvent(System.Action<TaskItem> action, bool reg = true)
    {
        if (reg)
            taskMgr.onTaskStatusChanged += action;
        else
            taskMgr.onTaskStatusChanged -= action;
    }

    public NpcTaskStatus GetNpcTaskStatus(int npcID)
    {
        if(taskMgr != null)
        {
            return taskMgr.GetTaskStatusByNpc(npcID);
        }

        PECommon.Log("TaskManager is null", PEProtocol.LogType.Error);
        return NpcTaskStatus.None;
    }
  
    public void RspAcceptTask(GameMsg msg)
    {

    }

    public void RspSubmitTask(GameMsg msg)
    {

    }

    public void OnChangeTaskStatus(NTaskInfo info)
    {

    }

    public List<TaskItem> GetTaskList(int id, NpcTaskStatus status)
    {
        return taskMgr.GetNpcTaskList(id, status);
    }

    public void OpenNpcTaskWin(int npcId)
    {
        taskWin.SetCurrentNpcId(npcId);
        taskWin.SetWinState();
    }
}

public enum NpcTaskStatus
{
    None = 0,
    Complete = 1,
    Available = 2,
    Incomplete = 3,
}

public enum TaskType
{
    None = 0,
    Kill = 1,
    Talk = 2,
    Gather = 3,
    Arrive = 4,
}



