using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Let your singleton class inherit this class as a componnet. e.g., public class GameManager : MonoSingleton<GameManager>
/// </summary>
/// <typeparam name="T"></typeparam>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
                DebugF.LogWarning("Multiple singleton component detected on this GameObject; The older one will be deactivated automatically", this.gameObject);
                _Instance.enabled = false;
            }
        }
    }

    #endregion SINGLETON


   
}
