using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    public MonsterData md;
    private EntityPlayer player;
    private float checkTimeCount = 0;
    private float atkTimeCount = 0;

    public EntityMonster()
    {
        entityType = Message.EntityType.Monster;
    }
    public override void SetBattleProps(BattleProps props)
    {
        int lv = md.lv;

        BattleProps _props = new BattleProps
        {
            hp = lv * props.hp,
            ad = lv * props.ad,
            ap = lv * props.ap,
            addef = lv * props.addef,
            apdef = lv * props.apdef,
            dodge = lv * props.dodge,
            pierce = lv * props.pierce,
            critical = lv * props.critical,
        };

        HP = _props.hp;
        Props = _props;
    }

    bool runAI = true;
    public override void TickAILogic()
    {
        if(runAI)
        {
            return;
        }
        if (currentState == AniState.Idle || currentState == AniState.Move)
        {


            checkTimeCount += Time.deltaTime;
            if (checkTimeCount < Message.AICheckTimeSpace)
            {
                return;
            }
            else
            {
                Vector2 dir = GetClosedTarget();
                if (!CheckRange())
                {
                    SetDir(dir);
                    Move();
                }
                else
                {
                    atkTimeCount += checkTimeCount;
                    SetDir(Vector2.zero);
                    if (atkTimeCount >= Message.AIAtkTimeSpace)
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
                checkTimeCount = PETools.RdInt(0, 10) * .1f;
            }
        }
    }

    public override Vector2 GetClosedTarget()
    {
        player = battleMgr.ep;

        if(player != null && player.CurrentState != AniState.Die )
        {
            Vector3 target = player.GetPos();
            Vector3 self = GetPos();

            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        return Vector2.zero;
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
}


