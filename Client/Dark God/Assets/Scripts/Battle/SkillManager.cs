using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    ResService resSvc;
    TimerService timer;
    AudioService audioSvc;
    public double skillSpaceTime = 0d;

    public void InitManager()
    {
        resSvc = ResService.Instance;
        timer = TimerService.Instance;
        audioSvc = AudioService.Instance;
        PECommon.Log("SkillManager Loading");
    }

    public void SkillAttack(EntityBase entity, int skillId)
    {
        entity?.ResetSkillActionEffectCBIndex();
        AttackEffect(entity, skillId);
        AttackDamage(entity, skillId);

    }

    private void AttackDamage(EntityBase entity, int skillId)
    {
        if (entity == null)
            return;

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
                int actionID = timer.AddTimeTask((int tid) =>
                {
                    SkillAction(entity, data_skill, index);
                    entity?.RemoveSkillActionCBItem(tid);
                    entity?.PlayEntityAttackAudio(AttackType.Heavy);
                }, sum);

                entity.skillActionCBLst.Add(actionID);
            }
            else
            {
                SkillAction(entity, data_skill, index);
            }
        }
    }

    private void SkillAction(EntityBase entity, SkillCfg data_skill, int index)
    {
        if (entity == null)
            return;

        SkillActionCfg data = resSvc.GetSkillActionCfg(data_skill.skillActionLst[index]);
        int damage = data_skill.skillDamageLst[index];

        if (entity.entityType == Message.EntityType.Player)
        {
            List<EntityMonster> monsterLst = entity.battleMgr.GetMonsterLst();
            for (int i = 0; i < monsterLst.Count; i++)
            {
                var target = monsterLst[i];
                if (RangeCheck(entity.GetPos(), target.GetPos(), data.radius)
                    && CheckAngle(entity.GetTrans(), target.GetPos(), data.angle))
                {
                    CalcDamage(entity, target, data_skill, damage);
                }
            }
        }
        else if (entity.entityType == Message.EntityType.Monster)
        {
            EntityBase target = entity.battleMgr.ep;
            if (RangeCheck(entity.GetPos(), target.GetPos(), data.radius)
                    && CheckAngle(entity.GetTrans(), target.GetPos(), data.angle))
            {

                CalcDamage(entity, target, data_skill, damage);
            }
        }
    }

    System.Random rd = new System.Random();
    private void CalcDamage(EntityBase attacker, EntityBase target, SkillCfg data_skill, int damage)
    {
        if (attacker == null || target == null)
            return;

        int dmgSum = damage;
        if (data_skill.dmgType == DmgType.AD)
        {
            int dodgeNum = PETools.RdInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge)
            {
                target.SetDodge();
                return;
            }

            dmgSum += target.Props.ad;

            int cirticalNum = PETools.RdInt(1, 100, rd);
            int addef = target.Props.addef;


            if (cirticalNum < attacker.Props.critical)
            {
                float cirticalRate = 1 + (PETools.RdInt(1, 100, rd) / 100);
                dmgSum = (int)cirticalRate * dmgSum;
                attacker.SetCritical();
            }

            dmgSum -= (int)((1 - target.Props.pierce / 100) * addef);

        }
        else if (data_skill.dmgType == DmgType.AP)
        {
            dmgSum += attacker.Props.ap;

            dmgSum -= target.Props.apdef;

        }

        if (dmgSum < 0)
        {
            dmgSum = 0;
        }
        else if (dmgSum >= target.HP)
        {
            target.HP = 0;
            target.SetHurt(dmgSum);
            target.Die();

            target.battleMgr.OnTargetDie(target.EntityId);
            if (target.entityType == Message.EntityType.Monster)
            {
                target.battleMgr.RemoveMonster(target.Name);
            }
            else if (target.entityType == Message.EntityType.Player)
            {
                target.battleMgr.StopBattle(false, 0);
                target.battleMgr.ep = null;
            }
        }
        else
        {
            target.HP -= dmgSum;
            target.SetHurt(dmgSum);
            if (target.entityState == Message.EntityState.None && target.GetBreakState())
            {
                target.Hit();
            }
        }

        target.PlayEntityHitAudio();

    }

    private bool RangeCheck(Vector3 from, Vector3 to, float range)
    {
        if (Vector3.Distance(from, to) <= range)
        {
            return true;
        }

        return false;
    }

    private bool CheckAngle(Transform trans, Vector3 to, float angle)
    {
        if (angle == 360)
            return true;

        Vector3 start = trans.forward;
        Vector3 dir = (to - trans.position).normalized;
        if (Vector3.Angle(start, dir) <= angle / 2)
        {
            return true;
        }

        return false;
    }

    public void AttackEffect(EntityBase entity, int skillId)
    {
        //if (entity == null)
        //    return;
        //
        //if (entity.entityType == Message.EntityType.Player)
        //{
        //    if (entity.GetInputDir() == Vector2.zero)
        //    {
        //        entity.SetAtkRotation(entity.GetClosedTarget());
        //    }
        //    else
        //    {
        //        entity.SetAtkRotation(entity.GetInputDir(), true);
        //    }
        //}
        //
        //SkillCfg data_skill = resSvc.GetSkillData(skillId);
        //
        //if (data_skill.isCollide)
        //{
        //    //Debug.Log(LayerMask.NameToLayer("Player"));
        //    Physics.IgnoreLayerCollision(7, 8);
        //    timer.AddTimeTask((tid) =>
        //    {
        //        Physics.IgnoreLayerCollision(7, 8, false);
        //    }, data_skill.skillTime);
        //}
        //
        //if (!data_skill.isBreak)
        //{
        //    entity.entityState = Message.EntityState.BatiState;
        //}
        //
        //entity.Lock();
        //entity.SetDir(Vector2.zero);
        //entity.SetAction(data_skill.aniAction);
        //entity.SetFX(data_skill.fx, data_skill.skillTime);
        //SetSkillMove(entity, data_skill);
        //
        //entity.skillEndCBIndex = timer.AddTimeTask((int tid) =>
        //{
        //    entity?.Idle();
        //}, data_skill.skillTime);

    }

    private void SetSkillMove(EntityBase entity, SkillCfg data_skill)
    {
       // if (entity == null)
       //     return;
       //
       // if (data_skill != null)
       // {
       //     float sumTime = 0;
       //     for (int i = 0; i < data_skill.skillMoveLst.Count; i++)
       //     {
       //         SkillMoveCfg data_skillMove = resSvc.GetSkillMoveCfg(data_skill.skillMoveLst[i]);
       //         float _speed = data_skillMove.moveDis / (data_skillMove.moveTime / 1000f);
       //
       //         sumTime += data_skillMove.delayTime;
       //
       //         if (sumTime > 0)
       //         {
       //             int moveID = timer.AddTimeTask((int tid) =>
       //             {
       //                 entity?.SetSkillMoveState(true, _speed);
       //                 entity?.RemoveSkillMoveCBItem(tid);
       //             }, data_skillMove.delayTime);
       //
       //             entity.skillMoveCBLst.Add(moveID);
       //         }
       //         else
       //         {
       //             entity.SetSkillMoveState(true, _speed);
       //         }
       //
       //         sumTime += data_skillMove.moveTime;
       //
       //         int stopID = timer.AddTimeTask((int tid) =>
       //         {
       //             entity?.SetSkillMoveState(false);
       //             entity?.RemoveSkillMoveCBItem(tid);
       //         }, sumTime);
       //
       //         entity.skillMoveCBLst.Add(stopID);
       //     }
       //
       // }
    }
}
