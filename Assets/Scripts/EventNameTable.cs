using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks.Sources;
using UnityEngine;

public static class EventNameTable
{
    //与玩家属性有关的事件
    public const string ONCHANGEHP = "OnChangeHP";
    public const string ONCHANGEMAXHP = "OnChangeMaxHP";
    public const string ONCHANGEDEF = "OnChangeDEF";
    public const string ONCHANGESPEED = "OnChangeSpeed";
    public const string ONCHANGEDAMAGE = "OnChangeDamage";
    public const string ONEXPMAXBOOST = "OnExpMaxBoost";
    public const string ONEXPCURUPDATE = "OnExpCurUpdate";
    public const string ONLEVELBOOST = "OnLevelBoost";

    //与敌人有关的事件
    public const string ONENEMYDIE = "OnEnemyDie";

    //与商人有关的事件
    public const string ONOPENSHOP = "OnOpenShop";
    public const string ONOPENTELLUI = "OnOpenTellUI";
    public const string ONCLOSETELLUI = "OnCloseTellUI";
    public const string ONINTRODUCETRADER = "OnInTroduceTrader";

    //与背包有关的事件
    public const string ONGOLDCHANGE = "OnGoldChange";
    public const string ONITEMSCHANGE = "OnItemsChange";
}
