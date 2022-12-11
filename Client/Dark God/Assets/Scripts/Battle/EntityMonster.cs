using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    public MonsterData md;
    private EntityPlayer player;
    private float checkTimeCount = 0;
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

    protected override void TickAILogic()
    {
        checkTimeCount += Time.deltaTime;
        if(checkTimeCount < Message.AICheckTimeSpace)
        {
            return;
        }
        else
        {
            GetClosedTarget();
            CheckRange();
            checkTimeCount = 0;
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


