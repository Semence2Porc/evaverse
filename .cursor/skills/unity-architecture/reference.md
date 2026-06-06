# Unity Architecture reference (Evaverse)

## When to introduce a service

Create a service when:
- multiple systems need the same state/operations (sessions, player profile, wallet, matchmaking)
- it must outlive a scene
- it needs to be testable without Unity scene context

Avoid services for:
- one-off scene wiring
- direct view logic (HUD rendering, camera shake) unless it’s a reusable subsystem

## ScriptableObject usage

Prefer ScriptableObjects for:
- tuning values
- content definitions (items, races, maps)
- ID-based lookups / catalogs

Avoid ScriptableObjects as “service singletons” with hidden mutable state.

## Multiplayer readiness checklist (architecture level)

- init and shutdown are safe to call multiple times
- no reliance on scene order beyond explicit bootstrap
- avoid static state unless it’s truly global and resettable

