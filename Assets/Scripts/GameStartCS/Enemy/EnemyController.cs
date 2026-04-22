using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyStats enemySO;
    public Vector2 startPos;


    public void UpdateHP(int value)
    {   
        int v = value;
        if(enemySO.defense > 0 && v < 0) v = -MathTool.CalculateDefense(enemySO.defense,Mathf.Abs(v));
        enemySO.currentHP += v;

        if(enemySO.currentHP <= 0)
            Destroy(this.gameObject);
    }

    public void DieFallItem(int id,Vector3 pos)
    {
        string path = MathTool.WaresIDQuery(id);
        GameObject item = ResManager.Instance.CreatePrefab("Prefab/ItemEntity",pos,quaternion.identity);
        item.GetComponent<ItemEntity>().Wares = ResManager.Instance.Load<WaresDataSO>(path);
    }

    private void OnDestroy() 
    {
        EventCenter.EventTrigger<EnemyStats>(EventNameTable.ONENEMYDIE,enemySO);
        DieFallItem(enemySO.weresID,transform.position);
    }
}
