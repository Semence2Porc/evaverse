using Evaverse.Core.Runtime.App;
using Evaverse.Gameplay.Runtime.Avatar;
using Evaverse.Gameplay.Runtime.Hoverboard;
using Evaverse.Meta.Runtime.Saves;
using Evaverse.Networking.Runtime.Sessions;
using UnityEngine;

namespace Evaverse.UI.Runtime.Debug
{
    public sealed class PlaytestOverlay : MonoBehaviour
    {
        [SerializeField] private AvatarMotor avatarMotor;
        [SerializeField] private HoverboardMotor hoverboardMotor;
        [SerializeField] private bool visible = true;

        private GUIStyle labelStyle;

        private void Awake()
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                richText = true
            };
        }

        private void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            Rect area = new Rect(16f, 16f, 520f, 220f);
            GUILayout.BeginArea(area, GUI.skin.box);
            GUILayout.Label("<b>Evaverse Playtest Overlay</b>", labelStyle);
            GUILayout.Label($"Profile Path: {PlayerProfileStore.GetProfilePath()}", labelStyle);

            if (avatarMotor != null)
            {
                GUILayout.Label($"Avatar Move: {avatarMotor.MoveInput}", labelStyle);
                GUILayout.Label($"Avatar Sprint: {avatarMotor.IsSprinting}", labelStyle);
            }

            if (hoverboardMotor != null)
            {
                GUILayout.Label($"Hoverboard Speed: {hoverboardMotor.CurrentSpeed:F1}", labelStyle);
            }

            if (ServiceRegistry.TryResolve<ISessionService>(out var sessionService))
            {
                GUILayout.Label($"Session Connected: {sessionService.IsConnected}", labelStyle);
                GUILayout.Label($"Session Hosting: {sessionService.IsHosting}", labelStyle);
            }

            GUILayout.EndArea();
        }
    }
}
