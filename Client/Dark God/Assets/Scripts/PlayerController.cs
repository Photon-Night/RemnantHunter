using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Animator anim;
    public CharacterController cc;

    private Vector2 dir;
    private Vector3 camOffest;
    private bool isMove;

    private float currentBlend;
    private float targetBlend;

    public Vector2 Dir
    {
        get
        {
            return dir;
        }
        set
        {
            if(value == Vector2.zero)
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
    private Transform camTrans;
    // Start is called before the first frame update
    public void Init()
    {
        camTrans = Camera.main.transform;
        camOffest = transform.position - camTrans.position;
        Physics.autoSyncTransforms = true;
    }

    // Update is called once per frame
    void Update()
    {
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        //
        //Vector2 _dir = new Vector2(h, v).normalized;
        //if(_dir != Vector2.zero)
        //{
        //    Dir = _dir;
        //    SetBlend(Message.BlendWalk);
        //}
        //else
        //{
        //    Dir = Vector2.zero;
        //    SetBlend(Message.BlendIdle);
        //}

        if(currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if(isMove)
        {
            SetDir();
            SetMove();
            SetCam();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        this.transform.localEulerAngles = eulerAngles;
    }

    private void SetMove()
    {
        cc.Move(transform.forward * Time.deltaTime * Message.PlayerMoveSpeed);
    }

    private void SetCam()
    {
        if(camTrans != null)
        {
            camTrans.position = transform.position - camOffest;
        }
    }

    public void SetBlend(int blend)
    {
        targetBlend = blend;
    }

    private void UpdateMixBlend()
    {
        if(Mathf.Abs(currentBlend - targetBlend) < Message.AccelerSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }
        else if(currentBlend > targetBlend)
        {
            currentBlend -= Message.AccelerSpeed * Time.deltaTime;
        }
        else
        {
            currentBlend += Message.AccelerSpeed * Time.deltaTime;
        }
        anim.SetFloat("Blend", currentBlend);
    }
}
