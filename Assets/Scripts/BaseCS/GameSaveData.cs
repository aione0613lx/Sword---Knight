using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class GameSaveData
{
    public int saveSlot;
    public string saveTime;
    public string saveName;
    public PlayerSaveData playerData;
    public PlayerBackpackSavaData playerBackpackSaveData;
    public PlayerSkillTreeSaveData playerSkillTreeSaveData;
    public WorldSaveData worldData;
}

[SerializeField]
public class WorldSaveData
{
    public int seed;
    public int gameModel;
    public float[] localPos;
}

[SerializeField]
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

[SerializeField]
public class PlayerBackpackSavaData
{
    public int gold;
    public List<int> waresID;
    public List<int> wareCount;
}

[SerializeField]
public class PlayerSkillTreeSaveData
{
    public List<string> skillName;
    public List<int> level;
}