using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TraderSO",menuName = "Creat/SO/TraderSO")]
public class TraderSO : ScriptableObject
{
    public string trderName;
    public int HP;
    public float duration;
    public List<WaresDataSO> waress;
}
