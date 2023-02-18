using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class BattleManager : MonoBehaviour
{
    ResService resSvc = null;
    AudioService audioSvc = null;
    TimerService timeSvc = null;

    MapManager mapMgr = null;
    SkillManager skillMgr = null;
    StateManager stateMgr = null;

    public EntityPlayer ep;
    private MapCfg mapData;

    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();

     void Update()
    {
        foreach (var monster in monstersDic)
        {
            monster.Value.TickAILogic();
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

            mapMgr = map.gameObject.GetComponent<MapManager>();
            mapMgr.InitManager(this);

            map.transform.position = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position =mapData.mainCamPos;
            Camera.main.transform.localEulerAngles = mapData.mainCamRote;

            LoadPlayer();

            ActiveCurrentBatchMonsters();

            audioSvc.PlayBGMusic(Message.BGHuangYe);

            if(CB != null)
            {
                CB();
            }
        });
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

    private void OnBattleOver()
    {
        bool isExit = mapMgr.SetNextTrigger();
        if(isExit)
        {
            StopBattle(true, ep.HP);
        }
    }

    public void StopBattle(bool isWin, int restHP)
    {
        AudioService.Instance.StopBGAudio();
        BattleSystem.Instance.EndBattle(isWin, restHP);
    }

    public void LoadPlayer()
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab, true);
        player.transform.localPosition = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

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

        ep = new EntityPlayer
        {
            stateMgr = stateMgr,
            skillMgr = skillMgr,
            battleMgr = this,
            Name = pd.name,
        };
        ep.SetController(pc);
        ep.SetBattleProps(props);
        ep.CurrentState = AniState.Idle;

        //AudioService.Instance.AddAudio(player.name, pc.GetAudio());
    }

    public void LoadMonsterByWaveID(int waveIndex)
    {
        List<MonsterData> monsterLst = mapData.monsterLst;

        for(int i = 0; i < monsterLst.Count; i++)
        {
            if(monsterLst[i].mWave == waveIndex)
            {
                MonsterData mData = monsterLst[i];
                MonsterCfg cfg = mData.mCfg;
                GameObject go = resSvc.LoadPrefab(cfg.resPath, true);
                go.name = cfg.mName + "_" + waveIndex + "_" + i;
                go.transform.position = mData.mBornPos;
                go.transform.localEulerAngles = mData.mBornRote;

                EntityMonster em = new EntityMonster
                {
                    skillMgr = this.skillMgr,
                    battleMgr = this,
                    stateMgr = this.stateMgr
                    
                };

                MonsterController mc = go.GetComponent<MonsterController>();
                mc.Init();
                em.SetController(mc);
                em.md = mData;
                em.SetBattleProps(cfg.bps);
                em.Name = go.name;
                go.SetActive(false);
                monstersDic.Add(go.name, em);

                if (em.md.mCfg.mType == Message.MonsterType.Normal)
                    GameRoot.Instance.AddHpUIItem(go.name, em.Props.hp, mc.hpRoot);
                else if (em.md.mCfg.mType == Message.MonsterType.Boss)
                    BattleSystem.Instance.SetMonsterHPState(true);
            }
        }
    }

    public void ActiveCurrentBatchMonsters()
    {
        TimerService.Instance.AddTimeTask((int tid) => 
        {
            foreach(var monster in monstersDic)
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

    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAttack();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    public void SetMoveDir(Vector2 dir)
    {
        if (ep.LockCtrl)
        {
            return;
        }

        if(dir != Vector2.zero)
        {
            ep.Move();
            ep.SetDir(dir);
        }
        else
        {
            ep.Idle();
            ep.SetDir(Vector2.zero);
        }
    }

    private void ReleaseSkill1()
    {
        
        ep.Attack(101);
    }

    private void ReleaseSkill2()
    {
        
        ep.Attack(102);
    }

    private void ReleaseSkill3()
    {
        
        ep.Attack(103);
    }


    private double lastAtkTime = 0;
    private int[] comboArr = new int[] {111, 112, 113, 114, 115 };
    private int comboIndex = 0;
    private void ReleaseNormalAttack()
    {
        if(ep.CurrentState == AniState.Attack)
        {
            double currentTime = timeSvc.GetCurrentTime();
            if(currentTime - lastAtkTime < Message.ComboSpace && lastAtkTime != 0)
            {
                if (comboIndex < comboArr.Length - 1)
                {
                    comboIndex += 1;
                    ep.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = timeSvc.GetCurrentTime();
                }
                else
                {
                    comboIndex = 0;
                    lastAtkTime = 0;
                }

            }
        }
        else if(ep.CurrentState == AniState.Idle || ep.CurrentState == AniState.Move)
        {
            lastAtkTime = timeSvc.GetCurrentTime();
            ep.Attack(111);
        }
    }

    public void StopCombo()
    {
        comboIndex = 0;
        lastAtkTime = 0;
    }

    public virtual void SetFX()
    {

    }

    public Vector2 GetInputDir()
    {
        return BattleSystem.Instance.GetInputDir();
    }

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
}

