using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSystem : SystemRoot<TaskSystem>
{

    public override void InitSystem()
    {
        base.InitSystem();
        PECommon.Log("TaskSystem Loading");
    }

}
