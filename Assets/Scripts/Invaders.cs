using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Invaders : NetworkBehaviour
{
    [Header("Invaders")]
    public AnimationCurve speed = new AnimationCurve();
    public Invader[] prefabs = new Invader[5];
    public Transform InitialPosition;
    private ulong _id;

    [Header("Grid")]
    public int rows;
    public int columns;
    public float DistanceX;
    public float DistanceY;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;
    public int MaxInvadersAttack = 15;
    public int MinInvadersAttack = 3;
         
    public Dictionary<ulong, Invader> _invadersDic = new Dictionary<ulong, Invader>();

    public List<Invader> enemiesInGrid = new List<Invader>();

    private float _gridCount = 0;
    
    private void CreateInvaderGrid()
    {
        if (_gridCount > 0) return;
        _gridCount++;
        for (int i = 0; i < rows; i++)
        {
            float diffDistanceY = InitialPosition.position.y - DistanceY;
            Vector3 position = new Vector3(InitialPosition.gameObject.transform.position.x + DistanceX, diffDistanceY);

            for (int j = 0; j < columns; j++)
            {
                var obj = Instantiate(prefabs[i]);
                var networkObj = obj.GetComponent<NetworkObject>();
                networkObj.transform.position = position;
                networkObj.Spawn();
                _invadersDic.Add(obj.NetworkObjectId, networkObj.gameObject.GetComponent<Invader>());
                var invaderModel = obj.GetComponent<Invader>();
                enemiesInGrid.Add(invaderModel);
                position = new Vector3(obj.gameObject.transform.position.x + DistanceX, diffDistanceY);
                InitialPosition.position = new Vector3(InitialPosition.position.x , position.y);
            }
        }
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            CreateInvaderGrid();
            InvokeRepeating(nameof(RequestMissileAttack), missileSpawnRate, missileSpawnRate);
        }
    }
    
    
    private void RequestMissileAttack()
    {
        var count = Random.Range(MinInvadersAttack, MaxInvadersAttack);
        while (count > 0)
        {
            count--;
            var randomEnemy = Random.Range(0, enemiesInGrid.Count);
            var selectedEnemy = enemiesInGrid[randomEnemy];
            if (selectedEnemy != null)
            {
                var missile = Instantiate(missilePrefab);
                var networkObj = missile.GetComponent<NetworkObject>();
                networkObj.transform.position = enemiesInGrid[randomEnemy].transform.position;
                networkObj.Spawn();
                networkObj.GetComponent<Projectile>().Shoot(enemiesInGrid[randomEnemy], Vector2.up);
            }
        }
        // foreach(var obj in _invadersDic)
        // {
        //     if (!obj.Value.gameObject.activeInHierarchy) continue;
        //
        //     var isShoot = Random.Range(0, 1) == 1;
        //
        //     if (isShoot) 
        //     {
        //         count--;
        //         var missile = Instantiate(missilePrefab);
        //         var networkObj = missile.GetComponent<NetworkObject>();
        //         networkObj.transform.position = obj.Value.transform.position;
        //         networkObj.Spawn();
        //         if (count <= 0) return;
        //     }
        // }
    }
}
