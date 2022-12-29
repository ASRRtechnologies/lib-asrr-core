namespace ASRR.Core.Persistence
{
    public interface IPersistentStorageProvider
    {
<<<<<<< HEAD
        T Fetch<T>() where T : class, new();
        
=======
        T Fetch<T>() where T : class;

>>>>>>> f1eb5d85cf9b49e1e75e75055915f18ce41d774c
        bool Persist<T>(T toStore) where T : class;
    }
}