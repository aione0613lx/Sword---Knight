using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMgr : SingletonMono<MapMgr>
{
    public const int CHUNK_SIZE = 20;
    public Dictionary<Vector2Int, Chunk> mapChunks = new Dictionary<Vector2Int, Chunk>();
    public MultiTileSO[] multiTiles;
    public WorldSaveData saveData;

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
        waterGround = CreateTilemapChild("WaterGround", true);
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

    public int CreatMap()
    {
        // 1.生成噪声图，并获取实际使用的种子
        noiseTable = MapGenerator.GeneratePerlinNoise(width, height, useSeed, seed, lacunarity, out int actualSeed);
        if (noiseTable == null) return 0;

        // 2.绘制瓦片地图
        MapGenerator.TileMapGenerator(noiseTable, width, height);

        // 3.划分区块
        MapGenerator.DivideChunk(CHUNK_SIZE, width, height, mapChunks);

        // 4.生成结构
        if(!useSeed)
        {
            MapGenerator.MultiTilePlace(noiseTable, multiTiles, 0.2f, width, height);
        }
        else
        {   
            LoadMulitiTiles(saveData.mulitiPos);
            MapGenerator.LoadMultiTilePlace(mapChunks);
        }

        return actualSeed;
    }

    public void MapClear()
    {
        backGround?.ClearAllTiles();
        waterGround?.ClearAllTiles();
        coverMap?.ClearAllTiles();
        colliderMap?.ClearAllTiles();
    }

    public int GetTerrainType(int x, int y)
    {
        if (MapMgr.Instance == null || x >= width || y >= height)
        {
            Debug.Log("超出边界或MapMgr不存在");
            return -1;
        }

        float v = noiseTable[x, y];
        if (v < sandValue) return 0;        // 沙地
        else if (v < grassValue) return 1;   // 草地
        else if (v < waterValue) return 0;   // 沙地
        else return 2;                       // 水域
    }

    /// <summary>
    /// 加载多瓦片结果,
    /// </summary>
    /// <param name="poss">传入的是一个int类型的链表，由3n个数据组成，从0开始第一个元素代表这个多瓦片结构的类型，
    /// 后续相邻的两个元素代表这个多瓦片结构的位置，</param>
    internal void LoadMulitiTiles(List<int> poss)
    {
        for(int index = 0; index < poss.Count - 3; index += 3)
        {
            //TODO:先获得对应的多瓦片结构SO
            string path = MathTool.MulitTileSOIDQuery(poss[index]);
            MultiTileSO so = ResManager.Instance.Load<MultiTileSO>(path);

            //TODO：获得瓦片位置，并得到对应的chunk
            Vector2Int pos = new Vector2Int(poss[index + 1],poss[index + 2]);
            Chunk chunk = MapGenerator.GetChunkOfPos(pos.x,pos.y);

            //TODO：部署数据到mapChunks
            chunk.chunkHasMultiTile.Add(pos,so);
        }
    }
}