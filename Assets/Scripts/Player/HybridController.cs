using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.PUN;
using Unity.Netcode;
using UnityEngine;

public class HybridController : NetworkBehaviour
{
    private PlayerModel _model;

    private void Awake()
    {
        _model = GetComponent<PlayerModel>();
        if (!IsOwner)
        {
            enabled = false;
        }
    }

    private void Start()
    {
        //PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled = false;
    }

    private void Update()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        _model.Move(dir.normalized);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestShootServerRpc();
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
    public void RequestShootServerRpc()
    {
        _model.Shoot();
    }
}
