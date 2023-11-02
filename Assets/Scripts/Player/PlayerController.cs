using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private ulong _localID;

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            _localID = NetworkManager.Singleton.LocalClientId;
            var userTag = PlayerPrefs.GetString("Nickname");
            print("Client: " + userTag);
            MasterManager.Singleton.RequestSpawnPlayerServerRpc(_localID, userTag);
        }
    }

    private void Update()
    {
        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        MasterManager.Singleton.RequestMoveServerRpc(_localID, dir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            MasterManager.Singleton.RequestShootServerRpc(_localID);
        }
    }
}
