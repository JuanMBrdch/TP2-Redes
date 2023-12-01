using System;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnims : NetworkBehaviour
{
    public Animator anim;
    private PlayerModel _playerModel;
    private Rigidbody _rb;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerModel = GetComponent<PlayerModel>();
    }
    private void Start()
    {
        if (!IsServer)
        {
            enabled = false;
        }
    }

    public void OnDie()
    {
        anim.SetTrigger("OnDie");
        Invoke(nameof(Despawn), 1f);
    }

    void Despawn()
    {
        GetComponent<NetworkObject>().Despawn();
    }
    
    private void Update()
    {
        if (!IsServer) return;
    }
}
