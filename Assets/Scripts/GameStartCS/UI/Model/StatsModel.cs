using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsModel
{
    private int level;
    private int hP;
    private int aTK;
    private int dEF;
    private float speed;
    private int grow;

    public event Action<StatsType,float> OnChangeStats;

    public StatsModel()
    {
        level = 0;
        hP = 0;
        aTK = 0;
        dEF = 0;
        speed = 0;
        grow = 0;
    }

    public int Level {
        get {
            return level;
        }

        set {
            if(level != value)
            {
                level = value;
                OnChangeStats(StatsType.Level,level);
            }
        }
    }

    public int HP {
        get {
            return hP;
        }

        set {
            if(hP != value)
            {
                hP = value;
                OnChangeStats(StatsType.HP,hP);
            }
        }
    }

    public int ATK {
        get {
            return aTK;
        }

        set {
            if(aTK != value)
            {
                aTK = value;
                OnChangeStats(StatsType.ATK,aTK);
            }
        }
    }

    public int DEF {
        get {
            return dEF;
        }

        set {
            if(dEF != value)
            {
                dEF = value;
                OnChangeStats(StatsType.DEF,dEF);
            }
        }
    }

    public float Speed {
        get {
            return speed;
        }

        set {
            if(speed != value)
            {
                speed = value;
                OnChangeStats(StatsType.Speed,speed);
            }
        }
    }

    public int Grow {
        get {
            return grow;
        }

        set {
            if(grow != value)
            {
                grow = value;
                OnChangeStats(StatsType.Grow,grow);
            }
        }
    }
}
