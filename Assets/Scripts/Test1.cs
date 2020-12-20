using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke("Push", 2f);
    }

    private void Push()
    {
        BufferPoolManager.Instance.PushObject(gameObject.name, gameObject);

    }
}

