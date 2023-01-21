namespace Vulpes.Pooling
{
    public interface ISpawnable 
    {
        /// <summary>
        /// Called whenever an instance is spawned.
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called whenever an instance is despawned.
        /// </summary>
        void OnDespawn();
    }
}