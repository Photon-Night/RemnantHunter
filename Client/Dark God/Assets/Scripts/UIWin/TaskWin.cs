using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void RefreshUI()
    {
        taskLst.Clear();

        List<TaskRewardData> todoLst = new List<TaskRewardData>();
        List<TaskRewardData> doneLst = new List<TaskRewardData>(); 
        Debug.Log(pd.task.Length);

        for (int i = 0; i < pd.task.Length; i++)
        {
            string[] taskInfo = pd.task[i].Split('|');

            TaskRewardData data = new TaskRewardData
            {
                ID = int.Parse(taskInfo[0]),
                prgs = int.Parse(taskInfo[2]),
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

        for (int i = 0; i < todoLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItem);
            go.transform.SetParent(taskItemContent);
        }

        for (int i = 0; i < doneLst.Count; i++)
        {
            GameObject go = resSvc.LoadPrefab(PathDefine.TaskItem);
            go.transform.SetParent(taskItemContent);
        }
    }

    public void OnClickCloseBtn()
    {
        audioSvc.PlayUIAudio(Message.UIClickBtn);
        this.SetWinState(false);
    }
}
