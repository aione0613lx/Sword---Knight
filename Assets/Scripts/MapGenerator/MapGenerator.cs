using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 多瓦片结构所在的层级：遮蔽层、碰撞层
/// </summary>
public enum TileHierarchy
{
    isCover,
    isCollider
}

/// <summary>
/// 多瓦片结构的SO文件
/// </summary>
[CreateAssetMenu(fileName = "MulitiTileSO", menuName = "Creat/SO/MultiTileSO")]
public class MultiTileSO : ScriptableObject
{
    public int width;
    public int height;
    public float probability;
    public TileBase[] multiTiles;
    public TileHierarchy[] tileHierarchies;
    public int[] terrainType;
    public int minDistance;
}

public static class MapGenerator
{
    /// <summary>
    /// 生成二维噪声地图并返回
    /// </summary>
    public static float[,] GeneratePerlinNoise(int width, int height, bool useSeed,
        int seed, float lacunarity)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError("边长小于0！");
            return null;
        }

        // 优化：使用 System.Random 替代 UnityEngine.Random，避免与游戏逻辑随机干扰，并保持确定性
        System.Random random = useSeed ? new System.Random(seed) : new System.Random(Time.time.GetHashCode());
        float randomOffset = (float)(random.NextDouble() * 20000 - 10000); // 范围 -10000 ~ 10000

        float[,] noiseTable = new float[width, height];
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // 第一次遍历：生成原始噪声值并记录最值
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * lacunarity + randomOffset, y * lacunarity + randomOffset);
                noiseTable[x, y] = noise;
                if (noise < minValue) minValue = noise;
                if (noise > maxValue) maxValue = noise;
            }
        }

        // 第二次遍历：归一化到 0~1 范围（优化：合并到第一次遍历中无法实现，因为需要完整的最值）
        float range = maxValue - minValue;
        if (range > 0)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    noiseTable[x, y] = (noiseTable[x, y] - minValue) / range;
                }
            }
        }
        // 若 range == 0（极罕见情况），保持原值（0.5左右）

        return noiseTable;
    }

    /// <summary>
    /// 瓦片地图生成器
    /// </summary>
    public static void TileMapGenerator(float[,] noiseTable, int width, int height)
    {
        // 优化：缓存 MapMgr 实例引用，减少每帧多次属性访问开销
        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return;

        Tilemap background = mgr.backGround;
        Tilemap waterGround = mgr.waterGround;
        TileBase sandTile = mgr.sandTile;
        TileBase grassTile = mgr.grassTile;
        TileBase waterTile = mgr.waterTile;
        float sandValue = mgr.sandValue;
        float grassValue = mgr.grassValue;
        float waterValue = mgr.waterValue;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = noiseTable[x, y];
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (noise < sandValue)
                {
                    background.SetTile(pos, sandTile);
                }
                else if (noise < grassValue)
                {
                    background.SetTile(pos, grassTile);
                }
                else if (noise < waterValue)
                {
                    background.SetTile(pos, sandTile); // 注意：原逻辑此处为沙地，保留
                }
                else
                {
                    waterGround.SetTile(pos, waterTile);
                }
            }
        }
    }

    /// <summary>
    /// 将地图划分成n个区块
    /// </summary>
    public static void DivideChunk(int chunkSize, int width, int height, Dictionary<Vector2Int, Chunk> mapChunks)
    {
        mapChunks.Clear(); // 优化：清空旧数据，防止重复调用时残留

        int xCount = Mathf.CeilToInt((float)width / chunkSize);
        int yCount = Mathf.CeilToInt((float)height / chunkSize);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                Vector2Int startPos = new Vector2Int(x * chunkSize, y * chunkSize);
                Vector2Int endPos = new Vector2Int(
                    Mathf.Min((x + 1) * chunkSize, width),
                    Mathf.Min((y + 1) * chunkSize, height)
                );
                Chunk newChunk = new Chunk(startPos, endPos);
                mapChunks[new Vector2Int(x, y)] = newChunk;
            }
        }
    }

    /// <summary>
    /// 根据世界坐标获取所在区块（优化：修复区块索引计算错误）
    /// </summary>
    private static Chunk GetChunkOfPos(int x, int y)
    {
        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return null;

        int chunkX = x / MapMgr.CHUNK_SIZE;
        int chunkY = y / MapMgr.CHUNK_SIZE;

        // 优化：直接使用 TryGetValue 替代 ContainsKey + 取值
        mgr.mapChunks.TryGetValue(new Vector2Int(chunkX, chunkY), out Chunk chunk);
        return chunk;
    }

    /// <summary>
    /// 获取指定坐标周围 3x3 区块列表（优化：修复区块索引计算错误）
    /// </summary>
    private static List<Chunk> GetChunksOfPos(int x, int y)
    {
        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return null;

        List<Chunk> chunks = new List<Chunk>();
        int chunkX = x / MapMgr.CHUNK_SIZE;
        int chunkY = y / MapMgr.CHUNK_SIZE;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int key = new Vector2Int(chunkX + i, chunkY + j);
                if (mgr.mapChunks.TryGetValue(key, out Chunk chunk))
                {
                    chunks.Add(chunk);
                }
            }
        }
        return chunks;
    }

    /// <summary>
    /// 多瓦块结构放置
    /// </summary>
    public static void MultiTilePlace(float[,] noiseTable, MultiTileSO[] multiTileSOs, float richness, int width, int height)
    {
        if (richness <= 0 || richness > 0.5)
        {
            Debug.LogError("瓦块结构的丰富程度异常！");
            return;
        }

        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return;

        // 优化：标记数组，防止同一位置生成多个结构
        bool[,] occupied = new bool[width, height];

        // 优化：根据概率权重随机选择一种结构，避免每个格子遍历所有类型导致过密
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (occupied[x, y]) continue;

                // 计算总概率
                float totalProb = 0f;
                foreach (var type in multiTileSOs)
                {
                    totalProb += type.probability + richness / 2;
                }

                if (totalProb <= 0f) continue;

                // 随机选择一种结构
                float rand = UnityEngine.Random.Range(0f, totalProb);
                float accum = 0f;
                MultiTileSO chosen = null;
                foreach (var type in multiTileSOs)
                {
                    accum += type.probability + richness / 2;
                    if (rand <= accum)
                    {
                        chosen = type;
                        break;
                    }
                }

                if (chosen == null) continue;

                // 地形合法性判断
                if (!TerrainLegalityJudgment(noiseTable, x, y, chosen, width, height)) continue;

                // 最小距离判断
                if (!MinDistanceJudgment(noiseTable, x, y, chosen)) continue;

                // 创建结构
                CreatMulitiTile(x, y, chosen, occupied);

                // 记录到区块中
                Chunk currentChunk = GetChunkOfPos(x, y);
                currentChunk?.chunkHasMultiTile.Add(new Vector2Int(x, y), chosen);
            }
        }
    }

    /// <summary>
    /// 创建多瓦片结构（优化：修复索引计算错误）
    /// </summary>
    private static void CreatMulitiTile(int x, int y, MultiTileSO multiTile, bool[,] occupied)
    {
        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return;

        Tilemap colliderMap = mgr.colliderMap;
        Tilemap coverMap = mgr.coverMap;

        for (int j = 0; j < multiTile.height; j++)
        {
            for (int i = 0; i < multiTile.width; i++)
            {
                // 优化：正确的一维索引计算
                int index = i + j * multiTile.width;
                if (index >= multiTile.multiTiles.Length)
                {
                    Debug.LogWarning($"多瓦片结构索引越界：{index} >= {multiTile.multiTiles.Length}");
                    continue;
                }

                TileBase tile = multiTile.multiTiles[index];
                if (tile == null) continue;

                Vector3Int pos = new Vector3Int(x + i, y + j, 0);

                // 优化：根据层级放置到对应 Tilemap
                if (multiTile.tileHierarchies[index] == TileHierarchy.isCollider)
                {
                    colliderMap.SetTile(pos, tile);
                }
                else if (multiTile.tileHierarchies[index] == TileHierarchy.isCover)
                {
                    coverMap.SetTile(pos, tile);
                }

                // 标记占用
                int worldX = x + i;
                int worldY = y + j;
                if (worldX < occupied.GetLength(0) && worldY < occupied.GetLength(1))
                {
                    occupied[worldX, worldY] = true;
                }
            }
        }
    }

    /// <summary>
    /// 最小距离判断（优化：修复距离计算逻辑）
    /// </summary>
    private static bool MinDistanceJudgment(float[,] noiseTable, int x, int y, MultiTileSO multiTile)
    {
        List<Chunk> chunks = GetChunksOfPos(x, y);
        if (chunks == null) return true;

        Rect newRect = new Rect(x, y, multiTile.width, multiTile.height);
        float minDist = multiTile.minDistance;

        foreach (var chunk in chunks)
        {
            foreach (var kvp in chunk.chunkHasMultiTile)
            {
                Vector2Int otherPos = kvp.Key;
                MultiTileSO otherSO = kvp.Value;
                Rect otherRect = new Rect(otherPos.x, otherPos.y, otherSO.width, otherSO.height);

                // 优化：使用矩形间最短距离算法，而非粗略的角点距离
                float dist = RectDistance(newRect, otherRect);
                if (dist < minDist)
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 计算两个矩形之间的最短距离（不相交时返回最近边距离，相交返回负数）
    /// </summary>
    private static float RectDistance(Rect a, Rect b)
    {
        float dx = Mathf.Max(a.xMin - b.xMax, b.xMin - a.xMax, 0);
        float dy = Mathf.Max(a.yMin - b.yMax, b.yMin - a.yMax, 0);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// 地形合法性判断（优化：正确检查结构所占全部格子）
    /// </summary>
    private static bool TerrainLegalityJudgment(float[,] noiseTable, int x, int y, MultiTileSO multiTile, int mapWidth, int mapHeight)
    {
        // 边界检查
        if (x < 0 || y < 0 || x + multiTile.width > mapWidth || y + multiTile.height > mapHeight)
            return false;

        MapMgr mgr = MapMgr.Instance;
        if (mgr == null) return false;

        float sandValue = mgr.sandValue;
        float grassValue = mgr.grassValue;
        float waterValue = mgr.waterValue;

        for (int j = 0; j < multiTile.height; j++)
        {
            for (int i = 0; i < multiTile.width; i++)
            {
                float noise = noiseTable[x + i, y + j];
                int terrainType = GetTerrainType(noise, sandValue, grassValue, waterValue);

                // 检查当前格子地形是否在允许列表中
                bool allowed = false;
                foreach (int allowedType in multiTile.terrainType)
                {
                    if (terrainType == allowedType)
                    {
                        allowed = true;
                        break;
                    }
                }
                if (!allowed) return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 返回当前地块的地形类型（优化：传入阈值参数，减少对 MapMgr 的依赖）
    /// </summary>
    private static int GetTerrainType(float v, float sandValue, float grassValue, float waterValue)
    {
        if (v < sandValue) return 0;
        else if (v < grassValue) return 1;
        else if (v < waterValue) return 0;
        else return 2;
    }
}