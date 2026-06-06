# Unity Multiplayer + Netcode reference (Evaverse)

## Common mistakes to avoid

- Forgetting `.meta` files for prefabs/asmdefs (causes broken prefab links / GUID churn)
- Putting `UnityEditor` APIs in runtime assemblies
- Using `FindObjectOfType` in runtime netcode paths (race conditions on spawn)
- Enabling cameras/UI on remote player instances

## Practical “first checks” when something doesn’t replicate

- Is the object spawned (`NetworkObject.IsSpawned`)?
- Is the script running on the expected side (server vs client vs owner)?
- Does the prefab exist in the network prefab list / is it registered?
- Are you compiling under an asmdef that references all required packages?

