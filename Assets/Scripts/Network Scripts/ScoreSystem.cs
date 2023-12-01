using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ScoreSystem : NetworkBehaviour
{
    
    private static ScoreSystem _instance;
    public static ScoreSystem Singleton => _instance;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if (!IsServer)
        {
            this.enabled = false;
        }
        else
        {
            
        }
    }
    
    
}
