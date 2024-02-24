using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private PlayerHybridModel _ownerModel;
    private Rigidbody2D _rb;
    public float speed = 10;
    public float timeToDestroy = 5;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var playerModel = other.GetComponent<PlayerHybridModel>();
        var enemyModel = other.GetComponent<Invader>();
        var enemy2Model = other.GetComponent<InvaderSinusoidal>();
        if (playerModel == _ownerModel) return;
        Debug.Log("entramos");

        if (enemyModel != null)
        {
            enemyModel.TakeDamage();
            _ownerModel.AddScore(enemyModel.score);
        }
        else if(enemy2Model != null)
        {
            enemy2Model.TakeDamage();
        }
        //Destroy();
        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn();
    } 

    private void Destroy()
    {
        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn();
        Destroy(gameObject);
    }
    public void Shoot(PlayerHybridModel ownerModel, Vector2 dir)
    {
        _ownerModel = ownerModel;
        _rb.velocity = dir * speed;
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy();
    }

}
