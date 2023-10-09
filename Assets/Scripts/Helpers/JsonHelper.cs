using System;
using Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace Helpers
{
    public static class JsonHelper
    {
        static JsonHelper()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }
        
        public static bool TryDeserializeObject<T>(string json, out T result)
        {
            bool managedToDeserialize;
            
            try
            {
                result = DeserializeObject<T>(json);
                managedToDeserialize = result != null;
            }
            catch (Exception exception)
            {
                result = default;
                managedToDeserialize = false;
                Debug.LogError($"Failed to deserialize JSON to object of type {typeof(T).Name}. Reason: {exception.Message}. JSON:\n{json}");
            }
            
            return managedToDeserialize;
        }

        public static T DeserializeObject<T>(string json)
        {
            if (json == "null")
            {
                return default;
            }

            var isCollection = ObjectExtensions.IsCollectionType<T>();

            if (!isCollection)
            {
                json = json.TrimStart('[').TrimEnd(']');
            }
            
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeObject<T>(T obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, formatting);;
        }
    }
}