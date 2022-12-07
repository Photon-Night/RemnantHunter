using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    public override Vector2 GetInputDir()
    {
        return battleMgr.GetInputDir();
    }

    public override Vector2 GetClosedTarget()
    {
        EntityMonster target = battleMgr.FindClosedMonster(this.GetTrans());
        if (target != null)
        {
            Vector2 dir = new Vector2(target.GetPos().x - this.GetPos().x, target.GetPos().z - this.GetPos().z);
            return dir.normalized;
        }
        return Vector2.zero;
    }
}
