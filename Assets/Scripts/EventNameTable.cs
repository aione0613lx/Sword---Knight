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
    public const string ONSEEDPLAYERHEALTH = "OnSeedPlayerHealth";
    public const string ONSEEDPLAYERSO = "OnSeedPlayerSO";

    //与敌人有关的事件
    public const string ONENEMYDIE = "OnEnemyDie";

    //与商人有关的事件
    public const string ONOPENSHOP = "OnOpenShop";
    public const string ONOPENTELLUI = "OnOpenTellUI";
    public const string ONCLOSETELLUI = "OnCloseTellUI";
    public const string ONINTRODUCETRADER = "OnInTroduceTrader";
    public const string ONTRADERLEAVE = "OnTraderLeave";

    //与背包有关的事件
    public const string ONGOLDCHANGE = "OnGoldChange";
    public const string ONITEMSCHANGE = "OnItemsChange";

    //与使用Item有关的事件
    public const string ONUSEBEFF = "OnUseBeef";
    public const string ONUSEBADGE = "OnUseBadge";
    public const string ONGAINGOLD = "OnGainGold";

    //与DieCanvas有关的事件
    public const string ONREVIVEBUTTONDOWN = "OnReviveButtonDown";

    //与技能有关的事件
    public const string ONTELLSKILLUIUPDATE = "OnTellSkillUIUpdate";

    //与日期有关的事件
    public const string ONCALENDARCHANGE = "OnCalendarChange";

    public const string ONTELLSAVEUIUPDATE = "OnTellSaveUIUpdate";
}
