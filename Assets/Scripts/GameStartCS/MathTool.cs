using System.Collections;
using System.Collections.Generic;
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
            case 1 : return "SO/Itesm/KnightBadge";
            default :
            return null;
        }
    }
}
