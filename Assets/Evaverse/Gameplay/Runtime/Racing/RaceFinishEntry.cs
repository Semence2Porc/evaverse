using System;
using Unity.Collections;
using Unity.Netcode;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public struct RaceFinishEntry : INetworkSerializable, IEquatable<RaceFinishEntry>
    {
        public ulong ClientId;
        public int Place;
        public float FinishSeconds;
        public FixedString32Bytes DisplayName;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Place);
            serializer.SerializeValue(ref FinishSeconds);
            serializer.SerializeValue(ref DisplayName);
        }

        public bool Equals(RaceFinishEntry other)
        {
            return ClientId == other.ClientId
                && Place == other.Place
                && FinishSeconds.Equals(other.FinishSeconds)
                && DisplayName.Equals(other.DisplayName);
        }

        public override bool Equals(object obj)
        {
            return obj is RaceFinishEntry other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, Place, FinishSeconds, DisplayName);
        }
    }
}
