using UnityEngine;
using Unity.Netcode;
using Photon.Voice.PUN;

public class PlayerHybrid : NetworkBehaviour
{
    PlayerHybridModel _model;

    void Awake()
    {
        _model = GetComponent<PlayerHybridModel>();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            enabled = false;
        }
    }
    void Update()
    {
        if (!IsOwner) return;

        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        _model.Move(dir);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestShootServerRpc(transform.forward);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
        }
    }

    [ServerRpc]
    public void RequestShootServerRpc(Vector2 dir)
    {
        _model.Shoot();
    }
}
