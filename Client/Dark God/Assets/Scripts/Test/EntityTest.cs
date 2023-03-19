using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Test
{
    public class EntityTest : MonoBehaviour
    {
        public Vector3 bornPos;
        public Transform target;
        public bool IsBack;
        public NavMeshAgent agent;
        public float attackRange_sqr;
        public float range;

        public int EntityType;
        public void Start()
        {
            bornPos = transform.position;
            attackRange_sqr = range * range;
        }

        public void MoveTo(Vector3 pos)
        {
            agent.SetDestination(pos);
        }

        public float GetDistanceToTarget_Sqr(Vector3 pos)
        {
            return Vector3.SqrMagnitude(transform.position - pos);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, range);
        }
    }
}
