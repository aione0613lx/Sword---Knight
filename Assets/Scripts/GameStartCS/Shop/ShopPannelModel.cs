using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPannelModel
{
    private TraderSO traderSO;

    public event Action<TraderSO> OnIntroduceTrader;

    public TraderSO TraderSO {
        get {
            return traderSO;
        }

        set {
            traderSO = value;
            if(traderSO != null)
                OnIntroduceTrader?.Invoke(traderSO);
            else
                OnIntroduceTrader?.Invoke(null);
        }
    }
}
