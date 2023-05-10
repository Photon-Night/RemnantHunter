using System.Collections;
using System;
using UnityEngine;
using Game.Event;

public class NPCController : MonoBehaviour
{
    private Animator anim;
    public NPCCfg NpcData { get; private set; }
    private bool isInteractive = false;
    private PlayerController pc;
    private Vector3 startForward;
    private bool turnBack = false;
    private NpcTaskStatus taskStatus;
    public NpcTaskStatus TaskStatus
    { 
        get
        {
            return taskStatus;
        }
        set
        {
            if(taskStatus != value)
            {
                taskStatus = value;
                GameEventManager.TriggerEvent(EventNode.Event_OnNPCTaskStatusChange, NpcData.ID, (int)taskStatus);
                Debug.Log(value);
            }
        }
    }
    public Transform uiRoot;
    public int StatusIndex { get; set; }

    public void InitNPC(NPCCfg data, PlayerController player, NpcTaskStatus status)
    {
        NpcData = data;
        anim = this.GetComponent<Animator>();
        pc = player;
        startForward = this.transform.forward;
        taskStatus = status;
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
            GameEventManager.TriggerEvent(EventNode.Event_OnPlayerCloseToNpc, NpcData);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            GameEventManager.TriggerEvent(EventNode.Event_OnPlayerFarToNpc, NpcData);
            if (turnBack)
            {
                StartCoroutine(FaceToTheTarget(startForward));
                turnBack = false;
            }
        }
    }
}
