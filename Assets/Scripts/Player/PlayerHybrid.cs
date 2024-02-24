using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerHybrid : NetworkBehaviour
{
    PlayerHybridModel _model;

    void Awake()
    {
        _model = GetComponent<PlayerHybridModel>();

        
       
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (!IsOwner) return;

        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        _model.Move(dir);

        Debug.Log(dir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestShootServerRpc(transform.forward);
        }
    }

    [ServerRpc]
    public void RequestShootServerRpc(Vector2 dir)
    {
        _model.Shoot();
    }
}
