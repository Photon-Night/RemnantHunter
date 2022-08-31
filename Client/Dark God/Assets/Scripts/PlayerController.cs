using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Animator anim;
    public CharacterController cc;

    private Vector2 dir;

    private bool isMove;

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
    void Start()
    {
        camTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 _dir = new Vector2(h, v).normalized;
        if(_dir != Vector2.zero)
        {
            Dir = _dir;
        }
        else
        {
            Dir = Vector2.zero;
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

    }

    private void SetMove()
    {

    }

    private void SetCam()
    {

    }
}
