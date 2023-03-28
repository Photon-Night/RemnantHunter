using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.AI;
using Game.Common;
using Game.Manager;
using Cinemachine;

public class MainCitySystem : SystemRoot<MainCitySystem>, IPlayerInputSet
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

    public System.Action<int> OnTalkOverEvent;
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

    public override void InitSystem()
    {
        base.InitSystem();
        npcMgr = NPCManager.Instance;
        PECommon.Log("MainCitySystem Loading");
    }

    #region PlayerInput
    public void Move(float ver, float hor)
    {

        var dir = (camTrans.forward * ver + camTrans.right * hor);
        dir.y = 0;
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
        pc?.SetJump();
    }

    public void Roll()
    {
        return;
    }

    public void Sprint(bool isSprint)
    {
        pc?.SetSprint(isSprint);
    }
    #endregion
    #region MainCityWin
    public void EnterMainCity()
    {
        MapData = resSvc.GetMapCfgData(Message.MainCityMapID);
        mainCityWin.SetBtnTalkActive(false);
        resSvc.LoadSceneAsync(MapData.sceneName, () =>
        {
            PlayerData pd = GameRoot.Instance.PlayerData;
            LoadPlayer(MapData,pd);
            LoadMainViewCam();          
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            audioSvc.PlayBGMusic(Message.BGMMainCity);
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
                InputManager.Instance.SetInputRoot(this);
            }
            PECommon.Log("Enter MainCity");
        });
    }   
    public void OpenMissionWin()
    {
        MissionSystem.Instance.OpenMissionWin();
        freeLookCam.enabled = false;
    }

    public void CloseMainCityWin()
    {
        mainCityWin.SetWinState(false);
        npcMgr.OnSceneChange();
        InputManager.Instance.CloseInput();
    }
    #endregion
    #region Player
    public void OnPlayerCloseNPC(NPCCfg npc)
    {
        mainCityWin.SetBtnTalkActive(true, npc.name);
        nearNPCData = npc;
    }
    public void FarToPlayer()
    {
        mainCityWin.SetBtnTalkActive(false);
    }
    public void StartTalk()
    {
        pointCam.enabled = true;
        pc.OnPlayerTalk();
        npcMgr.Interactive(nearNPCData.ID);
        talkWin.InitTalkData(nearNPCData.ID);
        talkWin.SetWinState();
        mainCityWin.SetWinState(false);
    }
    public void OnPlayerOverTalk(int id)
    {
        if (OnTalkOverEvent != null)
            OnTalkOverEvent(id);

        pc.OnPlayerOverTalk();
        mainCityWin.SetWinState();
        pointCam.enabled = false;
    }

    public void OpenTaskWin()
    {
        TaskSystem.Instance.OpenOwnerTaskWin();
        freeLookCam.enabled = false;
    }
    #endregion
    #region LoadSetting
    private void LoadPlayer(MapCfg mapData, PlayerData pd)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.DogStandard, true);
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.position = mapData.playerBornPos;
        //Camera.main.transform.position = mapData.mainCamPos;
        //Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        pc = player.GetComponent<PlayerController>();
        //agent = player.GetComponent<NavMeshAgent>();
        
        pc.InitPlayer(pd.modle);      
    }

    private void LoadMainViewCam()
    {        
        freeLookCam = resSvc.LoadPrefab(PathDefine.FreeLookCam1).GetComponent<CinemachineFreeLook>();
        freeLookCam.LookAt = pc.transform;
        freeLookCam.Follow = pc.transform;

        pointCam = resSvc.LoadPrefab(PathDefine.PointCam).GetComponent<CinemachineVirtualCamera>();
        pointCam.LookAt = pc.transform;
        pointCam.LookAt = pc.transform;
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
        freeLookCam.enabled = false;
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
        EnableCam();
    }
    #endregion

    #region StrongWin

    public void OpenStrongWin()
    {
        freeLookCam.enabled = false;
        strongWin.SetWinState();
       // StopNavTask();
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
        freeLookCam.enabled = false;
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

    public void ChangeRole()
    {
        pc.ChangeMesh(MeshType.Role);
    }

    public void ChangeBody()
    {
        pc.ChangeMesh(MeshType.Body);
    }

    public void OnRenameRsp(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByCreate(msg.rspRename);      
        createWin.SetWinState(false);
        createCam.enabled = false;
        mainCityWin.SetWinState();
        InputManager.Instance.SetInputRoot(this);
    }
    #endregion

    public void EnableCam()
    {
        freeLookCam.enabled = true;
    }

    public string GetPlayerModleIndex()
    {
        return pc?.GetModleIndex_Str();
    }
}
