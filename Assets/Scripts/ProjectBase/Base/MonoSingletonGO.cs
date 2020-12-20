using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Use this Singleton by directly creating a class that inherit it. 
/// The Singleton Manager will be created as an empty GameObject.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingletonGO<T> : MonoBehaviour where T : MonoBehaviour
{
    #region SINGLETON
    private static T _Instance = null;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
                CreateSigObject();
            return _Instance;
        }
    }
    
    private static void CreateSigObject()
    {
        // Create Singleton Manager GameObject
        GameObject obj = new GameObject();
        obj.name = typeof(T).ToString();
        DontDestroyOnLoad(obj);
        _Instance = obj.AddComponent<T>();
    }
    #endregion SINGLETON
}
