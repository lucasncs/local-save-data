using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Seven.SaveSystem
{
    public sealed partial class LocalData
    {
        #region Data Manipulation

        // String
        public static string GetString(string key, string defaultValue = default)
        {
            return Data.Strings.GetValue(key, defaultValue);
        }

        public static string SetString(string key, string value = default)
        {
            return Data.Strings.SetValue(key, value);
        }

        // Bool
        public static bool GetBool(string key, bool defaultValue = default)
        {
            return Data.Bools.GetValue(key, defaultValue);
        }

        public static bool SetBool(string key, bool value)
        {
            return Data.Bools.SetValue(key, value);
        }

        // Int
        public static int GetInt(string key, int defaultValue = default)
        {
            return Data.Ints.GetValue(key, defaultValue);
        }

        public static int SetInt(string key, int value)
        {
            return Data.Ints.SetValue(key, value);
        }

        // FLoat
        public static float GetFloat(string key, float defaultValue = default)
        {
            return Data.Floats.GetValue(key, defaultValue);
        }

        public static float SetFloat(string key, float value)
        {
            return Data.Floats.SetValue(key, value);
        }
        
        // Vector2
        public static Vector2 GetVector2(string key, Vector2 defaultValue = default)
        {
            return Data.Vector2.GetValue(key, defaultValue);
        }

        public static Vector2 SetVector2(string key, Vector2 value)
        {
            return Data.Vector2.SetValue(key, value);
        }

        // Vector3
        public static Vector3 GetVector3(string key, Vector3 defaultValue = default)
        {
            return Data.Vector3.GetValue(key, defaultValue);
        }

        public static Vector3 SetVector3(string key, Vector3 value)
        {
            return Data.Vector3.SetValue(key, value);
        }

        /// <summary>Returns true if key exist in data types.</summary>
        public static bool HasKey(string key)
        {
            foreach (IData data in Data.DataTypesDictionary.Values)
            {
                if (data.ContainsKey(key)) return true;
            }

            return false;
        }

        /// <summary>Clear all keys and values from all data types. Use with caution.</summary>
        public static void DeleteAll()
        {
            foreach (IData data in Data.DataTypesDictionary.Values)
            {
                data.DeleteAll();
            }
        }

        /// <summary>Remove key and it's value from data types.</summary>
        public static bool DeleteKey(string key)
        {
            bool removed = false;
            foreach (IData data in Data.DataTypesDictionary.Values)
            {
                if (data.RemoveKey(key))
                {
                    removed = true;
                    break;
                }
            }

            return removed;
        }

        #endregion

        #region Generic functions

        /// <summary>Returns true if key exist in given data.</summary>
        public static bool HasKey<T>(string key)
        {
            Type t = typeof(T);
            bool isSupported = false;
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                isSupported = true;
                if (data.ContainsKey(key))
                    return true;
            }

            if (!isSupported)
                Debug.LogError(TypeIsNotSupported("HasKey<T>", t));
            return false;
        }

        public static bool TryGetValue<T>(string key, out T value)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                bool containsKey = data.ContainsKey(key);
                value = containsKey ? (T)data.GetValue(key) : default;
                return containsKey;
            }

            value = default;
            return false;
        }

        /// <summary>Remove key and it's value from given data.</summary>
        public static bool DeleteKey<T>(string key)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                if (data.RemoveKey(key))
                {
                    data.RemoveKey(key);
                    return true;
                }
            }

            Debug.LogError(TypeIsNotSupported("RemoveKey<T>", t));
            return false;
        }

        /// <summary>Clear all keys and values from given data. Use with caution.</summary>
        public static void DeleteAll<T>()
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                data.DeleteAll();
                return;
            }

            Debug.LogError(TypeIsNotSupported("ClearAll", t));
        }

        /// <summary>Find key in all data types and change it.</summary>
        public static string ChangeKey(string oldKey, string newKey)
        {
            foreach (IData data in Data.DataTypesDictionary.Values)
            {
                data.ChangeKey(oldKey, newKey);
            }

            return newKey;
        }

        /// <summary>Find key in given data and change it.</summary>
        public static string ChangeKey<T>(string oldKey, string newKey)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                return data.ChangeKey(oldKey, newKey);
            }

            Debug.LogError(TypeIsNotSupported("ChangeKey", t));
            return oldKey;
        }

        /// <summary>Find first key with given value and change it to new one.
        /// <para>This operation is much slower that changing by previous key. Use only if performance is not a consideration.</para></summary>
        public static string ChangeKey<T>(T value, string newKey)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                string key = data.KeyByValue(value);
                return data.ChangeKey(key, newKey);
            }

            Debug.LogError(TypeIsNotSupported("ChangeKey<T>", t));
            return default;
        }

        /// <summary>Look up for one key with given value and removes it.<para />
        /// Returns true if key is found.<para />
        /// This operation is slow, don't use it constantly.</summary>
        public static bool RemoveKeyByValue<T>(T value)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                string key = data.KeyByValue(value);
                if (key != default)
                    data.RemoveKey(key);
                return key != default;
            }

            Debug.LogError(TypeIsNotSupported("RemoveKeyByValue", t));
            return false;
        }

        /// <summary>Look up for keys with given value and remove them.<para /> 
        /// Returns true if at least one key is found.<para />
        /// This operation is slow, don't use it constantly.</summary>
        public static bool RemoveKeysByValue<T>(T value)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                List<string> keys = data.KeysByValue(value);
                data.RemoveKeys(keys);
                return keys.Count > 0;
            }

            Debug.LogError(TypeIsNotSupported("RemoveKeysByValue", t));
            return false;
        }

        /// <summary>Find value in this data by key and set new value to it.</summary>
        public static T Set<T>(string key, T value)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                data.SetValue(key, value);
                return value;
            }

            Debug.LogError($"{ClassName}: Trying to Set not supported type ({t.Name})");
            return default;
        }

        /// <summary>Return value of key in this data.</summary>
        public static T Get<T>(string key, T defaultValue = default)
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                return (T)data.GetValue(key, defaultValue);
            }

            Debug.LogError($"{ClassName}: Trying to Get not supported type ({t.Name})");
            return default;
        }

        /// <summary>Returns the count of keys presented in this data.</summary>
        public static int KeysCount<T>()
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                return data.Count;
            }

            Debug.LogError(TypeIsNotSupported("KeysCount", t));
            return 0;
        }

        /// <summary>Returns every existing key in this data. Consider to use it only once.</summary>
        public static string[] AllKeys<T>()
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                return data.AllKeys(t);
            }

            Debug.LogError(TypeIsNotSupported("GetAllKeys", t));
            return null;
        }

        /// <summary>Returns every existing value of this type. It's a very slow operation, consider to use it only once.</summary>
        public static List<T> AllValues<T>()
        {
            Type t = typeof(T);
            if (Data.DataTypesDictionary.TryGetValue(t, out IData data))
            {
                List<object> tempValues = data.AllValues(t);
                List<T> allValues = new List<T>(tempValues.Count);
                allValues.AddRange(tempValues.Select(v => (T)v));
                return allValues;
            }

            Debug.LogError(TypeIsNotSupported("GetAllValues", t));
            return null;
        }

        #endregion

        private static string TypeIsNotSupported(string methodName, Type t)
            => $"{ClassName} {methodName}: Type \"{t.Name}\" is not supported.";

        private static string ClassName => nameof(LocalData);
    }
}