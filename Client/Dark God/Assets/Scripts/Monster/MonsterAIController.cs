using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Monster
{
    public class MonsterAIController
    {
        private StateManager stateMgr;
        private Transform target;
        public void InitAIController(StateManager stateMgr)
        {
            this.stateMgr = stateMgr;
        }

        public void LogicRoot(MonsterGroup group)
        {           
            if(group.IsTargetInRange(target))
            {
                group.ChangeRange_Battle();
                group.ActiveMonsters(TickAILogic_Battle);               
            }
            else
            {
                group.ChangeRange_Normal();
                group.ActiveMonsters(TickAILogic_Normal);
            }
            
        }


        private void TickAILogic_Normal(MonsterGroup from, EntityMonster entity)
        {
            if (entity is null || !entity.RunAI)
                return;
            if(entity.IsBattle)
            {
                if (entity.CurrentAniState != AniState.Back)
                {
                    var distance = entity.GetDistanceToTarget_Sqr(from.GroupPos);
                    if (distance > from.CheckRange_Sqr)
                    {
                        stateMgr.ChangeState(entity, AniState.Back);
                    }
                    else
                    {
                        if (IsEntityBackToBorn(entity))
                        {
                            stateMgr.ChangeState(entity, AniState.Idle);
                            entity.IsBattle = false;
                        }
                    }
                }                
            }
            else
            {
                
                stateMgr.ChangeState(entity, AniState.Idle);
            }
        }
        private void TickAILogic_Battle(MonsterGroup from, EntityMonster entity)
        {
            if(entity.CurrentAniState == AniState.Hit)
            {
                return;
            }

            if(entity.CurrentAniState == AniState.Back)
            {
                if(IsEntityBackToBorn(entity))
                {
                    stateMgr.ChangeState(entity, AniState.Idle);
                }
                else
                {
                    return;
                }
            }

            var distance = entity.GetDistanceToTarget_Sqr(target.position);

            if(distance < entity.attackRange_sqr)
            {
                stateMgr.ChangeState(entity, AniState.Attack);
            }
            else
            {
                stateMgr.ChangeState(entity, AniState.Chase);
            }
                        
        }

        private bool IsEntityBackToBorn(EntityMonster entity)
        {
            if (entity is null)
                return false;

            var distance = entity.GetDistanceToTarget_Sqr(entity.bornPos);
            return distance < .1f;
        }
       
    }
}
