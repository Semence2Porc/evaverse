---
name: unity-architecture
description: Senior Unity architecture patterns for scalable gameplay code. Use when adding new Unity systems, designing gameplay features, structuring folders/namespaces, or refactoring MonoBehaviour-heavy code. Focuses on separation of concerns, data-driven design, deterministic update flow, ScriptableObject configs, and testable service boundaries compatible with asmdefs.
---

# Unity Architecture (Evaverse)

## Default patterns (use unless there’s a strong reason not to)

- **Data-driven configs**: use `ScriptableObject` for tunables, IDs, and content catalogs; avoid hardcoded magic numbers in behaviours.
- **Thin `MonoBehaviour`**: behaviours should mostly bind Unity lifecycle/events to pure C# “domain” classes.
- **Explicit composition root**: keep a small set of bootstraps that register services (e.g. `ServiceRegistry`) and wire scene-level objects.
- **Stable IDs**: use explicit string/Guid-like IDs for gameplay content rather than relying on Unity object instance IDs.

## Update loop hygiene

- Prefer **event-driven** state changes (input → command → state → view) over polling.
- Avoid hidden coupling through `Find*` APIs in runtime code; allow it only in Editor-only tooling.
- Keep `Update` / `FixedUpdate` responsibilities clear:
  - `FixedUpdate`: physics & motor integration
  - `Update`: input sampling, camera targets, UI

## Scene and prefab boundaries

- Treat scenes as **wiring** and prefabs as **reusable units**; don’t let scenes become “code containers”.
- For multiplayer, assume objects may be spawned/despawned often; design init/shutdown to be idempotent.

## Assembly definition strategy

- Keep “core domain” assemblies independent of Unity packages where possible.
- Put Editor tools under Editor-only asmdefs; avoid leaking `UnityEditor` into runtime assemblies.

## Verification expectations

- After adding/refactoring a system, run the Unity batch compile step (see `unity-workflow`) and ensure the first error is fixed before continuing.

