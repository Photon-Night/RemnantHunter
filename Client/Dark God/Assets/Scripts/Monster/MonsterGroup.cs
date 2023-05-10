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
        public bool IsActive { get; private set; }
        private GroupData data;
        private EntityPlayer player;

        public Vector3 GroupPos { get; private set; }
        private float normalRange_sqr;
        private float battleRange_sqr;
        public float CheckRange_Sqr { get; private set; }

        public int GroupID
        {
            get
            {
                return data.ID;
            }
        }
        private List<EntityMonster> monsters;
        public List<EntityMonster> MonsterLst
        {
            get
            {
                return monsters;
            }
        }

        public float range1;
        public float range2;

        private bool canActiveAllMonster = false;

        public void InitGroup(GroupData data, EntityPlayer target, StateManager stateMgr)
        {
            this.data = data;
            player = target;

            GroupPos = data.pos;
            normalRange_sqr = data.normalRange * data.normalRange;
            battleRange_sqr = data.battleRange * data.battleRange;
            range1 = data.normalRange;
            range2 = data.battleRange;
            CheckRange_Sqr = normalRange_sqr;
        }
        public void TickGroupLogic(Transform target)
        {
            if (IsTargetInRange(target))
            {
                if(IsActive == false)
                {
                    SetHpUIItem(true);
                }
                IsActive = true;
                CheckRange_Sqr = battleRange_sqr;
                ActiveMonsters(TickAILogic_Active);
            }
            else
            {
                if(IsActive == true)
                {
                    SetHpUIItem(false);
                }
                IsActive = false;
                CheckRange_Sqr = normalRange_sqr;
                ActiveMonsters(TickAILogic_Standby);
            }
        }
        private void SetHpUIItem(bool active)
        {
            for(int i = 0; i < monsters.Count; i++)
            {
                GameRoot.Instance.SetHpUIItemActive(monsters[i].Name, active);
            }
        }
        private void TickAILogic_Active(EntityMonster entity)
        {
            entity.TickMonsterAILogic_Active(player.GetPos(), out canActiveAllMonster);
            if(canActiveAllMonster)
            {
                StartBattle_All();
            }
        }

        private void StartBattle_All()
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                monsters[i].IsBattle = true;
            }
        }

        private void TickAILogic_Standby(EntityMonster entity)
        {
            entity.TickMonsterAILogic_Standby(GroupPos, CheckRange_Sqr);
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
            if (monsters.Count == 0)
            {
                Enable = false;
            }
        }

        public void ActiveMonsters(System.Action<EntityMonster> action)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (!monsters[i].IsDie)
                    action(monsters[i]);
            }
        }

        public void ChangeRange_Battle()
        {
            CheckRange_Sqr = battleRange_sqr;
        }

        public void ChangeRange_Normal()
        {
            CheckRange_Sqr = normalRange_sqr;
        }

        public EntityMonster GetClosestMonsterToPlayer(out float min_dis)
        {
            int index = -1;
            float dis = 0;
            min_dis = float.MaxValue;
            for(int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsDie) continue;

                dis = monsters[i].GetDistanceToTarget_Sqr(player.GetPos());
                if (dis < min_dis)
                {
                    min_dis = dis;
                    index = i;
                }
            }

            return index == -1 ? null : monsters[index];
        }

        public EntityMonster GetClosestMonsterToPlayer()
        {
            float min_dis = 0;
            return GetClosestMonsterToPlayer(out min_dis);
        }

    }

}
