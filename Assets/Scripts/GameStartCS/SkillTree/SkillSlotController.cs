using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    HPBoost,
    Posion,
    Flame,
    SpeedUp
}

public class SkillSlotController : MonoBehaviour
{
    [SerializeField] private SkillSlotView curSkillSlot;
    [SerializeField] private SkillSlotView nextSkillSlot;
    private SkillSlotModel skillSlotModel;
    public SkillSO skillSO;
    public SkillType skillType;
    public int skillRank;

    private void Awake() 
    {
        skillSO = Instantiate(skillSO);
        skillSlotModel = new SkillSlotModel(skillSO);

        curSkillSlot.OnSkillBoost += CurSkillBoost;

        skillSlotModel.OnSkillLevelBoost += curSkillSlot.UpdateLevelText;
        skillSlotModel.OnUnlockSkill += curSkillSlot.UnlockSkillSlot;

        if(nextSkillSlot != null) 
        {
            skillSlotModel.OnUnlockNextSkill += nextSkillSlot.UnlockSkillSlot;
            skillSlotModel.OnUnlockNextSkill += SetNextSkillIsUnlock;
        }

        EventCenter.AddListener<SkillType,int>(EventNameTable.ONTELLSKILLUIUPDATE,ListenerLoadUpdateUI);
    }

    private void Start() 
    {
        skillSlotModel.Init();
        curSkillSlot.SetImageIcon(skillSO);
    }

    /// <summary>
    /// 监听加载存档触发的更新UI事件，并根据当前Skill的层级来判断并分配curLevel
    /// </summary>
    /// <param name="type">传入的这个技能的类型</param>
    /// <param name="level">传入的这个技能的总等级</param>
    public void ListenerLoadUpdateUI(SkillType type,int level)
    {
        if(skillType != type) return;

        if(level >= skillSlotModel.MaxLevel * skillRank)
        {
            skillSlotModel.CurLevel = Mathf.Min(skillSlotModel.MaxLevel,level - (skillSlotModel.MaxLevel * skillRank));
        }
    }

    /// <summary>
    /// 设置下一个技能槽的IsUnlock
    /// </summary>
    /// <param name="isUnlock"></param>
    public void SetNextSkillIsUnlock(bool isUnlock)
    {
        nextSkillSlot.gameObject.GetComponent<SkillSlotController>().skillSlotModel.IsUnlock = isUnlock;
    }

    /// <summary>
    /// 升级技能等级
    /// </summary>
    public void CurSkillBoost()
    {
        if(GameManager.Instance.playerSO.skillPoint >= skillSO.needPoint &&
            skillSlotModel.CurLevel < skillSlotModel.MaxLevel)
        {   
            skillSlotModel.CurLevel++;
            GameManager.Instance.playerSO.skillPoint -= skillSO.needPoint;

            switch(skillType)
            {
                case SkillType.HPBoost:
                SkillEffectManager.Instance.HPBoostLevel++;
                break;
                case SkillType.Posion:
                SkillEffectManager.Instance.PosionLevel++;
                //TODO：攻击敌人时会累计毒层
                break;
                case SkillType.Flame :
                SkillEffectManager.Instance.FlameLevel++;
                //TODO：被攻击的敌人获得燃烧效果，持续损失血量
                break;
                case SkillType.SpeedUp :
                SkillEffectManager.Instance.SpeedUp++;
                //TODO：角色速度获得提升，并获得冲刺功能
                break;
            }
        }
    }

    private void OnDestroy() 
    {   
        curSkillSlot.OnSkillBoost -= CurSkillBoost;

        skillSlotModel.OnSkillLevelBoost -= curSkillSlot.UpdateLevelText;
        skillSlotModel.OnUnlockSkill -= curSkillSlot.UnlockSkillSlot;

        if(nextSkillSlot != null) 
        {
            skillSlotModel.OnUnlockNextSkill -= nextSkillSlot.UnlockSkillSlot;
            skillSlotModel.OnUnlockNextSkill += SetNextSkillIsUnlock;
        }

        EventCenter.RemoveListener<SkillType,int>(EventNameTable.ONTELLSKILLUIUPDATE,ListenerLoadUpdateUI);
    }
    
}
