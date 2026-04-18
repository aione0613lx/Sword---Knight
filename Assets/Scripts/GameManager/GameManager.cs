using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{   
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 locateSpwanPoint;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] public PlayerStatsSO playerSO;
    private GameObject player;

    private void Start() {
        StartGame();
    }
    public void StartGame()
    {
        //1.生成地图：
        if(MapMgr.Instance == null)
        {
            GameObject MapMgr = new GameObject("MapMgr");
            MapMgr.AddComponent<MapMgr>();
        }

        MapMgr.Instance.Init();
        MapMgr.Instance.CreatMap();
        //2.创建角色：
        LocateSpawnPoint();
        player = Instantiate(playerPrefab,locateSpwanPoint,quaternion.identity);
        player.SetActive(true);
        if(player.GetComponent<PlayerController>() == null) player.AddComponent<PlayerController>();
        if(player.GetComponent<PlayerHealth>() == null) player.AddComponent<PlayerHealth>();
        if(playerSO == null)
        {
            //TODO:新建一个PlayerStatsSO文件并为其初始化
        }
        player.GetComponent<PlayerController>().playerSO = playerSO;
        player.GetComponent<PlayerHealth>().playerSO = playerSO;
        //3.创建一个虚拟摄像机，并让它跟随Player
        
        if(virtualCamera == null)
        {
            //TODO 创建一个虚拟摄像机
        }
        virtualCamera.Follow = player.transform;

        //4.管理游戏进程：
    }

    private void LocateSpawnPoint()
    {   
        Vector2Int point = GetWorldPoint();

        while(MapMgr.Instance.GetTerrainType(point.x,point.y) == 2)
        {
            point = GetWorldPoint();
        }

        locateSpwanPoint = new Vector3(point.x,point.y);
    }

    private Vector2Int GetWorldPoint()
    {
        int x = UnityEngine.Random.Range(0,MapMgr.Instance.width);
        int y = UnityEngine.Random.Range(0,MapMgr.Instance.height);
        return new Vector2Int(x,y);
    }
}
