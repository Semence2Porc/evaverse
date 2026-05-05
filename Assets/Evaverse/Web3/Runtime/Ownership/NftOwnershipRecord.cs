using System;

namespace Evaverse.Web3.Runtime.Ownership
{
    [Serializable]
    public sealed class NftOwnershipRecord
    {
        public string walletAddress;
        public string collectionId;
        public string chainId;
        public string contractAddress;
        public string tokenId;
    }
}
