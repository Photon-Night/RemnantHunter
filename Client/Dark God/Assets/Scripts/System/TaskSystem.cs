using Game.Bag;
using Game.Event;
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

        PECommon.Log("TaskSystem Loading");

        taskMgr = new TaskManager();
        List<NTaskInfo> infos = null;
        if (GameRoot.Instance.PlayerData.taskDatas != null)
        {
            infos = new List<NTaskInfo>(GameRoot.Instance.PlayerData.taskDatas);
        }
        taskMgr.InitManager(infos);

        //BattleSystem.Instance.onTargetDie += ChangeKillTaskPrg;
        //NPCManager.Instance.RegisterNPCEvent(NPCFunction.OpenTaskWin, OpenTaskWin);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnOverTalk, OpenTaskWin_EventAction);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnKillMonster, ChangeKillTaskPrg);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnBattleWin, OnBattleWin);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnBuyCoin, OnBuyCoin);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnStrong, OnStrong);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnChangeEquipment, OnChangeEquipment);
    }

    private void OpenTaskWin_EventAction(params int[] args)
    {
        int npcId = args[0];
        int actionId = args[1];

        if(actionId == (int)NPCFunction.OpenTaskWin)
        OpenNpcTaskWin(npcId);
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
        if (taskMgr != null)
        {
            return taskMgr.GetTaskStatusByNpc(npcID);
        }

        PECommon.Log("TaskManager is null", PEProtocol.LogType.Error);
        return NpcTaskStatus.None;
    }

    public void RspUpdateTaskInfo(GameMsg msg)
    {
        RspUpdateTaskInfo data = msg.rspUpdateTaskInfo;

        taskMgr.RefreshTaskState(data.info);
        if (taskWin.GetWinState())
        {
            taskWin.Refresh();
        }



        if(data.info.taskState == TaskStatus.Finished)
        {
            TaskDefine taskData = resSvc.GetTaskData(data.info.taskID);
            GameRoot.Instance.SetPlayerDataByFinishTask(data);
            TaskDefine task = resSvc.GetTaskData(data.info.taskID);
            BagSystem.Instance.UpdateBagInfo(task.item);
            GameRoot.AddTips("金币+" + taskData.coin);
            GameRoot.AddTips("经验+" + taskData.exp);
        }

        NPCManager.Instance.UpdateNpcTaskStatus();
    }

    public void RspUpdateTaskPrg(GameMsg msg)
    {
        RspUpdateTaskPrg data = msg.rspUpdateTaskPrg;
        taskMgr.ChangeTaskPrg(data.taskId, data.prg);

        if (taskWin.GetWinState())
        {
            taskWin.Refresh();
        }
    }

    public void ChangeTaskStatus(TaskItem task, TaskStatus status)
    {
        TaskDefine data = resSvc.GetTaskData(task.data.ID);
        if (status == TaskStatus.Finished)
        {
            if (data.targetCount > task.npcInfo.prg)
            {
                GameRoot.AddTips("未达成任务目标");
                return;
            }
        }       
        

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskInfo
        };

        msg.reqUpdateTaskInfo = new ReqUpdateTaskInfo
        {
            taskId = task.data.ID,
            prg = status == TaskStatus.InProgress ? 0 : task.npcInfo.prg,
            newStatus = status
        };


        netSvc.SendMessage(msg);
    }

    public List<TaskItem> GetTaskList(int id, NpcTaskStatus status)
    {
        return taskMgr.GetNpcTaskList(id, status);
    }

    public List<TaskItem> GetOwnerTask()
    {
        return taskMgr.GetOwnerTaskLst();
    }

    public void OpenNpcTaskWin(int npcId)
    {
        taskWin.SetCurrentNpcId(npcId);
        taskWin.SetWinState();
    }

    public void OpenOwnerTaskWin()
    {
        taskWin.OpenOwnerTaskWin();
    }

    private void ChangeKillTaskPrg(params int[] args)
    {
        int entityId = args[1];
        TaskType type = (TaskType)args[0];

        if (type != TaskType.Kill) return;

        TaskItem task = taskMgr.GetPlayerTask(entityId, TaskType.Kill);
        
        if (task == null || task.npcInfo.prg == task.data.targetCount)
            return;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskPrg
        };

        msg.reqUpdateTaskPrg = new ReqUpdateTaskPrg
        {
            taskId = task.data.ID,
            count = 1,
        };


        NetService.Instance.SendMessage(msg);
    }

    private void OnBattleWin(params int[] args)
    {
        TaskItem task = taskMgr.GetPlayerTask(0, TaskType.Win);

        if (task == null || task.npcInfo.prg == task.data.targetCount)
            return;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskPrg
        };

        msg.reqUpdateTaskPrg = new ReqUpdateTaskPrg
        {
            taskId = task.data.ID,
            count = 1,
        };


        NetService.Instance.SendMessage(msg);
    }

    private void OnStrong(params int[] args)
    {
        TaskItem task = taskMgr.GetPlayerTask(1, TaskType.Strong);

        if (task == null || task.npcInfo.prg == task.data.targetCount)
            return;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskPrg
        };

        msg.reqUpdateTaskPrg = new ReqUpdateTaskPrg
        {
            taskId = task.data.ID,
            count = 1,
        };


        NetService.Instance.SendMessage(msg);
    }

    private void OnBuyCoin(params int[] args)
    {
        TaskItem task = taskMgr.GetPlayerTask(2, TaskType.Buy);

        if (task == null || task.npcInfo.prg == task.data.targetCount)
            return;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskPrg
        };

        msg.reqUpdateTaskPrg = new ReqUpdateTaskPrg
        {
            taskId = task.data.ID,
            count = 1,
        };


        NetService.Instance.SendMessage(msg);
    }

    private void OnChangeEquipment(params int[] args)
    {
        TaskItem task = taskMgr.GetPlayerTask(3, TaskType.ChangeEquipment);

        if (task == null || task.npcInfo.prg == task.data.targetCount)
            return;

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqUpdateTaskPrg
        };

        msg.reqUpdateTaskPrg = new ReqUpdateTaskPrg
        {
            taskId = task.data.ID,
            count = 1,
        };


        NetService.Instance.SendMessage(msg);
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
    Win = 4,
    Damage = 5,
    Strong = 6,
    Buy = 7,
    ChangeEquipment = 8,
}



