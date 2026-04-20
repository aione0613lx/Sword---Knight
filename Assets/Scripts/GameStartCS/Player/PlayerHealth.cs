using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{   
    public PlayerStatsSO playerSO;
    

    private void Start() 
    {
        EventCenter.AddListener<EnemyStats>(EventNameTable.ONENEMYDIE,EXPBoost);    
    }

    public void UpdatePlayerHP(int value)
    {
        playerSO.currentHP += value;

        if(playerSO.currentHP > playerSO.maxHP) playerSO.currentHP = playerSO.maxHP;

        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEHP,playerSO.currentHP);

        if(playerSO.currentHP <= 0)
        {
            Die();
        }
    }

    public void ReceiveDamage(int value)
    {
        int v = Mathf.Abs(value);
        if(playerSO.defense > 0) v = MathTool.CalculateDefense(playerSO.defense,v);
        playerSO.currentHP -= v;

        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEHP,playerSO.currentHP);

        if(playerSO.currentHP <= 0)
        {
            Die();
        }
    }

    public void Revive()
    {
        playerSO.currentHP = playerSO.maxHP;
        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEHP,playerSO.currentHP);
    }

    private void Die()
    {   
        Debug.Log("Player die");
        //TODO:处理玩家死亡逻辑
        if(GameManager.Instance == null)
        {
            Debug.Log("GameManager为空");
            return;
        }
        GameManager.Instance.MainCanvas = false;
        GameManager.Instance.DieCanvas = true;
        Time.timeScale = 0;
    }

    public void UpdatePlayerMaxHP(int value)
    {
        playerSO.maxHP += value;

        if(playerSO.maxHP <= 0) playerSO.maxHP = 1;

        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEMAXHP,playerSO.maxHP);
    }

    public void UpdateDefense(int value)
    {
        playerSO.defense += value;
        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEDEF,playerSO.defense);
    }

    public void UpdateSpeed(float value)
    {
        playerSO.speed += value;
        EventCenter.EventTrigger<float>(EventNameTable.ONCHANGESPEED,playerSO.speed);
    }

    public void UpdateDamage(int value)
    {
        playerSO.damage += value;
        EventCenter.EventTrigger<int>(EventNameTable.ONCHANGEDAMAGE,playerSO.damage);
    }

    public void EXPBoost(EnemyStats enemy)
    {
        playerSO.curExp += enemy.exp;

        if(playerSO.curExp >= playerSO.growExp)
        {
            LevelBoost();
            playerSO.curExp -= playerSO.growExp;
            playerSO.growExp = (int)(playerSO.multiplier * playerSO.growExp);
        }

        EventCenter.EventTrigger<int>(EventNameTable.ONEXPMAXBOOST,playerSO.growExp);
        EventCenter.EventTrigger<int>(EventNameTable.ONEXPCURUPDATE,playerSO.curExp);
    }

    public void EXPBoost(int value)
    {
        playerSO.curExp += value;

        if(playerSO.curExp >= playerSO.growExp)
        {
            LevelBoost();
            playerSO.curExp -= playerSO.growExp;
            playerSO.growExp = (int)(playerSO.multiplier * playerSO.growExp);
        }

        EventCenter.EventTrigger<int>(EventNameTable.ONEXPMAXBOOST,playerSO.growExp);
        EventCenter.EventTrigger<int>(EventNameTable.ONEXPCURUPDATE,playerSO.curExp);
    }

    public void LevelBoost()
    {
        playerSO.level ++;
        playerSO.maxHP += 2;
        playerSO.defense ++;
        playerSO.skillPoint ++;
        if(playerSO.level % 2 == 0) playerSO.damage ++;

        EventCenter.EventTrigger<int>(EventNameTable.ONLEVELBOOST,playerSO.level);
    }
}
