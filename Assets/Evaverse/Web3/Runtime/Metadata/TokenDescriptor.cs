using System;
using System.Collections.Generic;

namespace Evaverse.Web3.Runtime.Metadata
{
    [Serializable]
    public sealed class TokenDescriptor
    {
        public string collectionId;
        public string chainId;
        public string contractAddress;
        public string tokenId;
        public List<TokenTrait> traits = new();
    }

    [Serializable]
    public sealed class TokenTrait
    {
        public string category;
        public string value;
    }
}
