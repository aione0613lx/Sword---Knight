using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPannelController : MonoBehaviour
{
    [SerializeField] private ShopPannelView shopPannelView;
    private ShopPannelModel shopPannelModel;

    private void Awake() 
    {
        shopPannelModel = new ShopPannelModel();

        shopPannelModel.OnIntroduceTrader += shopPannelView.UpdateShopName;
        shopPannelModel.OnIntroduceTrader += shopPannelView.SetShopSlots;

        shopPannelView.OnCloseShop += shopPannelView.CloseShop;

        EventCenter.AddListener<TraderSO>(EventNameTable.ONINTRODUCETRADER,SetModelTrader);    
    }

    public void SetModelTrader(TraderSO traderSO)
    {
        shopPannelModel.TraderSO = traderSO;
        shopPannelView.OpenShop();
    }

    private void OnDestroy() 
    {
        shopPannelModel.OnIntroduceTrader -= shopPannelView.UpdateShopName;
        shopPannelModel.OnIntroduceTrader -= shopPannelView.SetShopSlots;

        shopPannelView.OnCloseShop -= shopPannelView.CloseShop;

        EventCenter.RemoveListener<TraderSO>(EventNameTable.ONINTRODUCETRADER,SetModelTrader);
    }
}
