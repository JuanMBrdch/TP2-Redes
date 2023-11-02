using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct CustomData : INetworkSerializable
{
    public FixedString128Bytes nickname;
    public int score;
    public bool isDead;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref nickname);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref isDead);
    }
}
