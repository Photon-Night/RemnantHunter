using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityController : MonoBehaviour
{
    protected bool isMove;
    protected Vector2 dir;
    protected int action;
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

    public virtual void SetBlend(int blend)
    {

    }

    public virtual void SetAction(int action)
    {

    }
}
