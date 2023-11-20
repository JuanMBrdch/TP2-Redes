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
    public Dictionary<ulong, Invader> _dicEnemy = new Dictionary<ulong, Invader>();
    public Dictionary<Invader, ulong> _dicInverseEnemy = new Dictionary<Invader, ulong>();
    public Invaders invaders;
    public InvaderSinusoidal invaderEspecial;
    [SerializeField] private Transform zone1;
    [SerializeField] private Transform zone2;
    [SerializeField] private Transform zone3;
    [SerializeField] private Transform zone4;
    public List<Transform> spawnAreas = new List<Transform>();
    private List<Transform> availableSpawnAreas = new List<Transform>();
    public List<Invader> invaderList = new List<Invader>();
    private List<Color> colorList = new List<Color>(){Color.green,Color.blue,Color.red,Color.white};
    
    
    
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
        obj.GetComponent<SpriteRenderer>().color = colorList[^1];
        colorList.RemoveAt(colorList.Count - 1);
        obj.Spawn();
        UpdateColorClientRpc();
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
    }

    [ClientRpc]
    public void UpdateColorClientRpc()
    {
        
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
        invaders.Shoot();
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
                    WinCondition.Singleton.WinScreenClientRpc(id, p);
                    players.Clear();
                }
            }
        }
    }
    
    public void RemovePlayerGame(ulong networkObjectId)
    {
        ClientRpcParams p = new ClientRpcParams();
        List<ulong> playersL = new List<ulong>();
        var players = FindObjectsByType<PlayerModel>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].NetworkObjectId == networkObjectId)
            {
                var id = _dicInverse[players[i]];
                playersL.Add(id);
                p.Send.TargetClientIds = playersL;
                _dicInverse.Remove(players[i]);
                _dic.Remove(id);
                WinCondition.Singleton.LoseScreenClientRpc(id, p);
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
    
    public ulong GetID(PlayerModel model)
    {
        return _dicInverse[model];
    }
}
