using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntity : MonoBehaviour
{   
    private Animator anim;
    private WaresDataSO wares;
    private int count;
    [SerializeField] private SpriteRenderer icon;

    public bool allowPickUp;

    public WaresDataSO Wares {
        get {
            return wares;
        }

        set {
            if(value != null)
            {
                wares = value;
                gameObject.name = wares.waresName;
                icon.sprite = wares.waresIcon;
            }
            else
            {
                Debug.LogError("ItemEntity的Wares不能为空!");
                Destroy(gameObject);
            }
        }
    }

    public int Count {
        get {
            return count;
        }

        set {
            if(value > 0) count = value;
            else count = 1;
        }
    }

    private void Awake() 
    {
        count = 1;    
    }

    private void Start() 
    {
        anim = GetComponent<Animator>();    
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && allowPickUp == true)
        {
            anim.Play("Collect");
            gameObject.GetComponent<Collider2D>().enabled = false;
        }    
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.CompareTag("Player") && allowPickUp == false)
        {
            allowPickUp = true;
        }    
    }


    /// <summary>
    /// Collect动画最后一帧调用
    /// </summary>
    public void GainWares()
    {   
        Debug.Log("销毁自身");
        if(PlayerBackpack.Instance.items != null && PlayerBackpack.Instance.items.Count > 0)
        {
            foreach(var item in PlayerBackpack.Instance.items)
            {
                if(item.waresDataSO == wares)
                {
                    item.count += count;
                    EventCenter.EventTrigger(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
                    Destroy(gameObject);
                    return;
                }
            }
        }

        PlayerBackpack.Instance.items.Add(new Item(wares,count));
        EventCenter.EventTrigger(EventNameTable.ONITEMSCHANGE,PlayerBackpack.Instance.items);
        Destroy(gameObject);
    }
}
