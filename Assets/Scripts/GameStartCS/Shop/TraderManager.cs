using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TraderManager : SingletonMono<TraderManager>
{
    public List<TraderSO> traderSOs = new List<TraderSO>();
    public List<Trader> traders = new List<Trader>();
    private float probability;

    protected override void Awake() 
    {
        base.Awake();
        EventCenter.AddListener<Trader>(EventNameTable.ONTRADERLEAVE,TraderLeaveForWrold);
        EventCenter.AddListener<int>(EventNameTable.ONCALENDARCHANGE,TraderGenerator);
    }

    private void OnDestroy() 
    {
        EventCenter.RemoveListener<Trader>(EventNameTable.ONTRADERLEAVE,TraderLeaveForWrold);
        EventCenter.RemoveListener<int>(EventNameTable.ONCALENDARCHANGE,TraderGenerator);
    }

    /// <summary>
    /// 商人生成器，每当游戏时间度过一日，调用一次该函数，通过判断这一日会不会刷新商人
    /// </summary>
    /// <param name="day"></param>
    public void TraderGenerator(int day)
    {
        if(traderSOs.Count == 0) return;
        
        probability = MathTool.CalculateTraderSpawnProbability(traders.Count);

        if(UnityEngine.Random.Range(0,1) > probability) return;

        Vector2Int creatPos = GetTraderCreatPos();

        foreach (var trader in traders)
        {
            if(trader.pos == creatPos)
            {
                return;
            }
        }
        
        TraderSO newTraderSO = traderSOs[UnityEngine.Random.Range(0,traderSOs.Count)];

        foreach (var trader in traders)
        {
            if(trader.traderSO == newTraderSO)
            {
                return;
            }
        }

        RefreshTradarForWorlld(newTraderSO,creatPos);
    }

    /// <summary>
    /// 得到一个商人能够存在的位置：非水域
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetTraderCreatPos()
    {
        Vector2Int creatPos = GameManager.Instance.GetWorldPoint();
        while(MapMgr.Instance.GetTerrainType(creatPos.x,creatPos.y) == 2)
        {
            creatPos = GameManager.Instance.GetWorldPoint();
        }

        return creatPos;
    }

    /// <summary>
    /// 生成商人
    /// </summary>
    /// <param name="traderSO"></param>
    /// <param name="pos"></param>
    public void RefreshTradarForWorlld(TraderSO traderSO,Vector2 pos)
    {
        GameObject traderObj = Instantiate(ResManager.Instance.Load<GameObject>("Prefab/Trader"),pos,quaternion.identity);
        Trader traderCS = traderObj.AddComponent<Trader>();
        traderCS.pos = pos;
        traderCS.traderSO = traderSO;
        traders.Add(traderCS);
    }

    /// <summary>
    /// 销毁商人
    /// </summary>
    /// <param name="trader"></param>
    public void TraderLeaveForWrold(Trader trader)
    {
        traders.Remove(trader);
        //TODO: 如果在玩家访问商人期间商人被销毁有可能造成BUG
        Destroy(trader.gameObject);
    }

    /// <summary>
    /// 加载商人数据
    /// </summary>
    /// <param name="data"></param>
    internal void LoadTraders(TraderSaveData data)
    {   
        if(data.traderID.Count == 0) return;

        int index = 0;
        foreach(var id in data.traderID)
        {
            string path = MathTool.TraderSOIDQuery(id);
            if(path == null) return;
            TraderSO traderSO = ResManager.Instance.Load<TraderSO>(path);
            
            Vector3 pos = new Vector3();
            pos.x = data.traderPos[index++];
            pos.y = data.traderPos[index++];
            pos.z = data.traderPos[index++];

            RefreshTradarForWorlld(traderSO,pos);
        }

        index = 0;
        foreach(var trader in traders)
        {
            trader.Day = data.day[index];
        }
    }
}
