using Unity.Netcode;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public struct RaceTrackerSnapshot : INetworkSerializable
    {
        public RaceRunState State;
        public int CurrentLap;
        public int NextCheckpointIndex;
        public float CountdownRemaining;
        public float ElapsedSeconds;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref State);
            serializer.SerializeValue(ref CurrentLap);
            serializer.SerializeValue(ref NextCheckpointIndex);
            serializer.SerializeValue(ref CountdownRemaining);
            serializer.SerializeValue(ref ElapsedSeconds);
        }
    }
}
