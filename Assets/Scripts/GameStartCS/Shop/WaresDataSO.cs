using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaresSO",menuName = "Creat/SO/WaresSO")]
public class WaresDataSO : ScriptableObject
{
    public string waresName;
    public int waresID;
    public int cost;
    public Sprite waresIcon;

}
