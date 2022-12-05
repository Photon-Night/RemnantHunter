using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    ResService resSvc = null;
    AudioService audioSvc = null;

    MapManager mapMgr = null;
    SkillManager skillMgr = null;
    StateManager stateMgr = null;

    private EntityPlayer ep;
    private MapCfg mapData;

    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();
    public void InitManager(int mapId)
    {
        PECommon.Log("BattleManager Loading");
        resSvc = ResService.Instance;
        audioSvc = AudioService.Instance;

        skillMgr = gameObject.AddComponent<SkillManager>();
        skillMgr.InitManager();
        stateMgr = gameObject.AddComponent<StateManager>();
        stateMgr.InitManager();

        mapData = resSvc.GetMapCfgData(mapId);
        resSvc.LoadSceneAsync(mapData.sceneName, () =>
        {
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");

            mapMgr = map.gameObject.AddComponent<MapManager>();
            mapMgr.InitManager(this);

            map.transform.position = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position =mapData.mainCamPos;
            Camera.main.transform.localEulerAngles = mapData.mainCamRote;

            LoadPlayer();

            ActiveCurrentBatchMonsters();

            audioSvc.PlayBGMusic(Message.BGHuangYe);
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

                GameRoot.Instance.AddHpUIItem(go.name, em.md.mCfg.bps.hp, mc.hpRoot); ;
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
        PECommon.Log("skill1");
        ep.Attack(101);
    }

    private void ReleaseSkill2()
    {
        PECommon.Log("skill2");
        
    }

    private void ReleaseSkill3()
    {
        PECommon.Log("skill3");
    }

    private void ReleaseNormalAttack()
    {
        PECommon.Log("normal");
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
}

