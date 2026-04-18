using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPModel
{
    private int maxValue;
    private int curValue;
    private int level;

    public event Action<int> OnChangeMaxValue;
    public event Action<int> OnChangeCurValue;
    public event Action<int> OnChangeLevel;
    
    public EXPModel()
    {
        maxValue = 10;
        curValue = 0;
        level = 1;
    }

    public int MaxValue {
        get {
            return maxValue;
        }

        set {
            if(maxValue != value)
            {
                maxValue = value;
                OnChangeMaxValue?.Invoke(maxValue);
            }
        }
    }

    public int CurValue {
        get {
            return curValue;
        }

        set {
            if(curValue != value)
            {
                curValue = value;
                OnChangeCurValue?.Invoke(curValue);
            }
        }
    }

    public int Level {
        get {
            return level;
        }

        set {
            if(level != value)
            {
                level = value;
                OnChangeLevel?.Invoke(level);
            }
        }
    }
}
