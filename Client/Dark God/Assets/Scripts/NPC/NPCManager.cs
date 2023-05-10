using Game.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoSingleton<NPCManager>
{
    public delegate void NPCHandler(int npcID);
    private ResService resSvc = null;
   
    Dictionary<int, NPCController> npcDic = new Dictionary<int, NPCController>();

    public void InitManager()
    {
        resSvc = ResService.Instance;
    }

    public void LoadNPC(List<int> npcs, PlayerController pc)
    {

        foreach (var npcId in npcs)
        {
            NPCCfg data = resSvc.GetNPCData(npcId);

            GameObject npcGo = resSvc.LoadPrefab(data.resPath);
            npcGo.transform.position = data.pos;

            NPCController controller = npcGo.GetComponent<NPCController>();
            controller.InitNPC(data, pc, TaskSystem.Instance.GetNpcTaskStatus(npcId));
            npcDic.Add(npcId, controller);
            GameRoot.Instance.AddTaskStatusItem(npcId,controller.TaskStatus, controller.uiRoot);

        }
    }

    public void Interactive(int npcID)
    {
        NPCController nc = null;
        if(npcDic.TryGetValue(npcID, out nc))
        {
            nc.Interactive();
        }
    }

    public void OnSceneChange()
    {
        npcDic.Clear();
    }

    public void UpdateNpcTaskStatus()
    {
        var e = npcDic.GetEnumerator();
        while(e.MoveNext())
        {
            e.Current.Value.TaskStatus = TaskSystem.Instance.GetNpcTaskStatus(e.Current.Key);
        }

        e.Dispose();
    }
}

public enum NPCType
{
    None = 0,
    Functional = 1,
}

