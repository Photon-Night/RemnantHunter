using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Monster;
using Cinemachine;

public class BattleManager : MonoBehaviour
{
    ResService resSvc = null;
    AudioService audioSvc = null;
    TimerService timeSvc = null;

    //MapManager mapMgr = null;
    SkillManager skillMgr = null;
    StateManager stateMgr = null;

    public EntityPlayer ep;
    private MapCfg mapData;

    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();
    private List<MonsterGroup> groups;
    private List<EntityMonster> activeMonsterLst = new List<EntityMonster>();

    private Transform camTrans;
    private CinemachineFreeLook freeLookCam;
    private CinemachineVirtualCamera battleEndCam;
    private bool runAI = false;

    private float skillInputSpace = .1f;
    private float skillInputTimeOrigin = .1f;

    void Update()
    {
        if (!runAI || ep == null) return;

        for(int i = 0; i < groups.Count; i++)
        {
            groups[i].TickGroupLogic(ep.GetTrans());
        }

        if(skillInputTimeOrigin < skillInputSpace)
        {
            skillInputTimeOrigin += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        ep?.RecoverPower(Time.fixedDeltaTime);
        ep?.RecoverSkillPoint(Time.fixedDeltaTime);
    }


    public void InitManager(int mapId, Action CB = null)
    {
        PECommon.Log("BattleManager Loading");
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;
        timeSvc = TimerService.Instance;

        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.InitManager();
        stateMgr = gameObject.AddComponent<StateManager>();
        stateMgr.InitManager();
    

        mapData = resSvc.GetMapCfgData(mapId);
        resSvc.LoadSceneAsync(mapData.sceneName, () =>
        {
            //GameObject map = GameObject.FindGameObjectWithTag("MapRoot");

            //mapMgr = map.gameObject.GetComponent<MapManager>();
            //mapMgr.InitManager(this);

            //map.transform.position = Vector3.zero;
            //map.transform.localScale = Vector3.one;

            //Camera.main.transform.position =mapData.mainCamPos;
            //Camera.main.transform.localEulerAngles = mapData.mainCamRote;
            //camTrans = Camera.main.transform;
            LoadPlayer();
            LoadCam();
            LoadGroup();           

            audioSvc.PlayBGMusic(Message.BGHuangYe);

            if(CB != null)
            {
                CB();
            }
        });
    }

    #region Loading Seting
    public void LoadPlayer()
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.DogStandard, true);
        player.transform.localPosition = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;

        //Camera.main.transform.position = mapData.mainCamPos;
        //Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        PlayerData pd = GameRoot.Instance.PlayerData;
        PlayerController pc = player.GetComponent<PlayerController>();
        var equipmentArr = pd.equipment.Split('|');
        int weaponID = int.Parse(equipmentArr[0]);
        int shieldID = int.Parse(equipmentArr[1]);
        var index_weapon = resSvc.GetGameItemCfg(weaponID).objPath;
        var index_shield = resSvc.GetGameItemCfg(shieldID).objPath;
        pc.InitPlayer(pd.modle, $"{index_weapon}|{index_shield}");

        BattleProps props = new BattleProps
        {
            power = 100 + pd.lv - 1, 
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            critical = pd.critical,
            pierce = pd.critical,
        };

        ep = new EntityPlayer(this, stateMgr, pc, props, pd.name, weaponID, shieldID);
        
    }
    private void LoadCam()
    {
        freeLookCam = resSvc.LoadPrefab(PathDefine.FreeLookCam1).GetComponent<CinemachineFreeLook>();
        freeLookCam.LookAt = ep.GetTrans();
        freeLookCam.Follow = ep.GetTrans();

        camTrans = Camera.main.transform;
    }

    public void SetLockCam(bool state)
    {
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

    public void LoadGroup()
    {
        groups = new List<MonsterGroup>();

        List<GroupData> groupLst = mapData.monsterGroups;
        for (int i = 0; i < groupLst.Count; i++)
        {
            MonsterGroup group = new MonsterGroup();
            group.InitGroup(groupLst[i], ep, stateMgr);

            GameObject go = new GameObject($"group{groupLst[i].ID}");
            go.transform.position = group.GroupPos;

            group.AddMonsters(LoadMonsterByGroup(groupLst[i].monsters, go.transform, group.GroupID));
            groups.Add(group);
        }

        runAI = true;
    }

    private List<EntityMonster> LoadMonsterByGroup(List<MonsterData> monsters, Transform parent, int groupID)
    {
        List<EntityMonster> entityMonsters = new List<EntityMonster>();
        for(int i = 0; i < monsters.Count; i++)
        {
            MonsterData data = monsters[i];
            MonsterCfg cfg = data.mCfg;
            GameObject go = resSvc.LoadPrefab(cfg.resPath, true);
            go.name = $"{cfg.mName}_{parent.name}_{i}";
            go.transform.SetParent(parent);
            go.transform.localPosition = data.mBornPos;
            Debug.Log(data.mBornPos);
            go.transform.localEulerAngles = data.mBornRote;

            MonsterController mc = go.GetComponent<MonsterController>();
            mc.Init();

            EntityMonster em = new EntityMonster(this, stateMgr, mc, cfg.bps, cfg.ID);
            em.InitMonster(data, ep, groupID);
            
            monstersDic.Add(em.Name, em);

            if (em.md.mCfg.mType == MonsterType.Normal)
                GameRoot.Instance.AddHpUIItem(go.name, em.Props.hp, mc.hpRoot);
            else if (em.md.mCfg.mType == MonsterType.Boss)
                BattleSystem.Instance.SetMonsterHPState(true);

            entityMonsters.Add(em);
        }

        return entityMonsters;

    }
    public void RemoveMonster(string name)
    {
        if(monstersDic.ContainsKey(name))
        {
            Debug.Log(name);
            monstersDic.Remove(name);
            
            GameRoot.Instance.RemoveHpUIItem(name);
        }

        Debug.Log(monstersDic.Count);
        if(monstersDic.Count == 0)
        {
            OnBattleOver();
        }
    }
    public void ActiveCurrentBatchMonsters()
    {
        //TimerService.Instance.AddTimeTask((int tid) =>
        //{
        //    foreach (var monster in monstersDic)
        //    {
        //        monster.Value.GetTrans().gameObject.SetActive(true);
        //        monster.Value.Born();
        //
        //        TimerService.Instance.AddTimeTask((int tid) =>
        //        {
        //            monster.Value.Idle();
        //        }, 1000);
        //    }
        //}, 500);
    }
    #endregion

    #region Battle Setting
    private void OnBattleOver()
    {
        StopBattle(true, ep.HP);
    }

    public void StopBattle(bool isWin, int restHP)
    {
        AudioService.Instance.StopBGAudio();
        BattleSystem.Instance.EndBattle(isWin, restHP);
    }
    #endregion

    #region Action Interface

    public List<EntityMonster> GetActiveMonsters()
    {
        activeMonsterLst.Clear();

        for(int i=  0; i < groups.Count; i++)
        {
            if(groups[i].IsActive)
            {
                activeMonsterLst.AddRange(groups[i].MonsterLst);
            }
        }

        return activeMonsterLst;
    }
    public EntityMonster FindClosedMonster(float distance = 0f)
    {
        float min_dis = float.MaxValue;
        EntityMonster cloest = null;

        for(int i = 0; i < groups.Count; i++)
        {
            if(groups[i].IsActive)
            {
                float dis = 0;
                var monster = groups[i].GetClosestMonsterToPlayer(out dis);

                if(dis < min_dis)
                {
                    cloest = monster;
                    min_dis = dis;
                }

            }
        }
        return distance < min_dis ? null : cloest;
    }

    public bool isPlayerAttack()
    {
        return ep.isAttack();
    }

    public void SetHPUI(int hp)
    {
        BattleSystem.Instance.SetHPUI(hp);
    }

    #endregion

    #region Player Control
    public void SetMove(float ver, float hor)
    {
        var dir = (camTrans.forward * ver + camTrans.right * hor).normalized;     
        ep?.SetMove(dir);
    }

    public void SetNormalAttack()
    {
        ep?.SetAttack();
    }

    public void SetCombo()
    {
        if(skillInputTimeOrigin >= skillInputSpace)
        {
            ep?.SetCombo();
            skillInputTimeOrigin = 0f;
        }     
    }

    public void SetJump()
    {
        ep?.SetJump();
    }

    public void SetRoll()
    {
        if (ep != null && ep.SetRoll())
        {

        }
    }

    public void SetSprint(bool isSprint)
    {
        ep?.SetSprint(isSprint);
    }

    #endregion

    #region Battle interface
    public void SetPlayerPropsByPotion(ItemFunction type, float funcNum, float duration)
    {
        switch (type)
        {                    
            case ItemFunction.Health:
                ep.HP += (int)funcNum;
                break;
            case ItemFunction.ADAtk:
                ep.SetADAtkByPotion((int)funcNum, duration);
                break;
            case ItemFunction.Stamina:
                break;
            case ItemFunction.ADDef:
                ep.SetADDefByPotion((int)funcNum, duration);
                break;
            case ItemFunction.Dodge:
                break;
            case ItemFunction.APAtk:
                break;
            case ItemFunction.APDef:
                break;
            default:
                break;
        }
    } 

    public bool GetPotionUseStatus(ItemFunction type)
    {
        bool res = false;
        switch (type)
        {
            case ItemFunction.Health:
                res = ep.HP < ep.Props.hp;
                break;
            case ItemFunction.ADAtk:
                res = ep.CanUsePotion_ADAtk;
                break;
            case ItemFunction.Stamina:
                break;
            case ItemFunction.ADDef:
                res = ep.CanUsePotion_AdDef;
                break;
            case ItemFunction.Dodge:
                break;
            case ItemFunction.APAtk:
                break;
            case ItemFunction.APDef:
                break;
        }

        return res;
    }
    #endregion

    public void SkillAttack(EntityBase entity, SkillData skill, int dmgIndex)
    {
        skillMgr.SkillAttack(entity, skill, dmgIndex);
    }

    public void ChangePlayerEquipment(int itemID)
    {
        ep?.ChangePlayerEquipment(resSvc.GetGameItemCfg(itemID));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(groups != null)
        {
            Gizmos.DrawWireSphere(groups[0].GroupPos, groups[0].range1);
            
            Gizmos.DrawWireSphere(groups[0].GroupPos, groups[1].range2);
        }
    }
}

