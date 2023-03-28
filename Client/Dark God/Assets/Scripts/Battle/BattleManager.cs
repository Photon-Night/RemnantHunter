using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Monster;

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
    private MonsterAIController monsterAI;

    private Transform camTrans;
     void Update()
    {
        if (monsterAI != null) return;

        foreach (var group in groups)
        {
            monsterAI.LogicRoot(group);
        }
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
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");

            //mapMgr = map.gameObject.GetComponent<MapManager>();
            //mapMgr.InitManager(this);

            map.transform.position = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position =mapData.mainCamPos;
            Camera.main.transform.localEulerAngles = mapData.mainCamRote;
            camTrans = Camera.main.transform;
            LoadPlayer();
            LoadGroup();

            monsterAI = new MonsterAIController();
            ActiveCurrentBatchMonsters();

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
        GameObject player = resSvc.LoadPrefab(PathDefine.DogKnight, true);
        player.transform.localPosition = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;

        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        PlayerController pc = player.GetComponent<PlayerController>();
        pc.Init();

        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps
        {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            critical = pd.critical,
            pierce = pd.critical,
        };

        ep = new EntityPlayer(pc, props);
        ep.InitPlayer(this, skillMgr, stateMgr, pd.name);

    }

    public void LoadGroup()
    {
        List<GroupData> groupLst = mapData.monsterGroups;
        for (int i = 0; i < groupLst.Count; i++)
        {
            MonsterGroup group = new MonsterGroup();
            group.InitGroup(groupLst[i], ep.GetTrans());
            group.AddMonsters(LoadMonsterByGroup(groupLst[i].monsters));

            groups.Add(group);
        }      
    }

    private List<EntityMonster> LoadMonsterByGroup(List<MonsterData> monsters)
    {
        List<EntityMonster> entityMonsters = new List<EntityMonster>();
        for(int i = 0; i < monsters.Count; i++)
        {
            MonsterData data = monsters[i];
            MonsterCfg cfg = data.mCfg;
            GameObject go = resSvc.LoadPrefab(cfg.resPath, true);
            go.name = $"{cfg.mName}_{i}";
            go.transform.position = data.mBornPos;
            go.transform.localEulerAngles = data.mBornRote;

            MonsterController mc = go.AddComponent<MonsterController>();
            mc.Init();

            EntityMonster em = new EntityMonster(mc, cfg.bps, cfg.ID);
            em.InitMonster(this, skillMgr, stateMgr, data);

            if (em.md.mCfg.mType == Message.MonsterType.Normal)
                GameRoot.Instance.AddHpUIItem(go.name, em.Props.hp, mc.hpRoot);
            else if (em.md.mCfg.mType == Message.MonsterType.Boss)
                BattleSystem.Instance.SetMonsterHPState(true);

            entityMonsters.Add(em);
        }

        return entityMonsters;
    }
    public void RemoveMonster(string name)
    {
        EntityMonster _monster = null;
        if(monstersDic.TryGetValue(name, out _monster))
        {
            monstersDic.Remove(name);
            GameRoot.Instance.RemoveHpUIItem(name);
        }
        

        if(monstersDic.Count == 0)
        {
            OnBattleOver();
        }
    }
    public void ActiveCurrentBatchMonsters()
    {
        TimerService.Instance.AddTimeTask((int tid) =>
        {
            foreach (var monster in monstersDic)
            {
                monster.Value.GetTrans().gameObject.SetActive(true);
                monster.Value.Born();

                TimerService.Instance.AddTimeTask((int tid) =>
                {
                    monster.Value.Idle();
                }, 1000);
            }
        }, 500);
    }
    #endregion

    #region Battle Setting
    private void OnBattleOver()
    {
        //bool isExit = mapMgr.SetNextTrigger();
        //if(isExit)
        //{
        //    StopBattle(true, ep.HP);
        //}
    }

    public void StopBattle(bool isWin, int restHP)
    {
        AudioService.Instance.StopBGAudio();
        BattleSystem.Instance.EndBattle(isWin, restHP);
    }
    #endregion

    #region Action Interface

    public List<EntityMonster> GetMonsterLst()
    {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        var e = monstersDic.GetEnumerator();
        while(e.MoveNext())
        {
            monsterLst.Add(e.Current.Value);
        }

        e.Dispose();
        return monsterLst;
    }

    public EntityMonster FindClosedMonster(Transform centerTran, float distance = 0f)
    {
        float closedDis = float.MaxValue;
        float _dis = 0f;
        List<EntityMonster> monsters = GetMonsterLst();
        EntityMonster target = null;

        if (monsters == null || monsters.Count == 0)
            return null;

        for (int i = 0; i < monsters.Count; i++)
        {
            _dis = Vector3.Distance(centerTran.position, monsters[i].GetPos());
            if(_dis < closedDis)
            {
                closedDis = _dis;
                target = monsters[i];
            }
        }

        return target;
    }

    public bool isPlayerAttack()
    {
        return ep.isAttack();
    }

    public void SetHPUI(int hp)
    {
        BattleSystem.Instance.SetHPUI(hp);
    }
    
    public void OnTargetDie(int targetId)
    {
        BattleSystem.Instance.onTargetDie(targetId);
    }

    #endregion

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
        ep?.SetCombo();
    }

    public void SetJump()
    {
        ep.SetJump();
    }

    public void SetRoll()
    {
        if(ep.SetRoll())
        {

        }
    }

    public void SetSprint(bool isSprint)
    {
        ep.SetSprint(isSprint);
    }

}

