using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using UnityEngine;
using Unity.Netcode;

public class Instantiator : NetworkBehaviour
{
    public NetworkObject playerPrefab;
    public ChatManager chat;

    public List<string> preMsjs = new List<string>();
    public KeyCode[] msjInputs;

    public void Start()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        var nickname = PlayerPrefs.GetString("Nickname");
        chat.RegisterUserServerRpc(id, nickname);
        MasterManager.Singleton.RequestSpawnPlayerServerRpc(id, nickname);
        PhotonNetwork.Instantiate("VoiceObject", Vector3.zero, Quaternion.identity);
        PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong id)
    { 
        var obj = Instantiate<NetworkObject>(playerPrefab);
        obj.SpawnWithOwnership(id);
    }
}