using System;
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

    private void Start() 
    {   
        //gold = 10;

        Debug.Log(items.Count);

        EventCenter.EventTrigger<int>(EventNameTable.ONGOLDCHANGE,gold);
        EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,items);
    }

    public void LoadItem(PlayerBackpackSavaData data)
    {
        Gold = data.gold;

        int index = 0;
        foreach(var id in data.waresID)
        {
            //通过ID获取对应的Item
            string path = MathTool.WaresIDQuery(id);
            WaresDataSO waresSO = ResManager.Instance.Load<WaresDataSO>(path);
            items.Add(new Item(waresSO,data.wareCount[index]));
            index++;
        }

        //更新UI
        EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,items);
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