using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TellPlayerOpenShop : MonoBehaviour
{
    public RectTransform tellPlayerOpenShop;

    private void Awake() 
    {
        CloseThisUI();
        EventCenter.AddListener(EventNameTable.ONOPENTELLUI,OpenThisUI);
        EventCenter.AddListener(EventNameTable.ONCLOSETELLUI,CloseThisUI);    
    }

    public void OpenThisUI()
    {
        Debug.Log("打开");
        tellPlayerOpenShop.gameObject.SetActive(true);
    }

    public void CloseThisUI()
    {
        Debug.Log("关闭");
        tellPlayerOpenShop.gameObject.SetActive(false);
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener(EventNameTable.ONOPENTELLUI,OpenThisUI);
        EventCenter.RemoveListener(EventNameTable.ONCLOSETELLUI,CloseThisUI);  
    }
}
