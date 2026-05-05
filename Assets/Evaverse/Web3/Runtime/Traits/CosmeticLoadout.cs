using System;
using System.Collections.Generic;

namespace Evaverse.Web3.Runtime.Traits
{
    [Serializable]
    public sealed class CosmeticLoadout
    {
        public string collectionId;
        public string tokenId;
        public List<CanonicalTrait> traits = new();
    }
}
