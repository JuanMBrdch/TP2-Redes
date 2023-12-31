using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Invader : NetworkBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1f;
    public int animationFrame { get; private set; }
    public int score = 10;
    public Projectile missilePrefab;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationSprites[0];
    }

    private void Start()
    {
        if (IsServer)
        {
            MasterManager.Singleton.invaderList.Add(this);
        }
        InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
    }
    
    
    
    private void AnimateSprite()
    {
        animationFrame++;
        if (animationFrame >= animationSprites.Length) 
        {
            animationFrame = 0;
        }
        spriteRenderer.sprite = animationSprites[animationFrame];
    }

    public void TakeDamage()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            MasterManager.Singleton.RemoveEnemy(this);
            GetComponent<NetworkObject>().Despawn(true);
            OnDestroy();
        }
    }

    public void Shoot()
    {
        var dir = transform.up;
        var netObj = Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        netObj.Spawn();
        netObj.GetComponent<Projectile>().Shoot(this, dir);
    }
    
    public override void OnDestroy()
    {
        MasterManager.Singleton.invaderList.Remove(this);
    }

}
