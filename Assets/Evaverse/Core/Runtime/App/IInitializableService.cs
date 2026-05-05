namespace Evaverse.Core.Runtime.App
{
    public interface IInitializableService
    {
        void Initialize();
        void Shutdown();
    }
}
