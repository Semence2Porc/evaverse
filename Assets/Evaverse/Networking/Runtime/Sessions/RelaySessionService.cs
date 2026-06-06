using System;
using System.Threading.Tasks;
using Evaverse.Core.Runtime.App;
using Evaverse.Networking.Runtime.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Evaverse.Networking.Runtime.Sessions
{
    public sealed class RelaySessionService : ISessionService, IInitializableService
    {
        private readonly NetcodeBootstrap bootstrap;
        private readonly MonoBehaviour coroutineHost;
        private ISession activeSession;

        public RelaySessionService(NetcodeBootstrap bootstrap, MonoBehaviour coroutineHost)
        {
            this.bootstrap = bootstrap;
            this.coroutineHost = coroutineHost;
        }

        public SessionBackend Backend => SessionBackend.MultiplayerRelay;
        public bool IsHosting { get; private set; }
        public bool IsConnected { get; private set; }
        public string JoinCode { get; private set; } = string.Empty;
        public string StatusMessage { get; private set; } = "Relay idle.";

        public void Initialize()
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                EvaLog.Error(StatusMessage);
                return;
            }

            StatusMessage = "Relay ready. Host or paste join code.";
            EvaLog.Info("Relay session service initialized.");
        }

        public void Shutdown()
        {
            Disconnect();
        }

        public void StartHost(SessionConfig config)
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                return;
            }

            if (coroutineHost == null)
            {
                StatusMessage = "Missing coroutine host for Relay startup.";
                return;
            }

            if (IsConnected)
            {
                Disconnect();
            }

            StatusMessage = "Starting Relay host...";
            coroutineHost.StartCoroutine(RunHost(config));
        }

        public void StartClient(string joinCode)
        {
            if (bootstrap == null)
            {
                StatusMessage = "Missing NetcodeBootstrap.";
                return;
            }

            if (coroutineHost == null)
            {
                StatusMessage = "Missing coroutine host for Relay startup.";
                return;
            }

            if (IsConnected)
            {
                Disconnect();
            }

            StatusMessage = "Joining Relay session...";
            coroutineHost.StartCoroutine(RunJoin(joinCode));
        }

        public void Disconnect()
        {
            if (activeSession != null)
            {
                _ = LeaveSessionAsync(activeSession);
                activeSession = null;
            }

            bootstrap?.ShutdownNetwork();
            IsHosting = false;
            IsConnected = false;
            JoinCode = string.Empty;
            StatusMessage = "Relay idle.";
            EvaLog.Info("Disconnected Relay session.");
        }

        private System.Collections.IEnumerator RunHost(SessionConfig config)
        {
            Task<bool> task = StartRelayHostAsync(config);
            yield return WaitForTask(task);

            if (task.Status != TaskStatus.RanToCompletion || !task.Result)
            {
                yield break;
            }

            IsHosting = true;
            IsConnected = true;
        }

        private System.Collections.IEnumerator RunJoin(string joinCode)
        {
            Task<bool> task = JoinRelayAsync(joinCode);
            yield return WaitForTask(task);

            if (task.Status != TaskStatus.RanToCompletion || !task.Result)
            {
                yield break;
            }

            IsHosting = false;
            IsConnected = true;
        }

        private static System.Collections.IEnumerator WaitForTask(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }

        private async Task<bool> StartRelayHostAsync(SessionConfig config)
        {
            try
            {
                if (!await EnsureServicesInitializedAsync())
                {
                    return false;
                }

                bootstrap.PreparePlayerPrefab();

                SessionOptions options = new SessionOptions
                {
                    MaxPlayers = config.MaxPlayers
                }.WithRelayNetwork();

                activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
                JoinCode = activeSession.Code;
                StatusMessage = $"Relay host ready. Code: {JoinCode}";
                EvaLog.Info($"Relay host created with join code '{JoinCode}'.");
                return true;
            }
            catch (Exception exception)
            {
                JoinCode = string.Empty;
                StatusMessage = $"Relay host failed: {exception.Message}";
                EvaLog.Error(StatusMessage);
                return false;
            }
        }

        private async Task<bool> JoinRelayAsync(string joinCode)
        {
            try
            {
                if (!await EnsureServicesInitializedAsync())
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(joinCode))
                {
                    StatusMessage = "Enter a Relay join code.";
                    return false;
                }

                bootstrap.PreparePlayerPrefab();
                activeSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode.Trim());
                JoinCode = joinCode.Trim();
                StatusMessage = $"Joined Relay session {JoinCode}.";
                EvaLog.Info($"Joined Relay session with code '{JoinCode}'.");
                return true;
            }
            catch (Exception exception)
            {
                StatusMessage = $"Relay join failed: {exception.Message}";
                EvaLog.Error(StatusMessage);
                return false;
            }
        }

        private static async Task<bool> EnsureServicesInitializedAsync()
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

                return true;
            }
            catch (Exception exception)
            {
                StatusMessage = $"Unity Services init failed: {exception.Message}";
                EvaLog.Error(StatusMessage);
                return false;
            }
        }

        private static async Task LeaveSessionAsync(ISession session)
        {
            try
            {
                await session.LeaveAsync();
            }
            catch (Exception exception)
            {
                EvaLog.Warn($"Relay session leave failed: {exception.Message}");
            }
        }
    }
}
