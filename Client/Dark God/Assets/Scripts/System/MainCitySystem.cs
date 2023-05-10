using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.AI;
using Game.Common;
using Game.Manager;
using Cinemachine;
using Game.Event;
using Game.Bag;

public class MainCitySystem : SystemRoot<MainCitySystem>, IPlayerInputSet, ICommonInputSet
{
    #region UIWin
    public MainCityWin mainCityWin;
    public InfoWin infoWin;
    public TalkWin talkWin;
    public StrongWin strongWin;
    public ChatWin chatWin;
    public BuyWin buyWin;
    public CreateWin createWin;
    #endregion
    public MapCfg MapData{ get; private set; }

    private PlayerController pc = null;
    private Transform charShowCam = null;
    #region NPC
    private NPCCfg nearNPCData = null;
    private NPCManager npcMgr = null;
    #endregion

    #region Camera
    private Transform camTrans;
    private CinemachineVirtualCamera pointCam;
    private CinemachineVirtualCamera createCam;
    private CinemachineFreeLook freeLookCam;
    #endregion

    public bool isUIWinOpen;
    private bool canInteract;
    public override void InitSystem()
    {
        base.InitSystem();
        npcMgr = NPCManager.Instance;

        GameEventManager.SubscribeEvent<bool>(EventNode.Event_OnSetUIWinState, SetCamLock);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnOverTalk, OnPlayerOverTalk);
        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnOverTalk, OpenStrongWin_EventAction);
        GameEventManager.SubscribeEvent<NPCCfg>(EventNode.Event_OnPlayerCloseToNpc, OnPlayerCloseNPC);
        GameEventManager.SubscribeEvent<NPCCfg>(EventNode.Event_OnPlayerFarToNpc, OnPlayerFarToNpc);
        PECommon.Log("MainCitySystem Loading");
    }

    #region PlayerInput
    public void Move(float ver, float hor)
    {
        if(isUIWinOpen)
        {
            pc?.SetMove(Vector3.zero);
            return;
        }
        var dir = (camTrans.forward * ver + camTrans.right * hor);
        
        pc?.SetMove(dir.normalized);

    }

    public void Attack()
    {
        return;
    }

    public void Combo()
    {
        return;
    }

    public void Jump()
    {
        if(isUIWinOpen)
        {
            return;
        }
        pc?.SetJump();
    }

    public void Roll()
    {
        if(isUIWinOpen)
        {
            return;
        }
        pc?.SetRoll();
    }

    public void Sprint(bool isSprint)
    {
        pc?.SetSprint(isSprint);
    }
    #endregion

    #region CommonInput
    public void SetCamLock(bool state)
    {
        if (isUIWinOpen)
        {
            return;
        }

        if (state)
        {
            Cursor.lockState = CursorLockMode.Confined;
            freeLookCam.m_XAxis.m_InputAxisName = "";
            freeLookCam.m_YAxis.m_InputAxisName = "";
            freeLookCam.m_XAxis.m_InputAxisValue = 0f;
            freeLookCam.m_YAxis.m_InputAxisValue = 0f;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
            freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";

        }
    }

    public void SetInteraction()
    {

        if(talkWin.GetWinState())
        {
            talkWin.OnClickNextTalkBtn();
        }

        else if (isUIWinOpen) return;

        else if (canInteract)
        {
            StartTalk();
            mainCityWin.SetBtnTalkActive(false);
        }
        else if (mainCityWin.MenuRootState)
        {
            EnterMenuFunction();
        }
    }

    private void EnterMenuFunction()
    {
        MenuFunction func = (MenuFunction)(mainCityWin.MenuIndex + 1);

        switch (func)
        {
            case MenuFunction.None:
                break;
            case MenuFunction.Task:
                OpenTaskWin();
                break;
            case MenuFunction.Mission:
                OpenMissionWin();
                break;
            case MenuFunction.Make:
                OpenBuyWin(Message.BuyCoin);
                break;
            case MenuFunction.Strong:
                OpenStrongWin();
                break;
            case MenuFunction.Bag:
                OpenBagWin();
                break;
            default:
                break;
        }
    }

    public void SetScrollInteraction(float axis)
    {
        if (isUIWinOpen) return;
        if (axis < 0)
        {
            mainCityWin.ChangeMenuIndex(-1);
        }
        else
        {
            mainCityWin.ChangeMenuIndex(1);
        }
    }

    public void SetOpenMenu()
    {
        if (isUIWinOpen) return;
        mainCityWin.OnClickMenuRoot();
    }
    #endregion
    #region MainCityWin
    public void EnterMainCity()
    {
        MapData = resSvc.GetMapCfgData(Message.MainCityMapID);
        mainCityWin.SetBtnTalkActive(false);
        resSvc.LoadSceneAsync(MapData.sceneName, () =>
        {
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            audioSvc.PlayBGMusic(Message.BGMMainCity);

            PlayerData pd = GameRoot.Instance.PlayerData;
            LoadPlayer(MapData,pd);
            LoadMainViewCam();          
            npcMgr.LoadNPC(MapData.npcs, pc);

            if (charShowCam != null)
            {
                charShowCam.gameObject.SetActive(false);
            }

            if (pd.name == "" || pd.name is null)
            {
                LoadCreateCam();
                createWin.SetWinState();
            }
            else
            {
                mainCityWin.SetWinState();
                InputManager.Instance.SetPlayerInputRoot(this);
                InputManager.Instance.SetCommonInputRoot(this);
            }

            

            PECommon.Log("Enter MainCity");
            
        });
    }   
    public void OpenMissionWin()
    {
        MissionSystem.Instance.OpenMissionWin();
    }

    public void CloseMainCityWin()
    {
        mainCityWin.SetWinState(false);
        npcMgr.OnSceneChange();
        InputManager.Instance.CloseInput();
    }
    #endregion
    #region Player
    public void OnPlayerCloseNPC(params NPCCfg[] args)
    {
        NPCCfg npc = args[0];
        mainCityWin.SetBtnTalkActive(true, npc.name);
        nearNPCData = npc;
        canInteract = true;
    }
    public void OnPlayerFarToNpc(params NPCCfg[] args)
    {
        mainCityWin.SetBtnTalkActive(false);
        canInteract = false;
    }
    public void StartTalk()
    {       
        pointCam.enabled = true;

        pc.OnPlayerTalk();
        npcMgr.Interactive(nearNPCData.ID);
        talkWin.InitTalkData(nearNPCData.ID);
        mainCityWin.SetWinState(false);
        talkWin.SetWinState();
    }
    public void OnPlayerOverTalk(params int[] args)
    {
        pc.OnPlayerOverTalk();
        mainCityWin.SetWinState();
        pointCam.enabled = false;
    }

    public void ChangePlayerEquipment(int itemID)
    {
        pc.ChangeEquipment(resSvc.GetGameItemCfg(itemID));      
    }

    public void OpenTaskWin()
    {
        TaskSystem.Instance.OpenOwnerTaskWin();      
    }

    public void OpenBagWin()
    {
        BagSystem.Instance.OpenBagWin();
    }
    #endregion
    #region LoadSetting
    private void LoadPlayer(MapCfg mapData, PlayerData pd)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.DogStandard, true);
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.position = mapData.playerBornPos;
        pc = player.GetComponent<PlayerController>();

        var equipmentArr = pd.equipment.Split('|');
        var index_weapon = resSvc.GetGameItemCfg(int.Parse(equipmentArr[0])).objPath;
        var index_shield = resSvc.GetGameItemCfg(int.Parse(equipmentArr[1])).objPath;
        pc.InitPlayer(pd.modle, $"{index_weapon}|{index_shield}");      
    }

    private void LoadMainViewCam()
    {        
        freeLookCam = resSvc.LoadPrefab(PathDefine.FreeLookCam1).GetComponent<CinemachineFreeLook>();
        freeLookCam.LookAt = pc.transform;
        freeLookCam.Follow = pc.transform;

        pointCam = resSvc.LoadPrefab(PathDefine.PointCam).GetComponent<CinemachineVirtualCamera>();
        pointCam.LookAt = pc.transform;
        pointCam.Follow = pc.transform;
        pointCam.enabled = false;

        camTrans = Camera.main.transform;
    }

    private void LoadCreateCam()
    {
        createCam = resSvc.LoadPrefab(PathDefine.CreateCam).GetComponent<CinemachineVirtualCamera>();
    }
    public void SetMoveDir(Vector2 dir)
    {
        //StopNavTask();
        //if (dir == Vector2.zero)
        //{
        //    pc.SetBlend(Message.BlendIdle);
        //}
        //else
        //{
        //    pc.SetBlend(Message.BlendWalk);
        //}
        //pc.Dir = dir;
    }
    private float startRoate = 0;
    public void SetStartRoate()
    {
        startRoate = pc.transform.localEulerAngles.y;
    }
    public void SetPlayerRoate(float roate)
    {
        pc.transform.localEulerAngles = new Vector3(0, startRoate + roate, 0);
    }
    #endregion

    #region InfoWin
    public void OpenInfoWin()
    {
        //StopNavTask();
        if (charShowCam == null)
        {
            charShowCam = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }
        charShowCam.localPosition = pc.transform.position + pc.transform.forward * 2.6f + new Vector3(0, 1.3f, 0);
        charShowCam.transform.localEulerAngles = new Vector3(0, 180 + pc.transform.localEulerAngles.y, 0);
        charShowCam.localScale = Vector3.one;

        charShowCam.gameObject.SetActive(true);
        infoWin.SetWinState();
    }

    public void CloseInfoWin()
    {
        if (charShowCam != null)
        {
            charShowCam.gameObject.SetActive(false);
        }
        infoWin.SetWinState(false);       
    }
    #endregion

    #region StrongWin

    public void OpenStrongWin()
    {        
        strongWin.SetWinState();
    }

    private void OpenStrongWin_EventAction(params int[] args)
    {
        int actionId = args[1];
        if(actionId == (int)NPCFunction.OpenStrongWin)
        {
            OpenStrongWin();
        }
    }

    public void RspStrong(GameMsg msg)
    {
        int lastFight = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
        int currentFight = PECommon.GetFightByProps(GameRoot.Instance.PlayerData);

        GameRoot.AddTips(Message.Color("战力增加   " + (currentFight - lastFight), Message.ColorBlue));

        strongWin.RefreshUI();
        mainCityWin.RefreshUI();
    }

    #endregion

    #region BuyWin
    public void OpenBuyWin(int type)
    {
        
        buyWin.SetBuyType(type);
        buyWin.SetWinState();
        //StopNavTask();
    }

    public void RspBuy(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByBuy(msg.rspBuy);
        mainCityWin.RefreshUI();
        buyWin.SetWinState(false);

        //if(msg.pushTaskPrgs != null)
        //{
        //    PushTaskPrgs(msg);
        //}
    }
    #endregion

    #region ChatWin
    public void PushChat(GameMsg msg)
    {
        PushChat data = msg.pushCHat;
        chatWin.AddChatMsg(data);

    }

    public void OpenChatWin()
    {
        chatWin.SetWinState();
        //StopNavTask();
    }
    #endregion

    #region Power
    public void PushPower(GameMsg msg)
    {
        PushPower data = msg.pushPower;
        GameRoot.Instance.SetPlayerDataByPower(data);
        if (mainCityWin.gameObject.activeSelf)
        {
            mainCityWin.RefreshUI();
        }
    }
    #endregion

    #region Create Role

    public void ChangeRole(int index)
    {
        pc.ChangeMesh(MeshType.Role, index);
    }

    public void ChangeBody(int index)
    {
        pc.ChangeMesh(MeshType.Body, index);
    }

    public void OnRenameRsp(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByCreate(msg.rspRename);      
        createWin.SetWinState(false);
        createCam.enabled = false;
        mainCityWin.SetWinState();
        InputManager.Instance.SetPlayerInputRoot(this);
        InputManager.Instance.SetCommonInputRoot(this);
    }
    #endregion

    private void SetCamLock(params bool[] args)
    {      
        bool isLock = args[0];
        isUIWinOpen = isLock;

        if (isLock)
        {
            Cursor.lockState = CursorLockMode.Confined;
            if (freeLookCam != null)
            {
                freeLookCam.m_XAxis.m_InputAxisName = "";
                freeLookCam.m_YAxis.m_InputAxisName = "";
                freeLookCam.m_YAxis.m_InputAxisValue = 0f;
                freeLookCam.m_XAxis.m_InputAxisValue = 0f;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            if(freeLookCam != null)
            {
                freeLookCam.m_XAxis.m_InputAxisName = "Mouse X";
                freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
            }
            
        }
    }

    public string GetPlayerModleIndex()
    {
        return pc?.GetModleIndex_Str();
    }



}
