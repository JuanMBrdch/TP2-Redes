using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerHybridModel : NetworkBehaviour, IPlayer
{

    private Rigidbody2D _rb;
    public float speed;
    public Bullet bulletPrefab;
    public NetworkVariable<int> score;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

    public void TakeDamage(IPlayer shooter)
    {
        if (shooter is PlayerHybridModel)
        {
            var playerHybridModel = (PlayerHybridModel)shooter;
            playerHybridModel.score.Value++;
        }
        //GetComponent<NetworkObject>().Despawn(true);
        var p = new ClientRpcParams();
        p.Send.TargetClientIds = new ulong[] { OwnerClientId };
    }
    public void AddScore(int scoreAdded)
    {
        score.Value += scoreAdded;
    }
}
