using UnityEngine;

namespace Evaverse.World.Runtime.Hub
{
    /// <summary>
    /// Loops a soft ambient bed for the hub. Uses a procedural tone when no clip is assigned.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class HubAmbientAudio : MonoBehaviour
    {
        [SerializeField] [Range(0f, 1f)] private float volume = 0.22f;

        private AudioSource source;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.loop = true;
            source.playOnAwake = true;
            source.spatialBlend = 0f;
            source.volume = volume;

            if (source.clip == null)
            {
                source.clip = CreateAmbientClip();
            }

            if (!source.isPlaying)
            {
                source.Play();
            }
        }

        private static AudioClip CreateAmbientClip()
        {
            const int sampleRate = 44100;
            const float durationSeconds = 4f;
            int sampleCount = Mathf.RoundToInt(sampleRate * durationSeconds);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float t = i / (float)sampleRate;
                float wind = Mathf.PerlinNoise(t * 0.12f, 0.17f) * 0.18f;
                float hum = Mathf.Sin(t * 42f) * 0.03f + Mathf.Sin(t * 63f) * 0.02f;
                samples[i] = Mathf.Clamp(wind + hum, -0.35f, 0.35f);
            }

            AudioClip clip = AudioClip.Create("evaverse-hub-ambient", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
