using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{
    public GameObject daggeratk1fx;


    public CharacterController cc;
  
    private Vector3 camOffest;

    public bool isGuide = false;

    private float currentBlend;
    private float targetBlend;

    
    
    private Transform camTrans;
    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();

        camTrans = Camera.main.transform;
        camOffest = transform.position - camTrans.position;
        Physics.autoSyncTransforms = true;

        if(daggeratk1fx != null)
        fxDic.Add(daggeratk1fx.name, daggeratk1fx);
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        //if (!LockCtrl)
        //{
        //
        //    Vector2 _dir = new Vector2(h, v).normalized;
        //    if (_dir != Vector2.zero)
        //    {
        //        Dir = _dir;
        //        SetBlend(Message.BlendWalk);
        //    }
        //    else
        //    {
        //        Dir = Vector2.zero;
        //        SetBlend(Message.BlendIdle);
        //    }
        //
        //}
        if (currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (isMove)
        {
            SetDir();
            SetMove();
            SetCam();
        }

        if (isSkillMove)
        {
            SetSkillMove();
            SetCam();
        }

    }



    public void SetCam()
    {
        if(camTrans != null)
        {
            camTrans.position = transform.position - camOffest;
        }
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

    public override void SetBlend(int blend)
    {
        targetBlend = blend;
    }

    private void SetSkillMove()
    {
        cc.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }

    public override void SetFX(string name, float destory)
    {
        GameObject go;
        if(fxDic.TryGetValue(name, out go))
        {
            go.SetActive(true);
            
            timer.AddTimeTask((int tid) =>
            {
                go.SetActive(false);
            }, destory);
        }
    }
}
