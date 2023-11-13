using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyFASpawner : MonoBehaviour
{
    private ulong _id;
    private float _shootTimer;
    public float shootInterval = 2f;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            _id = NetworkManager.Singleton.LocalClientId;
        }
    }

    private void Update()
    {
        _shootTimer += Time.deltaTime;

        if (_shootTimer >= shootInterval)
        {              
            MasterManager.Singleton.RequestShootEnemyServerRpc(_id);
            _shootTimer = 0;
        }
    }
}
