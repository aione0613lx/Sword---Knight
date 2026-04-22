using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalendarController : MonoBehaviour
{
    [SerializeField] private TMP_Text calendar;

    private void Awake() 
    {
        EventCenter.AddListener<int>(EventNameTable.ONCALENDARCHANGE,UpdateCalendar);    
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener<int>(EventNameTable.ONCALENDARCHANGE,UpdateCalendar);   
    }

    public void UpdateCalendar(int value)
    {
        calendar.text = value.ToString() + " DAY";
    }
}
