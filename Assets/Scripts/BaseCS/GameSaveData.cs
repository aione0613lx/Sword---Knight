using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int saveSlot;
    public string saveTime;
    public string saveName;
    public PlayerSaveData playerData;
    public PlayerBackpackSavaData playerBackpackSaveData;
    public PlayerSkillTreeSaveData playerSkillTreeSaveData;
    public TraderSaveData traderSaveData;
    public EnemySaveData enemySaveData;
    public WorldSaveData worldData;
    
}

[Serializable]
public class WorldSaveData
{
    public int seed;
    public int gameModel;
    public int day;
    public float[] playerPos;
    public float[] localPos;
    public List<int> mulitiPos;
}

[Serializable]
public class PlayerSaveData
{
    public int level;
    public int curExp;
    public int growExp;
    public float multiplier;
    public int maxHP;
    public int currentHP;

    public float speed;
    public int damage;

    public int defense;

    public int skillPoint;
}

[Serializable]
public class PlayerBackpackSavaData
{
    public int gold;
    public List<int> waresID;
    public List<int> wareCount;
}

[Serializable]
public class PlayerSkillTreeSaveData
{
    public List<string> skillName;
    public List<int> level;
}

[Serializable]
public class TraderSaveData
{
    public List<int> traderID;
    public List<int> day;
    public List<float> traderPos;  
}

[Serializable]
public class EnemySaveData
{
    public List<int> enemyID;
    public List<int> enemyPos;
}