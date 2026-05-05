# Evaverse Agent Guide

## Mission

Ship a credible Evaverse restoration slice that is fun as a game even before the full web3 layer is back.

## Core Values

- Restoration over speculation
- Fun over token mechanics
- Shipping over theorizing
- Clear provenance over fuzzy lore
- Replaceable integrations over vendor lock-in

## Mandatory Behavior

### 1. Respect the canon buckets

Whenever you add or describe a feature, decide whether it is:

- Confirmed Original
- Inferred Restoration
- New Evolution

Do not silently blur those together.

### 2. Keep gameplay decoupled from chain writes

The first playable versions must work without:

- sending blockchain transactions
- minting new NFTs
- direct stablecoin race-entry loops

### 3. Default to Unity-first simplicity

Use the GameObject stack first. Avoid premature DOTS, custom networking layers, or bespoke backend infrastructure unless the current milestone truly needs them.

### 4. Keep wallet connection optional

Players should be able to enter the game and feel the loop before being asked to sign anything.

### 5. Design for browser awareness, not browser domination

Desktop quality is the baseline. Browser support is real, but it should be a shaped slice with deliberate constraints.

## Implementation Priorities

### First

- Player movement
- Camera feel
- Hoverboard feel
- Hub flow
- Race loop
- Session flow

### Second

- Progression
- Cosmetics
- Trait assembly
- Save/load

### Third

- Voice/chat
- UGC
- governance
- advanced chain features

## Package Discipline

Before adding a package:

- prefer official Unity packages first
- add one blockchain SDK, not two
- record why the package is needed
- avoid hidden hard dependencies between gameplay and web3 code

## Coding Rules

- Prefer thin interfaces around external services
- Keep data normalization separate from rendering
- Keep session orchestration separate from game rules
- Prefer additive scenes over giant monolithic scenes
- Use Addressables instead of hard-linking every cosmetic asset into startup scenes

## Documentation Rules

If architecture changes, update:

- `plan.md`
- `architecture.md`

If workstreams change, update:

- `skills.md`

## What Not To Do

- Do not rebuild the whole roadmap before one mode is fun
- Do not assume lost 3D assets can be recovered as production-ready files
- Do not make the race economy legally risky by default
- Do not tie every gameplay reward to NFT ownership
- Do not overfit the project to one AI tool or one vendor

## Definition Of Good Progress

Good progress means the repo becomes easier to build, easier to reason about, and closer to one playable online slice.

Fancy design docs without a playable path are not enough.
