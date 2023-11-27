using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class Invaders : NetworkBehaviour
{
    
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve speed = new AnimationCurve();
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
    
    float gridCount = 0;
    
    private void CreateInvaderGrid()
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
            InvokeRepeating(nameof(RequestMissileAttackServerRpc), missileSpawnRate, missileSpawnRate);
        }
        
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
            }
        }
    }
}
