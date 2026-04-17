using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMgr : SingletonMono<MapMgr>
{
    public const int CHUNK_SIZE = 20;
    public Dictionary<Vector2Int, Chunk> mapChunks = new Dictionary<Vector2Int, Chunk>();
    public MultiTileSO[] multiTiles;

    [Header("基础Tile")]
    public TileBase sandTile;
    public TileBase grassTile;
    public TileBase waterTile;

    public float sandValue;
    public float grassValue;
    public float waterValue;

    public Tilemap backGround;
    public Tilemap waterGround;
    public Tilemap coverMap;
    public Tilemap colliderMap;

    [Header("噪声图数值设置")]
    public int width;
    public int height;
    public bool useSeed;
    public int seed;
    public float lacunarity;
    public float[,] noiseTable;

    public void Init()
    {
        // 优化：先销毁已存在的子对象，避免重复创建
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        // 创建 Tilemap 对象
        backGround = CreateTilemapChild("BackGround", false);
        waterGround = CreateTilemapChild("WaterGround", false);
        coverMap = CreateTilemapChild("CoverMap", false);
        colliderMap = CreateTilemapChild("ColliderMap", true);

        coverMap.GetComponent<TilemapRenderer>().sortingOrder = 5;
    }

    // 优化：封装 Tilemap 创建逻辑
    private Tilemap CreateTilemapChild(string name, bool addCollider)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        Tilemap tilemap = go.AddComponent<Tilemap>();
        go.AddComponent<TilemapRenderer>();
        if (addCollider)
        {
            go.AddComponent<TilemapCollider2D>();
        }
        return tilemap;
    }

    public void CreatMap()
    {
        // 1.生成噪声图
        noiseTable = MapGenerator.GeneratePerlinNoise(width, height, useSeed, seed, lacunarity);
        if (noiseTable == null) return;

        // 2.绘制瓦片地图
        MapGenerator.TileMapGenerator(noiseTable, width, height);

        // 3.划分区块
        MapGenerator.DivideChunk(CHUNK_SIZE, width, height, mapChunks);

        // 4.生成结构
        MapGenerator.MultiTilePlace(noiseTable, multiTiles, 0.2f, width, height);
    }

    public void MapClear()
    {
        backGround?.ClearAllTiles();
        waterGround?.ClearAllTiles();
        coverMap?.ClearAllTiles();
        colliderMap?.ClearAllTiles();
    }

    public int GetTerrainType(int x,int y)
    {
        if(MapMgr.Instance == null || x >= width || y >= height)
        {
            Debug.Log("超出边界或MapMgr不存在");
        }

        float v = noiseTable[x,y];
        if(v < sandValue) return 0;
        else if( v < sandValue) return 1;
        else if( v < waterValue) return 0;
        else return 2;
    }
}