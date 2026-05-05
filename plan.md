# Evaverse Rebuild Plan

## Working Thesis

Evaverse should be rebuilt as a restoration-first multiplayer game, not as a token-first product. The goal is to return real playable utility to the existing NFT holders while making the game fun and accessible for non-holders too.

## What Is Confirmed

- Evaverse was a free-to-play social game on Steam by Battlebound.
- The retired Steam page confirms these shipped or live-presented modes:
  - Hoverboarding / EVA Grand Prix
  - Cosmic Cup
  - Social mini-games: Treasure Hunting, Stepping Stones, Railgun, Low Gravity, Bowling, Soccer, Tag
- The same Steam page also advertised an upcoming Arena Shooter mode targeted for Q1 2023.
- SteamDB lists the game as using Unity Engine.
- OpenSea still exposes the three core collections through the expected chains/contracts:
  - First Arrivals / Evaverse: Ethereum
  - Hoverboards: Polygon
  - Turtle Troops: Ethereum
- OpenSea item pages still describe the avatars and turtles as rigged/animated in-game characters and the hoverboards as ridable game assets.
- Publicly exposed media on OpenSea currently resolves to MP4 preview assets and profile image endpoints, not to public `glb`, `gltf`, `fbx`, or `vrm` files.

## What I Believe After Validation

- Your source material is directionally strong.
- The broad product vision is worth pursuing.
- The weakest part of the earlier notes was treating some roadmap and ecosystem claims as equally certain.
- The safest path is to split everything into three buckets:
  - Confirmed Original: directly supported by Steam/OpenSea/official pages.
  - Inferred Restoration: very likely true, but not fully re-verified in this pass.
  - New Evolution: our deliberate 2026 rebuild design.

## Visual Rebuild Decision

The first map direction is `Snowbound Nexus`: a restored snowy neon sci-fi social hub with hoverboard race access, Cosmic Cup presence, pet/turtle social flavor, and minigame pads. Lava, jungle, desert, and cosmic areas are approved as expansion districts, but they are treated as `New Evolution` until proven otherwise.

Design rule: every district must contain at least one activity hook, one traversal hook, one social gathering hook, and one strong skyline landmark.

## Rebuild Strategy

### Phase 0

Documentation, architecture, package approval, Unity project bootstrap.

### Phase 1

Single vertical slice with:

- Social hub
- Third-person avatar controller
- Hoverboard controller
- One race loop
- Wallet-optional account flow
- No on-chain writes

Current Phase 1 implementation target:

- Generate the first `Snowbound Nexus` hub scene inside Unity.
- Store map metadata as a `WorldMapDefinition` asset.
- Add biome/district data so huge maps can be fed by seed JSON, editor tooling, or later GIS/concept-map inputs.
- Keep all high-fidelity art replaceable behind generated blockout anchors.

### Phase 2

Online session flow with:

- Multiplayer sessions
- Join/create flow
- Player spawning
- Cosmetic trait-driven avatar assembly
- Leaderboards and off-chain progression

### Phase 3

Restoration systems:

- NFT ownership read layer
- Metadata normalization
- Reconstructed avatar / board / turtle assembly pipeline
- Seasonal content hooks

### Phase 4

Expansion systems:

- Browser WebGL slice
- Social chat / voice
- Creator tools
- Track building
- Governance-backed persistent changes

## Hard Product Decisions For Now

- Gameplay comes first; tokenomics come later.
- Wallet connection is optional in early builds.
- Tickets are the in-game progression currency for the playable loop.
- Direct USDC race-entry loops are out until legal and operational review.
- Browser support is a secondary target and should not force the desktop build to become bland.

## Package Shortlist For Your Approval

### Approve Early

- `com.unity.render-pipelines.universal`
- `com.unity.inputsystem`
- `com.unity.cinemachine`
- `com.unity.addressables`
- `com.unity.ai.navigation`
- `com.unity.netcode.gameobjects`
- `com.unity.transport`
- `com.unity.services.multiplayer`
- `com.unity.services.authentication`
- `com.unity.services.cloudsave`
- `com.unity.services.leaderboards`

### Approve When We Reach Multiplayer QA

- Multiplayer Play Mode
- Multiplayer Tools

### Approve Later Only If Needed

- Vivox Unity SDK
- Nethereum Unity (`com.nethereum.unity`)
- thirdweb Unity SDK

## Current Recommendation On Blockchain SDK

- If phase 1 is read-only wallet + ownership + metadata fetch, I recommend `Nethereum.Unity` first because it is package-manager friendly and explicitly documents Unity/WebGL support.
- If you want broader wallet UX, marketplace helpers, and heavier EVM product tooling, `thirdweb` is a reasonable alternative.
- We should not install both in the first pass.

## Open Questions I Am Intentionally Deferring

- Dedicated servers vs relay-hosted sessions
- Full creator-economy contracts
- DAO governance implementation
- Stablecoin payout mechanics
- Browser parity with desktop

## Immediate Next Step

Build the first generated hub scene and map-definition pipeline, then move into playable traversal/race integration.

## Model Recommendation

Stay on GPT-5.5 extra-high or high while we are making architecture, map-generation tooling, multiplayer boundaries, and Web3 choices. Once the hub generator, first scene, movement/race loop, and package stack are stable, routine Unity implementation can move to GPT-5.4 high/medium. Composer is fine later for repetitive scene dressing, UI wiring, prefab cleanup, and asset iteration, but I would not use it as the lead model for architecture, networking, blockchain boundaries, or large refactors.
