---
name: unity-multiplayer-netcode
description: Multiplayer + Unity Netcode for GameObjects (NGO) and Unity Transport (UTP) workflow patterns. Use when implementing spawning, ownership, player objects, session hosting/joining, transport configuration, replication, or debugging network behaviour. Emphasizes correct asmdef references, deterministic spawn flows, and safe client/server separation.
---

# Unity Multiplayer + Netcode (Evaverse)

## Core rules

- **Server-authoritative by default**: gameplay state should be owned/validated by the server unless explicitly client-authoritative.
- **Ownership is not authority**: use ownership for input/control; keep validation server-side for anything that matters.
- **Explicit spawn pipeline**: don’t rely on “scene objects magically exist” for multiplayer.

## Player object pattern

- Player prefab contains:
  - `NetworkObject`
  - movement/controller components (enabled only for owner)
  - camera rig (enabled only for owner)
- Non-owner instances:
  - should not run input, local-only camera, or expensive local HUD logic

## Transport configuration

- Set UTP connection data explicitly for:
  - host listen: bind `0.0.0.0` + port
  - client dial: address + port
- Ensure asmdefs include:
  - `Unity.Netcode.Runtime`
  - `Unity.Networking.Transport` (required for UTP types in some compilation contexts)

## Session flow expectations

- **StartHost**:
  - transport configure
  - register prefabs
  - `NetworkManager.StartHost()`
- **StartClient**:
  - parse address/port
  - transport configure
  - register prefabs
  - `NetworkManager.StartClient()`
- **Disconnect**:
  - `NetworkManager.Shutdown()`
  - reset local session flags/state

## Debugging approach

- Reproduce in batchmode compile first (asmdefs/metas).
- Then reproduce with two instances:
  - host
  - client
- Fix the first clear invariant break (spawn/ownership/transport) before tuning behaviour.

