using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Instantiator : NetworkBehaviour
{
    public NetworkObject playerPrefab;

    public void Start()
    {
     
        ulong id = NetworkManager.Singleton.LocalClientId;
        RequestSpawnPlayerServerRpc(id);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong id)
    {
      
            var obj = Instantiate<NetworkObject>(playerPrefab);
            obj.SpawnWithOwnership(id);
        
    }
}