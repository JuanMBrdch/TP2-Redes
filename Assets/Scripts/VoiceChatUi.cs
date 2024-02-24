using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Realtime;
using TMPro;

public class VoiceChatUi : MonoBehaviour
{
    private Dictionary<Speaker, Player> _dic = new Dictionary<Speaker, Player>();
    public TextMeshProUGUI text;
    
    public void AddSpeaker(Speaker speaker, Player player)
    {
        _dic[speaker] = player;
    }

    private void Update()
    {
        string chat = "";
        foreach (var item in _dic)
        {
            var speaker = item.Key;
            if (speaker.IsPlaying)
            {
                chat += item.Value.NickName + "\n";
            }
        }

        text.text = chat;
    }
}
