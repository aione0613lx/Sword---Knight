using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO",menuName = "Creat/SO/PlayerSO")]
public class PlayerStatsSO : ScriptableObject 
{
    public int maxHP;
    public int currentHP;

    public float speed;
    public int damage;

    public PlayerStatsSO()
    {
        maxHP = 10;
        currentHP = 10;
        speed = 5;
        damage = 2;
    }
}
