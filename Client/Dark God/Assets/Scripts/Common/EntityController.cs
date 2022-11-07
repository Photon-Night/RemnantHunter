using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    protected bool isMove;
    protected bool isSkillMove;
    protected Vector2 dir;
    protected int action;
    protected float skillMoveSpeed;
    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    protected TimerService timer;
    public Transform hpRoot;
    public bool LockCtrl = false;

    public Vector2 Dir
    {
        get
        {
            return dir;
        }
        set
        {
            if (value == Vector2.zero)
            {
                isMove = false;
            }
            else
            {
                isMove = true;
            }
            dir = value;
        }
    }
    public Animator anim;


    public virtual void Init()
    {
        timer = TimerService.Instance;
        if(!anim)
        {
            anim = this.GetComponent<Animator>();
        }
    }

    public virtual void SetBlend(int blend)
    {
        anim.SetFloat("Blend", blend);
    }

    public virtual void SetAction(int action)
    {
        anim.SetInteger("Action", action);
    }

    public virtual void SetFX(string name, float destory)
    {

    }

    public void SetSkillMoveState(bool move, float skillSpeed = 0)
    {
        isSkillMove = move;
        skillMoveSpeed = skillSpeed;
    }

}
