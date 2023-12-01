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
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        anim.SetFloat("Vel", _rb.velocity.magnitude);
    }
}
