using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.AI;

public class MainCitySystem : SystemRoot<MainCitySystem>
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
    public void EnterMainCity()
    {
        MapCfg mapData = resSvc.GetMapCfgData(Message.MainCityMapID);
        mainCityWin.SetBtnTalkActive(false);
        resSvc.LoadSceneAsync(mapData.sceneName, () =>
        {
            PECommon.Log("Enter MainCity");

            //TODO ���ؽ�ɫģ��
            LoadPlayer(mapData);
            //��������ui
            mainCityWin.SetWinState();
            GameRoot.Instance.GetComponent<AudioListener>().enabled = false;
            audioSvc.PlayBGMusic(Message.BGMMainCity);

            //TODO ��������

            if (charShowCam != null)
            {
                charShowCam.gameObject.SetActive(false);
            }

            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");

            
            npcMgr.LoadNPC(ref mapData.npcs, pc);
        });
    }

    public void OpenMissionWin()
    {
        MissionSystem.Instance.OpenMissionWin();
        //StopNavTask();
    }

    public void CloseMainCityWin()
    {
        mainCityWin.SetWinState(false);
    }
    #region MainWin
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
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnCityPlayerPrefab, true);
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.position = mapData.playerBornPos;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        pc = player.GetComponent<PlayerController>();
        //agent = player.GetComponent<NavMeshAgent>();
        pc.Init();

        
    }

    public void SetMoveDir(Vector2 dir)
    {
        //StopNavTask();
        if (dir == Vector2.zero)
        {
            pc.SetBlend(Message.BlendIdle);
        }
        else
        {
            pc.SetBlend(Message.BlendWalk);
        }
        pc.Dir = dir;
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

    #region Task
   
    #endregion
}
