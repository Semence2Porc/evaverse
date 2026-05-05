# Evaverse Architecture

## Architecture Stance

Build the first version with Unity's GameObject stack, not DOTS. The project needs a playable restoration slice quickly, and the original product shape is much closer to character controllers, minigames, UI, session flows, and runtime cosmetics than to massive-simulation ECS problems.

## Canon Rules

Every feature should be labeled internally as one of:

- Confirmed Original
- Inferred Restoration
- New Evolution

That rule matters because this rebuild will otherwise drift into fan fiction and lose the value of being a credible Evaverse successor.

## Revival Visual Direction

The first public-facing world is `Snowbound Nexus`, a restoration-forward hub inspired by the original Evaverse media: snowy sci-fi plaza, chunky white geometry, icy spires, hex floor language, hoverboard ramps, neon cyan/orange/pink activity trims, and social minigame pads.

Expansion districts are allowed, but must be labeled as `New Evolution` unless we later verify they existed in the original roadmap:

- `Lava Forge`: high-contrast arena, molten gaps, industrial rails, combat prototype space.
- `Jungle Canopy`: treasure hunt, pet/turtle encounters, vertical exploration, hidden paths.
- `Desert Dunes`: wide hoverboard race lines, ruins, wind-sculpted visibility beats.
- `Cosmic Stadium`: Cosmic Cup, low-gravity events, portals, seasonal spectacle.

The hub should feel like one huge destination with distinct districts, not a menu disguised as a map.

## Runtime Targets

### Primary

- Windows desktop
- macOS desktop

### Secondary

- WebGL desktop browsers only

### Not A Phase 1 Target

- Mobile browsers
- Full feature parity between browser and desktop

## Engine Stack

- Unity 6 project based on the `Universal 3D` template
- URP for cross-platform rendering and saner WebGL budgets
- Input System for keyboard, mouse, and controller abstraction
- Cinemachine for follow/orbit/race cameras
- Addressables for streaming heavy assets and keeping browser builds survivable

## Multiplayer Stack

- Netcode for GameObjects for phase 1 and phase 2 multiplayer
- Unity Transport as the low-level transport
- Multiplayer Services SDK for sessions, Lobby, Relay, and Matchmaker through one API
- Relay-backed session hosting first
- Dedicated server support later if concurrency or fairness requires it

## Browser Constraint Notes

- Unity WebGL is desktop-browser focused and not a reliable mobile target.
- Browser multiplayer should use Relay with secure WebSockets where needed.
- Browser voice must be treated as limited.
- Vivox WebGL support exists with limitations; desktop can later support richer voice than the browser.
- Do not promise positional voice parity in WebGL.

## Persistence Stack

- Authentication for anonymous and later linked identity
- Cloud Save for off-chain player progression and config
- Leaderboards for race and activity ranking
- Cloud Code later for server-validated reward rules, creator moderation, and world-state workflows

## Web3 Boundary

The blockchain layer must be isolated from the core gameplay loop.

### Phase 1 Responsibilities

- Read wallet ownership
- Read NFT metadata
- Normalize trait data
- Drive cosmetics and access flags

### Explicitly Deferred

- Mandatory wallet login
- Smart-contract writes in normal gameplay
- Stablecoin race-entry loops
- Fully on-chain UGC economy

## Content Reconstruction Pipeline

### Step 1

Read collection metadata and map it into a canonical internal trait schema.

### Step 2

Map canonical traits to modular art pieces:

- Base body
- Hair / headgear
- Eye / face layer
- Upper / lower garb
- Armor pieces
- Board shell / trim / core
- Turtle shell / accessories

### Step 3

Assemble runtime prefabs from those modular pieces rather than attempting to resurrect one-off lost models.

## World Creation Pipeline

### Step 1

Capture map intent as data: map definition asset, seed JSON, region bounds, spawn points, biome zones, points of interest, and blockout primitives.

### Step 2

Generate a navigable Unity blockout from that data. Blockouts should include collider-safe floors, portals, race gates, social pads, and landmark silhouettes before high-fidelity art exists.

### Step 3

Replace generated primitives with modular kits:

- Unity-authored prefabs for layout-critical pieces.
- Blender-generated meshes for hero props, ice spires, arches, boards, rails, and district dressing.
- Addressables groups for heavy district art and future streamed map chunks.

### Step 4

Bake scene-specific lighting, nav surfaces, spawn validation, race checkpoint validation, and WebGL LOD variants.

## Blender Policy

Blender 5.1 can be driven by batch Python scripts from this machine, so an MCP server is not required to create meshes, export GLB/FBX, or generate modular kits. A Blender MCP/live-control setup may become useful later when we want interactive inspection, viewport iteration, or agent-guided sculpting inside an open Blender scene, but it is not a blocker for the first generated hub.

## Recommended Code Boundaries

Use assembly definitions and keep the following modules separate:

- `Evaverse.Core`
- `Evaverse.Gameplay`
- `Evaverse.Networking`
- `Evaverse.Meta`
- `Evaverse.Web3`
- `Evaverse.UI`
- `Evaverse.Editor`

## Suggested Folder Layout

```text
Assets/
  _Project/
    Art/
    Audio/
    Materials/
    Prefabs/
    Scenes/
    Settings/
  Evaverse/
    Core/
    Gameplay/
      Avatar/
      Hoverboard/
      Hub/
      Racing/
      Minigames/
    Networking/
    Meta/
      Progression/
      Leaderboards/
      Saves/
    Web3/
      Ownership/
      Metadata/
      Traits/
    UI/
    UGC/
    Editor/
  AddressablesData/
```

## Economy Safety Rule

The playable game loop should use off-chain progression first:

- Tickets for game activity
- XP for board and avatar progression
- Cosmetics and unlocks as primary reward surface

If stablecoins or creator payouts appear later, they should sit outside the immediate match result loop and be implemented only after legal review.

## Networking Rule Of Thumb

- Hub and casual race prototype: NGO + Relay is enough
- Competitive anti-cheat or large concurrency: reevaluate authority model later
- Do not switch to Entities just because multiplayer exists

## Browser Delivery Rule Of Thumb

Desktop gets the full vertical slice first.

Browser gets a performance-shaped slice:

- smaller hub shard
- lower avatar density
- lower VFX density
- streamed assets
- simpler chat and social features

## Architectural Non-Goals For The First Build

- DAO-governed permanent world edits
- Roblox-scale creator scripting
- universal asset interoperability with every community
- full land economy
- all original modes at once

## First Technical Milestone

One shared codebase that can do all of the following:

- boot into a stylized hub
- move a player avatar
- mount a hoverboard
- run one race
- host/join an online session
- save off-chain progression
- reserve hooks for NFT trait-driven cosmetics
