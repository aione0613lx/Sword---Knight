using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerBackpack : SingletonMono<PlayerBackpack>
{
    private int gold;
    public List<Item> items = new List<Item>();

    public int Gold {
        get {
            return gold;
        }

        set {
            if(gold != value)
            {
                gold = value;
                EventCenter.EventTrigger<int>(EventNameTable.ONGOLDCHANGE,gold);
            }
        }
    }
}

public class Item
{   
    public Item(){}

    public Item(WaresDataSO waresDataSO,int value)
    {
        this.waresDataSO = waresDataSO;
        count = value;
    }

    public WaresDataSO waresDataSO;
    public int count;
}