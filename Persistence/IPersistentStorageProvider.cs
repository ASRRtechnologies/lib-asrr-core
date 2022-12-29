namespace ASRR.Core.Persistence
{
    public interface IPersistentStorageProvider
    {
        T Fetch<T>() where T : class;

        bool Persist<T>(T toStore) where T : class;
    }
}