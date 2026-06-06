using System.Collections.Generic;
using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.World.Runtime.Definitions;
using UnityEngine;

namespace Evaverse.UI.Runtime.Hub
{
    /// <summary>
    /// Top-down minimap showing the local player and district portal markers.
    /// </summary>
    public sealed class HubMinimapHud : MonoBehaviour
    {
        [SerializeField] private WorldMapDefinition mapDefinition;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float worldRadius = 520f;
        [SerializeField] private float mapSize = 168f;

        private GUIStyle panelStyle;
        private Texture2D playerIcon;
        private Texture2D portalIcon;

        private void LateUpdate()
        {
            if (playerTransform == null)
            {
                AvatarMotor motor = FindFirstObjectByType<AvatarMotor>();
                if (motor != null)
                {
                    playerTransform = motor.transform;
                }
            }
        }

        private void OnGUI()
        {
            if (playerTransform == null)
            {
                return;
            }

            EnsureStyles();

            Rect panel = new Rect(Screen.width - mapSize - 24f, Screen.height - mapSize - 24f, mapSize, mapSize);
            GUI.Box(panel, GUIContent.none, panelStyle);

            Rect mapArea = new Rect(panel.x + 10f, panel.y + 10f, panel.width - 20f, panel.height - 20f);
            GUI.DrawTexture(mapArea, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f,
                new Color(0.08f, 0.12f, 0.18f, 0.92f), 0f, 0f);

            DrawPlayer(mapArea);
            DrawPortals(mapArea);
        }

        private void DrawPlayer(Rect mapArea)
        {
            Vector2 mapPoint = WorldToMap(playerTransform.position, mapArea);
            const float iconSize = 8f;
            GUI.DrawTexture(
                new Rect(mapPoint.x - iconSize * 0.5f, mapPoint.y - iconSize * 0.5f, iconSize, iconSize),
                playerIcon,
                ScaleMode.StretchToFill);
        }

        private void DrawPortals(Rect mapArea)
        {
            if (mapDefinition == null)
            {
                return;
            }

            IReadOnlyList<WorldPointOfInterestDefinition> points = mapDefinition.PointsOfInterest;
            for (int i = 0; i < points.Count; i++)
            {
                WorldPointOfInterestDefinition point = points[i];
                if (point.PointType != WorldPointOfInterestType.Portal)
                {
                    continue;
                }

                Vector2 mapPoint = WorldToMap(point.Position, mapArea);
                const float iconSize = 6f;
                GUI.DrawTexture(
                    new Rect(mapPoint.x - iconSize * 0.5f, mapPoint.y - iconSize * 0.5f, iconSize, iconSize),
                    portalIcon,
                    ScaleMode.StretchToFill);
            }
        }

        private Vector2 WorldToMap(Vector3 worldPosition, Rect mapArea)
        {
            float normalizedX = Mathf.Clamp(worldPosition.x / worldRadius, -1f, 1f);
            float normalizedZ = Mathf.Clamp(worldPosition.z / worldRadius, -1f, 1f);
            float x = mapArea.x + mapArea.width * (normalizedX * 0.5f + 0.5f);
            float y = mapArea.y + mapArea.height * (1f - (normalizedZ * 0.5f + 0.5f));
            return new Vector2(x, y);
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            Texture2D panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, new Color(0.02f, 0.04f, 0.07f, 0.88f));
            panelTexture.Apply();
            panelStyle = new GUIStyle(GUI.skin.box) { normal = { background = panelTexture } };

            playerIcon = CreateIcon(new Color(0.2f, 0.95f, 1f));
            portalIcon = CreateIcon(new Color(1f, 0.62f, 0.2f));
        }

        private static Texture2D CreateIcon(Color color)
        {
            Texture2D texture = new Texture2D(4, 4);
            Color[] pixels = new Color[16];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
    }
}
