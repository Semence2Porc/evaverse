using System.Collections.Generic;
using Evaverse.Web3.Runtime.Metadata;

namespace Evaverse.Web3.Runtime.Traits
{
    public static class TraitNormalizer
    {
        public static CosmeticLoadout Normalize(TokenDescriptor descriptor)
        {
            var loadout = new CosmeticLoadout
            {
                collectionId = descriptor.collectionId,
                tokenId = descriptor.tokenId
            };

            if (descriptor.traits == null)
            {
                return loadout;
            }

            foreach (var trait in descriptor.traits)
            {
                loadout.traits.Add(new CanonicalTrait
                {
                    slot = NormalizeSlot(trait.category),
                    value = trait.value,
                    sourceCollection = descriptor.collectionId
                });
            }

            return loadout;
        }

        private static string NormalizeSlot(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return "unknown";
            }

            string lowered = category.Trim().ToLowerInvariant();

            return lowered switch
            {
                "hair" => "head.hair",
                "eyes" => "face.eyes",
                "mouth" => "face.mouth",
                "top" => "body.upper",
                "bottom" => "body.lower",
                "shell" => "pet.shell",
                "board" => "board.body",
                _ => lowered
            };
        }
    }
}
