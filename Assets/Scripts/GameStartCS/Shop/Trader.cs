using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Trader : MonoBehaviour
{
    public TraderSO traderSO;
    public Vector2 pos;
    private bool playerInRange = false;
    private int day;

    public int Day {
        get {
            return day;
        }

        set {
            if(day > 0)
            {
                day = value;
                if(day <= 0)
                {
                    EventCenter.EventTrigger<Trader>(EventNameTable.ONTRADERLEAVE,this);
                }
            }
        }
    }

    private void Awake()
    {
        EventCenter.AddListener<int>(EventNameTable.ONCALENDARCHANGE,DayUpdate);
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener<int>(EventNameTable.ONCALENDARCHANGE,DayUpdate);
    }

    public void DayUpdate(int day)
    {
        Day--;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && !playerInRange)
        {   
            playerInRange = true;

            EventCenter.AddListener(EventNameTable.ONOPENSHOP,OpenShop);

            DisplayPromptUI();
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && playerInRange)
        {
            playerInRange = false;

            EventCenter.RemoveListener(EventNameTable.ONOPENSHOP,OpenShop);

            ClosePromptUI();
        }    
    }

    private void OpenShop()
    {
        ClosePromptUI();
        EventCenter.EventTrigger<TraderSO>(EventNameTable.ONINTRODUCETRADER,this.traderSO);
    }

    /// <summary>
    /// 打开提示词UI
    /// </summary>
    private void DisplayPromptUI()
    {   
        Debug.Log("打开提示词！");
        EventCenter.EventTrigger(EventNameTable.ONOPENTELLUI);
    }

    /// <summary>
    /// 关闭提示词UI
    /// </summary>
    private void ClosePromptUI()
    {
        Debug.Log("关闭提示词！");
        EventCenter.EventTrigger(EventNameTable.ONCLOSETELLUI);
    }
}

public enum TraderState
{
    Idel,
    Escape
}
