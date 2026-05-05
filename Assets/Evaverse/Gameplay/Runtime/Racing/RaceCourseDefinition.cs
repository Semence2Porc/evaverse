using System.Collections.Generic;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public sealed class RaceCourseDefinition : MonoBehaviour
    {
        [SerializeField] private string courseId = "moonbase-01";
        [SerializeField] private int lapCount = 3;
        [SerializeField] private List<RaceCheckpoint> checkpoints = new();

        public string CourseId => courseId;
        public int LapCount => Mathf.Max(1, lapCount);
        public IReadOnlyList<RaceCheckpoint> Checkpoints => checkpoints;
        public int CheckpointCount => checkpoints.Count;

        public bool IsValidCheckpointIndex(int checkpointIndex)
        {
            return checkpointIndex >= 0 && checkpointIndex < checkpoints.Count;
        }
    }
}
