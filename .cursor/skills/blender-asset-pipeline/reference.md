# Blender Asset Pipeline reference (Evaverse)

## Quick export checklist (FBX)

- Apply transforms (scale 1,1,1)
- Correct forward/up orientation for Unity (verify by importing a test cube first)
- Export only what you need (selection-only)
- Ensure mesh names are stable and match expected prefab naming
- If exporting animations:
  - bake actions as needed
  - keep a consistent armature/root setup

## Evaverse folder targets (current repo)

- World art (generated/authored): `Assets/_Project/Art/Generated/World/`
- World materials: `Assets/_Project/Materials/World/`

## PBR map conventions for Unity URP (practical)

- BaseColor: sRGB
- Normal: mark as Normal Map in Unity importer
- Metallic: linear
- Roughness → Smoothness:
  - Unity smoothness is usually \(1 - roughness\)
  - decide whether smoothness lives in alpha channel depending on your material workflow
- AO: linear (optional)

## Colliders (authoring guidance)

- Simple colliders are best:
  - boxes, capsules, spheres where possible
  - avoid high-poly mesh colliders for runtime objects
- If using FBX naming-based colliders, keep them separate and clearly named (`UCX_*`).

## LODs

- Keep silhouette fidelity in LOD0
- Aggressively remove interior faces and tiny details in lower LODs
- Ensure all LOD meshes share the same pivot/origin

