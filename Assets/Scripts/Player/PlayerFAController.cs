using System.Collections.Generic;
using Photon.Pun;
using Unity.Netcode;
using UnityEngine;

public class PlayerFAController : MonoBehaviour
{
    private ulong _id;
    public ChatManager chat;

    public List<string> preMsjs = new List<string>();
    public KeyCode[] msjInputs;
    
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.enabled = false;
        }
        else
        {
            _id = NetworkManager.Singleton.LocalClientId;
            var nickname = PlayerPrefs.GetString("Nickname");
            chat.RegisterUserServerRpc(_id, nickname);
            MasterManager.Singleton.RequestSpawnPlayerServerRpc(_id, nickname);
            PhotonNetwork.Instantiate("VoiceObject", Vector3.zero, Quaternion.identity);
        }
        
    }

    private void Update()
    {
        var dir = new Vector3(Input.GetAxisRaw("Horizontal"),0,0);
        MasterManager.Singleton.RequestMoveServerRpc(_id, dir);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MasterManager.Singleton.RequestShootServerRpc(_id);
        }

        for (int i = 0; i < msjInputs.Length; i++)
        {
            if (Input.GetKeyDown(msjInputs[i]))
            {
                ChatManager.Singleton.SendChatMessage(preMsjs[i]);
            }
        }
    }
   
}
