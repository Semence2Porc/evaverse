using UnityEngine;

namespace Evaverse.Meta.Runtime.Identity
{
    public static class PlayerDisplayNameStore
    {
        private const string PrefsKey = "Evaverse.DisplayName";
        private const int MaxLength = 16;

        public static string Load()
        {
            return Sanitize(PlayerPrefs.GetString(PrefsKey, string.Empty));
        }

        public static void Save(string displayName)
        {
            string sanitized = Sanitize(displayName);
            PlayerPrefs.SetString(PrefsKey, sanitized);
            PlayerPrefs.Save();
        }

        public static string ResolveOrDefault(ulong clientId)
        {
            string stored = Load();
            if (!string.IsNullOrWhiteSpace(stored))
            {
                return stored;
            }

            return $"Racer {clientId}";
        }

        public static string Sanitize(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            string trimmed = raw.Trim();
            if (trimmed.Length > MaxLength)
            {
                trimmed = trimmed.Substring(0, MaxLength);
            }

            return trimmed;
        }
    }
}
