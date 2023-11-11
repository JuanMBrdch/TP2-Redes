using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Timer : NetworkBehaviour
{
    private float timer;
    private Text timerText;
    private float timerRefresh;
    private float currentTimerRefresh;

    private void Update()
    {
        timer += Time.deltaTime;
        timerText.text = "Time :" + timer;

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
