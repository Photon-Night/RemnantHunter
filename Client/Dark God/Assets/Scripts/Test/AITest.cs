using Game.Monster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Test
{
    public class AITest : MonoBehaviour
    {        
        public Transform target;
        public GroupTest[] groups;     

        public void Update()
        {
            for(int i = 0; i < groups.Length; i++)
            {
                LogicRoot(groups[i]);
            }
        }

        public void LogicRoot(GroupTest group)
        {
            if(group.IsTargetInRange(target))
            {
                group.ChangeRnage(2);
                group.ActiveMonsters(TickAILogic_Battle);
                Debug.Log("Battle");
            }
            else
            {
                group.ActiveMonsters(TickAILogic_Standby);
                Debug.Log("Standby");
            }

        }

        private void TickAILogic_Standby(GroupTest form, EntityTest entity)
        {
            form.ChangeRnage(1);

            if(entity.target != null)
            {
                float distance;
                if (!entity.IsBack)
                {
                    distance = entity.GetDistanceToTarget_Sqr(transform.position);

                    if (distance > form.checkRange_sqr)
                    {
                        entity.MoveTo(entity.bornPos);
                        entity.IsBack = true;
                        Debug.Log("back");
                    }

                    else
                    {
                        entity.MoveTo(entity.bornPos);
                        distance = entity.GetDistanceToTarget_Sqr(entity.bornPos);
                        if (distance < .1f)
                        {
                            entity.transform.position = entity.bornPos;
                            entity.target = null;
                            entity.IsBack = false;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("idle");
            }
            
        }

        private void TickAILogic_Battle(GroupTest form, EntityTest entity)
        {
            if (entity.IsBack)
            {
                float dis = entity.GetDistanceToTarget_Sqr(entity.bornPos);
                if(dis < .1f)
                {
                    entity.transform.position = entity.bornPos;
                    entity.IsBack = false;
                }
                else
                {
                    return;
                }
            }
            if (entity.target == null)
                entity.target = target;

            var distance = entity.GetDistanceToTarget_Sqr(target.position);

            if(distance < entity.attackRange_sqr)
            {
                Debug.Log("attack");
            }
            else
            {
                Debug.Log("chase");

                if (entity.EntityType == 1)
                {
                    entity.MoveTo(target.position);
                }
                else if(entity.EntityType == 2)
                {
                    Vector3 pos = target.position - target.forward * entity.range;
                    entity.MoveTo(pos);
                }
                else if(entity.EntityType == 3)
                {
                    Vector3 pos = target.position + target.right * entity.range;
                    entity.MoveTo(pos);
                }
                else if (entity.EntityType == 4)
                {
                    Vector3 pos = target.position - target.right * entity.range;
                    entity.MoveTo(pos);
                }
            }
        }

        
    }
}
