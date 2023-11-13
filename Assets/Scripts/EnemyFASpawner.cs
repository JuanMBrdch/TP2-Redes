using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyFASpawner : MonoBehaviour
{
    private ulong _id;
    private float _shootTimer;
    private float tpTimer;
    public float tpInterval = 10f;
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
        tpTimer += Time.deltaTime;

        if (_shootTimer >= shootInterval)
        {
            MasterManager.Singleton.RequestShootEnemyServerRpc(_id);
            _shootTimer = 0;
        }
        if (tpTimer >= tpInterval)
        {
            Debug.Log("se tepeo");

            MasterManager.Singleton.ComeBackInvaderServerRpc(_id);
            tpTimer = 0;
        }

    }
}