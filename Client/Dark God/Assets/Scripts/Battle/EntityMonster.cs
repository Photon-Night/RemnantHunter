using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMonster : EntityBase
{
    public MonsterData md;
    private EntityPlayer player;
    private float checkTimeCount = 0;
    private float atkTimeCount = 0;
    private float rdCheckTime = 0;
    
    public MonsterState CurMonsterState { get; private set; }
    private NavMeshAgent agent;

    public float attackRange_sqr;
    public Vector3 bornPos; 
    public bool IsBattle { get; set; }
    public bool IsPatroller { get; private set; }
    public bool InGroupRange { get; set; }
    public bool RunAI { get; private set; }
    public Vector3 patrolPos { get; private set; }
    public override void SetBattleProps(BattleProps props)
    {
        int lv = md.lv;

        Props = new BattleProps
        {
            hp = lv * props.hp,
            ad = lv * props.ad,
            ap = lv * props.ap,
            addef = lv * props.addef,
            apdef = lv * props.apdef,
            dodge = lv * props.dodge,
            pierce = lv * props.pierce,
            critical = lv * props.critical,
            atkDis = props.atkDis
        };

        HP = Props.hp;
        
    }
    public EntityMonster(MonsterController mc, BattleProps bps, int entityId = 0) : base(mc, bps, entityId)
    {
        entityType = Message.EntityType.Monster;
        rdCheckTime = Message.AICheckTimeSpace;
        agent = mc.Agent;
    }
    public void InitMonster(BattleManager bm, SkillManager skm, StateManager stm, MonsterData md)
    {
        base.InitEntity(bm, skm, stm);
        this.md = md;
    }
    public override Vector2 GetClosedTarget()
    {
        player = battleMgr.ep;

        if (player != null && player.CurrentAniState != AniState.Die)
        {
            Vector3 target = player.GetPos();
            Vector3 self = GetPos();

            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            RunAI = false;
            return Vector2.zero;
        }
    }

    public override bool GetBreakState()
    {
        if(md.mCfg.isStop)
        {
            if (currentSkillCfg != null)
            {
                return currentSkillCfg.isBreak;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    protected override void SetHpVal(int oldHp, int newHp)
    {
        if (md.mCfg.mType == Message.MonsterType.Normal)
            base.SetHpVal(oldHp, newHp);
        else if(md.mCfg.mType == Message.MonsterType.Boss)
        {           
            BattleSystem.Instance.SetBossHPVal(oldHp, newHp, Props.hp);
        }
    }

    public override void MoveTo(Vector3 pos, bool immediately = false)
    {
        if(immediately)
        {
            controller.transform.position = pos;
            return;
        }
        agent.SetDestination(pos);
    }
    public override void StopMove()
    {
        agent.SetDestination(GetPos());
    }

    public bool IsCloseToTarget(Vector3 target)
    {
        return GetDistanceToTarget_Sqr(target) < 0.5f;
    }

    public override void SetAttack()
    {
        int r = Random.Range(0, normalAttackLst.Count);
        if(controller.SetNormalAttack(normalAttackLst[r].skillName))
        {
            
        }
    }

    public override void SetDie()
    {
        base.SetDie();
        CloseCollider();

        timer.AddTimeTask((int id) =>
        {
            SetActive(false);
        }, Message.DieAniLength);
    }
}

public enum MonsterState
{
    None = 0,
    Normal = 1,
    Battle = 2,
}



