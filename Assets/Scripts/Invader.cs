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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = animationSprites[0];
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), animationTime, animationTime);
    }

    private void AnimateSprite()
    {
        animationFrame++;

        // Loop back to the start if the animation frame exceeds the length
        if (animationFrame >= animationSprites.Length) {
            animationFrame = 0;
        }

        spriteRenderer.sprite = animationSprites[animationFrame];
    }
    /* public void TakeDamage()
     {
         if (NetworkManager.Singleton.IsServer)
         {
             MasterManager.Singleton.RemoveEnemy(this);
             GetComponent<NetworkObject>().Despawn(true);
         }
         else
         {
             MasterManager.Singleton.RemoveEnemyServerRpc();
         }
     }*/

    public void TakeDamage()
    {

        if (NetworkManager.Singleton.IsServer)
        {
            MasterManager.Singleton.RemoveEnemy(this);
            GetComponent<NetworkObject>().Despawn(true);
        }
        else
        {
            MasterManager.Singleton.RemoveEnemyServerRpc(NetworkObjectId);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            //GameManager.Instance.OnInvaderKilled(this);
        } else if (other.gameObject.layer == LayerMask.NameToLayer("Boundary")) {
            GameManager.Instance.OnBoundaryReached();
        }
    }

}
