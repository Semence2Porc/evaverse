using System.Collections.Generic;
using Evaverse.Web3.Runtime.Ownership;
using Evaverse.Web3.Runtime.Traits;
using UnityEngine;

namespace Evaverse.Web3.Runtime.Cosmetics
{
    public static class DemoCosmeticFactory
    {
        public static CosmeticLoadout CreateDemoLoadout()
        {
            return new CosmeticLoadout
            {
                collectionId = "evaverse-demo",
                tokenId = "demo-001",
                traits = new List<CanonicalTrait>
                {
                    new CanonicalTrait { slot = "body.upper", value = "Neon Cyan", sourceCollection = "evaverse-demo" },
                    new CanonicalTrait { slot = "board.body", value = "Orange Trim", sourceCollection = "evaverse-demo" }
                }
            };
        }

        public static NftOwnershipRecord CreateDemoOwnership(string walletAddress)
        {
            return new NftOwnershipRecord
            {
                walletAddress = walletAddress,
                collectionId = "evaverse-demo",
                chainId = "1",
                contractAddress = "0xdemo",
                tokenId = "demo-001"
            };
        }

        public static Color ResolveAvatarAccent(CosmeticLoadout loadout)
        {
            string value = FindTraitValue(loadout, "body.upper");
            if (value.IndexOf("Orange", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new Color(1f, 0.45f, 0.08f);
            }

            if (value.IndexOf("Magenta", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new Color(0.95f, 0.1f, 1f);
            }

            return new Color(0.04f, 0.93f, 1f);
        }

        public static Color ResolveBoardAccent(CosmeticLoadout loadout)
        {
            string value = FindTraitValue(loadout, "board.body");
            if (value.IndexOf("Cyan", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return new Color(0.04f, 0.93f, 1f);
            }

            return new Color(1f, 0.45f, 0.08f);
        }

        private static string FindTraitValue(CosmeticLoadout loadout, string slot)
        {
            if (loadout?.traits == null)
            {
                return string.Empty;
            }

            for (int i = 0; i < loadout.traits.Count; i++)
            {
                if (loadout.traits[i] != null && loadout.traits[i].slot == slot)
                {
                    return loadout.traits[i].value ?? string.Empty;
                }
            }

            return string.Empty;
        }
    }
}
