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

        if (!IsOwner)
        {
            this.enabled = false;
        }
       
    }

    // Update is called once per frame
    void Update()
    {


        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);

        _model.Move(dir.normalized);
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
