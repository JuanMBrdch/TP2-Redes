using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    public TextMeshProUGUI content;
    public TMP_InputField inputFieldMessage;
    public ScrollRect scrollRect;
    Dictionary<ulong, string> _nicknames = new Dictionary<ulong, string>();
    Dictionary<string, ulong> _nicknamesInverse = new Dictionary<string, ulong>();
    ulong _localId;
    
    private static ChatManager _instance;
    public static ChatManager Singleton => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
        _localId = NetworkManager.Singleton.LocalClientId;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterUserServerRpc(ulong id, string nickname)
    {
        _nicknames[id] = nickname;
        _nicknamesInverse[nickname] = id;
    }
    
    public void SendChatMessage(string message)
    {
        MessageServerRpc(_localId, message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MessageServerRpc(ulong id, string message)
    {
        var nickname = _nicknames[id];
        UpdateChatClientRpc(id, nickname, message);
    }
    [ClientRpc]
    public void UpdateChatClientRpc(ulong userId, string userNickname, string message)
    {
        string commandColor;
        if (userId == _localId)
        {
            commandColor = "<color=red>";
        }
        else
        {
            commandColor = "<color=blue>";
        }

        content.text += commandColor + userNickname + ": " + "</color>" + message + "\n";
        if (scrollRect.verticalNormalizedPosition < 0.25f)
            StartCoroutine(WaitToScroll());
    }

    IEnumerator WaitToScroll()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }
}
