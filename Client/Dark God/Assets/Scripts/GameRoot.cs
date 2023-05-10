using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using Game.Common;
using EventMgr = Game.Event.GameEventManager;
using Game.Event;
using Game.Bag;

public class GameRoot : MonoSingleton<GameRoot>
{
    public LoadingWin loadingWin;  
    public DynamicWin dynamicWin;

    // Start is called before the first frame update
    void Start()
    {
        ClearUIRoot();

        InitService();
        InitManager();
        InitSystem();

        NPCManager.Instance.InitManager();
        PECommon.Log("Game Start");
        LoginSystem.Instance.OnLoginEnter();


        dynamicWin.SetWinState();
        EventMgr.SubscribeEvent<int>(EventNode.Event_OnNPCTaskStatusChange, UpdateTaskStatus);
        EventMgr.SubscribeEvent<bool>(EventNode.Event_OnSetUIWinState, SetNpcTaskStatusItemState);
    }
    private void InitService()
    {
        IService[] services = this.GetComponents<IService>();
        for (int i = 0; i < services.Length; i++)
        {
            services[i].ServiceInit();
        }
    }
    private void InitSystem()
    {
        MainCitySystem.Instance.InitSystem();
        MissionSystem.Instance.InitSystem();
        BattleSystem.Instance.InitSystem();
        BagSystem.Instance.InitSystem();
        LoginSystem.Instance.InitSystem();
        

    }
    public void InitManager()
    {
        NPCManager.Instance.InitManager();
        EventMgr.InitManager();
    }

    private void ClearUIRoot()
    {
        Transform canvas = this.transform.Find("Canvas");
        for(int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }      
    }

    public static void AddTips(string tip)
    {
        Instance.dynamicWin.AddTips(tip);
    }

    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }

    public void SetPlayerData(RspLogin data)
    {
        playerData = data.playerData;
        TaskSystem.Instance.InitSystem();
        BagSystem.Instance.InitBagManager(PlayerData.bag);
    }

    public void SetPlayerDataByCreate(RspRename data)
    {
        PlayerData.name = data.name;
        PlayerData.modle = data.modle;
    }

    public void SetPlayerDataByGuide(RspGuide data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.guideid = data.guideid;
    }

    public void SetPlayerDataByStrong(RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.crystal = data.crystal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;

        playerData.strong = data.strong;

    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        PlayerData.coin = data.coin;
        PlayerData.diamond = data.diamond;
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByPower(PushPower data)
    {
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByUseProp(RspUseProp data)
    {
        PlayerData.bag = data.bag;
    }

    public void SetPlayerDataByChangeEquipment(RspChangeEquipment data)
    {
        PlayerData.equipment = data.equipmentStr;
    }

    public void SetPlayerDataByMissionEnter(RspMissionEnter data)
    {
        PlayerData.power = data.power;
    }

    public void SetPlayerDataByFightEnd(RspFBFightEnd data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
        PlayerData.crystal = data.crystal;
        PlayerData.mission = data.mission;
    }

    public void SetPlayerDataByFinishTask(RspUpdateTaskInfo data)
    {
        PlayerData.coin = data.coin;
        PlayerData.lv = data.lv;
        PlayerData.exp = data.exp;
    }
 
    public void AddHpUIItem(string name, int hp, Transform trans)
    {
        dynamicWin.AddHpUIItem(name, hp, trans);
    }

    public void SetHpUIItemActive(string name, bool active = true)
    {
        dynamicWin.SetHpUIItemActive(name, active);
    }

    public void AddTaskStatusItem(int npcId, NpcTaskStatus status, Transform trans)
    {
        dynamicWin.AddTaskStatusItem(npcId, status, trans);
    }

    public void RemoveHpUIItem(string name)
    {
        dynamicWin.ReMoveHpUIItem(name);
    }

    public void RemoveAllHPUIItem()
    {
        dynamicWin.RemoveAllHPUIItem();
    }

    public void RemoveAllTaskUIItem()
    {
        dynamicWin.RemoveAllTaskUIItem();
    }
    private void UpdateTaskStatus(params int[] args)
    {
        int npcId = args[0];
        int status = args[1];
        dynamicWin.UpdateTaskStatus(npcId, (NpcTaskStatus)status);
    }
    
    private void SetNpcTaskStatusItemState(params bool[] args)
    {
        bool state = args[0];
        dynamicWin.SetNpcTaskStatusItemState(!state);
    }

    #region DynamicWin
    public void SetHurt(string index, int hurt)
    {
        dynamicWin.SetHurt(index, hurt);
    }

    public void SetDodge(string index)
    {
        dynamicWin.SetDodge(index);
    }

    public void SetCritical(string index)
    {
        dynamicWin.SetCritical(index);
    }

    public void SetHpVal(string index, int oldHp, int newHp)
    {
        dynamicWin.SetHpVal(index, oldHp, newHp);
    }

    public void SetDodgePlayer()
    {
        dynamicWin.SetDodgePlayer();
    }
    #endregion
}
