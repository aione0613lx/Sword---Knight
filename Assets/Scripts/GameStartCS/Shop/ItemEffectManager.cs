using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEffectManager : SingletonMono<ItemEffectManager>
{   
    public enum ItemType
    {
        Beef,
        Badge
    }

    [SerializeField] private PlayerHealth playerHealth;

    private bool beedAffot = false;
    private bool badgeAffot = false;

    protected override void Awake() 
    {   
        base.Awake();
        EventCenter.AddListener<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,GainPlayerHealth);
        EventCenter.AddListener<bool>(EventNameTable.ONUSEBEFF,UseBeef);
        EventCenter.AddListener<bool>(EventNameTable.ONUSEBADGE,UseKnightBadge);
        EventCenter.AddListener<bool>(EventNameTable.ONGAINGOLD,GainGold);     
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,GainPlayerHealth);
        EventCenter.RemoveListener<bool>(EventNameTable.ONUSEBEFF,UseBeef);
        EventCenter.RemoveListener<bool>(EventNameTable.ONUSEBADGE,UseKnightBadge);
        EventCenter.RemoveListener<bool>(EventNameTable.ONGAINGOLD,GainGold); 
    }

    public void GainPlayerHealth(PlayerHealth playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    /// <summary>
    /// 使用 Beef ：立即获得生命值
    /// </summary>
    /// <returns>告诉监听者该效果是否还在生效</returns>
    private bool UseBeef()
    {
        if(beedAffot) return !beedAffot;

        playerHealth.UpdatePlayerHP(5);
        print("UseBeef");
        StartCoroutine(EffectCD(5,ItemType.Beef));

        return beedAffot;
    }

    /// <summary>
    /// 使用 KnightBadge : 大幅度提升攻击力与速度
    /// </summary>
    /// <returns></returns>
    private bool UseKnightBadge()
    {
        if(badgeAffot) return !badgeAffot;

        playerHealth.UpdateDamage(5);
        playerHealth.UpdateSpeed(5);

        StartCoroutine(EffectCD(10,ItemType.Badge));

        return badgeAffot;
    }

    private bool GainGold()
    {
        PlayerBackpack.Instance.Gold += UnityEngine.Random.Range(5,10);
        return true;
    }

    private void UseBadgeEnd()
    {
        playerHealth.UpdateDamage(-5);
        playerHealth.UpdateSpeed(-5);
    }

    private IEnumerator EffectCD(float time,ItemType type)
    {   
        switch(type)
        {
            case ItemType.Beef : beedAffot = true;
            break;
            case ItemType.Badge : badgeAffot = true;
            break;
        }

        yield return new WaitForSeconds(time);

        switch(type)
        {
            case ItemType.Beef : beedAffot = false;
            break;
            case ItemType.Badge : badgeAffot = false;
            UseBadgeEnd();
            break;
        }
    }
}
