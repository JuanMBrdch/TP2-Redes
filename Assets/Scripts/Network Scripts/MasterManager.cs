using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class MasterManager : MonoBehaviour
{
    public NetworkObject playerPrefab;
    //public CameraController cameraController;
    public PlayerList playerList;

    private static MasterManager _instance;
    Dictionary<ulong, PlayerModel> _dic = new Dictionary<ulong, PlayerModel>();
    Dictionary<PlayerModel, ulong> _dicInverse = new Dictionary<PlayerModel, ulong>();
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

    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnPlayerServerRpc(ulong id, string nickname)
    {
        print("Server Nickname:" + nickname);

        var obj = Instantiate<NetworkObject>(playerPrefab);
        obj.transform.position = Vector3.up;

        obj.Spawn();

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

        var p = new ClientRpcParams();
        p.Send.TargetClientIds = new ulong[] { id };
        //SetCameraClientRpc(objID, p);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestMoveServerRpc(ulong id, Vector3 dir)
    {
        if (!_dic.ContainsKey(id)) return;
        _dic[id].Move(dir);
        _dic[id].look(dir);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestShootServerRpc(ulong id)
    {
        print("Hola");
        if (!_dic.ContainsKey(id)) return;
        print("Chau");
        _dic[id].Shoot();
    }

    public void RemovePlayer(PlayerModel player)
    {
        var id = _dicInverse[player];
        _dicInverse.Remove(player);
        _dic.Remove(id);
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

    /*[ClientRpc]
    public void SetCameraClientRpc(ulong objID, ClientRpcParams p)
    {
        var players = FindObjectsByType<PlayerModel>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].NetworkObjectId == objID)
            {
                cameraController.SetTarget(players[i].transform);
                break;
            }
        }
    }*/
    public ulong GetID(PlayerModel model)
    {
        return _dicInverse[model];
    }
}
