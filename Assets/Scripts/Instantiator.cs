using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Instantiator : NetworkBehaviour
{

    public NetworkObject playerPrefab;
        
    void Start()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        RequestSpawnPlayerServerRPC(id);
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnPlayerServerRPC(ulong id)
    {
        var obj = Instantiate<NetworkObject>(playerPrefab);
        obj.SpawnWithOwnership(id);
    }
    
}
