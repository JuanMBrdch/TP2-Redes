using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PhotonCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom("Gameplay", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        print("Joined Room");
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = false;
        NetworkManager.Singleton.SceneManager.LoadScene("Space Invaders", LoadSceneMode.Single);
    }

    public override void OnLeftRoom()
    {
        print("Left Room");
    }
}
