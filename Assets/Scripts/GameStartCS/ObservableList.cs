using System;
using System.Collections.Generic;

public class ObservableList<T>
{
    private List<T> items = new List<T>();

    // 定义事件：参数为操作类型、被操作的项
    public event Action<CollectionChangeType, T> OnCollectionChanged;

    public int Count => items.Count;
    public T this[int index] => items[index];

    public void Add(T item)
    {
        items.Add(item);
        OnCollectionChanged?.Invoke(CollectionChangeType.Add, item);
    }

    public bool Remove(T item)
    {
        bool removed = items.Remove(item);
        if (removed)
            OnCollectionChanged?.Invoke(CollectionChangeType.Remove, item);
        return removed;
    }

    public void Clear()
    {
        items.Clear();
        OnCollectionChanged?.Invoke(CollectionChangeType.Clear, default(T));
    }

    // 其他需要监听的方法自行封装...
}

public enum CollectionChangeType
{
    Add,
    Remove,
    Clear
}