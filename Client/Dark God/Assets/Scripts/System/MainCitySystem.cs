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

    public MainCityWin mainCityWin;
    public InfoWin infoWin;
    public TalkWin talkWin;
    public StrongWin strongWin;
    public ChatWin chatWin;
    public BuyWin buyWin;

    public System.Action<int> OnTalkOverEvent;

    private PlayerController pc = null;
    private Transform charShowCam = null;

    private NPCCfg nearNPCData = null;
    private NPCManager npcMgr = null;
    public override void InitSystem()
    {
        base.InitSystem();
        npcMgr = NPCManager.Instance;
        PECommon.Log("MainCitySystem Loading");
    }

    #region PlayerInput
    public void Move(float ver, float hor)
    {
        pc.SetMove(ver, hor);
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
        MapCfg mapData = resSvc.GetMapCfgData(Message.MainCityMapID);
        mainCityWin.SetBtnTalkActive(false);
        resSvc.LoadSceneAsync(mapData.sceneName, () =>
        {
            LoadPlayer(mapData);
            npcMgr.LoadNPC(ref mapData.npcs, pc);

            LoadCam();
     

            mainCityWin.SetWinState();
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            audioSvc.PlayBGMusic(Message.BGMMainCity);

            if (charShowCam != null)
            {
                charShowCam.gameObject.SetActive(false);
            }

            InputManager.Instance.SetInputRoot(this);
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
    }

    public void OpenTaskWin()
    {
        TaskSystem.Instance.OpenOwnerTaskWin();
    }
    #endregion
    #region LoadSetting
    private void LoadPlayer(MapCfg mapData)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.DogStandard, true);
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.position = mapData.playerBornPos;
        //Camera.main.transform.position = mapData.mainCamPos;
        //Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        pc = player.GetComponent<PlayerController>();
        //agent = player.GetComponent<NavMeshAgent>();
        pc.Init();

        
    }

    private void LoadCam()
    {
        GameObject cam = resSvc.LoadPrefab(PathDefine.FreeLookCam1);
        var fl = cam.GetComponent<CinemachineFreeLook>();
        fl.LookAt = pc.transform;
        fl.Follow = pc.transform;
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

    #region GuideSetting
    
    #endregion

    #region StrongWin

    public void OpenStrongWin()
    {
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
}
