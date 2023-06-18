using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3.5f;
    public float sprintSpeed = 5f;
    public float rotateSpeed = 8f;
    public float jumpSpeed = 15f;

    [Header("States")]
    public bool onGround;
    public bool sprint;
    public bool jump;
    public bool normalAttack;
    public bool comboAttack;
    public bool roll;
    public bool CanMove { get; protected set; }

    public bool lockCtrl;

    public int ComboIndex { get; protected set; }
    public int ShieldID { get; protected set; }
    public int WeaponID { get; protected set; }

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    public Transform hpRoot;

    protected Transform camTrans;
    public AudioSource audioSource;
    public Animator anim;
    public Collider bodyCollider;

    public event System.Action<int> AnimationAttackDamageEvent;
    
    public bool IsMove { get; protected set; }
    public bool IsReleaseAttack { get; protected set; }

    protected Vector3 dir;
    public Vector3 Dir
    {
        get
        {
            return dir;
        }
        protected set
        {
            if (dir == Vector3.zero)
                IsMove = false;
            else
                IsMove = true;

            dir = value;
        }
    }

    private void Update()
    {
        CanMove = anim.GetBool("canMove");          
    }

    public virtual void Init()
    {       
        if(!anim)
        {
            anim = this.GetComponent<Animator>();
            audioSource = this.GetComponent<AudioSource>();
        }
    }

    #region Action
    public virtual void SetMove(Vector3 dir = default)
    {
        
    }

    public virtual bool SetJump()
    {       
        return true;
    }

    public virtual bool SetCombo(string comboName)
    {
        if (onGround)
        {
            anim.CrossFade(comboName, .1f);
            IsReleaseAttack = true;
            return true;
        }
        return false;
    }

    public virtual bool SetRoll()
    {
        if (onGround)
        {
            anim.SetTrigger("roll");
            return true;
        }
        return false;
    }

    public virtual bool SetAttack(string animName)
    {       
        return true;
    }

    public virtual void SetSprint(bool isSprint)
    {
        sprint = isSprint;
    }

    public virtual void SetHit()
    {
        anim.CrossFade("Hit", .1f);
    }


    public virtual void StopMove()
    {
        anim.SetFloat("Blend", Message.BlendIdle);
    }
    public virtual void SetDie()
    {
        bodyCollider.enabled = false;
        anim.CrossFade("Die", .1f);
    }
    #endregion

    public virtual void SetAtkRotationLocal(Vector3 atkDir)
    {
        Quaternion tr = Quaternion.LookRotation(atkDir);
        //Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed);
        //transform.rotation = targetRotation;
        transform.rotation = tr;
    }

    public virtual void ChangeMesh(MeshType type, int index)
    {
        
    }
    public virtual void ChangeEquipment(GameItemCfg equipmentData) { }
    public AudioSource GetAudio()
    {       
        return audioSource ??= GetComponent<AudioSource>();
    }

    protected void OnAnimationAttackDamage(int dmgIndex)
    {
        IsReleaseAttack = false;
        AnimationAttackDamageEvent?.Invoke(dmgIndex);
    }
    protected void OnTriggerStay(Collider col)
    {        
        if (!onGround && col.gameObject.layer == 10)
        {
        
            onGround = true;
            anim.SetBool("onGround", true);
        }
    }
    protected void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == 10)
        {
            onGround = false;
            anim.SetBool("onGround", false);
        }
    }

}
