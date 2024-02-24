using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private PlayerModel _ownerModel;
    private Rigidbody2D _rb;
    public float speed = 10;
    public float timeToDestroy = 5;
    IPlayer owner;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var playerModel = other.GetComponent<PlayerModel>();
        var enemyModel = other.GetComponent<Invader>();
        var enemy2Model = other.GetComponent<InvaderSinusoidal>();

        if (playerModel == _ownerModel) return;
        if (enemyModel != null)
        {
            enemyModel.TakeDamage();
            _ownerModel.AddScore(enemyModel.score);
        }
        else if(enemy2Model != null)
        {
            enemy2Model.TakeDamage();
        }
        Destroy();
    } 

    private void Destroy()
    {
        var netObj = GetComponent<NetworkObject>();
        netObj.Despawn();
        Destroy(gameObject);
    }
    public void Shoot(IPlayer ownerModel, Vector2 dir)
    {
        owner = ownerModel;
        _rb.velocity = dir * speed;
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy();
    }

}
