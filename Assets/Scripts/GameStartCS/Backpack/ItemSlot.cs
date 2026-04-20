using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour,IPointerClickHandler
{   
    public Item item;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text count;
    private bool allowOnclick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(allowOnclick == false) return;

        switch(eventData.button)
        {
            case PointerEventData.InputButton.Left : UseItem();
            break;
            case PointerEventData.InputButton.Right : DumpItem();
            break;
        }
    }

    /// <summary>
    /// 鼠标左键使用Item处理逻辑
    /// </summary>
    public void UseItem()
    {
        if(CountDest() == false) return;

        //触发对应的事件 isUse:如果该事件效果还没结束则返回true，如果该事件效果结束则返回false
        bool isUse;
        isUse = EventCenter.EventTrigger<bool>(item.waresDataSO.effect);

        if(isUse == true)
        {
            item.count --;
            EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
            CountDest();
        }
    }

    /// <summary>
    /// 鼠标右键丢弃Item处理逻辑
    /// </summary>
    public void DumpItem()
    {
        //先判断Count数量够不够，如果不够直接返回，并且更新仓库UI
        if(CountDest() == false) return;

        Vector2 dumpPos = GameManager.Instance.Player.position;

        //数量足够则在原地创建一个ItemEntity
        GameObject itemEntity = ResManager.Instance.CreatePrefab("Prefab/ItemEntity",dumpPos,quaternion.identity);
        itemEntity.GetComponent<ItemEntity>().Wares = item.waresDataSO;
        itemEntity.GetComponent<ItemEntity>().Count = 1;

        item.count --;
        EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
        CountDest();   
    }

    /// <summary>
    /// 数量测试，如果数量 <=0 则返回True，并且更新仓库UI
    /// </summary>
    private bool CountDest()
    {
        if(item.count <= 0)
        {
            PlayerBackpack.Instance.items.Remove(item);
            EventCenter.EventTrigger<List<Item>>(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
            return false;
        }
        
        return true;
    }

    public void UpdateSlot()
    {
        if(item == null)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            count.text = "";
            allowOnclick = false;
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = item.waresDataSO.waresIcon;
        count.text = item.count.ToString();
        allowOnclick = true;
    }
}
