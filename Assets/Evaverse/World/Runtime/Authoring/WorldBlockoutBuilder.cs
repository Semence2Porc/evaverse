using System;
using Evaverse.World.Runtime.Seeds;
using UnityEngine;

namespace Evaverse.World.Runtime.Authoring
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class WorldBlockoutBuilder : MonoBehaviour
    {
        [SerializeField] private TextAsset seedJson;
        [SerializeField] private string generatedRootName = "_GeneratedBlockout";
        [SerializeField] private bool clearBeforeBuild = true;
        [SerializeField] private bool keepColliders = true;

        [ContextMenu("Rebuild From Seed")]
        private void RebuildFromSeedContext()
        {
            if (TryBuild(out string message))
            {
                Debug.Log(message, this);
            }
            else
            {
                Debug.LogWarning(message, this);
            }
        }

        public bool TryBuild(out string message)
        {
            if (!WorldMapSeedUtility.TryParse(seedJson, out WorldMapSeedPayload payload, out string error))
            {
                message = error;
                return false;
            }

            Transform container = GetOrCreateContainer();

            if (clearBeforeBuild)
            {
                ClearContainer(container);
            }

            for (int i = 0; i < payload.blockout.Length; i++)
            {
                BuildElement(container, payload.blockout[i], i);
            }

            string mapName = string.IsNullOrWhiteSpace(payload.metadata.displayName)
                ? payload.metadata.mapId
                : payload.metadata.displayName;

            message = $"Built {payload.blockout.Length} blockout elements for {mapName}.";
            return true;
        }

        private Transform GetOrCreateContainer()
        {
            string childName = string.IsNullOrWhiteSpace(generatedRootName) ? "_GeneratedBlockout" : generatedRootName;
            Transform existing = transform.Find(childName);
            if (existing != null)
            {
                return existing;
            }

            GameObject root = new(childName);
            root.transform.SetParent(transform, false);
            return root.transform;
        }

        private void ClearContainer(Transform container)
        {
            for (int i = container.childCount - 1; i >= 0; i--)
            {
                GameObject child = container.GetChild(i).gameObject;
                if (Application.isPlaying)
                {
                    Destroy(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }

        private void BuildElement(Transform container, WorldMapSeedBlockout element, int index)
        {
            PrimitiveType primitive = ResolvePrimitive(element.primitive);
            GameObject instance = GameObject.CreatePrimitive(primitive);
            instance.name = string.IsNullOrWhiteSpace(element.id) ? $"block-{index:000}" : element.id;
            instance.transform.SetParent(container, false);
            instance.transform.localPosition = element.position;
            instance.transform.localRotation = Quaternion.Euler(element.rotationEuler);
            instance.transform.localScale = ResolveScale(primitive, element.scale);

            if (!keepColliders && instance.TryGetComponent(out Collider collider))
            {
                if (Application.isPlaying)
                {
                    Destroy(collider);
                }
                else
                {
                    DestroyImmediate(collider);
                }
            }
        }

        private static PrimitiveType ResolvePrimitive(string primitiveName)
        {
            if (Enum.TryParse(primitiveName, true, out PrimitiveType primitive))
            {
                return primitive;
            }

            return PrimitiveType.Cube;
        }

        private static Vector3 ResolveScale(PrimitiveType primitive, Vector3 authoredScale)
        {
            Vector3 safeScale = new(
                Mathf.Max(0.01f, authoredScale.x),
                Mathf.Max(0.01f, authoredScale.y),
                Mathf.Max(0.01f, authoredScale.z));

            if (primitive == PrimitiveType.Plane)
            {
                safeScale.x /= 10f;
                safeScale.z /= 10f;
            }

            return safeScale;
        }
    }
}
