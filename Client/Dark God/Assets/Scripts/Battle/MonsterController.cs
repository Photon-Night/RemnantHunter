using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : EntityController
{
    void Update()
    {
        if(isMove)
        {
            SetDir();
            SetMove();
        }
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        this.transform.localEulerAngles = eulerAngles;
    }
    private void SetMove()
    {       
        cc.Move(transform.forward * Time.deltaTime * Message.MonsterMoveSpeed);
        cc.Move(Vector3.down * Time.deltaTime * Message.MonsterMoveSpeed);
    }
}
