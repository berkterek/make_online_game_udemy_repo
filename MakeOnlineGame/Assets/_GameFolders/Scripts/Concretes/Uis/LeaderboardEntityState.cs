using System;
using Unity.Collections;
using Unity.Netcode;

namespace MakeOnlineGame.Uis
{
    public struct LeaderboardEntityState : INetworkSerializable,IEquatable<LeaderboardEntityState>
    {
        public ulong ClientId;
        public FixedString32Bytes PlayerName;
        public int Coin;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref Coin);
        }

        public bool Equals(LeaderboardEntityState other)
        {
            return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Coin == other.Coin;
        }

        public override bool Equals(object obj)
        {
            return obj is LeaderboardEntityState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, PlayerName, Coin);
        }
    }
}