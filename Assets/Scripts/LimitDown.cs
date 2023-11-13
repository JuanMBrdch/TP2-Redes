using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LimitDown : NetworkBehaviour
{
    private ulong _id;
    private void Start()
    {
       
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {
            var networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                var invaderSinusoidal = networkObject.GetComponent<InvaderSinusoidal>();
                if (invaderSinusoidal != null)
                {
                    MasterManager.Singleton.ComeBackInvaderServerRpc(_id);
                    Debug.Log("aa");
                }
            }
        }
    }
}
