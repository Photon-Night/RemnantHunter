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

    [SerializeField] private GameObject[] roleArr;
    [SerializeField] private GameObject[] bodyArr;
    [SerializeField] private GameObject[] weaponArr;
    [SerializeField] private GameObject[] shieldArr;
    


    public int index_Role;
    public int index_Body;
    private int index_Weapon;
    private int index_Shield;

    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();
        camTrans = Camera.main.transform;     
        Physics.autoSyncTransforms = true;
    }

    public void InitPlayer(string code)
    {
        Init();

        index_Role = 2;
        index_Body = 5;

        if (code == "") return;

        string[] modleArr = code.Split('|');
        ChangeMesh(MeshType.Role, int.Parse(modleArr[0]));
        ChangeMesh(MeshType.Body, int.Parse(modleArr[1]));
    }

    public void ChangeMesh(MeshType type, int index = -1)
    {
        switch (type)
        {
            case MeshType.None:
                break;
            case MeshType.Role:
                SetMeshsActive(roleArr, index, ref index_Role);
                break;
            case MeshType.Body:
                SetMeshsActive(bodyArr, index, ref index_Body);
                break;         
        }
    }

    private void SetMeshsActive(GameObject[] meshs, int activeIndex, ref int currentIndex)
    {
        if(activeIndex == -1)
        {
            activeIndex = currentIndex + 1 < meshs.Length ? currentIndex + 1 : 0;
        }

        meshs[currentIndex].SetActive(false);
        meshs[activeIndex].SetActive(true);
        currentIndex = activeIndex;
    }

    // Update is called once per frame
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

    public override void SetMove(Vector3 moveDir)
    {
        if (isTalk) return;

        float targetSpeed = moveSpeed;
        if(sprint)
        {
            targetSpeed = sprintSpeed;
        }
        Dir = moveDir * (targetSpeed * moveAmount);
        dir.y = rigid.velocity.y;
        float m = moveDir.sqrMagnitude;
        moveAmount = Mathf.Clamp01(m);        
    }

    public override bool SetJump()
    {
        if (isTalk) return false;

        if (onGround && canMove)
        {
            anim.CrossFade("falling", .1f);
            this.transform.position += (rigid.velocity.y * Vector3.up * Time.deltaTime * 10);
            rigid.AddForce(0, jumpForce, 0);
            return true;
        }
        return false;
    }

    public override bool SetNormalAttack(string animName)
    {
        if (isTalk) return false;

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

    public string GetModleIndex_Str()
    {
        return $"{index_Role}|{index_Body}";
    }
    private void OnAnimatorMove()
    {
        if (canMove || !onGround)
            return;

        rigid.drag = 0;

        Vector3 dPosition = anim.deltaPosition;
        dPosition.y = 0;
        Vector3 vPosition = (dPosition * .6f) / Time.deltaTime;
        rigid.velocity = vPosition;

    }
}

public enum MeshType
{
    None = 0,
    Role = 1,
    Body = 2,
}

