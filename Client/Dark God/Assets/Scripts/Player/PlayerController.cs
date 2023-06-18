using DM;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    public string[] randomAttacks;

    public Rigidbody rigid;
    public bool isGuide = false;
    private float targetJumpSpeed;
    private float delta;
    private float moveAmount;
    public float MoveAmount
    {
        get
        {
            return moveAmount;
        }
        private set
        {
            moveAmount = value;
        }
    }

    public bool CheckCombo { get; private set; }
    
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

    public void InitPlayer(string modleCode, string equipmentCode)
    {
        Init();

        index_Role = 2;
        index_Body = 5;
        index_Shield = 0;
        index_Weapon = 0;

        if (modleCode == "" || equipmentCode == "") return;

        ChangeMeshByCode(modleCode, equipmentCode);
    }
    public void ChangeMeshByCode(string modleCode, string equipmentCode)
    {
        string[] modleArr = modleCode.Split('|');
        string[] equipmentArr = equipmentCode.Split('|');
        ChangeMesh(MeshType.Role, int.Parse(modleArr[0]) - index_Role);
        ChangeMesh(MeshType.Body, int.Parse(modleArr[1]) - index_Body);
        ChangeMesh(MeshType.Weapon, int.Parse(equipmentArr[0]) - index_Weapon);
        ChangeMesh(MeshType.Shield, int.Parse(equipmentArr[1]) - index_Shield);
    }
    public override void ChangeEquipment(GameItemCfg equipmentData)
    {
        
        switch(equipmentData.equipmentType)
        {
            case EquipmentType.Shield:
                ChangeMesh(MeshType.Shield, int.Parse(equipmentData.objPath) - index_Shield);
                ShieldID = equipmentData.ID;
                break;
            case EquipmentType.Weapon:
                ChangeMesh(MeshType.Weapon, int.Parse(equipmentData.objPath) - index_Weapon);
                WeaponID = equipmentData.ID;
                break;
        }

    }
    
    public override void ChangeMesh(MeshType type, int index)
    {
        switch (type)
        {
            case MeshType.None:
                break;
            case MeshType.Role:
                SetMeshsActive(roleArr, index_Role + index, ref index_Role);
                break;
            case MeshType.Body:               
                SetMeshsActive(bodyArr, index_Body + index, ref index_Body);
                break;
            case MeshType.Shield:
                SetMeshsActive(shieldArr, index_Shield + index, ref index_Shield);
                break;
            case MeshType.Weapon:
                SetMeshsActive(weaponArr, index_Weapon + index, ref index_Weapon);
                break;
        }
    }

    private void SetMeshsActive(GameObject[] meshs, int activeIndex, ref int currentIndex)
    {
        if (activeIndex < 0)
        {
            activeIndex = meshs.Length - 1;
        }
        else if(activeIndex >= meshs.Length)
        {
            activeIndex = 0;
        }

        meshs[currentIndex].SetActive(false);
        meshs[activeIndex].SetActive(true);
        currentIndex = activeIndex;
    }

    private void FixedUpdate()
    {
        FixedTick(Time.fixedDeltaTime);
    }

    private void Update()
    {
        CanMove = anim.GetBool("canMove");
        Tick(Time.deltaTime);
    }

    private void FixedTick(float d)
    {
        delta = d;
        if(onGround)
        {
            if(CanMove)
            {                                      
                rigid.velocity = Dir;
               //cc.Move(Dir);
            }
        }       
    }

    private void Tick(float d)
    {
        if (CanMove && !IsReleaseAttack)
        {
            SetRotate(Dir);
        }

        SetMovementAnim();
    }

    public void SetRotate(Vector3 dir, bool immediately = false)
    {
        Vector3 targetDir = dir;
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
            targetDir = transform.forward;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        if(!immediately)
        {
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * MoveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }
        else
        {
            transform.rotation = tr;
        }
    }

    public override void SetAtkRotationLocal(Vector3 atkDir)
    {
        SetRotate(atkDir, true);
    }
   
    private void SetMovementAnim()
    {
        anim.SetBool("sprint", sprint);
        if(MoveAmount == 0)
        {
            anim.SetBool("sprint", false);
        }

        anim.SetFloat("vertical", MoveAmount, .2f, Time.deltaTime);
    }

    public override void SetMove(Vector3 moveDir)
    {        
        float targetSpeed = moveSpeed;
        if(sprint)
        {
            targetSpeed = sprintSpeed;
        }
        Dir = moveDir * (targetSpeed);
        dir.y = rigid.velocity.y + targetJumpSpeed;
        targetJumpSpeed = 0f;
        float m = moveDir.sqrMagnitude;
        MoveAmount = Mathf.Clamp01(m);
     
    }

    public override bool SetJump()
    {
        return false;

        if (onGround && CanMove)
        {
            targetJumpSpeed = jumpSpeed;

            anim.CrossFade("falling", .1f);           
            return true;
        }
        return false;
    }

    public override bool SetAttack(string animName)
    {
        if (!CanMove) return false;
        IsReleaseAttack = true;
        anim.CrossFade(animName, .1f);

        if(!onGround)
        {
            anim.CrossFade("JumpAttack", .1f);
        }

        return true;
    }

    public override void SetHit()
    {
        base.SetHit();
        IsReleaseAttack = false;
    }

    public override void SetDie()
    {
        base.SetDie();
        rigid.useGravity = false;
    }

    public void OnPlayerTalk()
    {
        rigid.useGravity = false;
        bodyCollider.enabled = false;
        //cc.enabled = false;
    }

    public void OnPlayerOverTalk()
    {
        bodyCollider.enabled = true;
        rigid.useGravity = true;
        //cc.enabled = true;
    }

    public string GetModleIndex_Str()
    {
        return $"{index_Role}|{index_Body}";
    }
    private void OnAnimatorMove()
    {
        if (CanMove || !onGround)
            return;        

        Vector3 dPosition = anim.deltaPosition;
        Vector3 vPosition = (dPosition * .6f) / Time.deltaTime;
        rigid.velocity = vPosition;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + Dir * 10);
    }
}

public enum MeshType
{
    None = 0,
    Role = 1,
    Body = 2,
    Weapon = 3,
    Shield = 4,
}

