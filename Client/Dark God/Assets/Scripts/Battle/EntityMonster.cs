using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    public MonsterData md;
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
}
