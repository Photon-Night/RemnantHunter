using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Monster
{
    public class MonsterGroup
    {
        public bool Enable { get; private set; }
        
        private GroupData data;
        private Transform player;

        public Vector3 GroupPos { get; private set; }
        private float normalRange_sqr;
        private float battleRange_sqr;
        public float CheckRange_Sqr { get; private set; }
        private List<EntityMonster> monsters;

        private EntityMonster patroller;

        private Vector3 startPos_patrol;
        private Vector3 endPos_patrol;

        public void InitGroup(GroupData data, Transform target)
        {
            this.data = data;
            player = target;

            GroupPos = data.pos;
            normalRange_sqr = data.normalRange * data.normalRange;
            battleRange_sqr = data.battleRange * data.battleRange;
            CheckRange_Sqr = normalRange_sqr;
            startPos_patrol = patroller.GetPos();
            endPos_patrol = data.patrolPos;
        }

        public void AddMonsters(List<EntityMonster> monsters)
        {
            this.monsters = monsters;
        }
        public float GetDistanceToTarget_Sqr(Vector3 target)
        {
            if (target == null)
                return float.NaN;
            return Vector3.SqrMagnitude(data.pos - target);          
        }

        public float GetDistanceToTarget_Sqr(Transform target)
        {
            return GetDistanceToTarget_Sqr(target.position);
        }

        public bool IsTargetInRange(Transform target)
        {
            return IsTargetInRange(target.position);
        }
        public bool IsTargetInRange(Vector3 target)
        {
            var distance_sqr = GetDistanceToTarget_Sqr(target);
            return distance_sqr < CheckRange_Sqr;           
        }
        public void SetMonster(EntityMonster monster)
        {
            monsters.Add(monster);
        }

        public void RemoveMonster(EntityMonster monster)
        {
            monsters.Remove(monster);
            if(monsters.Count == 0)
            {
                Enable = false;
            }
        }

        public void ActiveMonsters(System.Action<MonsterGroup, EntityMonster> action)
        {
            for(int i = 0; i < monsters.Count; i++)
            {
                action(this, monsters[i]);
            }
        }

        public void SetPatrol(Vector3 nextPos)
        {

        }

        public void ChangeRange_Battle()
        {
            CheckRange_Sqr = battleRange_sqr;
        }

        public void ChangeRange_Normal()
        {
            CheckRange_Sqr = normalRange_sqr;
        }

        public void IsTargetOutOfRange(Vector3 target)
        {
            
        }
    }

}
