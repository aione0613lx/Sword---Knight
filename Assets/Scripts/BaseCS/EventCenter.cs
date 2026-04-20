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

    /// <summary>
    /// 添加一个无参有返回值的监听者
    /// </summary>
    public static void AddListener<TResult>(string eventName, Func<TResult> callback)
    {
        if (!eventTable.ContainsKey(eventName))
            eventTable[eventName] = callback;
        else
            eventTable[eventName] = Delegate.Combine(eventTable[eventName], callback);
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

    /// <summary>
    /// 移除带两个参数事件监听
    /// </summary>
    public static void RemoveListener<T1,T2>(string eventName, Action<T1,T2> callback)
    {
        if (eventTable.TryGetValue(eventName, out var del))
        {
            var newDel = (del as Action<T1,T2>) - callback;
            if (newDel == null)
                eventTable.Remove(eventName);
            else
                eventTable[eventName] = newDel;
        }
    }

    /// <summary>
    /// 移除一个无参有返回值的监听者
    /// </summary>
    public static void RemoveListener<TResult>(string eventName, Func<TResult> callback)
    {
        if (eventTable.TryGetValue(eventName, out Delegate del))
        {
            Delegate newDel = Delegate.Remove(del, callback);
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

    /// <summary>
    /// 触发无参有返回值的事件，返回最后一个监听者的结果
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    /// <param name="eventName">事件名</param>
    /// <returns>最后一个监听者的返回值，若无监听者则返回 default(TResult)</returns>
    public static TResult EventTrigger<TResult>(string eventName)
    {
        if (eventTable.TryGetValue(eventName, out Delegate del))
        {
            // 多个委托时，依次调用，但只返回最后一个结果（符合委托链语义）
            if (del is Func<TResult> func)
                return func.Invoke();

            // 若委托链中包含多个 Func<TResult>，需要逐个调用并返回最后一个
            Delegate[] invocationList = del.GetInvocationList();
            TResult result = default(TResult);
            foreach (Delegate d in invocationList)
            {
                if (d is Func<TResult> f)
                    result = f.Invoke();
            }
            return result;
        }
        return default(TResult);
    }

    /// <summary>
    /// 触发无参有返回值的事件，并通过 out 参数指示是否有监听者被调用
    /// </summary>
    /// <typeparam name="TResult">返回值类型</typeparam>
    /// <param name="eventName">事件名</param>
    /// <param name="hasListener">输出：是否有至少一个监听者</param>
    /// <returns>最后一个监听者的返回值，若无监听者则返回 default(TResult)</returns>
    public static TResult EventTrigger<TResult>(string eventName, out bool hasListener)
    {
        if (eventTable.TryGetValue(eventName, out Delegate del))
        {
            hasListener = true;
            Delegate[] invocationList = del.GetInvocationList();
            TResult result = default(TResult);
            foreach (Delegate d in invocationList)
            {
                if (d is Func<TResult> f)
                    result = f.Invoke();
            }
            return result;
        }
        hasListener = false;
        return default(TResult);
    }
    #endregion

    /// <summary>
    /// 清空指定事件的所有监听者
    /// </summary>
    public static void ClearEvent(string eventName)
    {
        if (eventTable.ContainsKey(eventName))
            eventTable.Remove(eventName);
    }

    /// <summary>
    /// 清空所有事件（场景切换时必用，防止内存泄漏）
    /// </summary>
    public static void Clear()
    {
        eventTable.Clear();
    }


}