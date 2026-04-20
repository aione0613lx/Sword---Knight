using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackpackView : MonoBehaviour
{
    [SerializeField] private TMP_Text goldCount;
    [SerializeField] private List<ItemSlot> itemSlots; 

    public void UpdateBackpackItems(List<Item> items)
    {
        if(items == null || items.Count == 0)
        {
            foreach(var itemSlot in itemSlots)
            {
                itemSlot.item = null;
                itemSlot.UpdateSlot();
            }
        }
        else
        {
            int index = 0;
            foreach(var item in items)
            {
                itemSlots[index].item = item;
                index++;
            }

            foreach(var itemSlot in itemSlots)
            {
                itemSlot.UpdateSlot();
            }
        }

        
    }

    public void UpdateGoldCount(int value)
    {
        goldCount.text = value.ToString();
    }
}
