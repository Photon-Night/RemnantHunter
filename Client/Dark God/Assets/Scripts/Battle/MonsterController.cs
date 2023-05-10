using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : EntityController
{
    public NavMeshAgent Agent { get; private set; }

    public float checkRange;

    private float moveAmount = 1;
    public override void Init()
    {
        base.Init();
        Agent = GetComponent<NavMeshAgent>();
    }

    public override bool SetAttack(string animName)
    {
        if (!CanMove) return false;
        
        anim.CrossFade(animName, .1f);
        return true;
    }


    public override void SetMove(Vector3 dir = default)
    {
        anim.SetFloat("Blend", moveAmount);
    }
    public override void SetDie()
    {
        anim.CrossFade("Die", .1f);
        Agent.isStopped = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, checkRange);
    }
}
