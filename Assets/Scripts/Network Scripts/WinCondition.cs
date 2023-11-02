using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class WinCondition : NetworkBehaviour
{
    public GameObject winLoseScreen;
    public TextMeshProUGUI winLoseText;
    bool _isFinish;
    private void OnTriggerEnter(Collider other)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (_isFinish) return;
        var player = other.GetComponent<PlayerModel>();
        if (player == null) return;
        _isFinish = true;
        ulong winnerID = MasterManager.Singleton.GetID(player);
        WinLoseScreenClientRpc(winnerID, player.customData.Value.nickname);
    }

    [ClientRpc]
    void WinLoseScreenClientRpc(ulong winnerID, FixedString128Bytes winnerNickname)
    {
        winLoseScreen.SetActive(true);
        if (winnerID == NetworkManager.Singleton.LocalClientId)
        {
            winLoseText.text = "GANASTE";
        }
        else
        {
            winLoseText.text = "Te gano: " + winnerNickname+", Loser jaja";
        }
    }
}
