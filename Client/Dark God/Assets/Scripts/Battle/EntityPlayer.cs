using PEProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    private float dodgeTime;

    private float power;
    private float powerRecoverSpeed = 10;
    private float powerNum;
    private float spRecoverTimeOrigin = 0f;

    private int currentComboIndex;
    private bool canCombo;
    private GameItemCfg weaponData;
    private GameItemCfg shieldData;

    public bool CanUsePotion_ADAtk { get; private set; }
    public bool CanUsePotion_AdDef { get; private set; }
    public override int EquipmentADAtk => weaponData.funcType == ItemFunction.ADAtk ? (int)weaponData.funcNum : base.EquipmentADAtk;
    public override int EquipmentADDef => shieldData.funcType == ItemFunction.ADDef ? (int)shieldData.funcNum : base.EquipmentADDef;
    public float Power
    {
        get
        {
            return power;
        }

        set
        {
            SetPowerVal(value);
            power = value;
        }
    }

    public override int ADAtk => base.ADAtk + (int)weaponData.funcNum;
    public override int ADDef => base.ADDef + (int)shieldData.funcNum;
    private int skillPointCount = 5;
    public int SkillPointCount
    {
        get
        {
            return skillPointCount;
        }
        set
        {
            SetSkillPointVal(value);
            skillPointCount = value;
        }
    }

    public EntityPlayer(BattleManager bm, StateManager sm, PlayerController pc, BattleProps bps, string name, int weaponID, int shieldID) : base(bm, sm, pc, bps)
    {
        entityType =EntityType.Player;
        power = Props.power;
        InitPlayer(name, weaponID, shieldID);
    }

    #region Base Setting
    public void InitPlayer(string name, int weaponID, int shieldID)
    {
        base.InitEntity();
        this.name = name;
        currentAniState = AniState.Idle;

        powerNum = power;
        canCombo = true;
        currentComboIndex = 0;
        dodgeTime = GetAnimationClipLength("Roll");
        shieldData = ResService.Instance.GetGameItemCfg(shieldID);
        weaponData = ResService.Instance.GetGameItemCfg(weaponID);
        CanUsePotion_ADAtk = true;
        CanUsePotion_AdDef = true;
    }

    #endregion

    #region Battle Interface

    public override Vector3 GetClosedTarget()
    {
        EntityMonster target = BattleSystem.Instance.FindClosedMonster(25f);
        
        if (target != null)
        {
            Vector3 dir = new Vector3(target.GetPos().x - this.GetPos().x, 0f, target.GetPos().z - this.GetPos().z);
            Debug.Log(target.Name);
            return dir.normalized;
        }
        return Vector3.zero;
    }

    public override Vector3 GetInputDir()
    {
        if(controller is PlayerController pc)
        {
            if(pc.MoveAmount != 0)
            {
                return pc.Dir;
            }
        }

        return Vector3.zero;
    }

    protected override void SetHpVal(int oldHp, int newHp)
    {
        BattleSystem.Instance.SetHPUI(newHp);
    }

    public override void SetDodge()
    {
        GameRoot.Instance.SetDodgePlayer();
    }
    private void SetPowerVal(float newPower)
    {
        BattleSystem.Instance.SetPowerUI(newPower);
    }

    private void SetSkillPointVal(int count)
    {
        BattleSystem.Instance.SetSkillCount(count);
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
        var skill = normalAttackLst[r];
        if (Power >= skill.powerCost)
        {
            if (controller.SetAttack(skill.animName))
            {
                CurrentSkillData = normalAttackLst[r];
                PlayEntityAttackAudio(AttackType.Normal);
                SetAtkRotation(GetClosedTarget());
                Power -= skill.powerCost;
            }
        }
    }

    public override bool SetRoll()
    {
        if (controller is null)
            return false;

        if(Power >= Message.RollPowerCost)
        {
            if (controller.SetRoll())
            {
                CanHurt = false;
                Power -= Message.RollPowerCost;
                timer.AddTimeTask((tid) =>
                {
                    CanHurt = true;
                }, dodgeTime + .5f);

                return true;
            }
        }
        

        return false;
    }
    public override void SetSprint(bool isSprint)
    {
        controller?.SetSprint(isSprint);
    }

    public override void SetCombo()
    {

        if (SkillPointCount <= 0 || !canCombo) return;

        if (controller.CanMove)
        {           
            currentComboIndex = 0;
        }
        
        if (controller.SetCombo(comboLst[currentComboIndex].animName))
        {
            SetAtkRotation(GetClosedTarget());
            PlayEntityAttackAudio(AttackType.Heavy);
            SkillPointCount -= 1;
            CurrentSkillData = comboLst[currentComboIndex];
            currentComboIndex += 1;
            if(currentComboIndex >= comboLst.Count)
            {
                currentComboIndex = 0;
            }
            canCombo = false;
            timer.AddTimeTask((tid) =>
            {
                canCombo = true;
            }, comboLst[currentComboIndex].comboCheckTime);
        }
        
    }
    public override void SetJump()
    {
        return;

        if (controller is null)
            return;

        if (controller.SetJump())
        {
            //ÌøÔ¾Âß¼­
        }
    }
    public override void SetHurt(int hurt)
    {
        GameRoot.Instance.SetHurtPlayer(hurt);
    }
    #endregion

    public void RecoverPower(float delta)
    {
        if(controller.CanMove && Power <= powerNum)
        {
            Power += delta * powerRecoverSpeed;
        }
    }

    public void RecoverSkillPoint(float delta)
    {
        if (SkillPointCount < Message.SkillPointCount)
        {
            spRecoverTimeOrigin += delta;
            if (spRecoverTimeOrigin > Message.SkillPointRecoverTime)
            {
                SkillPointCount += 1;
                spRecoverTimeOrigin = 0;
            }
        }     
    }

    public void SetADAtkByPotion(int ad, float duration)
    {
        Props.ad += ad;
        if(duration > 0)
        {
            CanUsePotion_ADAtk = false;
            timer.AddTimeTask((tid) =>
            {
                if(Props != null)
                {
                    Props.ad -= ad;
                    CanUsePotion_ADAtk = true;
                    Debug.Log("endpotion");
                }               
            }, duration);
        }

    }

    public void SetADDefByPotion(int addef, float duration)
    {
        Props.addef += addef;
        if (duration > 0)
        {
            CanUsePotion_AdDef = false;
            timer.AddTimeTask((tid) =>
            {
                if (Props != null)
                {
                    Props.addef -= addef;
                    CanUsePotion_AdDef = true;
                    Debug.Log("endpotion");
                }
            }, duration);
        }
    }

    public void ChangePlayerEquipment(GameItemCfg equipmentData)
    {
        if (equipmentData.equipmentType == EquipmentType.Shield)
            shieldData = equipmentData;
        else if (equipmentData.equipmentType == EquipmentType.Weapon)
            weaponData = equipmentData;

        controller.ChangeEquipment(equipmentData);
    }
}
