using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferPoolManager: BaseSingleton<BufferPoolManager>
{

    public Dictionary<string, List<GameObject>> poolDic = new Dictionary<string, List<GameObject>>();

    public GameObject GetObject(string _name)
    {
        GameObject obj = null;
        if(poolDic.ContainsKey(_name) && poolDic[_name].Count > 0)
        {
            obj = poolDic[_name][0]; // Return the first element
            poolDic[_name].RemoveAt(0);
        } else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(_name));
            obj.name = _name;
        }

        obj.SetActive(true);
        return obj;
    }

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
