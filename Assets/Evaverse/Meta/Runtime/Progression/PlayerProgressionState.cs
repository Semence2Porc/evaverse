using System;
using UnityEngine;

namespace Evaverse.Meta.Runtime.Progression
{
    [Serializable]
    public sealed class PlayerProgressionState
    {
        public const int BoardUpgradeCost = 25;
        public const int AvatarUpgradeCost = 20;
        public const int MaxBoardLevel = 5;
        public const int MaxAvatarLevel = 5;

        [SerializeField] private int tickets;
        [SerializeField] private int avatarLevel = 1;
        [SerializeField] private int hoverboardLevel = 1;
        [SerializeField] private int racesCompleted;

        public int Tickets => tickets;
        public int AvatarLevel => avatarLevel;
        public int HoverboardLevel => hoverboardLevel;
        public int RacesCompleted => racesCompleted;

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

        public bool TryUpgradeHoverboard()
        {
            if (hoverboardLevel >= MaxBoardLevel || tickets < BoardUpgradeCost)
            {
                return false;
            }

            tickets -= BoardUpgradeCost;
            hoverboardLevel++;
            return true;
        }

        public bool TryUpgradeAvatar()
        {
            if (avatarLevel >= MaxAvatarLevel || tickets < AvatarUpgradeCost)
            {
                return false;
            }

            tickets -= AvatarUpgradeCost;
            avatarLevel++;
            return true;
        }

        public float ResolveBoardSpeedMultiplier()
        {
            return 1f + (hoverboardLevel - 1) * 0.08f;
        }

        public float ResolveWalkSpeedMultiplier()
        {
            return 1f + (avatarLevel - 1) * 0.05f;
        }
    }
}
