using System.Threading.Tasks;
using Evaverse.Core.Runtime.App;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Evaverse.Networking.Runtime
{
    /// <summary>
    /// Ensures Unity Services are initialized and an auth session exists.
    /// Keep this lightweight; gameplay systems should depend on higher-level services instead.
    /// </summary>
    [DefaultExecutionOrder(-200)]
    [DisallowMultipleComponent]
    public sealed class UnityServicesBootstrap : MonoBehaviour
    {
        private static Task initTask;

        private void Awake()
        {
            initTask ??= InitializeAsync();
        }

        public static Task EnsureInitializedAsync()
        {
            initTask ??= InitializeAsync();
            return initTask;
        }

        private static async Task InitializeAsync()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                EvaLog.Info("Unity Services initialized.");
            }
            catch (System.Exception ex)
            {
                EvaLog.Error($"Unity Services init failed: {ex.Message}");
            }
        }
    }
}

