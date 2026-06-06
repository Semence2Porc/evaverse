using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Racing
{
    public sealed class RaceCheckpointGuide : MonoBehaviour
    {
        [SerializeField] private RaceLapTracker tracker;
        [SerializeField] private Transform arrowRoot;
        [SerializeField] private float arrowHeight = 2.6f;
        [SerializeField] private float bobAmplitude = 0.18f;
        [SerializeField] private float bobSpeed = 2.4f;

        private void LateUpdate()
        {
            if (arrowRoot == null)
            {
                return;
            }

            if (tracker == null || !tracker.Started || tracker.Finished || tracker.CountdownActive)
            {
                arrowRoot.gameObject.SetActive(false);
                return;
            }

            RaceCourseDefinition course = tracker.Course;
            if (course == null || !course.IsValidCheckpointIndex(tracker.NextCheckpointIndex))
            {
                arrowRoot.gameObject.SetActive(false);
                return;
            }

            RaceCheckpoint checkpoint = course.Checkpoints[tracker.NextCheckpointIndex];
            if (checkpoint == null)
            {
                arrowRoot.gameObject.SetActive(false);
                return;
            }

            arrowRoot.gameObject.SetActive(true);

            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;
            Vector3 anchor = transform.position + Vector3.up * (arrowHeight + bob);
            Vector3 target = checkpoint.transform.position + Vector3.up * 2f;
            arrowRoot.position = anchor;
            arrowRoot.LookAt(target);
        }

        public void Configure(RaceLapTracker raceTracker, Transform arrow)
        {
            tracker = raceTracker;
            arrowRoot = arrow;
        }
    }
}
