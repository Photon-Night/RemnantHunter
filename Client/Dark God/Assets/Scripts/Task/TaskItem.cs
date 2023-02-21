using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskItem
{
    public TaskStatus taskState;
    public TaskDefine data;
    public NTaskInfo npcInfo;

    public TaskItem()
    {

    }
    public TaskItem(TaskDefine data)
    {
        this.data = data;
        this.npcInfo = null;
    }

    public TaskItem(NTaskInfo info)
    {
        this.npcInfo = info;
        this.data = ResService.Instance.GetTaskData(info.taskID);
    }
}
