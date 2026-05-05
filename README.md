# Evaverse

This repository has been bootstrapped as a Unity 6 project skeleton for the Evaverse rebuild.

## Current State

The project currently includes:

- Unity package manifest with the first approved runtime stack
- Project version pinned to Unity `6000.0.65f1`
- Assembly definition layout for:
  - Core
  - Gameplay
  - Networking
  - Meta
  - Web3
  - UI
  - World
- First-pass runtime code for:
  - application bootstrap
  - service registry
  - third-person avatar controller
  - hoverboard movement
  - race checkpoint / lap tracking
  - local session placeholder
  - local progression save
  - NFT metadata normalization primitives
  - debug overlay
- world definition assets
- world spawn / region / POI metadata
- JSON-driven map seed parsing
- graybox blockout generation from a seed file
- generated `Snowbound Nexus` hub scene
- generated world materials and primitive mesh kit
- local playable prototype avatar
- local prototype hoverboard with `E` mount/dismount
- generated race checkpoint tracking
- prototype HUD for controls, lap, checkpoint, timer, best time, and board speed
- green race start gate with countdown

## Current Editor Reality

The project has now been opened successfully in Unity `6000.0.65f1`, packages were resolved, `.meta` files were generated, the custom assemblies compile, and [Hub.unity](/C:/Users/Semence2Microsoft198/Desktop/UnityProject/Evaverse/Assets/_Project/Scenes/Hub.unity) can be regenerated from the editor menu `Evaverse > World > Build Revival Hub Scene`.

Still intentionally missing:

- render pipeline assets
- input action assets if we choose generated input maps
- real multiplayer flow

## Feeding A Map

You can now feed a map in several workable ways:

1. A Unity scene or prefab blockout
2. A top-down sketch or image with rough dimensions
3. A terrain heightmap plus a few landmark coordinates
4. A Blender, FBX, GLB, or OBJ environment file
5. A video/screenshot reference set with notes like "spawn here" and "race loop there"
6. A JSON seed like [EvaverseHubSeed.json](/C:/Users/Semence2Microsoft198/Desktop/UnityProject/Evaverse/Assets/_Project/MapSeeds/EvaverseHubSeed.json)

The JSON seed path is the easiest if you want me to iterate quickly. It supports:

- map metadata
- spawn points
- biome zones
- regions
- points of interest
- primitive blockout elements

There is a `WorldBlockoutBuilder` component in the project that can rebuild a graybox directly from that seed. For huge maps, the recommended workflow is: rough map sketch or JSON seed first, generated blockout second, Blender/Unity art-kit replacement third, Addressables streaming fourth.

## Recommended First Editor Pass

1. Open the project in Unity `6000.0.65f1` or the nearest compatible `6000.0` patch.
2. Let Unity generate the missing project metadata and import packages.
3. Open [Hub.unity](/C:/Users/Semence2Microsoft198/Desktop/UnityProject/Evaverse/Assets/_Project/Scenes/Hub.unity) or use `Evaverse > Playtest > Open Hub And Play`.
4. Add an `EvaverseApplication` object to the first runtime bootstrap scene.
5. Press Play in the hub scene and use `WASD`, `Shift`, `Space`, and `E` near the prototype hoverboard.
6. Ride through the green `START` gate in the `EVA Grand Prix` wing, wait for the countdown, then follow the checkpoint loop.
7. Watch the prototype HUD update lap, checkpoint, timer, and local best time. Press `R` to reset the race.
8. Promote the generated prototype avatar/board into reusable prefabs once the feel is approved.

Useful editor menus:

- `Evaverse > World > Build Revival Hub Scene`
- `Evaverse > Playtest > Open Hub`
- `Evaverse > Playtest > Open Hub And Play`
- `Evaverse > Playtest > Validate Hub Scene`

## Next Code Targets

- replace the local session placeholder with real NGO + Multiplayer Services flow
- polish mount/dismount feel, camera transitions, and board ownership rules
- add finish summary, checkpoint arrows, and cleaner best-time presentation to the race prototype
- promote local player and board prototypes into reusable prefabs
- add scriptable config assets for collections, courses, and progression tuning
- add play mode validation and smoke tests
