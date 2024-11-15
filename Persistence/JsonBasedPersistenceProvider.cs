using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;

namespace ASRR.Core.Persistence
{
    public class JsonBasedPersistenceProvider : IPersistentStorageProvider
    {
        public const string ProgramDataPath = @"C:\ProgramData\ASRR\Storage";
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly string _path;

        public JsonBasedPersistenceProvider(string directory)
        {
            _path = Path.Combine(ProgramDataPath, directory);
            Directory.CreateDirectory(_path);
        }

        public T Fetch<T>() where T : class, new()
        {
            var filePath = FilePath<T>();
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, JsonConvert.SerializeObject(new T(), Formatting.Indented));
                Log.Info("Settings don't exist... creating new file");
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));

            // Attempt to override with environment variables
            OverrideWithEnvironmentVariables(deserializedObject);

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

        public void Open<T>() where T : class
        {
            var filePath = FilePath<T>();
            Log.Info($"Opening file at path '{filePath}'");
            System.Diagnostics.Process.Start(@filePath);
        }

        private string FilePath<T>() where T : class
        {
            return Path.Combine(_path, $"{FileName<T>()}.json");
        }

        private string FileName<T>() where T : class
        {
            return typeof(T).Name;
        }

        private bool HasNullProperties<T>(T obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Select(x => x.GetValue(obj, null))
                .Any(y => y == null);
        }

        private void OverrideWithEnvironmentVariables<T>(T obj)
        {
            Log.Info("Attempting to override properties with environment variables");
            if (obj == null) return;

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    // Only process properties of type string or number
                    if (property.PropertyType == typeof(string) ||
                        property.PropertyType.IsPrimitive ||
                        property.PropertyType == typeof(decimal))
                    {
                        var envVarName = ConvertToCamelCaseUpper(property.Name);
                        var envVarValue = Environment.GetEnvironmentVariable(envVarName);
                        Log.Info($"Checking environment variable '{envVarName}' for property '{property.Name}'");
                        Log.Info($"Value: {envVarValue}");


                        if (!string.IsNullOrEmpty(envVarValue))
                        {
                            try
                            {
                                object convertedValue;
                                if (property.PropertyType == typeof(string))
                                {

                                    convertedValue = envVarValue;
                                }
                                else
                                {
                                    convertedValue = Convert.ChangeType(envVarValue, property.PropertyType);
                                }

                                property.SetValue(obj, convertedValue);
                                Log.Info($"Property '{property.Name}' overridden with environment variable value.");
                            }
                            catch (Exception ex)
                            {
                                Log.Warn($"Failed to convert environment variable value for property '{property.Name}': {ex.Message}");
                            }
                        }
                    }
                }
            }
        }

        private string ConvertToCamelCaseUpper(string propertyName)
        {
            return string.Concat(propertyName.Select((x, i) => char.IsUpper(x) && i > 0 ? "_" + x : char.ToUpper(x).ToString()));
        }
    }
}