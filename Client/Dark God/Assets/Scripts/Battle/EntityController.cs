using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3.5f;
    public float sprintSpeed = 5f;
    public float rotateSpeed = 5f;
    public float jumpForce = 600f;

    [Header("States")]
    public bool onGround;
    public bool sprint;
    public bool jump;
    public bool normalAttack;
    public bool comboAttack;
    public bool roll;
    public bool canMove;

    public bool lockCtrl;

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    public Transform hpRoot;

    protected Transform camTrans;

    public Rigidbody rigid;
    public AudioSource audioSource;
    public Animator anim;

    protected System.Action<int> OnAnimatorAttack;
    
    protected Vector3 dir;
    public Vector3 Dir
    {
        get
        {
            return dir;
        }
        set
        {           
            dir = value;
        }
    }


    public virtual void Init()
    {       
        if(!anim)
        {
            anim = this.GetComponent<Animator>();
            audioSource = this.GetComponent<AudioSource>();
        }
    }

    public void RegisterAttackEvent(System.Action<int> action)
    {
        if(OnAnimatorAttack != null)
        {
            OnAnimatorAttack += action;
        }
        else
        {
            OnAnimatorAttack = action;
        }
    }

    public void UnRegisterAttackEvent(System.Action<int> action)
    {
        if(OnAnimatorAttack != null)
        OnAnimatorAttack -= action;
    }
    #region Action
    public virtual void SetMove(float ver, float hor)
    {
        
    }

    public virtual bool SetJump()
    {       
        return true;
    }

    public virtual bool SetCombo()
    {
        if (onGround)
        {
            anim.SetTrigger("combo");
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

    public virtual bool SetNormalAttack(string animName)
    {       
        return true;
    }

    public virtual void SetSprint(bool isSprint)
    {
        sprint = isSprint;
    }

    public virtual void SetHit()
    {
        anim.SetTrigger("hit");
    }

    public virtual void SetIdle()
    {
        
    }
    public virtual void SetDie() {  }
    #endregion

    public virtual void SetAtkRotationLocal(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        this.transform.localEulerAngles = eulerAngles;
    }

    public AudioSource GetAudio()
    {
        return audioSource;
    }

    protected void OnAnimtionAttack(int id)
    {
        OnAnimatorAttack(id);
    }
    protected void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = true;
            anim.SetBool("onGround", true);
        }
    }
    protected void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ground")
        {
            onGround = false;
            anim.SetBool("onGround", false);
        }
    }

    protected void OnAnimatorMove()
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
