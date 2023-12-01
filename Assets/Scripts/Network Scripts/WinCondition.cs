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
   
    
     [ClientRpc]
     public void LoseScreenClientRpc(ulong id, ClientRpcParams p)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "Has perdido, los invasores te destrozaron";
     }
    
    
     [ClientRpc] 
     public void WinScreenClientRpc(ulong id, ClientRpcParams p)
     {
         winLoseScreen.SetActive(true);
         winLoseText.text = "Has Ganado, sobreviste a al ataque de los invasores";
     }
}
