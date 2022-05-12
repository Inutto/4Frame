using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoSingleton<TestManager>
{
    // Start is called before the first frame update
    void Start()
    {
        DebugF.Log("Test Message", gameObject);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
