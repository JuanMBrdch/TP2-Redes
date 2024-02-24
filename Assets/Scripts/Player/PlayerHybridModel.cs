using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHybridModel : NetworkBehaviour
{
    private Rigidbody2D _rb;
    private Vector3 _dir;
    public float speed;
    public Bullet bulletPrefab;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        //netObj.GetComponent<Bullet>().Shoot(this, dir);
    }
    
    
}    
