using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;

public class VoiceUI : MonoBehaviourPun
{
    private MicUI _micUI;
    public Speaker speaker;
    void Start()
    {
        if (photonView.IsMine)
        {
            _micUI = FindObjectOfType<MicUI>();
        }
        else
        {
            FindObjectOfType<VoiceChatUi>().AddSpeaker(speaker, photonView.Owner);
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            var v = PunVoiceClient.Instance.PrimaryRecorder.TransmitEnabled;
            _micUI.Show(v);
        }
    }
}
