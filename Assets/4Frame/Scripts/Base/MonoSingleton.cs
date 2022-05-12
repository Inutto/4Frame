using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                DebugF.LogWarning(" Multiple instance detected when trying to create singleton behavior at ", this.gameObject);
                _Instance.enabled = false;
            }
        }
    }

    #endregion SINGLETON


   
}
