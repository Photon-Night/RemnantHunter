using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TriggerOperator : StateMachineBehaviour
{
    public string[] triggerName;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {      
        for (int i = 0; i < triggerName.Length; i++)
        {
            animator.ResetTrigger(triggerName[i]);
        }
    }
}

