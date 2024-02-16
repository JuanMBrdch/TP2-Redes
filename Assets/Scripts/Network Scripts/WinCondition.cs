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
    
    
    private void EndGame()
    {
        var pModels = new List<PlayerModel>();
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
         if (Timer.Singleton.TimerGet <= 0)
         {
             winLoseText.text = "Has perdido, no destruiste los invasores a tiempo" + "\n" + nickname + ":" + score;
         }
         else
         {
             winLoseText.text = "Has perdido, los invasores te destrozaron" + "\n" + nickname + ":" + score;
         }
     }
    
    
     [ClientRpc] 
     public void WinScreenClientRpc(ulong id,int score,string nickname, ClientRpcParams p)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "Has Ganado, sobreviste a al ataque de los invasores" + "\n" + nickname + ":" + score;
     }
}
