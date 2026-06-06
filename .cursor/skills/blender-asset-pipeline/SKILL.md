---
name: blender-asset-pipeline
description: Blender-to-Unity realtime asset pipeline for Evaverse. Use when creating or editing 3D meshes, UVs, materials/textures, rigs/animations, LODs, or export settings for import into Unity (URP). Emphasizes clean transforms, naming, pivot/origin discipline, PBR map conventions, and predictable FBX/glTF export.
---

# Blender Asset Pipeline (Evaverse)

## Goals

- Assets import into Unity **without scale/orientation surprises**
- Textures/materials are **URP-friendly** and consistent
- Rigs/animations are **retargetable** and stable
- Files are **game-ready**: pivots, LODs, collider intent, naming

## Non‑negotiables (before export)

- **Units/scale**: treat 1 Blender unit as 1 meter for realtime work.
- **Transforms**: apply transforms so exported objects are clean:
  - Location/Rotation/Scale in sensible state
  - Scale should be **(1,1,1)** after applying (avoid hidden scaling)
- **Origin/pivot**:
  - Props: pivot at functional point (e.g., base center for statues, hinge for doors)
  - Characters: origin at ground between feet; facing forward consistently
- **Topology**:
  - remove doubles, non-manifold edges, loose geometry
  - ensure consistent normals; recalc and then manually fix hard edges where needed

## Naming conventions (practical)

- Use stable, engine-friendly names:
  - `env_`, `prop_`, `ch_` prefixes are fine
  - LODs: `AssetName_LOD0`, `AssetName_LOD1`, ...
  - Colliders (when authored in Blender): `UCX_AssetName_00`, `UCX_AssetName_01` (Unity-friendly FBX convention)

## UVs & texturing (PBR for Unity URP)

- **UVs**:
  - no overlaps for unique bakes (unless intentionally tiling)
  - keep consistent texel density within an asset set
- **Maps** (recommended set):
  - BaseColor/Albedo (sRGB, no lighting baked in)
  - Normal (tangent space)
  - Metallic (linear)
  - Roughness (linear) → in Unity this becomes **Smoothness**, so you usually **invert roughness**
  - AO (linear, optional)
- Prefer baking from high poly to low poly when needed (Cycles bake), but keep output maps simple and engine-oriented.

## Rigging for Unity

- Prefer a clean humanoid structure when using Unity Humanoid:
  - consistent left/right naming
  - clean hierarchy (root → hips/pelvis → spine → limbs)
  - neutral bind pose (T-pose/A-pose) without constraints driving the final exported bones
- Skinning: keep weights clean; avoid excessive influences per vertex unless necessary.

## Export choice

- Default: **FBX** for widest Unity compatibility (rigs/animations, colliders, LOD naming).
- Use **glTF** when you need more predictable PBR material transfer and are not relying on FBX-only features.

## Unity import expectations

- Don’t fight import settings with “mystery scale”:
  - fix scale/orientation in Blender first
- Ensure normals/tangents import correctly:
  - if seams show with normal maps, revisit smoothing/tangents strategy

## Project-specific workflow

- Keep source `.blend` files out of `Assets/` (store them outside the Unity project, or in a dedicated non-imported source folder if we add one later).
- **Evaverse convention (current repo)**:
  - Generated / authored world geometry assets live under `Assets/_Project/Art/Generated/`
  - Materials live under `Assets/_Project/Materials/` (example: `Assets/_Project/Materials/World/`)
  - Scenes/configs live under `Assets/_Project/Scenes/` and `Assets/_Project/Settings/`
- Until we add an explicit “import staging” directory, export finalized engine-ready meshes into the appropriate `Assets/_Project/Art/...` subtree and keep textures next to their material grouping under `Assets/_Project/Materials/...` (or a `Textures/` sibling we introduce intentionally).

## Additional references

- See [reference.md](reference.md) for export checklists and Unity URP map conventions.

