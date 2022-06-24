namespace ASRR.Core.Persistence
{
    public class JsonBasedPersistenceProvider : IPersistentStorageProvider
    {
        public T Fetch<T>() where T : class
        {
            throw new System.NotImplementedException();
        }

        public bool Persist<T>(T toStore) where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}