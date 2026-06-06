# Unity Workflow reference (Evaverse)

## Common compiler errors → likely fix

- **CS0012** “type is defined in an assembly that is not referenced”
  - Fix: add the missing assembly to the `.asmdef` that owns the failing script.
  - Netcode/UTP frequent pair:
    - `Unity.Netcode.Runtime`
    - `Unity.Networking.Transport`

- **CS0234** “namespace X does not exist in the namespace Y”
  - Fix: the current asmdef does not reference the package/assembly that contains that namespace.
  - Example pattern: Editor-only code uses Cinemachine types → Editor asmdef needs `Unity.Cinemachine`.

## Unity asset hygiene

- New folders in `Assets/` must have `<FolderName>.meta` committed.
- New scripts and asmdefs must have their `.meta` committed.
- Never delete/recreate metas to “fix” issues—use the Editor to reimport and keep GUIDs stable.

## Minimal safe batchmode compile check

- Use the project script:
  - `Tools/RebuildHub.ps1`
- If it fails:
  - open `logs/unity-rebuild-hub.log`
  - find the first `error CSxxxx`
  - fix → re-run until exit code 0

