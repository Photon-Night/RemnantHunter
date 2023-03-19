using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHit : IState
{
    public void OnEnter(EntityBase entity, params object[] args)
    {
        entity.CurrentAniState = AniState.Hit;
        entity.RemoveSkillCB();
    }

    public void OnExit(EntityBase entity, params object[] args)
    {
        entity.canReleaseSkill = true;
        entity.UnLock();
    }

    public void OnUpdate(EntityBase entity, params object[] args)
    {
        
    }

    public void Process(EntityBase entity, params object[] args)
    {
        //entity.canReleaseSkill = false;
        //
        //entity.SetAction(Message.ActionHit);
        //entity.SetDir(Vector2.zero);
        //entity.Lock();
        //if(entity.nextCombo != 0 || entity.comboQue.Count != 0)
        //{
        //    entity.nextCombo = 0;
        //    entity.comboQue.Clear();
        //
        //    entity.battleMgr.StopCombo();
        //}

        TimerService.Instance.AddTimeTask((int tid) =>
        {
            //entity.SetAction(Message.ActionNormal);
            entity.Idle();
        }, GetHitAniLen(entity) * 1000);
    }

    private float GetHitAniLen(EntityBase entity)
    {
        //AnimationClip[] clips = entity.controller.anim.runtimeAnimatorController.animationClips;
        //for (int i = 0; i < clips.Length; i++)
        //{
        //    string name = clips[i].name;
        //    if(name.Contains("hit") || name.Contains("Hit") || name.Contains("HIT"))
        //    {
        //        return clips[i].length;
        //    }
        //}
        AnimationClip result = entity.GetAnimationClip("hit", "Hit", "HIT");
        if(result != null)
        {
            return result.length;
        }

        return 1;
    }
}
