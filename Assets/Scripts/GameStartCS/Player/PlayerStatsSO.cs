using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO",menuName = "Creat/SO/PlayerSO")]
public class PlayerStatsSO : ScriptableObject 
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

    public PlayerStatsSO()
    {
        level = 1;
        curExp = 0;
        growExp = 10;
        multiplier = 1.3f;
        maxHP = 10;
        currentHP = 10;
        speed = 5;
        damage = 2;
        defense = 2;
        skillPoint = 1;
    }
}
