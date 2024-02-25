using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;

public class PlayerList : NetworkBehaviour
{
    public TextMeshProUGUI ui;
    public List<PlayerHybridModel> _playerList = new List<PlayerHybridModel>();

    public void AddPlayer(PlayerHybridModel PlayerHybridModel)
    {
        _playerList.Add(PlayerHybridModel);
        PlayerHybridModel.customData.OnValueChanged += OnCustomDataChanged;
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
