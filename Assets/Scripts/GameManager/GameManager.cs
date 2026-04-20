using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : SingletonMono<GameManager>
{   
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 locateSpwanPoint;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] public PlayerStatsSO playerSO;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas dieCanvas;
    private GameObject player;

    public Transform Player {
        get {
            return player.transform;
        }
    }

    public bool MainCanvas {
        get {
            return mainCanvas.gameObject.activeSelf;
        }

        set {
            if(mainCanvas != null)
                mainCanvas.gameObject.SetActive(value);
        }
    }

    public bool DieCanvas {
        get {
            return dieCanvas.gameObject.activeSelf;
        }

        set {
            if(dieCanvas != null)
                dieCanvas.gameObject.SetActive(value);
        }
    }

    private void Awake() 
    {
        base.Awake();
        EventCenter.AddListener(EventNameTable.ONREVIVEBUTTONDOWN,RevivePlayer);
    }

    private void Start() 
    {   
        ResAsset();
        StartGame();
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventNameTable.ONREVIVEBUTTONDOWN,RevivePlayer);    
    }

    public void ResAsset()
    {
        playerSO = Instantiate(ResManager.Instance.Load<PlayerStatsSO>("SO/PlayerSO"));
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

        //向需要PlayerHealth脚本的对象发送playerHealth
        EventCenter.EventTrigger<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,player.GetComponent<PlayerHealth>());
        EventCenter.EventTrigger<PlayerStatsSO>(EventNameTable.ONSEEDPLAYERSO,playerSO);
        //3.创建一个虚拟摄像机，并让它跟随Player
        
        if(virtualCamera == null)
        {
            //TODO 创建一个虚拟摄像机
        }
        virtualCamera.Follow = player.transform;

        //4.初始化UI Canvas：打开Canvas、关闭DieCanvas
        if(mainCanvas != null && dieCanvas != null)
        {
            DieCanvas = false;
        }
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

    public void RevivePlayer()
    {
        MainCanvas = true;
        DieCanvas = false;
        Time.timeScale = 1;
        player.GetComponent<PlayerHealth>().Revive();
        Player.position = locateSpwanPoint;
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    public void SwitchScene(int sceneID)
    {   
        //清理所有事件缓存
        EventCenter.Clear();
        //清理所有资源缓存
        ResManager.Instance.ClearAllCache();

        SceneManager.LoadScene(sceneID);
    }

    /// <summary>
    /// 保存游戏
    /// </summary>
    /// <param name="slot">保存在第几个槽位</param>
    /// <param name="saveName">保存的文件名字</param>
    public void SaveGame(int slot,string saveName)
    {
        GameSaveData data = new GameSaveData();
        data.saveName = saveName;
        data.saveSlot = slot;

        data.playerData = new PlayerSaveData()
        {
            level = playerSO.level,
            maxHP = playerSO.maxHP,
            currentHP = playerSO.curExp,
            damage = playerSO.damage,
            defense = playerSO.defense,
            speed = playerSO.speed,
            curExp = playerSO.curExp,
            growExp = playerSO.growExp,
            multiplier = playerSO.multiplier,
            skillPoint = playerSO.skillPoint
        };

        data.playerBackpackSaveData = new PlayerBackpackSavaData()
        {
            gold = PlayerBackpack.Instance.Gold,
            waresID = new List<int>(),
            wareCount = new List<int>()           
        };

        int index = 0;
        foreach (var item in PlayerBackpack.Instance.items)
        {
            data.playerBackpackSaveData.waresID[index] = item.waresDataSO.waresID;
            data.playerBackpackSaveData.wareCount[index] = item.count;
        }

        data.playerSkillTreeSaveData = new PlayerSkillTreeSaveData()
        {
            skillName = new List<string>()
            {
                "HPBoost","Posion","Flame","SpeedUp"
            },
            level = new List<int>()
            {
                SkillEffectManager.Instance.HPBoostLevel,
                SkillEffectManager.Instance.PosionLevel,
                SkillEffectManager.Instance.FlameLevel,
                SkillEffectManager.Instance.SpeedUp
            }
        };

        data.worldData = new WorldSaveData()
        {
            localPos = new float[]
            {
                locateSpwanPoint.x,
                locateSpwanPoint.y,
                locateSpwanPoint.z
            }
        };

        bool succes = SaveSystem.Save(slot,data,true);
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="slot">从第几个槽位加载存档</param>
    public void LoadGame(int slot)
    {
        GameSaveData data = SaveSystem.Load(slot);

        if(data == null) return;

        //恢复玩家的数据
        playerSO.level = data.playerData.level;
        playerSO.maxHP = data.playerData.maxHP;
        playerSO.currentHP = data.playerData.currentHP;
        playerSO.damage = data.playerData.damage;
        playerSO.defense = data.playerData.defense;
        playerSO.speed = data.playerData.speed;
        playerSO.curExp = data.playerData.curExp;
        playerSO.maxHP = data.playerData.maxHP;
        playerSO.skillPoint = data.playerData.skillPoint;

        //再次向所有订阅了PlayerSO的监听者发送更新后的PlayerSO
        EventCenter.EventTrigger<PlayerStatsSO>(EventNameTable.ONSEEDPLAYERSO,playerSO);

        //恢复玩家背包物品
        PlayerBackpack.Instance.LoadItem(data.playerBackpackSaveData);

        //恢复玩家的技能等级
        SkillEffectManager.Instance.LoadSkillLevel(data.playerSkillTreeSaveData);
    }
}
