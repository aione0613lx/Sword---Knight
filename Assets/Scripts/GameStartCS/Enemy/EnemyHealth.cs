using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyStats enemySO;

    public void UpdateHP(int value)
    {   
        int v = value;
        if(enemySO.defense > 0 && v < 0) v = -MathTool.CalculateDefense(enemySO.defense,Mathf.Abs(v));
        enemySO.currentHP += v;

        if(enemySO.currentHP <= 0)
            Destroy(this.gameObject);
    }

    private void OnDestroy() 
    {
        EventCenter.EventTrigger<EnemyStats>(EventNameTable.ONENEMYDIE,enemySO);
    }
}
