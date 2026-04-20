using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // 存档文件扩展名
    private const string FILE_EXTENSION = ".sav";
    
    // 存档根目录 (持久化路径)
    private static string SaveDirectory => Application.persistentDataPath + "/Saves/";

    /// <summary>
    /// 初始化存档目录（确保目录存在）
    /// </summary>
    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
    }

    /// <summary>
    /// 获取指定槽位的完整文件路径
    /// </summary>
    private static string GetFilePath(int slot)
    {
        return SaveDirectory + "Slot_" + slot + FILE_EXTENSION;
    }

    /// <summary>
    /// 检查指定槽位是否存在存档
    /// </summary>
    public static bool SaveExists(int slot)
    {
        return File.Exists(GetFilePath(slot));
    }

    /// <summary>
    /// 保存游戏数据到指定槽位
    /// </summary>
    /// <param name="slot">存档槽位（1, 2, 3...）</param>
    /// <param name="data">要保存的数据对象</param>
    /// <param name="overwrite">如果存档已存在，是否覆盖</param>
    /// <returns>保存是否成功</returns>
    public static bool Save(int slot, GameSaveData data, bool overwrite = true)
    {
        if (data == null)
        {
            Debug.LogError("存档数据为空，无法保存。");
            return false;
        }

        EnsureDirectoryExists();
        string path = GetFilePath(slot);

        // 如果不允许覆盖且文件已存在
        if (!overwrite && File.Exists(path))
        {
            Debug.LogWarning($"存档槽位 {slot} 已存在，且 overwrite 设置为 false。");
            return false;
        }

        try
        {
            // 更新存档元数据
            data.saveSlot = slot;
            data.saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 序列化为 JSON（格式化以便阅读）
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            Debug.Log($"存档成功保存至：{path}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"保存存档失败：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 加载指定槽位的存档
    /// </summary>
    /// <param name="slot">存档槽位</param>
    /// <returns>反序列化后的存档数据，若失败则返回 null</returns>
    public static GameSaveData Load(int slot)
    {
        string path = GetFilePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"存档槽位 {slot} 不存在。");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"加载存档失败：{e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 删除指定槽位的存档
    /// </summary>
    public static bool Delete(int slot)
    {
        string path = GetFilePath(slot);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"尝试删除不存在的存档槽位 {slot}");
            return false;
        }

        try
        {
            File.Delete(path);
            Debug.Log($"存档槽位 {slot} 已删除。");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"删除存档失败：{e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取所有存档槽位的元信息（用于显示存档列表）
    /// </summary>
    public static SaveSlotInfo[] GetAllSaveSlots()
    {
        EnsureDirectoryExists();
        string[] files = Directory.GetFiles(SaveDirectory, "*" + FILE_EXTENSION);
        SaveSlotInfo[] slots = new SaveSlotInfo[files.Length];

        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            try
            {
                string json = File.ReadAllText(file);
                GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
                slots[i] = new SaveSlotInfo
                {
                    slot = data.saveSlot,
                    saveName = data.saveName,
                    saveTime = data.saveTime,
                    playerLevel = data.playerData?.level ?? 1,
                    // 可扩展其他摘要信息
                };
            }
            catch
            {
                slots[i] = new SaveSlotInfo { slot = -1, saveName = "损坏的存档" };
            }
        }
        return slots;
    }
}

/// <summary>
/// 存档槽位摘要信息（用于 UI 显示）
/// </summary>
[Serializable]
public struct SaveSlotInfo
{
    public int slot;
    public string saveName;
    public string saveTime;
    public int playerLevel;
}