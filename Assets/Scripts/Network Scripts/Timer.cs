using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Timer : NetworkBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private float timerRefresh;
    private float currentTimerRefresh;
    private float timer;

    private static Timer _instance;
    public static Timer Singleton => _instance;

    
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

    private void Update()
    {
        timer += Time.deltaTime;
        timerText.text = "Time :" + (int) timer;

        if (IsServer)
        {
            currentTimerRefresh += Time.deltaTime;
            if (currentTimerRefresh >= timerRefresh)
            {
                RefreshTimerClientRpc(timer);
                currentTimerRefresh = 0;
            }
        }
    }

    [ClientRpc]
    private void RefreshTimerClientRpc(float t)
    {
        timer = t;
    }
}
