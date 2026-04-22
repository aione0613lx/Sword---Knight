using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TraderSO",menuName = "Creat/SO/TraderSO")]
public class TraderSO : ScriptableObject
{
    public string trderName;
    public int traderID;
    public int HP;
    public List<WaresDataSO> waress;
}
