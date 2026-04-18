using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    public WaresDataSO waresDataSO;

    public Button buyButton;
    public Image icon;
    public TMP_Text cost;

    private void Awake() 
    {
        buyButton.onClick.AddListener(() => PurchaseItems());
    }

    public void Init() 
    {
        icon.sprite = waresDataSO.waresIcon;
        cost.text = waresDataSO.cost.ToString();
    }

    public void PurchaseItems()
    {
        if(PlayerBackpack.Instance.Gold >= waresDataSO.cost)
        {   
            if(PlayerBackpack.Instance.items.Count > 0)
            {
                foreach (var item in PlayerBackpack.Instance.items)
                {
                    if(item.waresDataSO == waresDataSO)
                    {
                        item.count ++;
                        EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
                        PlayerBackpack.Instance.Gold -= waresDataSO.cost;
                        return;
                    }
                }
            }
            

            PlayerBackpack.Instance.items.Add(new Item(waresDataSO,1));
            EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);

            PlayerBackpack.Instance.Gold -= waresDataSO.cost;
        }
    }
}
