using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats",menuName = "Creat/SO/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public int enemyID;
    public string enemyName;
    public int currentHP;
    public int maxHP;
    public float speed;
    public int damage;
    public int defense;
    public int exp;
    public int weresID;

    public EnemyStats()
    {
        currentHP = 10;
        maxHP = 10;
        speed = 3;
        damage = 3;
        defense = 2;
        exp = 3;
    }
}
