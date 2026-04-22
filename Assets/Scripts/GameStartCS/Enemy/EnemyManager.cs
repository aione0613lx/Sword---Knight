using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{   
    [SerializeField] private List<EnemyStats> enemySOs;
    public List<EnemyController> enemys = new List<EnemyController>();

    protected override void Awake() 
    {
        base.Awake();
        EventCenter.AddListener<int>(EventNameTable.ONCALENDARCHANGE,GeneratorEnemy);
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener<int>(EventNameTable.ONCALENDARCHANGE,GeneratorEnemy);
    }

    /// <summary>
    /// 生成敌人：每当游戏度过一日则调用一次
    /// </summary>
    /// <param name="day"></param>
    public void GeneratorEnemy(int day)
    {   
        if(enemySOs.Count == 0) return;

        int count = UnityEngine.Random.Range(1,10);

        if(enemys.Count + count > MathTool.CalculateMaxEnemyCapacity(MapMgr.Instance.width,MapMgr.Instance.height)) return;
        
        for(int i = 0; i < count; i ++)
        {
            EnemyStats so = enemySOs[UnityEngine.Random.Range(0,enemySOs.Count)];
            Vector2 pos = GetEnemyStartPos();
            string path = MathTool.EnemyPrefabIDQuery(so.enemyID);
            GameObject enenmy = ResManager.Instance.CreatePrefab(path,pos,quaternion.identity);
            EnemyController enemyController = enenmy.GetComponent<EnemyController>();
            enemyController.startPos = pos;
            enemys.Add(enemyController);
        }
    }

    private Vector2 GetEnemyStartPos()
    {   
        Vector2Int pos = GetRanomPos();
        while(MapMgr.Instance.GetTerrainType(pos.x,pos.y) == 2)
        {
            pos = GetRanomPos();
        }

        return pos;
    }

    private Vector2Int GetRanomPos()
    {
        int x = UnityEngine.Random.Range(0,MapMgr.Instance.width);
        int y = UnityEngine.Random.Range(0,MapMgr.Instance.height);

        return new Vector2Int(x,y);
    }

    /// <summary>
    /// 加载敌人
    /// </summary>
    /// <param name="data"></param>
    internal void LoadEnemys(EnemySaveData data)
    {   
        int index = 0;
        foreach(var id in data.enemyID)
        {
            string path = MathTool.EnemyPrefabIDQuery(id);
            Vector2 pos = new Vector2(data.enemyPos[index++],data.enemyPos[index++]);
            GameObject enemy = ResManager.Instance.CreatePrefab(path,pos,quaternion.identity);
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemys.Add(enemyController);
        }
    }
}
