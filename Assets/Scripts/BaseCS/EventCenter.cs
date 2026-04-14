using System;
using System.Collections.Generic;

/// <summary>
/// 事件中心模块：实现去中心化通信
/// 利用 Dictionary + Delegate 实现观察者模式
/// </summary>
public static class EventCenter
{
    // 核心数据结构：事件类型 -> 回调委托
    // 注意：这里用 object 作为参数，用来传递任意数据
    private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

    #region 订阅事件（注册监听）
    /// <summary>
    /// 注册无参数事件
    /// </summary>
    /// <param name="eventName">事件名</param>
    /// <param name="callback">回调函数</param>
    public static void AddListener(string eventName, Action callback)
    {
        // 如果字典里没有这个Key，先初始化一个空的Action
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable.Add(eventName, null);
        }
        // 叠加委托（多播）
        eventTable[eventName] = (eventTable[eventName] as Action) + callback;
    }

    /// <summary>
    /// 注册带一个参数的事件（通用）
    /// </summary>
    public static void AddListener<T>(string eventName, Action<T> callback)
    {
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable.Add(eventName, null);
        }
        eventTable[eventName] = (eventTable[eventName] as Action<T>) + callback;
    }

    /// <summary>
    /// 注册带两个参数的事件
    /// </summary>
    public static void AddListener<T1, T2>(string eventName, Action<T1, T2> callback)
    {
        if (!eventTable.ContainsKey(eventName))
        {
            eventTable.Add(eventName, null);
        }
        eventTable[eventName] = (eventTable[eventName] as Action<T1, T2>) + callback;
    }
    #endregion

    #region 取消订阅事件（注销监听）
    /// <summary>
    /// 移除无参数事件监听
    /// </summary>
    public static void RemoveListener(string eventName, Action callback)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            // 移除委托
            var newDel = (del as Action) - callback;
            if (newDel == null)
            {
                // 如果没有订阅者了，从字典中移除Key，节省内存
                eventTable.Remove(eventName);
            }
            else
            {
                eventTable[eventName] = newDel;
            }
        }
    }

    /// <summary>
    /// 移除带参数事件监听
    /// </summary>
    public static void RemoveListener<T>(string eventName, Action<T> callback)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var newDel = (del as Action<T>) - callback;
            if (newDel == null)
                eventTable.Remove(eventName);
            else
                eventTable[eventName] = newDel;
        }
    }
    #endregion

    #region 发送事件（触发通知）
    /// <summary>
    /// 发送无参数事件
    /// </summary>
    public static void EventTrigger(string eventName)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            (del as Action)?.Invoke();
        }
    }

    /// <summary>
    /// 发送带参数事件
    /// </summary>
    public static void EventTrigger<T>(string eventName, T param)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            (del as Action<T>)?.Invoke(param);
        }
    }

    /// <summary>
    /// 发送带两个参数事件
    /// </summary>
    public static void EventTrigger<T1, T2>(string eventName, T1 param1, T2 param2)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            (del as Action<T1, T2>)?.Invoke(param1, param2);
        }
    }
    #endregion

    /// <summary>
    /// 清空所有事件（场景切换时必用，防止内存泄漏）
    /// </summary>
    public static void Clear()
    {
        eventTable.Clear();
    }
}