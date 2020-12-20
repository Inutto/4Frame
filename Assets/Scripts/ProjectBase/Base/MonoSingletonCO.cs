using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Use this Singleton by draging it to a GameObject as a Component. 
/// The Singleton Manager will be this attaching gameObject.
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingletonCO<T> : MonoBehaviour where T: MonoBehaviour
{
    #region SINGLETON
    public static T _Instance;

    public static T Instance
    {
        get
        {
            return _Instance;
        }
    }

    protected virtual void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
        }
        else
        {
            if (_Instance != this as T)
            {
                Debug.LogWarning("Singleton: Multiple Instance Detected and Destroy Current Latter Instance: " + gameObject.name);
                Destroy(gameObject);
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion SINGLETON
}
