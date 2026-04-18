using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private HealthView healthView;
    private HealthModel healthModel;
    private PlayerStatsSO playerSO;

    private void Awake() 
    {
        healthModel = new HealthModel();

        healthModel.OnChangeHPText += healthView.UpdateText;

        EventCenter.AddListener<int>(EventNameTable.ONCHANGEHP,SetHP);
        EventCenter.AddListener<int>(EventNameTable.ONCHANGEMAXHP,SetMaxHP);
    }

    private void Start()
    {
        playerSO = GameManager.Instance.playerSO;
        healthModel.CurrentHP = playerSO.currentHP;
        healthModel.MaxHP = playerSO.maxHP;
    }

    public void SetHP(int hp)
    {
        healthModel.CurrentHP = hp;
    }

    public void SetMaxHP(int maxHP)
    {
        healthModel.MaxHP = maxHP;
    }

    private void OnDestroy() 
    {
        healthModel.OnChangeHPText -= healthView.UpdateText;   
        EventCenter.RemoveListener<int>(EventNameTable.ONCHANGEHP,SetHP); 
        EventCenter.RemoveListener<int>(EventNameTable.ONCHANGEMAXHP,SetMaxHP);
    }
}
