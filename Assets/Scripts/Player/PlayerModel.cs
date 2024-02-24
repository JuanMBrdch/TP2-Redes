using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerModel : NetworkBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _dir;
    public float speed;
    public Bullet bulletPrefab;
    public NetworkVariable<int> score;
    public NetworkVariable<ulong> id;
    public NetworkVariable<CustomData> customData;
    public NetworkVariable<FixedString128Bytes> nickname;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        score = new NetworkVariable<int>();
        id = new NetworkVariable<ulong>();
        nickname = new NetworkVariable<FixedString128Bytes>("Player", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        customData = new NetworkVariable<CustomData>(new CustomData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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
       // netObj.GetComponent<Bullet>().Shoot(this, dir);
    }
    
    public void TakeDamage()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    public void ChangeColor()
    {
        var obj = GetComponent<SpriteRenderer>();
        var Rnd1 = Random.Range(0, 255);
        var Rnd2 = Random.Range(0, 255);
        var Rnd3 = Random.Range(0, 255);
        Color color = new Color(Rnd1, Rnd2, Rnd3);
        obj.color = color;
    }
    
    public void AddScore(int scoreAdded)
    {
        score.Value += scoreAdded;
    }
}
