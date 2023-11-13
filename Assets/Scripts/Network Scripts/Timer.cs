using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class Timer : NetworkBehaviour
{
    [SerializeField] private Text timerText;
    [SerializeField] private float timerRefresh;
    private float _currentTimerRefresh;
    public float TimerGet { get; private set; }

    public static Timer Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }
    private void Update()
    {
        TimerGet += Time.deltaTime;
        timerText.text = "Time :" + (int) TimerGet;

        if (IsServer)
        {
            _currentTimerRefresh += Time.deltaTime;
            if (_currentTimerRefresh >= timerRefresh)
            {
                RefreshTimerClientRpc(TimerGet);
                _currentTimerRefresh = 0;
            }
        }
    }

    [ClientRpc]
    private void RefreshTimerClientRpc(float t)
    {
        TimerGet = t;
    }
}
