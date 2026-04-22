using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SettingConfigManager : SingletonMono<SettingConfigManager>
{
    public bool isMusic = true;
    public bool isSoundEffect = true;
    public int gameDiffegame = 1;

    public int slot;
    public string saveName;
    public bool isNew;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
