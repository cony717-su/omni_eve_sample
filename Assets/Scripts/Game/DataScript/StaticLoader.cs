using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticLoader : MonoBehaviour
{
    private void Start()
    {
        gameObject.AddComponent<DBStaticLoader>();
    }
}