using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    public MonsterData md;
    private EntityPlayer player;
    private float checkTimeCount = 0;
    private float atkTimeCount = 0;
    private float rdCheckTime = 0;

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

    bool runAI = true;

    public EntityMonster(MonsterController mc, BattleProps bps, int entityId = 0) : base(mc, bps, entityId)
    {
        entityType = Message.EntityType.Monster;
        rdCheckTime = Message.AICheckTimeSpace;
    }

    public void InitMonster(BattleManager bm, SkillManager skm, StateManager stm, MonsterData md)
    {
        base.InitEntity(bm, skm, stm);
        this.md = md;
    }

    public override void TickAILogic()
    {
        if(!runAI)
        {
            return;
        }
        if (currentState == AniState.Idle || currentState == AniState.Move)
        {
            checkTimeCount += Time.deltaTime;
            if (checkTimeCount < rdCheckTime)
            {
                return;
            }
            else
            {
                Vector2 dir = GetClosedTarget();
                if (!CheckRange())
                {
                    SetDir(dir);
                    if(dir != Vector2.zero)
                    Move();
                }
                else
                {
                    SetDir(Vector2.zero);
                    atkTimeCount += checkTimeCount;
                    if (atkTimeCount > Message.AIAtkTimeSpace)
                    {
                        SetAtkRotation(dir);
                        Attack(md.mCfg.skillID);
                        atkTimeCount = 0;
                    }
                    else
                    {
                        Idle();
                    }
                }
                checkTimeCount = 0;
                rdCheckTime = PETools.RdInt(1, 5) * 1f / 10;
            }
        }
    }

    public override Vector2 GetClosedTarget()
    {
        player = battleMgr.ep;

        if (player != null && player.CurrentState != AniState.Die)
        {
            Vector3 target = player.GetPos();
            Vector3 self = GetPos();

            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            runAI = false;
            return Vector2.zero;
        }
    }

    private bool CheckRange()
    {
        player = battleMgr.ep;

        if(player != null && player.CurrentState != AniState.Die)
        {
            Vector3 target = player.GetPos();
            Vector3 self = GetPos();

            float dis = Vector3.Distance(target, self);
            if(dis <= md.mCfg.bps.atkDis)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        return false;
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
}


