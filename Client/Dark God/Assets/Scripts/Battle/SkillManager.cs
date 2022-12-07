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
    
    public void SkillAttack(EntityBase entity, int skillId)
    {
        AttackEffect(entity, skillId);
        AttackDamage(entity, skillId);
    }

    private void AttackDamage(EntityBase entity, int skillId)
    {

        SkillCfg data_skill = resSvc.GetSkillData(skillId);
        List<int> skillActionLst = data_skill.skillActionLst;
        float sum = 0f;
        for (int i = 0; i < skillActionLst.Count; i++)
        {
            SkillActionCfg data_skillAction = resSvc.GetSkillActionCfg(skillActionLst[i]);
            sum += data_skillAction.delayTime;
            int index = i;
            if (sum > 0)
            {
               timer.AddTimeTask((int tid) =>
               {
                   SkillAction(entity, data_skill, index);
               }, sum);
            }
            else
            {
                SkillAction(entity, data_skill, index);
            }
        }
    }

    private void SkillAction(EntityBase entity, SkillCfg data_skill, int index)
    {
        List<EntityMonster> monsterLst = entity.battleMgr.GetMonsterLst();
        SkillActionCfg data = resSvc.GetSkillActionCfg(data_skill.skillActionLst[index]);
        int damage = data_skill.skillDamageLst[index];
        for (int i = 0; i < monsterLst.Count; i++)
        {
            var monster = monsterLst[i];
            if(RangeCheck(entity.GetPos(), monster.GetPos(), data.radius)
                && CheckAngle(entity.GetTrans(), monster.GetPos(), data.angle))
            {
                CalcDamage(entity, monster, data_skill, damage);
            }
        }
    }

    System.Random rd = new System.Random();
    private void CalcDamage(EntityBase attacker, EntityBase target, SkillCfg data_skill, int damage)
    {
        int dmgSum = damage;
        if(data_skill.dmgType == Message.DmgType.AD)
        {
            int dodgeNum = PETools.RdInt(1, 100, rd);
            if(dodgeNum <= target.Props.dodge)
            {
                target.SetDodge();
                return;
            }

            dmgSum += target.Props.ad;

            int cirticalNum = PETools.RdInt(1, 100, rd);
            int addef = target.Props.addef;


            if(cirticalNum < attacker.Props.critical)
            {
                float cirticalRate = 1 + (PETools.RdInt(1, 100, rd) / 100);
                dmgSum = (int)cirticalRate * dmgSum;
                attacker.SetCritical();
                
            }

            dmgSum -= (int)((1 - target.Props.pierce / 100) * addef);
        }
        else if(data_skill.dmgType == Message.DmgType.AP)
        {
            dmgSum += attacker.Props.ap;

            dmgSum -= target.Props.apdef;
        }

        if(dmgSum < 0)
        {
            dmgSum = 0;
        }
        else if(dmgSum >= target.HP)
        {
            target.HP = 0;
            target.SetHurt(dmgSum);
            target.Die();
            target.battleMgr.RemoveMonster(target.Name);
        }
        else
        {
            target.HP -= dmgSum;
            target.SetHurt(dmgSum);
            target.Hit();
        }
        
    }

    private bool RangeCheck(Vector3 from, Vector3 to, float range)
    {
        if(Vector3.Distance(from, to) <= range)
        {
            return true;
        }

        return false;
    }

    private bool CheckAngle(Transform trans, Vector3 to, float angle)
    {
        if(angle == 360)
        return true;

        Vector3 start = trans.forward;
        Vector3 dir = (to - trans.position).normalized;
        if(Vector3.Angle(start, dir) <= angle / 2)
        {
            return true;
        }

        return false;
    }

    public void AttackEffect(EntityBase entity, int skillId)
    {
        if(entity.GetInputDir() == Vector2.zero)
        {
            entity.SetAtkRotation(entity.GetClosedTarget());
        }
        else
        {
            entity.SetAtkRotation(entity.GetInputDir());
        }
        entity.Lock();
        entity.SetDir(Vector2.zero);
        SkillCfg data_skill = resSvc.GetSkillData(skillId);

        SetSkillMove(entity, data_skill);
    }

    private void SetSkillMove(EntityBase entity, SkillCfg data_skill)
    {
       
        if (data_skill != null)
        {
            float sumTime = 0;
            for (int i = 0; i < data_skill.skillMoveLst.Count; i++)
            {
                SkillMoveCfg data_skillMove = resSvc.GetSkillMoveCfg(data_skill.skillMoveLst[i]);
                float _speed = data_skillMove.moveDis / (data_skillMove.moveTime / 1000f);
                
                entity.SetAction(data_skill.aniAction);
                entity.SetFX(data_skill.fx, data_skill.skillTime);
                sumTime += data_skillMove.delayTime;

                if (sumTime > 0)
                {
                    timer.AddTimeTask((int tid) =>
                    {
                        entity.SetSkillMoveState(true, _speed);
                    }, data_skillMove.delayTime);
                }
                else
                {
                    entity.SetSkillMoveState(true, _speed);
                }

                sumTime += data_skillMove.moveTime;

                timer.AddTimeTask((int tid) =>
                {
                    entity.SetSkillMoveState(false);
                }, sumTime);

                timer.AddTimeTask((int tid) =>
                {
                    entity.Idle();
                }, data_skill.skillTime);
            }

        }
    }
}
