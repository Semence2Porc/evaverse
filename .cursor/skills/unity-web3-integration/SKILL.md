---
name: unity-web3-integration
description: Web3 integration patterns for Unity games. Use when designing wallet sign-in, on-chain/off-chain data flow, transaction UX, signature challenges, token/NFT ownership gating, and secure persistence. Focuses on minimal trust, avoiding secrets in clients, and Unity-friendly async/service architecture.
---

# Unity Web3 Integration (Evaverse)

## Non-negotiables (security)

- **Never put private keys or signing secrets in the Unity client.**
- **Client is untrusted**: any gating that matters must be validated server-side (or via verifiable proofs).
- Prefer **“sign-in with wallet”** via challenge/response (nonce) rather than “connect wallet = authenticated”.

## On-chain vs off-chain split

- On-chain: ownership, transfers, approvals, immutable state you truly need.
- Off-chain: profiles, progression, matchmaking, session state, cosmetics selection, telemetry.

## UX patterns (transactions)

- Minimize prompts: batch actions where possible; avoid repeated signatures.
- Always provide:
  - clear action summary
  - expected gas/cost hint (if available)
  - retry/cancel paths

## Data & caching

- Cache ownership checks locally with a TTL, but treat cache as advisory.
- Persist only public identifiers locally (wallet address, chain id), not secrets.

## Architecture in Unity

- Web3 is a service boundary:
  - `IWalletService` (connect, current address, sign message)
  - `IWeb3OwnershipService` (query ownership / entitlements)
  - `IWeb3TxService` (submit tx + track confirmation)
- Keep the Unity UI layer dumb: it calls services and renders state.

