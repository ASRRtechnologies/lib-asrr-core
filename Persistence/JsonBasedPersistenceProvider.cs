using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NLog;

namespace ASRR.Core.Persistence
{
    public class JsonBasedPersistenceProvider : IPersistentStorageProvider
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        private const string ProgramDataPath = @"C:\ProgramData\ASRR\Storage";
        private readonly string _path;

        public JsonBasedPersistenceProvider(string directory)
        {
            _path = Path.Combine(ProgramDataPath, directory);
            Directory.CreateDirectory(_path);
        }
        
        public T Fetch<T>() where T : class
        {
            var filePath =Path.Combine(_path, FileName<T>());
            if (!File.Exists(filePath)) 
                throw new FileNotFoundException($"File at path '{filePath}' does not exist");

            var deserializedObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));

            if (HasNullProperties(deserializedObject))
                Log.Warn($"File at path '{filePath}' contains null properties");
            
            return deserializedObject;
        }

        public bool Persist<T>(T toStore) where T : class
        {
            var filePath = FilePath<T>();
            Log.Info($"Persisting {typeof(T).Name} to file at path '{filePath}'");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(toStore, Formatting.Indented));
            return true;
        }

        private string FilePath<T>() where T : class
        {            
            return Path.Combine(_path, $"{FileName<T>()}.json");
        }

        private string FileName<T>() where T : class
        {
            return nameof(T);
        }
        
        private bool HasNullProperties<T>(T obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Select(x => x.GetValue(obj, null))
                .Any(y => y == null);
        }
    }
}