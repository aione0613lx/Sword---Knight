using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //动态创建 动态挂载
                GameObject obj = new GameObject();

                //得到T脚本的类名并将其赋给obj对象，这样就可以在编译器明确的看到该脚本挂载的对象
                obj.name = typeof(T).ToString();

                instance = obj.AddComponent<T>();
                //保证在过场景时，这个对象不会被销毁，在整个生命周期中都存在
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
