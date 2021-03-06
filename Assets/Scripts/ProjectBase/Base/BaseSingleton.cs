using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Single thread classic C# Singleton
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseSingleton<T> where T : new()
{
    private static T _Instance;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new T();
            return _Instance;
        }
    }

}
