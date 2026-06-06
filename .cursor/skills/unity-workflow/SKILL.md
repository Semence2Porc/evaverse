---
name: unity-workflow
description: Unity project workflow guardrails for C# gameplay/editor code. Use when editing Unity scripts, creating or changing .asmdef files, adding folders/assets that require .meta files, working with Netcode for GameObjects/UTP, writing Editor tooling, or running Unity batchmode/CI compilation checks. Enforces: commit .meta, keep asmdef references complete, prefer batchmode compile verification, and avoid optional Unity modules/packages that are disabled in this project.
---

# Unity Workflow (Evaverse)

## Always-do rules

- **Commit `.meta` files** for any new/changed asset/script/asmdef/folder; never leave Unity to regenerate GUIDs.
- **When adding a script**, confirm which assembly it compiles under (asmdef scope) and ensure that asmdef has all needed references (Unity packages + other project assemblies).
- **Treat Unity batchmode compilation as truth**: run a batch compile/build step after substantive changes and fix the first error fully before continuing.

## Assembly Definition (`.asmdef`) checklist

When a compile error mentions:

- **`CS0012` “type is defined in an assembly that is not referenced”**  
  - Add the missing assembly name to the relevant `.asmdef` `references` list.
  - Common Netcode/Transport case: `Unity.Netcode.Runtime` is not enough—**`Unity.Networking.Transport`** may also be required for UTP types.

- **Namespace exists in one asmdef but not another** (Editor assemblies especially)  
  - Add the missing package assembly to that asmdef’s `references` (e.g. Cinemachine usage in an Editor asmdef requires `Unity.Cinemachine`).

## Package/module sensitivity (this repo)

- Assume **optional Unity modules may be disabled** (example encountered: Audio).  
  - Avoid hard dependencies on optional modules in shared gameplay/editor tooling unless the project explicitly uses them.
  - Prefer patterns that still compile if the module is absent (e.g. don’t require `AudioListener` for prototype cameras).

## Netcode for GameObjects / UTP workflow

- Prefer a dedicated runtime bootstrap component that:
  - Ensures `NetworkConfig` exists
  - Wires `UnityTransport`
  - Registers player prefab (explicit or Resources fallback)
- For editor-side prefab baking:
  - Put code under an Editor-only asmdef (`includePlatforms: ["Editor"]`)
  - Reference the runtime assemblies it depends on (`Evaverse.*`, `Unity.Netcode.*`, plus any other packages used by the baked components).

## Batchmode verification loop

- Default verification command in this repo: run `Tools/RebuildHub.ps1` (batchmode, `-executeMethod`).
- If batchmode fails with compiler errors:
  - Read the log and fix the **first** compiler error
  - Re-run batchmode until exit code is 0

## Additional references

- See [reference.md](reference.md) for common error → fix mappings and “don’t forget” items.

