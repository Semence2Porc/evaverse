# IDE Follow-up Todo (Cursor + Unity + Blender MCP)

Work that needs the **local Cursor IDE**, Unity Editor, and optionally **Blender MCP**. Cloud agents can write the code; these steps need the GUI tools.

## Immediate (unblocks playtesting)

- [ ] Open Unity 6000.0.65f1 on this project
- [ ] Run `Evaverse > Networking > Bake Network Player Prefab`
- [ ] Run `Evaverse > World > Build Netcode Hub Scene (Direct)`
- [ ] Fix any compile/asmdef/package import errors in the Console
- [ ] Two-client smoke test (ParrelSync / Multiplayer Play Mode / two builds):
  - [ ] Host / Join with display names
  - [ ] See each other walk + mount boards
  - [ ] Synced race start at green gate
  - [ ] Finish board updates
  - [ ] District portals with **F**
- [ ] Link Unity Gaming Services project + enable Relay, then rebuild Relay hub and test join codes

## Art / Blender MCP

- [ ] Replace capsule avatar with modular Evaverse-style body kit (body, head, hair slots)
- [ ] Replace hoverboard primitives with a ridable board mesh (deck, trim, glow)
- [ ] Author ice spire / hex plaza / portal pad hero props and export GLB/FBX into `Assets/_Project/Art/`
- [ ] Create district-specific landmark meshes for Lava / Jungle / Desert / Cosmic
- [ ] Simple VFX: portal shimmer, race checkpoint gate glow, boost trail
- [ ] Ambient snow / fog particles for Snowbound Nexus

## Unity Editor content

- [ ] Rebuild Hub after art drops; keep spawn IDs stable (`plaza-default`, `race-gate`, etc.)
- [ ] Assign URP materials / emission for neon cyan-orange language
- [ ] Bake lighting + navmesh for hub floors
- [ ] Audio: hub bed, race start sting, finish sting, board boost SFX
- [ ] Input Actions asset (replace raw Keyboard/Gamepad polls when ready)
- [ ] Addressables groups for district art streaming

## Gameplay polish in Editor

- [ ] Tune CharacterController / HoverboardMotor feel in Play Mode
- [ ] Verify race checkpoint order and trigger sizes while mounted
- [ ] Camera blends (foot vs board) with real avatar proportions
- [ ] Minimap or compass UI mock (uGUI, not OnGUI)
- [ ] Replace OnGUI panels with Canvas UI (join, race HUD, pilot log, finish board)

## Multiplayer QA

- [ ] Install Multiplayer Play Mode / Multiplayer Tools packages
- [ ] Host migration / host leave client recovery
- [ ] Disconnect mid-race cleanup
- [ ] 4-player stress on Direct Netcode LAN
- [ ] Relay region latency check

## Web3 (IDE + external services)

- [ ] Choose Nethereum vs thirdweb and add package
- [ ] Wallet connect prototype (optional login)
- [ ] Read First Arrivals / Hoverboards / Turtle Troops ownership
- [ ] Map OpenSea traits → `CosmeticLoadout` → modular mesh slots
- [ ] Keep gameplay loop offline-first (no mandatory wallet)

## Product / shipping

- [ ] Steam page draft + build pipeline
- [ ] WebGL performance slice (smaller hub, fewer players)
- [ ] Legal review before any USDC / ticket-to-cash loops
- [ ] One social minigame prototype pad (tag or treasure hunt)

## Already shipped in code (do not re-implement — wire/test in Unity)

- Multiplayer host/join (Direct + Relay)
- Networked players, boards, mount sync, race sync
- Finish board, name tags, synced countdown
- Display names, pilot ticket upgrades
- District portal teleports (**F**)
- Demo cosmetic tint hooks
