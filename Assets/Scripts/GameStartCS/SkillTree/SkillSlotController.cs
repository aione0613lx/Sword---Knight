using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlotController : MonoBehaviour
{
    [SerializeField] private SkillSlotView curSkillSlot;
    [SerializeField] private SkillSlotView nextSkillSlot;
    private SkillSlotModel skillSlotModel;
    public SkillSO skillSO;

    private void Awake() 
    {
        skillSlotModel = new SkillSlotModel(skillSO);

        curSkillSlot.OnSkillBoost += CurSkillBoost;

        skillSlotModel.OnSkillLevelBoost += curSkillSlot.UpdateLevelText;
        skillSlotModel.OnUnlockSkill += curSkillSlot.UnlockSkillSlot;

        if(nextSkillSlot != null) 
        {
            skillSlotModel.OnUnlockNextSkill += nextSkillSlot.UnlockSkillSlot;
            skillSlotModel.OnUnlockNextSkill += SetNextSkillIsUnlock;
        }
    }

    private void Start() 
    {
        skillSlotModel.Init();

        curSkillSlot.SetImageIcon(skillSO);
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
        if(GameManager.Instance.playerSO.skillPoint >= skillSO.needPoint)
        {
            skillSlotModel.CurLevel++;
            GameManager.Instance.playerSO.skillPoint -= skillSO.needPoint;
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
    }
    
}
