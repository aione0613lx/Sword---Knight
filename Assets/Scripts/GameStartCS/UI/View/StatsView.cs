using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsView : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text growText;

    public void UpdateStatsText(StatsType statsType,float value)
    {
        switch (statsType)
        {
            case StatsType.Level : levelText.text = "Lev:" + value.ToString();
            break;
            case StatsType.HP : hpText.text = "HP:" + value.ToString();
            break;
            case StatsType.ATK : atkText.text = "ATK:" + value.ToString();
            break;
            case StatsType.DEF : defText.text = "DEF" + value.ToString();
            break;
            case StatsType.Speed : speedText.text = "SPE" + value.ToString();
            break;
            case StatsType.Grow : growText.text = "GRO" + value.ToString();
            break;
        }
    }
}
