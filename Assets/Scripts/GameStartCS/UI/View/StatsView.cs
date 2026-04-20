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
            case StatsType.Level : levelText.text = "等级:" + value.ToString();
            break;
            case StatsType.HP : hpText.text = "生命:" + value.ToString();
            break;
            case StatsType.ATK : atkText.text = "攻击:" + value.ToString();
            break;
            case StatsType.DEF : defText.text = "防御:" + value.ToString();
            break;
            case StatsType.Speed : speedText.text = "速度:" + value.ToString();
            break;
            case StatsType.Grow : growText.text = "经验:" + value.ToString();
            break;
        }
    }
}
