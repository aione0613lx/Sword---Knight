using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSO",menuName = "Creat/SO/SkillSO")]
public class SkillSO : ScriptableObject
{   
    public string skillName;
    public int curLevel;
    public int maxLevel;
    public int needPoint;
    public bool isUnlock;
    public Sprite icon;
}
