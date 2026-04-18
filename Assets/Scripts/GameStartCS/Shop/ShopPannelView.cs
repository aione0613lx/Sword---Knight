using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShopPannelView : MonoBehaviour
{
    [SerializeField] private TMP_Text shopName;
    [SerializeField] private Button closeButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<RectTransform> shopSlots;

    public event Action OnCloseShop;

    private void Awake() 
    {
        closeButton.onClick.AddListener(() => OnCloseShop());    
    }

    public void SetShopSlots(TraderSO traderSO)
    {
        if(traderSO != null)
        {
            int index = 0;
            foreach(var wares in traderSO.waress)
            {   
                ShopSlot shopSlot = shopSlots[index].GetComponent<ShopSlot>();
                if(shopSlot.waresDataSO == null)
                {
                    shopSlot.waresDataSO = wares;
                    shopSlot.Init();
                }
                index++;
            }

            foreach(var shopSlot in shopSlots)
            {
                if(shopSlot.GetComponent<ShopSlot>().waresDataSO == null) shopSlot.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach(var shopSlot in shopSlots)
            {
                shopSlot.GetComponent<ShopSlot>().waresDataSO = null;
                shopSlot.gameObject.SetActive(false);
            }
        }
    }

    public void OpenShop()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void CloseShop()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void UpdateShopName(TraderSO traderSO)
    {
        if(traderSO != null)
            shopName.text = traderSO.trderName + "的商店";
        else
            shopName.text = "的商店";
    }
}
