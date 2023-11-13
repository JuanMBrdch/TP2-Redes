using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class LimitDown : NetworkBehaviour
{
    private ulong _id;
       public bool tepearse = true;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (NetworkManager.Singleton.IsServer)
        {
            var networkObject = other.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                var invaderSinusoidal = networkObject.GetComponent<InvaderSinusoidal>();
                if (invaderSinusoidal != null)
                {
                    tepearse = false;
                    _id = networkObject.NetworkObjectId;
                    Debug.Log(_id);
                }
            }
        }
    }
}