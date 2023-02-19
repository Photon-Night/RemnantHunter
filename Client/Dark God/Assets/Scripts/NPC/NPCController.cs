using System.Collections;
using System;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Animator anim;
    private NPCCfg NpcData;
    private bool isInteractive = false;
    private PlayerController pc;
    private Vector3 startForward;
    private bool turnBack = false;
    public Action<NPCCfg> OnPlayerGetClose;
    private NpcTaskStatus taskStatus;

    public void InitNPC(NPCCfg data, PlayerController player)
    {
        NpcData = data;
        anim = this.GetComponent<Animator>();
        pc = player;
        startForward = this.transform.forward;
        TaskSystem.Instance.SetRegisterEvent(OnTaskStatusChange);
    }

    public void OnDestroy()
    {
        TaskSystem.Instance.SetRegisterEvent(OnTaskStatusChange, false);
    }
    private void OnTaskStatusChange(TaskItem task)
    {
        if(task.data.submitNpcID == NpcData.ID)
        {
            taskStatus = TaskSystem.Instance.GetNpcTaskStatus(NpcData.ID);
        }
    }

    public void RegisterEvnet(Action<NPCCfg> action)
    {
        if (OnPlayerGetClose == null)
            OnPlayerGetClose = action;
        else
        OnPlayerGetClose += action;
    }

    public void OnPlayerExit()
    {
        MainCitySystem.Instance.FarToPlayer();
    }

    public void SetAction(int action)
    {
        anim.SetInteger("Action", action);
    }

    public void Interactive()
    {
        if(!isInteractive)
        {
            isInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    IEnumerator DoInteractive()
    {
        Vector3 faceTo = (pc.transform.position - this.transform.position).normalized;
        yield return FaceToTheTarget(faceTo);
        turnBack = true;
        yield return new WaitForSeconds(3f);
        isInteractive = false;
    }

    IEnumerator FaceToTheTarget(Vector3 target)
    {
        Vector3 faceTo = target;
        faceTo.y = startForward.y;
        while(Mathf.Abs(Vector3.Angle(this.transform.forward, faceTo)) > 5)
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (OnPlayerGetClose != null)
                OnPlayerGetClose(NpcData);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            OnPlayerExit();
            if (turnBack)
            {
                StartCoroutine(FaceToTheTarget(startForward));
                turnBack = false;
            }
        }
    }
}
