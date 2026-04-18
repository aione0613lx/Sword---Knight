using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatsType
{
    Level,
    HP,
    ATK,
    DEF,
    Speed,
    Grow
}

public class StatsController : MonoBehaviour
{
    [SerializeField] private StatsView statsView;
    private StatsModel statsModel;

    private void Awake() 
    {
        statsModel = new StatsModel();

        statsModel.OnChangeStats += statsView.UpdateStatsText;

        EventCenter.AddListener<int>(EventNameTable.ONLEVELBOOST,SetModelLevel);
        EventCenter.AddListener<int>(EventNameTable.ONCHANGEMAXHP,SetModelHP);
        EventCenter.AddListener<int>(EventNameTable.ONCHANGEDAMAGE,SetModelATK);
        EventCenter.AddListener<int>(EventNameTable.ONCHANGEDEF,SetModelDEF);
        EventCenter.AddListener<float>(EventNameTable.ONCHANGESPEED,SetModelSpeed);
        EventCenter.AddListener<int>(EventNameTable.ONEXPMAXBOOST,SetModelGrow);
        
        
    }

    private void Start() 
    {
        if(GameManager.Instance == null) return;

        statsModel.Level = GameManager.Instance.playerSO.level;
        statsModel.HP = GameManager.Instance.playerSO.maxHP;
        statsModel.ATK = GameManager.Instance.playerSO.damage;
        statsModel.DEF = GameManager.Instance.playerSO.defense;
        statsModel.Speed = GameManager.Instance.playerSO.speed;
        statsModel.Grow = GameManager.Instance.playerSO.growExp; 
    }

    private void OnDestroy() 
    {
        statsModel.OnChangeStats -= statsView.UpdateStatsText;    

        EventCenter.RemoveListener<int>(EventNameTable.ONLEVELBOOST,SetModelLevel);
        EventCenter.RemoveListener<int>(EventNameTable.ONCHANGEMAXHP,SetModelHP);
        EventCenter.RemoveListener<int>(EventNameTable.ONCHANGEDAMAGE,SetModelATK);
        EventCenter.RemoveListener<int>(EventNameTable.ONCHANGEDEF,SetModelDEF);
        EventCenter.RemoveListener<float>(EventNameTable.ONCHANGESPEED,SetModelSpeed);
        EventCenter.RemoveListener<int>(EventNameTable.ONEXPMAXBOOST,SetModelGrow);
    }

    private void SetModelLevel(int value)
    {
        statsModel.Level = value;
    }

    private void SetModelHP(int value)
    {
        statsModel.HP = value;
    }

    private void SetModelATK(int value)
    {
        statsModel.ATK = value;
    }

    private void SetModelDEF(int value)
    {
        statsModel.DEF = value;
    }

    private void SetModelSpeed(float value)
    {
        statsModel.Speed = value;
    }

    private void SetModelGrow(int value)
    {
        statsModel.Grow = value;
    }
}
