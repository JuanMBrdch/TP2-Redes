using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerList : NetworkBehaviour
{
    public TextMeshProUGUI ui;
    List<PlayerModel> _playerList = new List<PlayerModel>();

    public void AddPlayer(PlayerModel playerModel)
    {
        _playerList.Add(playerModel);
        playerModel.customData.OnValueChanged += OnCustomDataChanged;
        RefreshUI();
    }
    void OnCustomDataChanged(CustomData prev, CustomData next)
    {
        if (prev.nickname != next.nickname)
            RefreshUI();
    }
    void RefreshUI()
    {
        FixedString128Bytes text = "";
        ui.text = "";
        for (int i = 0; i < _playerList.Count; i++)
        {
            text += _playerList[i].customData.Value.nickname + "\n";
        }

        ui.text = text.ToString();
        UpdateUIClientRpc(text);
    }
    [ClientRpc]
    public void UpdateUIClientRpc(FixedString128Bytes text)
    {
        ui.text = text.ToString();
    }
}
