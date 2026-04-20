using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffectManager : SingletonMono<SkillEffectManager>
{   
    private PlayerHealth playerHealth;
    private int hpBoostLevel = 0;
    private int posionLevel = 0;
    private int flameLevel = 0;
    private int speedUp = 0;
    private bool recoverHPEnable = false;
    private float recoverHPCD = 20;

    public int HPBoostLevel {
        get {
            return hpBoostLevel;
        }

        set {
            if(value > 0)
            {
                hpBoostLevel++;

                if(hpBoostLevel <= 5) PlayerMaxHPBoost(1);
                else if(hpBoostLevel <= 10) PlayerMaxHPBoost(2);
                else if(hpBoostLevel <= 15) PlayerMaxHPBoost(3);
                else if(hpBoostLevel == 16)
                {
                    recoverHPEnable = true;
                    RecoverCurHP();
                }
                else if (hpBoostLevel <= 20)
                {
                    recoverHPCD -= 4;
                }
            }
        }
    }

    public int PosionLevel {
        get {
            return posionLevel;
        }

        set {
            if(value > 0)
            {
                posionLevel ++;
            }
        }
    }

    public int FlameLevel {
        get {
            return flameLevel;
        }

        set {
            if(value > 0)
            {
                flameLevel ++;
            }
        }
    }

    public int SpeedUp {
        get {
            return speedUp;
        }

        set {
            if(value > 0)
            {
                speedUp ++;
            }
        }
    }

    private void Awake() 
    {
        base.Awake();
        EventCenter.AddListener<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,GainPlayerHealth);
    }

    private void Start() 
    {
        if(hpBoostLevel >= 16) RecoverCurHP();    
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,GainPlayerHealth);    
    }

    public void GainPlayerHealth(PlayerHealth playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    public void PlayerMaxHPBoost(int value)
    {
        playerHealth.UpdatePlayerMaxHP(value);
    }

    public void RecoverCurHP()
    {
        playerHealth.UpdatePlayerHP(2);
        StartCoroutine(RecoverHPCD(recoverHPCD,recoverHPEnable));
    }

    private IEnumerator RecoverHPCD(float time,bool con)
    {
        yield return new WaitForSeconds(time);
        if(con) RecoverCurHP();
    }


    /// <summary>
    /// 加载技能的状态与等级
    /// </summary>
    /// <param name="data"></param>
    public void LoadSkillLevel(PlayerSkillTreeSaveData data)
    {   
        int index = 0;
        foreach(var skillName in data.skillName)
        {
            switch(skillName)
            {
                case "MaxHPBoost" : hpBoostLevel = data.level[index];
                break;
                case "Posion" : posionLevel = data.level[index];
                break;
                case "Flame" : flameLevel = data.level[index];
                break;
                case "SpeedUp" : SpeedUp = data.level[index];
                break;
                default :
                Debug.Log($"{skillName} 无法被识别！");
                break;
            }
            index ++;
        }

        //通知UI更新
        EventCenter.EventTrigger(EventNameTable.ONTELLSKILLUIUPDATE,SkillType.HPBoost,hpBoostLevel);
        EventCenter.EventTrigger(EventNameTable.ONTELLSKILLUIUPDATE,SkillType.Posion,posionLevel);
        EventCenter.EventTrigger(EventNameTable.ONTELLSKILLUIUPDATE,SkillType.Flame,flameLevel);
        EventCenter.EventTrigger(EventNameTable.ONTELLSKILLUIUPDATE,SkillType.SpeedUp,speedUp);
    }
}
