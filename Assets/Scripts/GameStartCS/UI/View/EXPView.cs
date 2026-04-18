using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EXPView : MonoBehaviour
{
    [SerializeField] private Slider expSilder;
    [SerializeField] private TMP_Text levelText;

    private void Start() 
    {
        expSilder = GetComponent<Slider>();    
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "LEVEL:" + level.ToString();
    }

    public void SetMaxValue(int value)
    {
        expSilder.maxValue = value;
    }

    public void UpdateEXPValue(int value)
    {
        expSilder.value = value;
    }
}
