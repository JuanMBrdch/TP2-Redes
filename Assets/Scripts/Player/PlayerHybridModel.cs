using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;


public class PlayerHybridModel : NetworkBehaviour
{

    private Rigidbody2D _rb;
    public float speed;
    public Bullet bulletPrefab;
    public NetworkVariable<int> score;
    public NetworkVariable<ulong> id;
    public NetworkVariable<CustomData> customData;
    public NetworkVariable<FixedString128Bytes> nickname;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        id = new NetworkVariable<ulong>();

        score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        nickname = new NetworkVariable<FixedString128Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        customData = new NetworkVariable<CustomData>(new CustomData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            id.Value = NetworkManager.Singleton.LocalClientId;
        }
        nickname.OnValueChanged += OnNicknameChange;
    }

    private void OnNicknameChange(FixedString128Bytes prev, FixedString128Bytes next)
    {
        print("OnNicknameChange: " + next);
    }

    public void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }
    public void Shoot()
    {
        var dir = transform.up;
        var netObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        netObj.Spawn();
        netObj.GetComponent<Bullet>().Shoot(this, dir);
    }

    public void TakeDamage()
    {
        GetComponent<NetworkObject>().Despawn(true);

    }
    public void AddScore(int scoreAdded)
    {
        score.Value += scoreAdded;
    }
}
