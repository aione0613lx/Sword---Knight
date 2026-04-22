using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : SingletonMono<GameManager>
{   
    [SerializeField] private Vector3 locateSpwanPoint;
    [SerializeField] private Vector3 playerInPos;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] public PlayerStatsSO playerSO;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas dieCanvas;
    private GameObject player;
    private int day = 0;
    private float oneDayTime = 720f;
    private int seed;
    private int gameModel = 1;

    public int Day {
        get {
            return day;
        }

        set {
            day = value;
            EventCenter.EventTrigger<int>(EventNameTable.ONCALENDARCHANGE,day);
        }
    }

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

    protected override void Awake() 
    {
        base.Awake();
        EventCenter.AddListener(EventNameTable.ONREVIVEBUTTONDOWN,RevivePlayer);
        LoadAsset();
    }

    private void Start() 
    {   
        if(SettingConfigManager.Instance.isNew == false) LoadGame(SettingConfigManager.Instance.slot);

        CreatTileMap(!SettingConfigManager.Instance.isNew,seed);

        CreatPlayer(SettingConfigManager.Instance.isNew);

        InitCameraAndCanvas();

        InitGameTime(SettingConfigManager.Instance.isNew);

    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventNameTable.ONREVIVEBUTTONDOWN,RevivePlayer);    
    }

    #region 创建游戏全流程

    /// <summary>
    /// 加载资源
    /// </summary>
    public void LoadAsset()
    {
        //加载PlayerSO文件
        if(playerSO == null)
        {
            playerSO = Instantiate(ResManager.Instance.Load<PlayerStatsSO>("SO/PlayerSO"));
        }
    }

    /// <summary>
    /// 创建地图
    /// </summary>
    /// <param name="useSeed"></param>
    /// <param name="seed"></param>
    public void CreatTileMap(bool useSeed,int seed)
    {
        if(MapMgr.Instance == null)
        {
            GameObject MapMgr = new GameObject("MapMgr");
            MapMgr.AddComponent<MapMgr>();
        }

        MapMgr.Instance.Init();

        if(useSeed)
        {
            MapMgr.Instance.useSeed = true;
            MapMgr.Instance.seed = seed;
            MapMgr.Instance.CreatMap();
        }
        else
        {
            this.seed = MapMgr.Instance.CreatMap();
        }

        
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="isNew">是否是新的存档</param>
    public void CreatPlayer(bool isNew)
    {
        if(isNew == true) LocateSpawnPoint();
        Debug.Log(locateSpwanPoint);

        player = ResManager.Instance.CreatePrefab("Prefab/Player",locateSpwanPoint,quaternion.identity);
        player.SetActive(true);
        
        if(player.GetComponent<PlayerController>() == null) player.AddComponent<PlayerController>();
        if(player.GetComponent<PlayerHealth>() == null) player.AddComponent<PlayerHealth>();

        player.GetComponent<PlayerController>().playerSO = playerSO;
        player.GetComponent<PlayerHealth>().playerSO = playerSO;

        //向需要PlayerHealth脚本的对象发送playerHealth
        EventCenter.EventTrigger<PlayerHealth>(EventNameTable.ONSEEDPLAYERHEALTH,player.GetComponent<PlayerHealth>());
        EventCenter.EventTrigger<PlayerStatsSO>(EventNameTable.ONSEEDPLAYERSO,playerSO);

        if(playerInPos != Vector3.zero) player.transform.position = playerInPos;
    }

    /// <summary>
    /// 初始化摄像机与图层
    /// </summary>
    public void InitCameraAndCanvas()
    {
        //创建一个虚拟摄像机，并让它跟随Player
        if(virtualCamera == null)
        {
            //TODO 创建一个虚拟摄像机
        }
        virtualCamera.Follow = player.transform;

        //初始化UI Canvas：打开Canvas、关闭DieCanvas
        if(mainCanvas != null && dieCanvas != null)
        {
            DieCanvas = false;
        }
    }

    /// <summary>
    /// 初始化游戏时间
    /// </summary>
    /// <param name="isNew"></param>
    public void InitGameTime(bool isNew)
    {
        if(isNew)
        {
            Day = 1;
        }

        SpendOneDay();

        EventCenter.EventTrigger<int>(EventNameTable.ONCALENDARCHANGE,day);
    }

    #endregion

    private void LocateSpawnPoint()
    {   
        Vector2Int point = GetWorldPoint();

        while(MapMgr.Instance.GetTerrainType(point.x,point.y) == 2)
        {
            point = GetWorldPoint();
        }

        locateSpwanPoint = new Vector3(point.x,point.y);
    }

    public Vector2Int GetWorldPoint()
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

    public void SpendOneDay()
    {
        StartCoroutine(OneDayTimeChange(oneDayTime));
    }

    public IEnumerator OneDayTimeChange(float time)
    {
        yield return new WaitForSeconds(time);
        Day++;
        SpendOneDay();
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
        //销毁ConfigManager
        Destroy(SettingConfigManager.Instance.gameObject);

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

        if(PlayerBackpack.Instance.items.Count > 0)
        {
            int index = 0;
            foreach (var item in PlayerBackpack.Instance.items)
            {
                data.playerBackpackSaveData.waresID.Add(item.waresDataSO.waresID);
                data.playerBackpackSaveData.wareCount.Add(item.count);
                index++;
            }
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
            day = this.day,
            seed = this.seed,
            gameModel = this.gameModel,

            playerPos = new float[]
            {
                player.transform.position.x,
                player.transform.position.y,
                player.transform.position.z
            },

            localPos = new float[]
            {
                locateSpwanPoint.x,
                locateSpwanPoint.y,
                locateSpwanPoint.z
            },

            mulitiPos = new List<int>()
        };

        foreach (var chunk in MapMgr.Instance.mapChunks.Values)
        {   
            foreach (var pos in chunk.chunkHasMultiTile)
            {   
                data.worldData.mulitiPos.Add(pos.Value.id);
                data.worldData.mulitiPos.Add(pos.Key.x);
                data.worldData.mulitiPos.Add(pos.Key.y);           
            }
        }

        data.traderSaveData = new TraderSaveData()
        {
            traderID = new List<int>(),
            day = new List<int>(),
            traderPos = new List<float>()
        };

        foreach(var trader in TraderManager.Instance.traders)
        {
            data.traderSaveData.traderID.Add(trader.traderSO.traderID);
            data.traderSaveData.day.Add(trader.Day);
            data.traderSaveData.traderPos.Add(trader.pos.x);
            data.traderSaveData.traderPos.Add(trader.pos.y);
            data.traderSaveData.traderPos.Add(0);
        }

        data.enemySaveData = new EnemySaveData()
        {
            enemyID = new List<int>(),
            enemyPos = new List<int>()
        };

        foreach(var enemy in EnemyManager.Instance.enemys)
        {
            data.enemySaveData.enemyID.Add(enemy.enemySO.enemyID);
            data.enemySaveData.enemyPos.Add((int)enemy.startPos.x);
            data.enemySaveData.enemyPos.Add((int)enemy.startPos.y);
        }

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

        if(data.playerData != null)
        {
            //恢复玩家的数据
            playerSO.level = data.playerData.level;
            playerSO.maxHP = data.playerData.maxHP;
            playerSO.currentHP = data.playerData.currentHP;
            playerSO.damage = data.playerData.damage;
            playerSO.defense = data.playerData.defense;
            playerSO.speed = data.playerData.speed;
            playerSO.curExp = data.playerData.curExp;
            playerSO.growExp = data.playerData.growExp;
            playerSO.skillPoint = data.playerData.skillPoint;
        }
        

        //再次向所有订阅了PlayerSO的监听者发送更新后的PlayerSO
        EventCenter.EventTrigger<PlayerStatsSO>(EventNameTable.ONSEEDPLAYERSO,playerSO);

        //恢复玩家背包物品
        if(data.playerBackpackSaveData != null) 
            PlayerBackpack.Instance.LoadItem(data.playerBackpackSaveData);

        //恢复玩家的技能等级
        if(data.playerSkillTreeSaveData != null)
            SkillEffectManager.Instance.LoadSkillLevel(data.playerSkillTreeSaveData);

        //恢复商人数据
        if(data.traderSaveData != null)
            TraderManager.Instance.LoadTraders(data.traderSaveData);

        //处理世界级的数据
        if(data.worldData != null)
        {
            day = data.worldData.day;
            seed = data.worldData.seed;
            gameModel = data.worldData.gameModel;
            playerInPos = new Vector3(data.worldData.playerPos[0],data.worldData.playerPos[1],data.worldData.localPos[2]);
            locateSpwanPoint = new Vector3(data.worldData.localPos[0],data.worldData.localPos[1],data.worldData.localPos[2]);

            MapMgr.Instance.saveData = data.worldData;
        }

        //处理敌人的数据
        if(data.enemySaveData != null)
            EnemyManager.Instance.LoadEnemys(data.enemySaveData);


    }
}