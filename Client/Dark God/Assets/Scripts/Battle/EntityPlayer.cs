using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    public float DodgeTime { get; private set; }
    public EntityPlayer(PlayerController pc, BattleProps bps) : base(pc, bps)
    {
        entityType = Message.EntityType.Player;
    }

    public bool IsDodge { get; private set; }

    #region Base Setting
    public void InitPlayer(BattleManager bm, SkillManager skm, StateManager stm, string name)
    {
        base.InitEntity(bm, skm, stm);
        this.name = name;
        currentAniState = AniState.Idle;
    }
    #endregion

    #region Battle Interface

    public override Vector2 GetClosedTarget()
    {
        EntityMonster target = battleMgr.FindClosedMonster(this.GetTrans());
        if (target != null)
        {
            Vector2 dir = new Vector2(target.GetPos().x - this.GetPos().x, target.GetPos().z - this.GetPos().z);
            return dir.normalized;
        }
        return Vector2.zero;
    }

    protected override void SetHpVal(int oldHp, int newHp)
    {
        battleMgr.SetHPUI(newHp);
    }

    public override void SetDodge()
    {
        GameRoot.Instance.dynamicWin.SetDodgePlayer();
    }

    public override void PlayEntityHitAudio()
    {
        AudioService.Instance.PlayeEntityAudioByAudioSource(GetEntityAudioSource(), Message.Hit1);
    }
    public override void PlayEntityAttackAudio(AttackType type = AttackType.None)
    {
        switch (type)
        {
            case AttackType.Heavy:
                AudioService.Instance.PlayeEntityAudioByAudioSource(GetEntityAudioSource(), Message.SwordWave_Heavy);
                break;
            case AttackType.Normal:
                AudioService.Instance.PlayeEntityAudioByAudioSource(GetEntityAudioSource(), Message.SwordWave_Normal);
                break;
            default:
                break;
        }
    }

    public override void SetMove(Vector3 dir)
    {
        if (dir.sqrMagnitude != 0)
            currentAniState = AniState.Move;
        else
            currentAniState = AniState.Idle;
        controller.SetMove(dir);
    }

    public override void SetAttack()
    {
        int r = Random.Range(0, normalAttackLst.Count);
        controller?.SetNormalAttack(normalAttackLst[r].skillName);
    }

    public override bool SetRoll()
    {
        if (controller is null)
            return false;
        if(controller.SetRoll())
        {
            //ÉÁ±ÜÂß¼­
            IsDodge = true;
            return true;
        }

        return false;
    }

    public override void SetHit()
    {
        controller?.SetHit();
    }
    public override void SetSprint(bool isSprint)
    {
        controller?.SetSprint(isSprint);
    }

    public override void SetCombo()
    {
        if (controller is null)
            return;
        if (controller.SetCombo())
        {

        }
    }
    public override void SetJump()
    {
        if (controller is null)
            return;

        if (controller.SetJump())
        {
            //ÌøÔ¾Âß¼­
        }
    }
    #endregion
}
