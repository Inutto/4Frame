using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoSingletonGO<Test>
{

    [SerializeField] private int num;

    public void AddNum()
    {
        Debug.Log("I am a manager and add number");
        num += 5;
    }

}

public class TestManager
{

}

