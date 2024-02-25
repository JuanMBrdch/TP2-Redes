using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WinCondition : NetworkBehaviour
{
    public GameObject winLoseScreen;
    public TextMeshProUGUI winLoseText;
    public static WinCondition Singleton { get; private set; }

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
        winLoseScreen.SetActive(false);
    }


    private void Update()
    {
        if (Timer.Singleton.TimerGet >= 90)
        {
            winLoseScreen.SetActive(true);
            winLoseText.text = "Has perdido, no destruiste los invasores a tiempo";
        }
    }

    private void EndGame()
    {
        var pModels = new List<PlayerHybridModel>();
        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var playerModel = MasterManager.Singleton.GetPlayerModel(id);
            pModels.Add(playerModel);
        }
    }
    
     [ClientRpc]
     public void LoseScreenClientRpc(ulong id, int score,string nickname,ClientRpcParams p)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "Has perdido, los invasores te destrozaron" + "\n" + nickname + ":" + score + Timer.Singleton.TimerGet; //agregar tiempo sobrevivido
     }
     [ClientRpc]
     public void WinScreenClientRpc(ulong id,int score ,string nickname ,ClientRpcParams p)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "Has Ganado, sobreviste a al ataque de los invasores" + "\n" + nickname + ":" + score; //mostar el puntaje del resto de jugadores
     }

     [ClientRpc]
     public void AltWinScreenClientRpc(ulong id)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "En este server, cualquier mencion o comando incluyendo a crash te da victoria automatica, como ahora.";
     }
     
     [ClientRpc]
     public void AltLoseScreenClientRpc(ulong id)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "En este server no permitimos ninguna mencion a mario, toda mencion resulta en derrota, como ahora.";
     }
}
