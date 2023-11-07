using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : NetworkBehaviour
{
    public new BoxCollider2D collider { get; private set; }
    public Vector3 direction = Vector3.up;
    public float speed = 20f;
    Rigidbody2D _rb;
    PlayerModel _ownerModel;
    public float timeToDestroy = 5;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        //if (!IsOwner) return;
        var playerModel = other.GetComponent<PlayerModel>();
        if (playerModel == _ownerModel) return;
        if (playerModel != null)
        {
            playerModel.TakeDamage();
        }

        Destroy();
    }

    void Destroy()
    {
        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn(true);
        Destroy(gameObject);
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy();
    }

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    CheckCollision(other);
    //}

    //private void CheckCollision(Collider2D other)
    //{
    //    Bunker bunker = other.gameObject.GetComponent<Bunker>();

    //    if (bunker == null || bunker.CheckCollision(collider, transform.position)) {
    //        Destroy(gameObject);
    //    }
    //}

}