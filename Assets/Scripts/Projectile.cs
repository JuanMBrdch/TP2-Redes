using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public class Projectile : NetworkBehaviour
{
    public float speed = 10;
    public float timeToDestroy = 5;
    Rigidbody2D _rb;
    Invaders _ownerModel;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }
    //private void Update()
    //{
    //   if (!NetworkManager.Singleton.IsServer) return;
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var playerModel = other.GetComponent<PlayerModel>();
        if (playerModel == _ownerModel) return;

        var netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            if (playerModel != null)
            {
                playerModel.TakeDamage();
            }
           // Destroy();
        }
    }

    private void Destroy()
    {
        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn(true);
        Destroy(gameObject);
    }
    public void Shoot(Invaders ownerModel, Vector2 dir)
    {
        _ownerModel = ownerModel;
        _rb.velocity = -dir * speed;
        StartCoroutine(WaitToDestroy());
    }
    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy();
    }

}