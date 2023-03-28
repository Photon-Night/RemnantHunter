using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoSingleton<NPCManager>
{
    public delegate void NPCHandler(int npcID);
    private ResService resSvc = null;
    Dictionary<NPCFunction, NPCHandler> npcEventDic = new Dictionary<NPCFunction, NPCHandler>();
    Dictionary<int, NPCController> npcDic = new Dictionary<int, NPCController>();
    public void RegisterNPCEvent(NPCFunction func, NPCHandler action)
    {
        if(!npcEventDic.ContainsKey(func))
        {
            npcEventDic[func] = action;
        }
        else
        {
            npcEventDic[func] += action;
        }
    }

    public void InitManager()
    {
        resSvc = ResService.Instance;
    }

    public void LoadNPC(List<int> npcs, PlayerController pc)
    {

        foreach (var npcID in npcs)
        {
            NPCCfg data = resSvc.GetNPCData(npcID);
            GameObject npcGo = resSvc.LoadPrefab(data.resPath);
            npcGo.transform.position = data.pos;
            NPCController controller = npcGo.GetComponent<NPCController>();
            controller.InitNPC(data, pc);
            controller.RegisterEvnet(OnPlayerCloseNPC);           
            npcDic.Add(npcID, controller);
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

    public void OnPlayerCloseNPC(NPCCfg npc)
    {
        MainCitySystem.Instance.OnPlayerCloseNPC(npc);
    }

    public void InteractiveNpcFunction(NPCFunction type, int npcID)
    {
        if(npcEventDic.ContainsKey(type))
        {
            npcEventDic[type](npcID);
        }
    }

    public void OnSceneChange()
    {
        npcDic.Clear();
    }
}
public enum NPCFunction
{
    None = 0,
    OpenTaskWin = 1,
}
public enum NPCType
{
    None = 0,
    Functional = 1,
}
