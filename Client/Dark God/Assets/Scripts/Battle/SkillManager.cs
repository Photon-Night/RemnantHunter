using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    ResService resSvc;
    TimerService timer;

    public void InitManager()
    {
        resSvc = ResService.Instance;
        timer = TimerService.Instance;
        PECommon.Log("SkillManager Loading");
    }
    /// <summary>
    /// ¹¥»÷¾ßÌåÂß¼­
    /// </summary>
    public void AttackEffect(EntityBase entity, int skillId)
    {
        SkillCfg data = resSvc.GetSkillData(skillId);

        if(data != null)
        {
            entity.SetAction(data.aniAction);
            entity.SetFX(data.fx, data.skillTime);
            
            timer.AddTimeTask((int tid) =>
            {
                entity.Idle();
            }, data.skillTime);
        }
    }
}
