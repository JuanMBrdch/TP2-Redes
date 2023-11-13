using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MasterManager : NetworkBehaviour
{
    public NetworkObject enemyPrefab;
    public NetworkObject playerPrefab;
    public PlayerList playerList;
    Dictionary<ulong, PlayerModel> _dic = new Dictionary<ulong, PlayerModel>();
    Dictionary<PlayerModel, ulong> _dicInverse = new Dictionary<PlayerModel, ulong>();
    public Dictionary<Invader, ulong> _dicInverseEnemy = new Dictionary<Invader, ulong>();
    public Dictionary<ulong, Invader> _dicEnemy = new Dictionary<ulong, Invader>();
    public Invaders invaders;
    public InvaderSinusoidal invaderEspecial;
    [SerializeField] private Transform zone1;
    [SerializeField] private Transform zone2;
    [SerializeField] private Transform zone3;
    [SerializeField] private Transform zone4;
    public List<Transform> spawnAreas = new List<Transform>();
    private List<Transform> availableSpawnAreas = new List<Transform>();
    private float _missileTimer = 0f;


    private static MasterManager _instance;
    public static MasterManager Singleton => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        spawnAreas = new List<Transform> { zone1, zone2, zone3, zone4 };
        availableSpawnAreas.AddRange(spawnAreas);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong id, string nickname)
    {
        var randomIndex = Random.Range(0, availableSpawnAreas.Count);
        var spawnPoint = availableSpawnAreas[randomIndex];
        var obj = Instantiate<NetworkObject>(playerPrefab, spawnPoint);
        obj.Spawn();
        availableSpawnAreas.RemoveAt(randomIndex);

        var playerModel = obj.GetComponent<PlayerModel>();
        _dic[id] = playerModel;
        _dicInverse[playerModel] = id;
        playerModel.nickname.Value = nickname;

        var customData = new CustomData();
        customData.nickname = nickname;
        customData.score = Random.Range(0, 100);
        customData.isDead = Random.value > 0.5f;

        playerModel.customData.Value = customData;

        playerList.AddPlayer(playerModel);

        var objID = obj.NetworkObjectId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(ulong id, Vector3 dir)
    {
        if (!_dic.ContainsKey(id)) return;
        _dic[id].Move(dir);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestShootServerRpc(ulong id)
    {
        print("Hola");
        if (!_dic.ContainsKey(id)) return;
        print("Chau");
        _dic[id].Shoot();
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestShootEnemyServerRpc(ulong id)
    {
        invaders.Shoot();
    }
    public void RemovePlayer(PlayerModel player)
    {
        var id = _dicInverse[player];
        _dicInverse.Remove(player);
        _dic.Remove(id);
    }

    public void RemoveEnemy(Invader invader)
    {
        var id = _dicInverseEnemy[invader];
        _dicInverseEnemy.Remove(invader);
        _dicEnemy.Remove(id);
    } 

    [ServerRpc(RequireOwnership = false)]
    public void RemoveEnemyServerRpc(ulong networkObjectId)
    {
        var invader = FindObjectsByType<Invader>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < invader.Length; i++)
        {
            if (invader[i].NetworkObjectId == networkObjectId)
            {
                var id = _dicInverseEnemy[invader[i]];
                _dicInverseEnemy.Remove(invader[i]);
                _dicEnemy.Remove(id);
                invader[i].GetComponent<NetworkObject>().Despawn(true);
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayerServerRpc(ulong networkObjectId)
    {
        var players = FindObjectsByType<PlayerModel>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].NetworkObjectId == networkObjectId)
            {
                var id = _dicInverse[players[i]];
                _dicInverse.Remove(players[i]);
                _dic.Remove(id);
                players[i].GetComponent<NetworkObject>().Despawn(true);
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ComeBackInvaderServerRpc(ulong id)
    {
            invaderEspecial.ComeBack();
        Debug.Log("MasterManager");
    }

    public ulong GetID(PlayerModel model)
    {
        return _dicInverse[model];
    }
}
