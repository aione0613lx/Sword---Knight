using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiePannelController : MonoBehaviour
{
    [SerializeField] private Button reviveButton;

    private void Awake() 
    {
        reviveButton.onClick.AddListener(() => ReviveButtonDown());    
    }

    public void ReviveButtonDown()
    {
        EventCenter.EventTrigger(EventNameTable.ONREVIVEBUTTONDOWN);
    }
}
