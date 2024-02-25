using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ChatManager : NetworkBehaviour
{
    private ulong _localId;
    public ScrollRect scrollRect;
    public GameObject chatViewport;
    public TextMeshProUGUI content;
    public TMP_InputField inputField;
    Dictionary<ulong, string> _nicknames = new Dictionary<ulong, string>();
    Dictionary<string, ulong> _nicknamesInverse = new Dictionary<string, ulong>();
    private Dictionary<string, Action<ulong, string>> _commands = new Dictionary<string, Action<ulong, string>>();
    
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
        GenerateCommands();
        _localId = NetworkManager.Singleton.LocalClientId;
    }

    void GenerateCommands()
    {
        _commands["w/"] = DMCommand;
        _commands["reset/"] = ResetGameServerRpc;
        _commands["crash/"] = WinGameCommandServerRpc;
        _commands["mario/"] = LoseGameCommandServerRpc;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void RegisterUserServerRpc(ulong id, string nickname)
    {
        _nicknames[id] = nickname;
        _nicknamesInverse[nickname] = id;
    }
    
    public void SendChatMessage(string message)
    {
        if(string.IsNullOrEmpty(inputField.text) || string.IsNullOrWhiteSpace(inputField.text)) return;
        inputField.text = "";
        if (message.StartsWith("/"))
        {
            HandleCommandLocally(message);
        }
        else
        {
            MessageServerRpc(_localId, message);
        }
    }

    void HandleCommandLocally(string message)
    {
        string command = message.Substring(1);

        switch (command)
        {
            case "hidechat":
                HideChatCommand();
                break;
            case "showchat":
                ShowChatCommand();
                break;
            case "colorchange":
                ColorChangeCommand();
                break;
            default:
                Debug.LogWarning("Unknown command: " + command);
                break;
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void MessageServerRpc(ulong id, string message)
    {
        string[] split = message.Split(' ');
        var nickname = _nicknames[id];
        if (_commands.ContainsKey(split[0]))
        {
            var join = string.Join(" ", split, 1, split.Length - 1);
            _commands[split[0]](id, join);
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
    
    [ClientRpc]
    public void DMChatClientRpc(ulong userId, string userNickname, string message)
    {
        string commandColor;
        if (userId == _localId)
        {
            commandColor = "<color=orange>";
        }
        else
        {
            commandColor = "<color=yellow>";
        }

        content.text += commandColor + userNickname + ": " + "</color>" + message + "\n";
        if (scrollRect.verticalNormalizedPosition < 0.25f)
            StartCoroutine(WaitToScroll());
    }

    void DMCommand(ulong id, string message)
    {
        var split = message.Split(' ');
        if(!_nicknamesInverse.ContainsKey(split[0])) return;
        
        var p = new ClientRpcParams();
        p.Send.TargetClientIds = new ulong[] { id, _nicknamesInverse[split[0]] };
        var join = string.Join(" ", split, 1, split.Length - 1);
        DMChatClientRpc(id, _nicknames[id], message);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetGameServerRpc(ulong id, string reset)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        NetworkManager.Singleton.SceneManager.LoadScene("Space Invaders", LoadSceneMode.Single);
    }

    
    [ServerRpc]
    void LoseGameCommandServerRpc(ulong id, string text)
    {
        WinCondition.Singleton.AltLoseScreenClientRpc(id);
    }

    [ServerRpc]
    void WinGameCommandServerRpc(ulong id, string text)
    {
        WinCondition.Singleton.AltWinScreenClientRpc(id);
    }
    
    void HideChatCommand()
    {
        chatViewport.SetActive(false);
    }

    void ShowChatCommand()
    {
        chatViewport.SetActive(true);
    }

    void ColorChangeCommand()
    {
        var list = FindObjectsByType<PlayerModel>(FindObjectsInactive.Include,FindObjectsSortMode.None);
        var colorList = new List<Color>() { Color.magenta, Color.gray, Color.yellow, Color.cyan };
        foreach (var item in list)
        {
            var rnd = Random.Range(0, colorList.Count);
            item.GetComponent<SpriteRenderer>().color = colorList[rnd];
            colorList.RemoveAt(rnd);
        }
    }
    
    IEnumerator WaitToScroll()
    {
        yield return new WaitForEndOfFrame();
        scrollRect.verticalNormalizedPosition = 0;
    }
}
