# Unity Web3 reference (Evaverse)

## Wallet auth (challenge-response) outline

1. Client requests nonce/challenge from server for a wallet address.
2. Client signs the challenge with the wallet.
3. Server verifies signature and issues a session token.
4. Client uses session token for gameplay services.

## What can live on-chain safely

- Ownership records (NFTs)
- Allowlist memberships (if needed)
- Simple entitlements (with careful cost/UX consideration)

## What should NOT be on-chain (for a game)

- High-frequency progression state
- Match results / per-frame state
- Anything requiring fast writes or low cost per action

