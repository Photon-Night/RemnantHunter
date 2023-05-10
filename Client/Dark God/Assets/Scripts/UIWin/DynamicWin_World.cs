using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWin_World : WinRoot
{
    public Transform hpItemRoot;
    public Transform taskStatusItemRoot;

    public Text txtPlayerDodge;
    public Animation playerDodge;
    private Dictionary<string, ItemEntityHP> hpUIItemDic = new Dictionary<string, ItemEntityHP>();
    private Dictionary<int, NpcTaskStatusItem> taskStatusUIDic = new Dictionary<int, NpcTaskStatusItem>();
    public void AddHpUIItem(string name, int hp, Transform trans)
    {
        ItemEntityHP item = null;
        if (hpUIItemDic.TryGetValue(name, out item))
        {
            return;
        }

        GameObject go = resSvc.LoadPrefab(PathDefine.HpUIItem, true);
        go.transform.SetParent(hpItemRoot);
        go.transform.localPosition = new Vector3(-1000, 0, 0);
        item = go.GetComponent<ItemEntityHP>();
        item.InitItemInfo(hp, trans);

        hpUIItemDic.Add(name, item);
    }
    public void AddTaskStatusItem(int npcId, NpcTaskStatus status, Transform trans)
    {
        GameObject go = resSvc.LoadPrefab(PathDefine.TaskStatusItem, true);
        go.transform.SetParent(taskStatusItemRoot);
        go.transform.localPosition = new Vector3(-1000, 0, 0);
        NpcTaskStatusItem item = go.GetComponent<NpcTaskStatusItem>();
        item.InitItem(status, trans);

        taskStatusUIDic.Add(npcId, item);
    }

    public void UpdateTaskStatus(int npcId, NpcTaskStatus status)
    {
        if (taskStatusUIDic.ContainsKey(npcId))
        {
            taskStatusUIDic[npcId].SetItemState(status);
        }
    }
    public void ReMoveHpUIItem(string name)
    {
        ItemEntityHP _uiItem = null;
        if (hpUIItemDic.TryGetValue(name, out _uiItem))
        {
            Destroy(_uiItem.gameObject);
            hpUIItemDic.Remove(name);
        }
    }

    public void RemoveAllHPUIItem()
    {
        foreach (var item in hpUIItemDic)
        {
            Destroy(item.Value.gameObject);
        }

        hpUIItemDic.Clear();
    }

    public void SetHurt(string index, int hurt)
    {
        ItemEntityHP item = null;
        if (hpUIItemDic.TryGetValue(index, out item))
        {
            item.SetHurt(hurt);
        }
    }

    public void SetDodge(string index)
    {
        ItemEntityHP item = null;
        if (hpUIItemDic.TryGetValue(index, out item))
        {
            item.SetDodge();
        }
    }

    public void SetCritical(string index)
    {
        ItemEntityHP item = null;
        if (hpUIItemDic.TryGetValue(index, out item))
        {
            item.SetCritical();
        }
    }

    public void SetHpVal(string index, int oldHp, int newHp)
    {
        ItemEntityHP item = null;
        if (hpUIItemDic.TryGetValue(index, out item))
        {
            item.SetHpVal(oldHp, newHp);
        }
    }

    public void SetDodgePlayer()
    {
        SetActive(txtPlayerDodge);
        playerDodge.Stop();
        playerDodge.Play();
    }
}
