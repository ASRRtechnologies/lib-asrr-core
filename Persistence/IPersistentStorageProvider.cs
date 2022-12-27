namespace ASRR.Core.Persistence
{
    public interface IPersistentStorageProvider
    {
        T Fetch<T>() where T : class, new();
        
        bool Persist<T>(T toStore) where T : class;
    }
}