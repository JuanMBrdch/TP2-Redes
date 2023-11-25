using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Lobby : NetworkBehaviour
{
    public int MinPlayers;
    public NetworkVariable<int> currentPlayer;
    private void Awake()
    {
        currentPlayer = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    }

    private void Start()
    {
        SpawnPlayerConnectionServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerConnectionServerRpc()
    {
        currentPlayer.Value += 1;

        if (currentPlayer.Value >= MinPlayers) 
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Space Invaders", LoadSceneMode.Single);
        }
    }
}
