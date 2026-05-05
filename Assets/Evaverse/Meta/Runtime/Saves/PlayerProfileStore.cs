using System.IO;
using Evaverse.Meta.Runtime.Progression;
using UnityEngine;

namespace Evaverse.Meta.Runtime.Saves
{
    public static class PlayerProfileStore
    {
        private const string FileName = "player-profile.json";

        public static string GetProfilePath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }

        public static PlayerProgressionState Load()
        {
            string path = GetProfilePath();
            if (!File.Exists(path))
            {
                return new PlayerProgressionState();
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerProgressionState>(json) ?? new PlayerProgressionState();
        }

        public static void Save(PlayerProgressionState state)
        {
            string path = GetProfilePath();
            string directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(state, true);
            File.WriteAllText(path, json);
        }
    }
}
