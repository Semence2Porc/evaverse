using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public enum RaceRunState
    {
        Idle,
        Countdown,
        Running,
        Finished
    }

    public sealed class RaceLapTracker : MonoBehaviour
    {
        [SerializeField] private RaceCourseDefinition course;

        public RaceCourseDefinition Course => course;
        public int CurrentLap { get; private set; } = 1;
        public int NextCheckpointIndex { get; private set; }
        public RaceRunState State { get; private set; } = RaceRunState.Idle;
        public bool Started => State == RaceRunState.Running || State == RaceRunState.Finished;
        public bool Finished => State == RaceRunState.Finished;
        public bool CountdownActive => State == RaceRunState.Countdown;
        public int CheckpointCount => course != null ? course.CheckpointCount : 0;
        public float CountdownRemaining { get; private set; }
        public float ElapsedSeconds
        {
            get
            {
                if (!Started)
                {
                    return 0f;
                }

                return Finished ? finishedSeconds : Time.time - startedAtSeconds;
            }
        }

        private float startedAtSeconds;
        private float finishedSeconds;

        private void Start()
        {
            ResetProgress();
        }

        private void Update()
        {
            if (!CountdownActive)
            {
                return;
            }

            CountdownRemaining -= Time.deltaTime;
            if (CountdownRemaining <= 0f)
            {
                BeginRun();
            }
        }

        public bool StartCountdown(float durationSeconds)
        {
            if (course == null || course.CheckpointCount == 0 || State == RaceRunState.Countdown || State == RaceRunState.Running)
            {
                return false;
            }

            CurrentLap = 1;
            NextCheckpointIndex = 0;
            finishedSeconds = 0f;
            CountdownRemaining = Mathf.Max(0.1f, durationSeconds);
            State = RaceRunState.Countdown;
            return true;
        }

        public void ResetProgress()
        {
            CurrentLap = 1;
            NextCheckpointIndex = 0;
            State = RaceRunState.Idle;
            CountdownRemaining = 0f;
            startedAtSeconds = 0f;
            finishedSeconds = 0f;
        }

        public bool TryPassCheckpoint(RaceCheckpoint checkpoint)
        {
            if (course == null || checkpoint == null || Finished || CountdownActive || course.CheckpointCount == 0)
            {
                return false;
            }

            if (checkpoint.Index != NextCheckpointIndex)
            {
                return false;
            }

            if (State == RaceRunState.Idle)
            {
                BeginRun();
            }

            NextCheckpointIndex++;

            if (NextCheckpointIndex >= course.Checkpoints.Count)
            {
                NextCheckpointIndex = 0;
                CurrentLap++;

                if (CurrentLap > course.LapCount)
                {
                    State = RaceRunState.Finished;
                    finishedSeconds = Time.time - startedAtSeconds;
                }
            }

            return true;
        }

        private void BeginRun()
        {
            State = RaceRunState.Running;
            CountdownRemaining = 0f;
            startedAtSeconds = Time.time;
        }
    }
}
