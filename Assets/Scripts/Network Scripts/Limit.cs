using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Limit : MonoBehaviour
{
    public Transform spawn;
    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var player = other.GetComponent<PlayerModel>();
        if (player == null) return;
        player.transform.position = spawn.position;
    }
}
