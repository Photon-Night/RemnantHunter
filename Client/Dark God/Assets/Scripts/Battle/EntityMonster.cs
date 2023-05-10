using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMonster : EntityBase
{
    private NavMeshAgent agent;
    public MonsterData md;
    
    public MonsterState CurMonsterState { get; private set; }

    public float attackRange_sqr;
    public Vector3 bornPos; 
    public bool IsBattle { get; set; }
    public bool IsPatroller { get; private set; }
    public bool RunAI { get; private set; }

    public int GroupID { get; private set; }
    public Vector3 patrolPos { get; private set; }
    private float checkRange;

    private List<int> AttackTaskLst = new List<int>();

    private float attackCoolDownTime = 0f;
    public float CheckRange_Sqr
    {
        get
        {
            return checkRange * checkRange;
        }
    }
    public EntityMonster(BattleManager bm, StateManager sm, MonsterController mc, BattleProps bps, int entityId = 0) : base(bm, sm, mc, bps, entityId)
    {
        entityType = EntityType.Monster;
        agent = mc.Agent;
        BornPos = mc.transform.position;
        controller = mc;
        checkRange = bps.atkDis * 1.5f;
        mc.checkRange = checkRange;
    }
    public void InitMonster(MonsterData md, EntityBase target, int groupID)
    {
        base.InitEntity();
        this.md = md;
        RunAI = true;
        attackRange_sqr = Props.atkDis * Props.atkDis;
        Target = target;
        GroupID = groupID;
    }

    float distance = 0;
    public void TickMonsterAILogic_Active(Vector3 targetPos, out bool canActiveAllMonster)
    {
        canActiveAllMonster = false;

        if (CurrentAniState == AniState.Back)
        {
            if (GetDistanceToTarget_Sqr(BornPos) < .1f)
            {
                sm.ChangeState(this, AniState.Idle);
                attackCoolDownTime = 0f;
            }
            else
            {
                return;
            }
        }

        if (IsHit) return;

        distance = GetDistanceToTarget_Sqr(targetPos);

        if(!IsBattle && distance <= CheckRange_Sqr)
        {
            canActiveAllMonster = true;
            return;
        }

        if(!IsBattle)
        {
            return;
        }
       
        if(attackCoolDownTime < 0)
        {

            if (distance <= attackRange_sqr)
            {              
                sm.ChangeState(this, AniState.Attack);
            }
            else
            {
                sm.ChangeState(this, AniState.Chase);
            }
        }
        else
        {
            attackCoolDownTime -= Time.deltaTime;
            agent.SetDestination(GetPos());
        }
        
    }
    public void TickMonsterAILogic_Standby(in Vector3 groupPos, float groupRange)
    {
        if (!RunAI)
            return;
        if (IsBattle)
        {
            if (CurrentAniState != AniState.Back)
            {
                var distance = GetDistanceToTarget_Sqr(groupPos);
                if (distance > groupRange)
                {
                    sm.ChangeState(this, AniState.Back);
                }
            }
            else
            {
                if (GetDistanceToTarget_Sqr(BornPos) < .2f)
                {
                    sm.ChangeState(this, AniState.Idle);
                    IsBattle = false;             
                }
            
            }
        }
        else
        {
            sm.ChangeState(this, AniState.Idle);
        }
    }
    public override Vector3 GetClosedTarget()
    {       
        if (Target != null && Target.CurrentAniState != AniState.Die)
        {
            Vector3 target = Target.GetPos();
            Vector3 self = GetPos();

            Vector3 dir = new Vector3(target.x - self.x,0f, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            RunAI = false;
            return Vector3.zero;
        }
    }

    public override bool GetBreakState()
    {

        if(md.mCfg.isStop)
        {
            if (CurrentSkillData != null)
            {
                return CurrentSkillData.isBreak;
            }
            else
            {
                return true;
            }
        }
        else
        
            return false;
        
    }

    protected override void SetHpVal(int oldHp, int newHp)
    {
        if (md.mCfg.mType == MonsterType.Normal)
            base.SetHpVal(oldHp, newHp);
        else if(md.mCfg.mType == MonsterType.Boss)
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
        controller.SetMove();
    }
    public override void StopMove()
    {
        agent.SetDestination(GetPos());
        controller.StopMove();
    }

    public bool IsCloseToTarget(Vector3 target)
    {
        return GetDistanceToTarget_Sqr(target) < 0.5f;
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

    public override void SetAttack()
    {
        attackCoolDownTime = Random.Range(1f, 4f);

        float r = Random.Range(0, .5f);
        int tid = timer.AddTimeTask((tid) =>
        {
            base.SetAttack();
        }, r);

        AttackTaskLst.Add(tid);
    }

    public override void SetHit()
    {
        PlayEntityHitAudio();
        if (currentAniState == AniState.Back) return;

        if (!IsHit)
        {
            controller.SetHit();
            IsHit = true;
            StopAttackTask();
            timer.AddTimeTask((tid) =>
            {
                IsHit = false;
            }, hitTime);
        }
    }

    public void StopAttackTask()
    {
        for(int i = 0; i < AttackTaskLst.Count; i++)
        {
            timer.DeleteTimeTask(AttackTaskLst[i]);
        }

        AttackTaskLst.Clear();
    }
}

public enum MonsterState
{
    None = 0,
    Standby = 1,
    Check = 2,
    Battle = 3,
}



