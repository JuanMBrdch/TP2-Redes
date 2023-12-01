using Unity.Collections;
using Unity.Netcode;

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
