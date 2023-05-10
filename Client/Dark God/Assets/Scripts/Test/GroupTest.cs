using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Test
{
    public class GroupTest : MonoBehaviour
    {
        public Transform target;
        public float range;
        public float checkRange_sqr;
        public float firstRange;
        public float secondRange;
        public EntityTest[] monsters;

        public void Awake()
        {
            checkRange_sqr = firstRange * firstRange;
            range = firstRange;
        }
        public float GetDistanceToTarget_Sqr(Vector3 target)
        {
            if (target == null)
                return float.NaN;
            return Vector3.SqrMagnitude(this.transform.position - target);
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
            return distance_sqr < checkRange_sqr;
        }
        public void ActiveMonsters(System.Action<GroupTest, EntityTest> action)
        {
            for(int i = 0; i < monsters.Length; i++)
            {
                action(this, monsters[i]);
            }
        }

        public void ActiveAll()
        {
            for(int i = 0; i < monsters.Length; i++)
            {
                monsters[i].Battle = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, range);
        }

        public void ChangeRnage(int index)
        {
            if(index == 1)
            {
                checkRange_sqr = firstRange * firstRange;
                range = firstRange;
            }
            else if(index == 2)
            {
                checkRange_sqr = secondRange * secondRange;
                range = secondRange;
            }
        }

    }
}
