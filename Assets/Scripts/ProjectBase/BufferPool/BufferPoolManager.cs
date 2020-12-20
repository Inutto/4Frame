using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferPoolManager: BaseSingleton<BufferPoolManager>
{

    public Dictionary<string, List<GameObject>> poolDic = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// Get an Object from BufferPool by keyname
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public GameObject GetObject(string _name)
    {
        GameObject obj = null;
        var hasObjectInDic = poolDic.ContainsKey(_name) && poolDic[_name].Count > 0;
        if (hasObjectInDic)
        {
            // Return the first element
            obj = poolDic[_name][0]; 
            poolDic[_name].RemoveAt(0);
        } else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(_name));
            obj.name = _name;
        }

        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// Push an object in BufferPool by keyname and object
    /// </summary>
    /// <param name="_name"></param>
    /// <param name="_obj"></param>
    public void PushObject(string _name, GameObject _obj)
    {
        _obj.SetActive(false);
        if (poolDic.ContainsKey(_name))
        {
            poolDic[_name].Add(_obj);
        } else
        {
            poolDic.Add(_name, new List<GameObject>() { _obj });
        }
    }
}
