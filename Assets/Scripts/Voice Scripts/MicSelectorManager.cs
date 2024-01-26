using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice;
using Photon.Voice.Unity;

public class MicSelectorManager : MonoBehaviour
{
    public Dropdown dDown;
    public Recorder rec;
    private void Awake()
    {
        var list = new List<string>(Microphone.devices);
        dDown.AddOptions(list);
    }

    public void SetMic(int i)
    {
        var mic = Microphone.devices[i];
        rec.MicrophoneDevice = new DeviceInfo(mic);
    }
}
