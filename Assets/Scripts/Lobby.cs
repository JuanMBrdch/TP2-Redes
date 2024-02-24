using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Lobby : NetworkBehaviour
{
    public int MinPlayers;
    public NetworkVariable<int> currentPlayer;
    public NetworkObject netObj;
    private void Awake()
    {
        currentPlayer = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(WaitingForSpawn());
        }
        else
        {
            SpawnPlayerConnectionServerRpc();
        }
    }

    
    
    public void StartRoom() 
    {
        if (true)
        {
            StartRoomClientRpc();
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    [ClientRpc]
    public void StartRoomClientRpc() 
    {
        if (NetworkManager.Singleton.IsServer) 
        {
            return;
        }
        PhotonNetwork.ConnectUsingSettings();
    }
    
    
    
    [ServerRpc (RequireOwnership = false)]
    public void SpawnPlayerConnectionServerRpc()
    {
        print("algo");
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        currentPlayer.Value += 1;
        print(currentPlayer.Value + " " + NetworkManager.Singleton.ConnectedClientsIds.Count);
        
        if (currentPlayer.Value >= MinPlayers) 
        {
            StartRoom();
        }
    }

    IEnumerator WaitingForSpawn()
    {
        while (!netObj.IsSpawned)
        {
            yield return null;
        }
        print(NetworkManager.Singleton.IsServer + " " + NetworkManager.Singleton.LocalClientId);
        SpawnPlayerConnectionServerRpc();
    }
}
