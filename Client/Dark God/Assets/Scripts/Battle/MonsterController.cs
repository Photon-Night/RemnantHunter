using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : EntityController
{
    public NavMeshAgent Agent { get; private set; }
    private void Update()
    {
        canMove = anim.GetBool("canMove");
    }
    public override void Init()
    {
        base.Init();
        Agent = GetComponent<NavMeshAgent>();
    }

    public override bool SetNormalAttack(string animName)
    {
        if (!canMove) return false;

        anim.CrossFade(animName, .1f);
        return true;
    }
}
