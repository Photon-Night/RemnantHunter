using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    public override Vector2 GetInputDir()
    {
        return battleMgr.GetInputDir();
    }
}
