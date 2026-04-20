using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkillSlotModel
{
    private bool isUnlock;
    private int curLevel;
    private int maxLevel;

    private SkillSO skillSO;

    public event Action<int,int> OnSkillLevelBoost;
    public event Action<bool> OnUnlockNextSkill;
    public event Action<bool> OnUnlockSkill;

    

    public SkillSlotModel(SkillSO skillSO)
    {
        this.skillSO = skillSO;
        maxLevel = skillSO.maxLevel;
        isUnlock = skillSO.isUnlock;
        curLevel = skillSO.curLevel;     
    }

    public void Init()
    {
        OnSkillLevelBoost?.Invoke(curLevel,maxLevel);
        OnUnlockSkill?.Invoke(isUnlock);
    }

    public int CurLevel {
        get {
            return curLevel;
        }

        set {
            if(curLevel != value)
            {
                curLevel = value;
                OnSkillLevelBoost?.Invoke(curLevel,maxLevel);

                if(curLevel == maxLevel)
                {
                    OnUnlockNextSkill?.Invoke(true);
                }

                skillSO.curLevel = curLevel;
            }
        }
    }

    public int MaxLevel {
        get {
            return maxLevel;
        }
    }

    public bool IsUnlock {
        get {
            return isUnlock;
        }

        set {
            if(isUnlock != value)
            {
                isUnlock = value;
                OnUnlockSkill?.Invoke(isUnlock);
                skillSO.isUnlock = value;
            }
        }
    }
}
