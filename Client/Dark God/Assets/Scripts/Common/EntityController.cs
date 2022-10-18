using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    protected bool isMove;
    protected Vector2 dir;
    protected int action;
    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();
    protected TimerService timer;
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
    }

    public virtual void SetBlend(int blend)
    {
        anim.SetInteger("Blend", blend);
    }

    public virtual void SetAction(int action)
    {
        anim.SetInteger("Action", action);
    }

    public virtual void SetFX(string name, float destory)
    {

    }


}
