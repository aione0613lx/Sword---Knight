using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;

    public void UpdateText(int currentHP,int maxHP)
    {
        healthText.text = "HP:" + currentHP.ToString() + "/" + maxHP.ToString();
    }
}
