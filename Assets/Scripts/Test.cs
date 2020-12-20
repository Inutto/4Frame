using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        if (Input.GetMouseButtonDown(0)){
            Debug.Log("Get Object From Pool");
            BufferPoolManager.Instance.GetObject("TestObject");
        } 
    }
}


