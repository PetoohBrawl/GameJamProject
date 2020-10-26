using System;

namespace SimpleJson 
{
    public static class SimpleJsonExtensions 
    {
        /// 
        public static int GetInt(this JsonObject obj, string key)
        {
            return (int)(long)obj[key];
        }

        /// <summary>
        /// Возвращает int значение из JsonObject, либо дефолтное, если такого ключа нет
        /// </summary>
        public static int GetInt(this JsonObject obj, string key, int defaultValue)
        {
            if (obj.ContainsKey(key))
            {
                return obj.GetInt(key);
            }

            return defaultValue;
        }

        ///
        public static float GetFloat(this JsonObject obj, string key) 
        {
            return Convert.ToSingle(obj[key]);
        }
        
        
        /// <summary>
        /// Возвращает float значение из JsonObject, либо дефолтное, если такого ключа нет
        /// </summary>
        public static float GetFloat(this JsonObject obj, string key, float defaultValue)
        {
            return obj.ContainsKey(key) ? obj.GetFloat(key) : defaultValue;
        }

        ///////////////
        public static bool GetBool(this JsonObject obj, string key)
        {
            return (bool)obj[key];
        }
        
        ///////////////
        public static bool GetBool(this JsonObject obj, string key, bool defaultValue)
        {
            return obj.ContainsKey(key) ? obj.GetBool(key) : defaultValue;
        }

        ///////////////
        public static string GetString(this JsonObject obj, string key, string defaultValue)
        {
            return obj.ContainsKey(key) ? (string)obj[key] : defaultValue;
        }

        ///
        public static double GetDouble(this JsonObject obj, string key, double defaultValue)
        {
            if (!obj.ContainsKey(key))
                return defaultValue;

            return obj.GetDouble(key);
        }

        ///
        public static double GetDouble(this JsonObject obj, string key)
        {
            if (obj[key] is double)
                return (double)obj[key];
            
            return Convert.ToDouble(obj[key]);
        }

        /// <summary>
        /// Приводит вложенный объект к указанному типу 
        /// </summary>
        public static T Get<T>(this JsonObject obj, string key)
        {
            obj.TryGetValue(key, out object result);
            return (T)result;
        }

        /// <summary>
        /// Приводит объект в массиве к указанному типу
        /// </summary>
        public static T GetAt<T>(this JsonArray obj, int index)
        {
            return (T)obj[index];
        }
        
        ///////////////
        public static T GetEnum<T>(this JsonObject obj, string key, T defaultValue, bool ignoreCase = true) where T : struct 
        {
            try
            {
                T t = (T)Enum.Parse(typeof(T), key, ignoreCase);

                if (!Enum.IsDefined(typeof(T), t))
                {
                    return defaultValue;
                }

                return t;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}