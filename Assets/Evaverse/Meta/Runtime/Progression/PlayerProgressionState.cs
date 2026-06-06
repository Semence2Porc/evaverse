using System;
using UnityEngine;

namespace Evaverse.Meta.Runtime.Progression
{
    [Serializable]
    public sealed class PlayerProgressionState
    {
        [SerializeField] private string displayName = "Racer";
        [SerializeField] private int tickets;
        [SerializeField] private int avatarLevel = 1;
        [SerializeField] private int hoverboardLevel = 1;
        [SerializeField] private int racesCompleted;

        public string DisplayName => SanitizeDisplayName(displayName);
        public int Tickets => tickets;
        public int AvatarLevel => avatarLevel;
        public int HoverboardLevel => hoverboardLevel;
        public int RacesCompleted => racesCompleted;

        public void SetDisplayName(string value)
        {
            displayName = SanitizeDisplayName(value);
        }

        public static string SanitizeDisplayName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Racer";
            }

            string trimmed = value.Trim();
            return trimmed.Length > 16 ? trimmed.Substring(0, 16) : trimmed;
        }

        public void AddTickets(int amount)
        {
            tickets = Mathf.Max(0, tickets + amount);
        }

        public void CompleteRace(int rewardTickets)
        {
            racesCompleted++;
            hoverboardLevel = Mathf.Max(1, hoverboardLevel);
            AddTickets(rewardTickets);
        }
    }
}
