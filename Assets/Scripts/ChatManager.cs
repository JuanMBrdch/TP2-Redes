using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    private ulong _localId;
    public ScrollRect scrollRect;
    public TMP_InputField InputField;
    public TextMeshProUGUI content;
    Dictionary<ulong, string> _nicknames = new Dictionary<ulong, string>();
    Dictionary<string, ulong> _nicknamesInverse = new Dictionary<string, ulong>();
    
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
        if(string.IsNullOrEmpty(InputField.text) || string.IsNullOrWhiteSpace(InputField.text)) return;
        InputField.text = "";
        MessageServerRpc(_localId, message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MessageServerRpc(ulong id, string message)
    {
        string[] split = message.Split(' ');
        var nickname = _nicknames[id];
        if(split[0] == "w/" && _nicknamesInverse.ContainsKey(split[1]))
        {
            var p = new ClientRpcParams();
            p.Send.TargetClientIds = new ulong[] { id, _nicknamesInverse[split[1]] };
            var join = string.Join(" ", split, 2, split.Length - 2);
            UpdateChatClientRpc(id, nickname, message);
        }
        else
        {
            UpdateChatClientRpc(id, nickname, message);
        }
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
