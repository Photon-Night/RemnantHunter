using Game.Event;
using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    protected EntityController controller;
    protected TimerService timer;
    public BattleManager bm;
    public StateManager sm;
    public bool IsHit { get; protected set; }
    public int EntityID { get; protected set; }
    public bool CanMove 
    {
        get
        {
            return controller.CanMove;
        }
    }
    public bool CanHurt { get; protected set; }
    protected string name;
    protected float hitTime;

    public bool canReleaseSkill = true;

    protected AniState currentAniState = AniState.None;
    public EntityType entityType = EntityType.None;
    public EntityState entityState = EntityState.None;

    public List<int> skillActionCBLst = new List<int>();
    public List<int> skillMoveCBLst = new List<int>();

    protected List<SkillData> normalAttackLst;
    protected List<SkillData> comboLst;
    public int skillEndCBIndex = -1;

    public SkillData CurrentSkillData { get; protected set; }
    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    protected BattleProps props;
    public BattleProps Props
    {
        get
        {
            return props;
        }
        protected set
        {
            props = value;
        }
    }
    public Vector3 BornPos { get; protected set; }
    public EntityBase Target { get; protected set; }

    private int hp;
    public int HP
    {
        get
        {
            return hp;
        }

        set
        {
            SetHpVal(hp, value);
            hp = value;
            if(hp <= 0)
            {
                IsDie = true;
            }
        }
    }
    public bool LockCtrl
    {
        get
        {
            return controller.lockCtrl;
        }
        protected set
        {
            controller.lockCtrl = value;
        }
    }
    public AniState CurrentAniState
    {
        get
        {
            return currentAniState;
        }

        set
        {
            currentAniState = value;
        }
    }
    public virtual int EquipmentADAtk => 0;
    public virtual int EquipmentADDef => 0;
    public bool IsDie { get; protected set; }
    public EntityBase()
    {

    }
    public EntityBase(BattleManager bm, StateManager sm, EntityController ec, BattleProps bps, int entityID = 0)
    {
        this.bm = bm;
        this.sm = sm;
        this.controller = ec;
        this.props = bps;
        this.EntityID = entityID;
        name = ec.gameObject.name;
        hp = bps.hp;
        timer = TimerService.Instance;
        CanHurt = true;
    }
    public void InitEntity()
    {      
        normalAttackLst = ResService.Instance.GetEntitySkillLst(EntityID, SkillType.Normal);
        comboLst = ResService.Instance.GetEntitySkillLst(EntityID, SkillType.Combo);
        hitTime = GetAnimationClipLength("Hit");

        controller.AnimationAttackDamageEvent += SkillAttack;

        GameEventManager.SubscribeEvent<int>(EventNode.Event_OnBattleEnd, OnBattleEnd);

    }
    public void SetActive(bool isActive = true)
    {
        if(controller != null)
        controller.gameObject.SetActive(isActive);
    }
    public AnimationClip GetAnimationClip(params string[] mask)
    {
        AnimationClip[] _clips = controller.anim.runtimeAnimatorController.animationClips;
        AnimationClip result = null;
        foreach (AnimationClip clip in _clips)
        {
            string name = clip.name;
            for(int i = 0; i < mask.Length; i++)
            {
                if(name.Contains(mask[i]))
                {
                    result = clip;
                    return clip;
                }
            }
        }
        return null;
    }
    public List<AnimationClip> GetAnimationClips(params string[] mask)
    {
        AnimationClip[] _clips = controller.anim.runtimeAnimatorController.animationClips;
        List<AnimationClip> result = new List<AnimationClip>();
        foreach (AnimationClip clip in _clips)
        {
            string name = clip.name;
            for (int i = 0; i < mask.Length; i++)
            {
                if (name.Contains(mask[i]))
                {
                    result.Add(clip);
                }
            }
        }

        return result;
    }

    public float GetAnimationClipLength(params string[] mask)
    {
        var clip = GetAnimationClip(mask);
        if(clip != null)
        {
            return clip.length;
        }
        else
        {
            PECommon.Log("Animation Not Find");
            return 0;
        }
    }
    public virtual void SetMove(Vector3 dir) { }
    public virtual void SetAttack()
    {
        int r = Random.Range(0, normalAttackLst.Count - 1);
        if(controller.SetAttack(normalAttackLst[r].animName))
        {
            SetAtkRotation(GetClosedTarget());
            CurrentSkillData = normalAttackLst[r];
            PlayEntityAttackAudio();
        }
    }
    public virtual void SetIdle()
    {
        controller.StopMove();
    }
    public virtual void SetCombo()
    {
        
    }
    public virtual void SetJump()
    {
        controller.SetJump();
    }
    public virtual bool SetRoll()
    {
        return true; 
    }
    public virtual void SetHit()
    {
        PlayEntityHitAudio();

        if(!IsHit)
        {
            Debug.Log("hit");
            controller.SetHit();
            IsHit = true;
            timer.AddTimeTask((tid) =>
            {
                IsHit = false;
            }, hitTime);
        }
       
    }
    public virtual void SetDie() { controller.SetDie(); }
    public virtual void SetSprint(bool isSprint)
    {
        controller.SetSprint(isSprint);
    }
    public virtual void MoveTo(Vector3 pos, bool immediately = false) { }
    protected virtual void SkillAttack(int dmgIndex)
    {
        BattleSystem.Instance.SkillAttack(this, CurrentSkillData, dmgIndex);
    }

    public virtual void StopMove() { }

    protected virtual  void OnBattleEnd(params int[] args)
    {
        controller.AnimationAttackDamageEvent -= SkillAttack;
    }
    #region old

    public void CloseCollider()
    {
        
    }

    public void Lock()
    {
        LockCtrl = true;
     
    }

    public void UnLock()
    {
        LockCtrl = false;
    }

    public virtual Vector3 GetInputDir()
    {
        return Vector3.zero;
    }

    public virtual Vector3 GetPos()
    {
        return controller.transform.position;
    }

    public virtual Transform GetTrans()
    {
        return controller.transform;
    }

    public virtual void SetDodge()
    {
        GameRoot.Instance.SetDodge(controller.name);
    }

    public void SetHurt(int hurt)
    {
        GameRoot.Instance.SetHurt(controller.name, hurt);
    }

    public void SetCritical()
    {
        GameRoot.Instance.SetCritical(controller.name);
    }

    protected virtual void SetHpVal(int oldHp, int newHp)
    {
        GameRoot.Instance.SetHpVal(controller.name, oldHp, newHp);
    }

    public void ExitCurrentAtk()
    {
        LockCtrl = false;              
    }

    public virtual void SetAtkRotation(Vector3 dir)
    {
        if(controller != null)
        {
            controller.SetAtkRotationLocal(dir);
        }
    }

    public virtual Vector3 GetClosedTarget()
    {
        return Vector3.zero;
    }

    public bool isAttack()
    {
        return !canReleaseSkill;
    }

    public AudioSource GetEntityAudioSource()
    {
        return controller.GetAudio();
    }

    public virtual void PlayEntityHitAudio()
    {
        AudioService.Instance.PlayeEntityAudioByAudioSource(GetEntityAudioSource(), Message.Hit2);
    }
    public virtual void PlayEntityAttackAudio(AttackType type = AttackType.None)
    {
        
    }
    public void RemoveSkillMoveCBItem(int tid)
    {
        int index = -1;
        for (int i = 0; i < skillMoveCBLst.Count; i++)
        {
            if(skillMoveCBLst[i] == tid)
            {
                index = i;
            }
        }

        if(index != -1)
        {
            skillMoveCBLst.RemoveAt(index);
        }
    }
    public void RemoveSkillActionCBItem(int tid)
    {
        int index = -1;
        for (int i = 0; i < skillActionCBLst.Count; i++)
        {
            if (skillActionCBLst[i] == tid)
            {
                index = i;
            }
        }

        if (index != -1)
        {
            skillActionCBLst.RemoveAt(index);
        }
    }

    public void ResetSkillActionEffectCBIndex()
    {
        skillActionCBLst.Clear();
        skillMoveCBLst.Clear();
        skillEndCBIndex = -1;
    }

    public virtual bool GetBreakState()
    {
        return true;
    }

    public void RemoveSkillCB()
    {
    //    SetSkillMoveState(false);
    //    SetDir(Vector2.zero);
    //
    //    for (int i = 0; i < skillMoveCBLst.Count; i++)
    //    {
    //        TimerService.Instance.DeleteTimeTask(skillMoveCBLst[i]);
    //    }
    //
    //    for (int i = 0; i < skillActionCBLst.Count; i++)
    //    {
    //        TimerService.Instance.DeleteTimeTask(skillActionCBLst[i]);
    //    }
    //
    //    if (skillEndCBIndex != -1)
    //    {
    //        TimerService.Instance.DeleteTimeTask(skillEndCBIndex);
    //    }
    //
    //    ResetSkillActionEffectCBIndex();
    }

    public float GetDistanceToTarget_Sqr(Vector3 target)
    {
        return Vector3.SqrMagnitude(target - GetPos());
    }

    public float GetDistanceToTarget_Sqr(Transform target)
    {
        return GetDistanceToTarget_Sqr(target.position);
    }
    #endregion
}
