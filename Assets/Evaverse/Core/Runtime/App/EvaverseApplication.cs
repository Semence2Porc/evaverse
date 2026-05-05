using UnityEngine;
using UnityEngine.SceneManagement;

namespace Evaverse.Core.Runtime.App
{
    public sealed class EvaverseApplication : MonoBehaviour
    {
        public static EvaverseApplication Instance { get; private set; }

        [SerializeField] private bool persistAcrossScenes = true;
        [SerializeField] private string initialGameplayScene = "Hub";

        public bool IsBootstrapped { get; private set; }
        public string InitialGameplayScene => initialGameplayScene;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }

            IsBootstrapped = true;
            EvaLog.Info("Application bootstrap complete.", this);
        }

        public void LoadInitialGameplayScene()
        {
            if (string.IsNullOrWhiteSpace(initialGameplayScene))
            {
                EvaLog.Warning("No initial gameplay scene has been configured.", this);
                return;
            }

            SceneManager.LoadScene(initialGameplayScene);
        }
    }
}
