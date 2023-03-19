using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBase
{
    protected EntityController controller;
    public BattleManager battleMgr;
    public SkillManager skillMgr;
    public StateManager stateMgr;
    protected TimerService timer;
    public bool IsCombo { get; set; }
    public bool IsHit { get; set; }
    protected string name;

    public int nextCombo;
    public SkillCfg currentSkillCfg;

    public bool canReleaseSkill = true;

    protected AniState currentAniState = AniState.None;
    public Message.EntityType entityType = Message.EntityType.None;
    public Message.EntityState entityState = Message.EntityState.None;

    public List<int> skillActionCBLst = new List<int>();
    public List<int> skillMoveCBLst = new List<int>();
    protected List<SkillData> normalAttackLst;
    protected List<SkillData> comboLst;
    public int skillEndCBIndex = -1;

    public readonly int EntityId;
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
    public EntityBase()
    {

    }
    public EntityBase(EntityController ec, BattleProps bps, int entityID = 0)
    {
        this.controller = ec;
        this.props = bps;
        this.EntityId = entityID;
        name = ec.gameObject.name;
        hp = bps.hp;
        timer = TimerService.Instance;
    }
    public void InitEntity(BattleManager bm, SkillManager skm, StateManager stm)
    {
        battleMgr = bm;
        skillMgr = skm;
        stateMgr = stm;       
    }
    public void SetController(EntityController controller)
    {
        this.controller = controller;
    }
    public void SetActive(bool isActive = true)
    {
        if(controller != null)
        controller.gameObject.SetActive(isActive);
    }
    public virtual void SetBattleProps(BattleProps props)
    {
        HP = props.hp;
        Props = props;
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
    public virtual void SetMove(float ver, float hor) { }
    public virtual void SetMove(Vector3 dir) { }
    public virtual void SetAttack() { }
    public virtual void SetIdle() { }
    public virtual void SetCombo() { }
    public virtual void SetJump() { }
    public virtual bool SetRoll() { return true; }
    public virtual void SetHit() { }
    public virtual void SetDie() { controller.SetDie(); }
    public virtual void SetSprint(bool isSprint) { }
    public virtual void MoveTo(Vector3 pos, bool immediately = false) { }
    protected virtual void AttackDamage(int id)
    {
        //skillMgr
    }
    public virtual void StopMove() { }
    #region old
    public void Move()
    {
        stateMgr.ChangeState(this, AniState.Move,null);
    }

    public void Idle()
    {
        stateMgr.ChangeState(this, AniState.Idle,null);
    }

    public void Attack(int skillId)
    {
        stateMgr.ChangeState(this, AniState.Attack, skillId);
    }

    public void Born()
    {
        stateMgr.ChangeState(this, AniState.Born);
    }

    public void Die()
    {
        stateMgr.ChangeState(this, AniState.Die);
    }

    public void Hit()
    {
        stateMgr.ChangeState(this, AniState.Hit);
    }

    public virtual void SetDir(Vector2 dir)
    {
        if (controller != null)
            controller.Dir = dir;
    }
    public void SkillAttack(int skillId)
    {
        skillMgr.SkillAttack(this, skillId);
    }


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

    public virtual Vector2 GetInputDir()
    {
        return Vector2.zero;
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
        GameRoot.Instance.dynamicWin.SetDodge(controller.name);
    }

    public void SetHurt(int hurt)
    {
        GameRoot.Instance.dynamicWin.SetHurt(controller.name, hurt);
    }

    public void SetCritical()
    {
        GameRoot.Instance.dynamicWin.SetCritical(controller.name);
    }

    protected virtual void SetHpVal(int oldHp, int newHp)
    {
        GameRoot.Instance.dynamicWin.SetHpVal(controller.name, oldHp, newHp);
    }

    public void ExitCurrentAtk()
    {
        LockCtrl = false;              
    }

    public virtual void SetAtkRotation(Vector2 dir)
    {
        if(controller != null)
        {
            
        }
    }

    public virtual Vector2 GetClosedTarget()
    {
        return Vector2.zero;
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
