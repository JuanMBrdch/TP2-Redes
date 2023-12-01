using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerModel : NetworkBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _dir;
    public float speed;
    public Bullet bulletPrefab;
    public NetworkVariable<int> score;
    public NetworkVariable<CustomData> customData;
    public NetworkVariable<FixedString128Bytes> nickname;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        score = new NetworkVariable<int>();
        nickname = new NetworkVariable<FixedString128Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        customData = new NetworkVariable<CustomData>(new CustomData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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
        if (NetworkManager.Singleton.IsServer)
        {
            MasterManager.Singleton.RemovePlayerGame(NetworkObjectId);
            GetComponent<NetworkObject>().Despawn();
        }
    }

    public void AddScore(int scoreAdded)
    {
        score.Value += scoreAdded;
    }
}
