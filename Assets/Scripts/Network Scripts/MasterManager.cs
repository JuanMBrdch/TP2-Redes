using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class MasterManager : NetworkBehaviour
{
    private List<Transform> availableSpawnAreas = new List<Transform>();
    public List<Color> colorList = new List<Color>(){Color.green,Color.blue,Color.red,Color.white, Color.cyan, Color.magenta, Color.yellow};
    public Dictionary<ulong, PlayerHybridModel> _dic = new Dictionary<ulong, PlayerHybridModel>();
    private Dictionary<PlayerHybridModel, ulong> _dicInverse = new Dictionary<PlayerHybridModel, ulong>();
    [SerializeField] private Transform zone1;
    [SerializeField] private Transform zone2;
    [SerializeField] private Transform zone3;
    [SerializeField] private Transform zone4;
    public NetworkObject playerPrefab;
    public NetworkObject enemyPrefab;
    public PlayerList playerList;
    public Invaders invaders;
    public InvaderSinusoidal invaderEspecial;
    public List<Transform> spawnAreas = new List<Transform>();
    public List<Invader> invaderList = new List<Invader>();

    
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
        obj.SpawnWithOwnership(id);
        availableSpawnAreas.RemoveAt(randomIndex);

        var playerModel = obj.GetComponent<PlayerHybridModel>();
        _dic[id] = playerModel;
        _dicInverse[playerModel] = id;
        playerModel.nickname.Value = nickname;

        var customData = new CustomData();
        customData.nickname = nickname;
        customData.score = Random.Range(0, 100);
        customData.isDead = Random.value > 0.5f;
        
        List<ulong> players = new List<ulong>();
        foreach (var _id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (GetPlayer(_id))
            {
                players.Add(_id);
            }
        }

        if (players.Contains(id))
        {
            UpdateColorClientRpc();
        }
        
        playerModel.customData.Value = customData;
        playerList.AddPlayer(playerModel);
    }
    
    //que el servidor decida que player tiene que color
    [ClientRpc]
    public void UpdateColorClientRpc()
    {
        PlayerModel[] playerModels = GameObject.FindObjectsByType<PlayerModel>(UnityEngine.FindObjectsSortMode.None);
        int index = 0;
        foreach (var playerModel in playerModels)
        {
            playerModel.GetComponent<SpriteRenderer>().color = colorList[index];
            index++;
        }
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
        if (!_dic.ContainsKey(id)) return;
        _dic[id].Shoot();
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestShootEnemyServerRpc(ulong id)
    {
        invaders.enemiesInGrid[Random.Range(0, invaders.enemiesInGrid.Count)].Shoot();
    }
    
    public void RemoveEnemy(Invader invader)
    {
        invaderList.Remove(invader);
        if (invaderList.Count == 0)
        {
            ClientRpcParams p = new ClientRpcParams();
            List<ulong> players = new List<ulong>();
            foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (GetPlayer(id))
                {
                    players.Add(id);
                    p.Send.TargetClientIds = players;
                    var playerModel = GetPlayerModel(id);
                    WinCondition.Singleton.WinScreenClientRpc(id, playerModel.score.Value, playerModel.nickname.Value.ToString(),p);
                    players.Clear();
                }
            }
        }
    }
    
    public void RemovePlayerGame(ulong networkObjectId)
    {
        ClientRpcParams p = new ClientRpcParams();
        List<ulong> playersL = new List<ulong>();
        var players = FindObjectsByType<PlayerHybridModel>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].NetworkObjectId == networkObjectId)
            {
                var id = _dicInverse[players[i]];
                playersL.Add(id);
                p.Send.TargetClientIds = playersL;
                var playerModel = GetPlayerModel(id);
                WinCondition.Singleton.LoseScreenClientRpc(id, playerModel.score.Value, playerModel.nickname.Value.ToString(),p);
                _dicInverse.Remove(players[i]);
                _dic.Remove(id);
                break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ComeBackInvaderServerRpc(ulong id)
    {
        invaderEspecial.ComeBack();
    }

    public bool GetPlayer(ulong id)
    {
        return _dic.ContainsKey(id);
    }
    
    public PlayerHybridModel GetPlayerModel(ulong id)
    {
        return _dic[id];
    }
    
    public ulong GetID(PlayerHybridModel model)
    {
        return _dicInverse[model];
    }
}
