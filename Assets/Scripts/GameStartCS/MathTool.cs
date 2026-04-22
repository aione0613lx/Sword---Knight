using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public static class MathTool
{
    /// <summary>
    /// 根据防御力计算实际受到的伤害
    /// </summary>
    /// <param name="defense">防御力（0~100，超过按100计）</param>
    /// <param name="value">原始伤害</param>
    /// <param name="steepness">前期增长速度（推荐 3~8，值越大前期越平缓）</param>
    /// <returns>实际伤害</returns>
    public static int CalculateDefense(int defense, int value, float steepness = 5f)
    {
        defense = Mathf.Clamp(defense, 0, 100);
        if (value <= 0) return 0;

        // 使用指数曲线控制前期增长速度
        // 当 defense=100 时，减免率≈0.99
        float normalized = defense / 100f;                         // 0~1
        float reduction = 0.99f * (1f - Mathf.Pow(1f - normalized, steepness));

        float finalDamage = value * (1f - reduction);

        // 最低伤害保护
        if (finalDamage < 1f) finalDamage = 1f;
        return Mathf.RoundToInt(finalDamage);
    }

    /// <summary>
    /// 根据ID查询对应的Item路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string WaresIDQuery(int id)
    {
        switch(id)
        {
            case 0 : return "SO/Items/Beef";
            case 1 : return "SO/Items/KnightBadge";
            case 2 : return "SO/Itesms/Gold";
            default :
            return "";
        }
    }

    /// <summary>
    /// 根据ID查询对应的TraderSO路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string TraderSOIDQuery(int id)
    {
        switch(id)
        {
            case 0 : return "SO/TraderSO/Ante";
            default : 
            return "";
        }
    }

    /// <summary>
    /// 根据ID查询对应的MulitiTileSO路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string MulitTileSOIDQuery(int id)
    {
        switch(id)
        {
            case 0 : return "SO/MulitiTileSO/Tree";
            case 1 : return "SO/MulitiTileSO/TreeStump";
            case 2 : return "SO/MulitiTileSO/WaterRock1";
            case 3 : return "SO/MulitiTileSO/Grass";
            case 4 : return "SO/MulitiTileSO/MoGu";
            default :
            return "";
        }
    }

    public static string EnemyPrefabIDQuery(int id)
    {
        switch(id)
        {
            case 0 : return "Prefab/Enemys/Lancer";
            default :
            return "";
        }
    }

    /// <summary>
    /// 计算商人刷新概率（线性递减）
    /// </summary>
    /// <param name="existingTraderCount">当前世界中已存在的商人数量</param>
    /// <returns>生成概率 (0~1)</returns>
    public static float CalculateTraderSpawnProbability(int existingTraderCount)
    {
        if (existingTraderCount <= 0)
            return 1f;
        else if (existingTraderCount >= 3)
            return 0f;
        else
            return 1f - (existingTraderCount / 3f); // count=1 → 0.666, count=2 → 0.333
    }

    /// <summary>
    /// 计算地图最大敌人容纳量
    /// </summary>
    /// <param name="width">地图宽度（格子数）</param>
    /// <param name="height">地图高度（格子数）</param>
    /// <param name="enemyPerLandTiles">每多少格陆地生成一个敌人（默认 100）</param>
    /// <returns>最大敌人数</returns>
    public static int CalculateMaxEnemyCapacity(int width, int height, int enemyPerLandTiles = 100)
    {
        // 计算陆地面积（总格子数的 60%）
        float landArea = width * height * 0.6f;
        
        // 根据密度计算最大数量（至少为 1）
        int capacity = Mathf.Max(1, Mathf.RoundToInt(landArea / enemyPerLandTiles));
        
        return capacity;
    }
}
