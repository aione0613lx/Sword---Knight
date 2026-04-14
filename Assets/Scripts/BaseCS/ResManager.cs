using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源管理模块（基于 Resources 实现）
/// 核心：缓存池 + 统一加载/卸载 + 解耦资源调用
/// </summary>
public class ResManager : SingletonAutoMono<ResManager>
{
    // 核心缓存池：存储已加载的资源（key=资源路径，value=资源对象）
    private Dictionary<string, Object> _resCache = new Dictionary<string, Object>();

    #region 1. 同步加载资源（泛型通用）
    /// <summary>
    /// 同步加载资源（自动缓存）
    /// </summary>
    /// <typeparam name="T">资源类型：GameObject/Sprite/ScriptableObject/TileBase等</typeparam>
    /// <param name="path">Resources文件夹下的相对路径</param>
    public T Load<T>(string path) where T : Object
    {
        // 1. 先查缓存：存在直接返回，避免重复加载
        if (_resCache.TryGetValue(path, out Object cacheObj))
        {
            return cacheObj as T;
        }

        // 2. 缓存不存在：从 Resources 文件夹加载
        T res = Resources.Load<T>(path);

        // 3. 加载失败判断
        if (res == null)
        {
            Debug.LogError($"资源加载失败：路径 = {path}，类型 = {typeof(T).Name}");
            return null;
        }

        // 4. 加入缓存池
        _resCache.Add(path, res);
        return res;
    }
    #endregion

    #region 2. 卸载资源
    /// <summary>
    /// 卸载单个资源（释放内存 + 清除缓存）
    /// </summary>
    public void Unload(string path)
    {
        if (_resCache.TryGetValue(path, out Object res))
        {
            // 销毁资源
            if (res is GameObject obj)
                Destroy(obj);

            // 移除缓存
            _resCache.Remove(path);

            // 强制释放内存
            Resources.UnloadUnusedAssets();
            Debug.Log($"资源卸载成功：{path}");
        }
    }

    /// <summary>
    /// 清空所有缓存（场景切换/游戏重启时调用）
    /// </summary>
    public void ClearAllCache()
    {
        _resCache.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        Debug.Log("所有资源已清空");
    }
    #endregion

    #region 3. 实例化预设（快捷方法）
    /// <summary>
    /// 加载预设并直接实例化（最常用）
    /// </summary>
    public GameObject CreatePrefab(string path, Vector3 pos, Quaternion rot)
    {
        GameObject prefab = Load<GameObject>(path);
        if (prefab == null) return null;

        return Instantiate(prefab, pos, rot);
    }

    /// <summary>
    /// 实例化预设（默认坐标/旋转）
    /// </summary>
    public GameObject CreatePrefab(string path)
    {
        return CreatePrefab(path, Vector3.zero, Quaternion.identity);
    }
    #endregion
}