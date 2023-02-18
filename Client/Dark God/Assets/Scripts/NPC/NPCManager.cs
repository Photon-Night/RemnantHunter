using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public delegate bool NPCHandler(NPCCfg npc);
    private ResService resSvc = null;
    Dictionary<Message.NPCFunction, NPCHandler> npcEventDic = new Dictionary<Message.NPCFunction, NPCHandler>();
    Dictionary<int, NPCController> npcDic = new Dictionary<int, NPCController>();
    public void RegisterNPCEvent(Message.NPCFunction func, NPCHandler action)
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

    public void LoadNPC(ref List<int> npcs, PlayerController pc)
    {
        foreach (var npcID in npcs)
        {
            NPCCfg data = resSvc.GetNPCData(npcID);
            GameObject npcGo = resSvc.LoadPrefab(data.resPath);
            npcGo.transform.position = data.pos;
            NPCController controller = npcGo.AddComponent<NPCController>();
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


}
