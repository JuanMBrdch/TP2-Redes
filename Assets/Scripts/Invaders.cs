using UnityEngine;
using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Invaders : NetworkBehaviour
{
    public GameObject mainParent;
    
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
    public Transform InitialPosition;
    
    [Header("Grid")]
    public int rows = 5;
    public int columns = 11;
    public float DistanceX;
    public float DistanceY;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;
    public int MaxInvadersAttack = 15;
    public int MinInvadersAttack = 3;
         
    Dictionary<ulong, Invader> _invadersDic = new Dictionary<ulong, Invader>();

    float gridCount = 0;
    [ServerRpc(RequireOwnership = false)]
    private void RequestCreateInvaderGridServerRpc()
    {
        if (gridCount > 0) return;
        gridCount++;

        for (int i = 0; i < rows; i++)
        {
            float diffDistanceY = InitialPosition.position.y - DistanceY;
            Vector3 position = new Vector3(InitialPosition.gameObject.transform.position.x + DistanceX, diffDistanceY);


            for (int j = 0; j < columns; j++)
            {
              

                var obj = Instantiate(prefabs[i]);
                var NetworkObj = obj.GetComponent<NetworkObject>();
                NetworkObj.transform.position = position;
                NetworkObj.Spawn();
                _invadersDic.Add(obj.NetworkObjectId, NetworkObj.gameObject.GetComponent<Invader>());

                position = new Vector3(obj.gameObject.transform.position.x + DistanceX, diffDistanceY);
                InitialPosition.position = new Vector3(InitialPosition.position.x , position.y);
            }
        }
    }

  

    private void Start()
    {
        RequestCreateInvaderGridServerRpc();

        InvokeRepeating(nameof(RequestMissileAttackServerRpc), missileSpawnRate, missileSpawnRate);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestMissileAttackServerRpc()
    {
       int count = Random.Range(MinInvadersAttack, MaxInvadersAttack);

      

        foreach(var obj in _invadersDic) 
        {
            if (!obj.Value.gameObject.activeInHierarchy) continue;

            var IsShoot = Random.Range(0, 1) == 1;

            if (IsShoot) 
            {
                count--;
                var missil = Instantiate(missilePrefab);
                var NetworkObj = missil.GetComponent<NetworkObject>();
                NetworkObj.transform.position = obj.Value.transform.position;
                NetworkObj.Spawn();

                if (count <= 0) return;

                continue;
            }
        }
     

        return;
    }

    private void Update()
    {
        // // Calculate the percentage of invaders killed
        // int totalCount = rows * columns;
        // int amountAlive = GetAliveCount();
        // int amountKilled = totalCount - amountAlive;
        // float percentKilled = (float)amountKilled / (float)totalCount;
        //
        // // Evaluate the speed of the invaders based on how many have been killed
        // float speed = this.speed.Evaluate(percentKilled);
        // transform.position += direction * (speed * Time.deltaTime);
        //
        // // Transform the viewport to world coordinates so we can check when the
        // // invaders reach the edge of the screen
        // Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        // Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        //
        // // The invaders will advance to the next row after reaching the edge of
        // // the screen
        // foreach (Transform invader in transform)
        // {
        //     // Skip any invaders that have been killed
        //     if (!invader.gameObject.activeInHierarchy) {
        //         continue;
        //     }
        //
        //     // Check the left edge or right edge based on the current direction
        //     if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1f))
        //     {
        //         AdvanceRow();
        //         break;
        //     }
        //     else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1f))
        //     {
        //         AdvanceRow();
        //         break;
        //     }
        // }
    }

    //private void AdvanceRow()
    //{
    //    // Flip the direction the invaders are moving
    //    direction = new Vector3(-direction.x, 0f, 0f);

    //    // Move the entire grid of invaders down a row
    //    Vector3 position = transform.position;
    //    position.y -= 1f;
    //    transform.position = position;
    //}

    //public void ResetInvaders()
    //{
    //    direction = Vector3.right;
    //    transform.position = initialPosition;

    //    foreach (Transform invader in transform) {
    //        invader.gameObject.SetActive(true);
    //    }
    //}

    public int GetAliveCount()
    {
        int count = 0;

        foreach (Transform invader in transform)
        {
            if (invader.gameObject.activeSelf) {
                count++;
            }
        }

        return count;
    }
   
    public void Shoot()
    {
        var dir = transform.up;
        var netObj = Instantiate(missilePrefab, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
        netObj.Spawn();
        netObj.GetComponent<Projectile>().Shoot(this, dir);
    }
}
