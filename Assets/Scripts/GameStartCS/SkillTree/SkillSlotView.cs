using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotView : MonoBehaviour
{
    [SerializeField] private Button skillButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cover;
    

    public event Action OnSkillBoost;

    public void UnlockSkillSlot(bool isUnlock)
    {
        if(isUnlock)
        {
            skillButton.onClick.AddListener(() => OnSkillBoost?.Invoke());

            cover.gameObject.SetActive(false);
        }
        else
        {
            cover.gameObject.SetActive(true);
        }
    }

    public void UpdateLevelText(int curLevel,int maxLevel)
    {
        levelText.text = curLevel.ToString() + "/" + maxLevel.ToString();
    }

    public void SetImageIcon(SkillSO skillSO)
    {
        skillIcon.sprite = skillSO.icon;
    }
}
