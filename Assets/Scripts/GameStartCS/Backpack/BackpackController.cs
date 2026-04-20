using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackController : MonoBehaviour
{
    [SerializeField] private BackpackView backpackView;

    private void Awake() 
    {
        EventCenter.AddListener<int>(EventNameTable.ONGOLDCHANGE,backpackView.UpdateGoldCount);
        EventCenter.AddListener<List<Item>>(EventNameTable.ONITEMSCHANGE,backpackView.UpdateBackpackItems);    
    }
    
    private void OnDestroy() 
    {
        EventCenter.RemoveListener<int>(EventNameTable.ONGOLDCHANGE,backpackView.UpdateGoldCount);
        EventCenter.RemoveListener<List<Item>>(EventNameTable.ONITEMSCHANGE,backpackView.UpdateBackpackItems); 
    }
}
