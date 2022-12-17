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

    protected Transform camTrans;

    public CharacterController cc;

    public AudioSource audioSource;
    public Animator anim;
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


    public virtual void Init()
    {
        timer = TimerService.Instance;
        if(!anim)
        {
            anim = this.GetComponent<Animator>();
            audioSource = this.GetComponent<AudioSource>();
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

    public virtual void SetAtkRotationLocal(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        this.transform.localEulerAngles = eulerAngles;
    }

    public virtual void SetAtkRotationCam(Vector2 atkDir)
    {
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        this.transform.localEulerAngles = eulerAngles;
    }

    public AudioSource GetAudio()
    {
        return GetComponent<AudioSource>();
    }
}
