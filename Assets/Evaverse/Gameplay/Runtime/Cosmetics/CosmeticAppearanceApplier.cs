using Evaverse.Web3.Runtime.Cosmetics;
using Evaverse.Web3.Runtime.Traits;
using UnityEngine;

namespace Evaverse.Gameplay.Runtime.Cosmetics
{
    [DisallowMultipleComponent]
    public sealed class CosmeticAppearanceApplier : MonoBehaviour
    {
        [SerializeField] private bool useDemoLoadout = true;

        public void ApplyDemoLoadout()
        {
            if (!useDemoLoadout)
            {
                return;
            }

            Apply(DemoCosmeticFactory.CreateDemoLoadout());
        }

        public void Apply(CosmeticLoadout loadout)
        {
            if (loadout == null)
            {
                return;
            }

            Color avatarAccent = DemoCosmeticFactory.ResolveAvatarAccent(loadout);
            Color boardAccent = DemoCosmeticFactory.ResolveBoardAccent(loadout);

            ApplyTint("avatar-capsule-visual", avatarAccent);
            ApplyTint("avatar-visor", boardAccent);
            ApplyTint("hoverboard-neon-trim", boardAccent);
        }

        private void ApplyTint(string childName, Color color)
        {
            Transform child = FindDeepChild(transform, childName);
            if (child == null || !child.TryGetComponent(out Renderer renderer))
            {
                return;
            }

            Material material = renderer.material;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 1.2f);
            }
        }

        private static Transform FindDeepChild(Transform parent, string childName)
        {
            if (parent.name == childName)
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform found = FindDeepChild(parent.GetChild(i), childName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
