﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Seven.SaveSystem
{
    [Serializable]
    public class Data<T> : ISerializationCallbackReceiver, IData where T : IEquatable<T>
    {
        private readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();
        [SerializeField] private string[] _keys = new string[0];
        [SerializeField] private T[] _values = new T[0];
        public int Length => _keys.Length;
        public int Count => _dictionary.Count;
        public Type Type { get; } = typeof(T);
        public bool IsDirty { get; private set; }

        public T GetValue(string key, T defaultValue)
        {
            return _dictionary.GetValueOrDefault(key, defaultValue);
        }

        public object GetValue(string key, object defaultValue)
        {
            return _dictionary.TryGetValue(key, out T value) ? value as object : defaultValue;
        }

        public T SetValue(string key, T newValue)
        {
            if (_dictionary.TryGetValue(key, out T value))
            {
                if (value.Equals(newValue))
                {
                    return value;
                }

                _dictionary[key] = newValue;
            }
            else
            {
                _dictionary.Add(key, newValue);
            }

#if UNITY_EDITOR
            Debug.Log($"Local Data || Set Value ({Type.Name}) [ \"{key}\" = {newValue} ]");
#endif

            IsDirty = true;
            return newValue;
        }

        public object SetValue(string key, object newValue)
        {
            T convertedValue = (T)newValue;
            if (_dictionary.TryGetValue(key, out T value))
            {
                if (value.Equals(convertedValue))
                {
                    return newValue;
                }

                _dictionary[key] = convertedValue;
            }
            else
            {
                _dictionary.Add(key, convertedValue);
            }

#if UNITY_EDITOR
            Debug.Log($"Local Data || Set Value ({Type.Name}) [ \"{key}\" = {convertedValue} ]");
#endif

            IsDirty = true;
            return newValue;
        }

        public string[] AllKeys(Type type)
        {
            return Type == type ? _dictionary.Keys.ToArray() : null;
        }

        public List<object> AllValues(Type type)
        {
            if (type != Type) return null;

            var allValues = new List<object>(_dictionary.Count);
            foreach (T val in _dictionary.Values)
            {
                allValues.Add(val);
            }

            return allValues;
        }

        public string ChangeKey(string oldKey, string newKey)
        {
            if (!_dictionary.Remove(oldKey, out T value)) return oldKey;

            _dictionary.Add(newKey, value);
            IsDirty = true;
            return newKey;
        }

        public string KeyByValue(object value)
        {
            string key = default;
            T other = (T)value;
            foreach (var pair in _dictionary)
            {
                if (pair.Value.Equals(other))
                {
                    key = pair.Key;
                    break;
                }
            }

            return key;
        }

        public List<string> KeysByValue(object value)
        {
            List<string> list = new List<string>();
            T Value = (T)value;
            foreach (var pair in _dictionary)
            {
                if (pair.Value.Equals(Value))
                {
                    list.Add(pair.Key);
                }
            }

            return list;
        }

        public bool RemoveKey(string key)
        {
            return Remove(key);
        }

        public void RemoveKeys(List<string> keysList)
        {
            foreach (string key in keysList)
            {
                Remove(key);
            }
        }

        private void Add(string name, T value)
        {
            _dictionary.Add(name, value);
            IsDirty = true;
        }

        private bool Remove(string key)
        {
            bool hasRemoved = _dictionary.Remove(key);
            IsDirty = hasRemoved;
            return hasRemoved;
        }

        public bool TryGetValue(string key, out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(T value)
        {
            return _dictionary.ContainsValue(value);
        }

        public bool ContainsValue(object value)
        {
            return value.GetType() == Type && _dictionary.ContainsValue((T)value);
        }

        public void DeleteAll()
        {
            _dictionary.Clear();
            _keys = new string[0];
            _values = new T[0];
            IsDirty = true;
        }

        // Unload from dictionary to array
        public void OnBeforeSerialize()
        {
            if (_dictionary.Count == 0) return;

            _keys = new string[_dictionary.Count];
            _values = new T[_dictionary.Count];
            int i = 0;
            foreach (var kvp in _dictionary)
            {
                _keys[i] = kvp.Key;
                _values[i] = kvp.Value;
                i++;
            }

            IsDirty = false;
        }

        // Load items to dictionary
        public void OnAfterDeserialize()
        {
            _dictionary.Clear();
            for (var i = 0; i < _keys.Length; i++)
            {
                _dictionary.Add(_keys[i], _values[i]);
            }
        }
    }
}