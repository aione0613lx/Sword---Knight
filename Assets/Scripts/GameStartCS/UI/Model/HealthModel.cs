using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class HealthModel
{
    private int currentHP;
    private int maxHP = 1;

    public event Action<int,int> OnChangeHPText;

    public void SyncCurrent()
    {
        currentHP = maxHP;
    }

    public int CurrentHP
    {
        get {
            return currentHP;
        }

        set{
            if(currentHP != value)
            {
                currentHP = value;
                OnChangeHPText?.Invoke(currentHP,maxHP);
            }
        }    
    }

    public int MaxHP
    {
        get {
            return maxHP;
        }

        set{
            if(value <= 0)
            {
                maxHP = 1;
            }

            if(maxHP != value)
            {
                maxHP = value;
                OnChangeHPText?.Invoke(currentHP,maxHP);
            }
        }
    }
}
