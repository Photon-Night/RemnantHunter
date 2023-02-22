using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocol;
using UnityEngine.AI;

public class MainCitySystem : SystemRoot<MainCitySystem>
{

    public MainCityWin mainCityWin;
    public InfoWin infoWin;
    public GuideWin guideWin;
    public StrongWin strongWin;
    public ChatWin chatWin;
    public BuyWin buyWin;
    //public TaskWin taskWin;

    private PlayerController pc = null;
    private Transform charShowCam = null;

    private NPCCfg nearNPCData = null;
    private NPCManager npcMgr = null;

    //private Transform[] npcPosTrans;

    //private NavMeshAgent agent;

    private GuideCfg currentTaskData = null;

    private bool isGuide;
    public override void InitSystem()
    {
        base.InitSystem();
       
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

            npcMgr = map.AddComponent<NPCManager>();
            npcMgr.InitManager();
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
        GuideCfg data = resSvc.GetGuideCfgData(nearNPCData.ID);
        npcMgr.Interactive(nearNPCData.ID);
        guideWin.SetTalkData(data);
        guideWin.SetWinState();
        guideWin.RegisterTalkOverEvent(() =>
        {
            pc.OnPlayerOverTalk();
        });
    }

    public void OpenTaskWin()
    {
        TaskSystem.Instance.OpenNpcTaskWin(nearNPCData.ID);
    }

    public void RegisterTalkOverEvent(System.Action func)
    {
        guideWin.RegisterTalkOverEvent(func);
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
    //public void RunTask(GuideCfg gc)
    //{
    //    if (gc != null)
    //    {
    //        currentTaskData = gc;
    //    }
    //    if (currentTaskData.npcID != -1)
    //    {
    //        float dis = Vector3.Distance(pc.transform.position, npcPosTrans[gc.npcID].position);
    //        if (dis < 1f)
    //        {
    //            isGuide = false;
    //            agent.enabled = false;
    //            agent.isStopped = true;
    //            pc.SetBlend(Message.BlendIdle);
    //
    //            OpenGuideWin();
    //        }
    //        else
    //        {
    //            isGuide = true;
    //            agent.enabled = true;
    //            agent.speed = Message.PlayerMoveSpeed;
    //            agent.SetDestination(npcPosTrans[gc.npcID].position);
    //            pc.SetBlend(Message.BlendWalk);
    //        }
    //    }
    //    else
    //    {
    //        OpenGuideWin();
    //    }
    //}

    //private void IsArriveNavPos()
    //{
    //    float dis = Vector3.Distance(pc.transform.position, npcPosTrans[currentTaskData.npcID].position);
    //    if (dis <= 1f)
    //    {
    //        StopNavTask();
    //
    //        OpenGuideWin();
    //    }
    //}

   //private void StopNavTask()
   //{
   //    if (isGuide)
   //    {
   //        isGuide = false;
   //        agent.isStopped = true;
   //        agent.enabled = false;
   //        pc.SetBlend(Message.BlendIdle);
   //    }
   //
   //}

    //public void Update()
    //{
    //    //if (isGuide)
    //    //{
    //    //    IsArriveNavPos();
    //    //    pc.SetCam();
    //    //}
    //}
    //private void OpenGuideWin()
    //{
    //    guideWin.SetWinState();
    //}

    //public GuideCfg GetCurrentTaskData()
    //{
    //    return currentTaskData;
    //}

    //public void RspGuide(GameMsg msg)
    //{
    //    GameRoot.AddTips("任务完成");
    //    GameRoot.AddTips("金币 " + msg.rspGuide.coin);
    //    GameRoot.AddTips("经验 " + msg.rspGuide.exp);
    //    switch (currentTaskData.actID)
    //    {
    //        case 0:
    //            //���߶Ի�
    //            break;
    //        case 1:
    //            //ǿ��װ��
    //            OpenStrongWin();
    //            break;
    //        case 2:
    //            //�򿪸���
    //            OpenMissionWin();
    //            break;
    //
    //        case 3:
    //            //��������
    //            OpenBuyWin(0);
    //            break;
    //
    //        case 4:
    //            //������
    //            OpenBuyWin(1);
    //            break;
    //
    //        case 5:
    //            //��������
    //            OpenChatWin();
    //            break;
    //
    //        default:
    //            break;
    //
    //    }
    //
    //    GameRoot.Instance.SetPlayerDataByGuide(msg.rspGuide);
    //    mainCityWin.RefreshUI();
    //}
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
    //public void OpenTaskWin()
    //{
    //    taskWin.SetWinState();
    //    //StopNavTask();
    //}

    //public void RspTakeTaskReward(GameMsg msg)
    //{
    //    RspTakeTaskReward data = msg.rspTakeTaskReward;
    //    PlayerData pd = GameRoot.Instance.PlayerData;
    //
    //    GameRoot.Instance.SetPlayerDataByTakeTaskReward(data);
    //
    //    taskWin.RefreshUI();
    //    mainCityWin.RefreshUI();
    //
    //}
    //
    //public void PushTaskPrgs(GameMsg msg)
    //{
    //    PushTaskPrgs data = msg.pushTaskPrgs;
    //    GameRoot.Instance.SetPlayerDataByTaskPrgs(data);
    //}
    #endregion
}
