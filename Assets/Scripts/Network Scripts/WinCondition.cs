using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class WinCondition : NetworkBehaviour
{
    public GameObject winLoseScreen;
    public TextMeshProUGUI winLoseText;
    public Action<ulong> OnGameFinished;
    public WinCondition Singleton { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }

        OnGameFinished += GameFinishedListenerServerRpc;
        winLoseScreen.SetActive(false);
    }

    private void Update()
    {
        var invaders = FindObjectsByType<Invader>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        if (invaders == null)
        {
            OnGameFinished(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc]
    public void GameFinishedListenerServerRpc(ulong id)
    {
        ClientRpcParams p = new ClientRpcParams();
        List<ulong> players = new List<ulong>();
        foreach (var ids in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (MasterManager.Singleton.GetPlayer(ids))
            {
                players.Add(ids);
            }
        }

        p.Send.TargetClientIds = players;
        WinScreenClientRpc(id, p);
        players.Clear();
        
        foreach (var ids in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!MasterManager.Singleton.GetPlayer(ids))
            {
                players.Add(ids);
            }
        }

        p.Send.TargetClientIds = players;
        LoseScreenClientRpc(id, p);
        players.Clear();
    }

    [ClientRpc]
    private void LoseScreenClientRpc(ulong id, ClientRpcParams p)
    {
        winLoseScreen.SetActive(true);
        winLoseText.text = "Has perdido, los invasores te destrozaron";
    }
    
    [ClientRpc]
    private void WinScreenClientRpc(ulong id, ClientRpcParams p)
    {
        winLoseScreen.SetActive(true);
        winLoseText.text = "Has Ganado, sobreviste a al ataque de los invasores";
    }
}
