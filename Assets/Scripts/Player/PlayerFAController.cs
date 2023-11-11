using Unity.Netcode;
using UnityEngine;

public class PlayerFAController : MonoBehaviour
{
    private ulong _id;

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
            MasterManager.Singleton.RequestSpawnPlayerServerRpc(_id, nickname);
        }
    }

    private void Update()
    {
        var dir = new Vector3(Input.GetAxisRaw("Horizontal"),0,0);
        MasterManager.Singleton.RequestMoveServerRpc(_id, dir);
    }
}
