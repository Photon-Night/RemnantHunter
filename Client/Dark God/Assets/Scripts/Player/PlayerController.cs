using DM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    public CapsuleCollider col;
    public string[] randomAttacks;

    public bool isGuide = false;
    public bool isTalk = false; 

    private float delta;
    private float moveAmount;

    public Transform CamTrans { get; set; }

    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();
        camTrans = Camera.main.transform;     
        Physics.autoSyncTransforms = true;
    }

    // Update is called once per frame
    void Update()
    {     
        canMove = anim.GetBool("canMove");
    }

    private void FixedUpdate()
    {
        delta = Time.fixedDeltaTime;
        FixedTick(delta);
    }

    private void FixedTick(float d)
    {
        delta = d;
        if(onGround)
        {
            if(canMove)
            {
                rigid.velocity = dir;
            }
        }

        if(canMove)
        {
            Vector3 targetDir = dir;
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        SetMovementAnim();
    }
    private void SetMovementAnim()
    {
        anim.SetBool("sprint", sprint);
        if(moveAmount == 0)
        {
            anim.SetBool("sprint", false);
        }

        anim.SetFloat("vertical", moveAmount, .2f, delta);
    }

    public override void SetMove(float ver, float hor)
    {
        Vector3 v = ver * camTrans.forward;
        Vector3 h = hor * camTrans.right;
        float targetSpeed = moveSpeed;
        if(sprint)
        {
            targetSpeed = sprintSpeed;
        }
        Dir = ((v + h).normalized) * (targetSpeed * moveAmount);
        dir.y = rigid.velocity.y;
        float m = Mathf.Abs(ver) + Mathf.Abs(hor);
        moveAmount = Mathf.Clamp01(m);        
    }

    public override bool SetJump()
    {
        if(onGround && canMove)
        {
            anim.CrossFade("falling", .1f);
            rigid.AddForce(0, jumpForce, 0);
            return true;
        }
        return false;
    }

    public override bool SetNormalAttack(string animName)
    {
        if (!canMove) return false;

        anim.CrossFade(animName, .1f);

        if(!onGround)
        {
            anim.CrossFade("JumpAttack", .1f);
        }

        return true;
    }

    public void OnPlayerTalk()
    {
        isTalk = true;
        rigid.useGravity = false;
        col.enabled = false;
    }

    public void OnPlayerOverTalk()
    {
        isTalk = false;
        col.enabled = true;
        rigid.useGravity = true;
    }  
}
