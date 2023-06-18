using Game.Event;
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

    public void SkillAttack(EntityBase entity, SkillData skill, int dmgIndex)
    {
        //entity?.ResetSkillActionEffectCBIndex();
        //AttackEffect(entity, skill);
        AttackDamage(entity, skill, dmgIndex);

    }

    private void AttackDamage(EntityBase entity, SkillData skill, int dmgIndex)
    {
        if (entity == null)
            return;
        SkillActionData action_data = resSvc.GetSkillActionData(skill.skillActionLst[dmgIndex]);
        int damge = skill.skillDamageLst[dmgIndex];

        if(entity.entityType == EntityType.Player)
        {
            List<EntityMonster> monsters = entity.bm.GetActiveMonsters();
            for(int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsDie) continue;

                var target = monsters[i];
                if(ActionCheck(entity, target, action_data))
                {
                    CalcDamage(entity, target, skill, damge);
                }
            }
        }
        else if(entity.entityType == EntityType.Monster)
        {
            var target = entity.Target;
            
            if (ActionCheck(entity, target, action_data))
            {
                CalcDamage(entity, target, skill, damge);
            }
        }
    }

    private bool ActionCheck(EntityBase attacker, EntityBase target, SkillActionData action)
    {
        return RangeCheck(attacker.GetPos(), target.GetPos(), action.radius) && CheckAngle(attacker.GetTrans(), target.GetPos(), action.angle);
    }


    System.Random rd = new System.Random();
    private void CalcDamage(EntityBase attacker, EntityBase target, SkillData data_skill, int damage)
    {
        if (attacker == null || target == null)
            return;
        
        if (!target.CanHurt) return;

        int dmgSum = damage;
        if (data_skill.dmgType == DmgType.AD)
        {
            int dodgeNum = PETools.RdInt(1, 100, rd);
            if (dodgeNum <= target.Props.dodge)
            {
                target.SetDodge();
                return;
            }
        
            dmgSum += attacker.Props.ad + attacker.EquipmentADAtk;
        
            int cirticalNum = PETools.RdInt(1, 100, rd);
            int addef = target.Props.addef;
        
        
            if (cirticalNum < attacker.Props.critical)
            {
                float cirticalRate = 1 + (PETools.RdInt(1, 100, rd) / 100);
                dmgSum = (int)cirticalRate * dmgSum;
                attacker.SetCritical();
            }
            dmgSum -= (int)((1 - target.Props.pierce / 100) * addef) + target.EquipmentADDef;
        }
        else if (data_skill.dmgType == DmgType.AP)
        {
            dmgSum += attacker.Props.ap;
        
            dmgSum -= target.Props.apdef;
        
        }
        
        if (dmgSum <= 0)
        {
            dmgSum = 0;
            if(target.entityType == EntityType.Player)
            {
                GameRoot.Instance.SetDefinePlayer();
            }
        }
        else if (dmgSum >= target.HP)
        {
            target.HP = 0;
            target.SetHurt(dmgSum);
            target.SetDie();
            
            if (target.entityType == EntityType.Monster)
            {
                target.bm.RemoveMonster(target.Name);
            }
            else if (target.entityType == EntityType.Player)
            {
                target.bm.StopBattle(false, 0);
                target.bm.ep = null;
            }
        
            GameEventManager.TriggerEvent<int>(EventNode.Event_OnKillMonster, (int)TaskType.Kill, target.EntityID);
        }
        else
        {
            target.HP -= dmgSum;
            target.SetHurt(dmgSum);
            if (target.entityState == EntityState.None && target.GetBreakState())
            {
                target.SetHit();
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

    public void AttackEffect(EntityBase entity, SkillData skill)
    {
        if (entity == null)
            return;
        
        if (entity.entityType == EntityType.Player)
        {
            if (entity.GetInputDir() == Vector3.zero)
            {
                entity.SetAtkRotation(entity.GetClosedTarget());
            }
            else
            {
                entity.SetAtkRotation(entity.GetInputDir());
            }
        }
        
        
        
        //if (skill.isCollide)
        //{
        //    //Debug.Log(LayerMask.NameToLayer("Player"));
        //    Physics.IgnoreLayerCollision(7, 8);
        //    timer.AddTimeTask((tid) =>
        //    {
        //        Physics.IgnoreLayerCollision(7, 8, false);
        //    }, skill.skillTime);
        //}
        
        //if (!skill.isBreak)
        //{
        //    entity.entityState = EntityState.BatiState;
        //}
        
        //entity.Lock();
        //entity.SetDir(Vector2.zero);
        //entity.SetAction(data_skill.aniAction);
        //entity.SetFX(data_skill.fx, data_skill.skillTime);
        //SetSkillMove(entity, data_skill);
        
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
