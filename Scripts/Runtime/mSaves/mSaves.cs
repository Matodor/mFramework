// ReSharper disable InlineOutVariableDeclaration
using mFramework.Storage;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.Saves
{
    public static class mSaves
    {
        public const int SavesVersion = 1;
        public const string SavesVersionKey = "mSaves_version";

        private static readonly Dictionary<string, Saveable> _saveables;

        static mSaves()
        {
            _saveables = new Dictionary<string, Saveable>();
        }

        public static void Save()
        {
            KeyValueStorage.Instance.SetValue(SavesVersionKey, SavesVersion);

            foreach (var pair in _saveables)
            {
                pair.Value.Save();
            }
        }

        public static bool Load()
        {
            if (!mStorage.Loaded)
            {
                Debug.LogWarning("[mSaves] mStorage not loaded, use method mStorage.Load first");
                return false;
            }

            int savesVersion;
            if (KeyValueStorage.Instance.GetValue(SavesVersionKey, out savesVersion))
                Debug.Log($"[mSaves] SavesVersion={savesVersion}");

            foreach (var pair in _saveables)
            {
                pair.Value.Load();
            }

            return true;
        }

        public static void Add(IEnumerable<Saveable> saveables)
        {
            foreach (var saveable in saveables)
            {
                _saveables.Add(saveable.SaveKey, saveable);
            }
        }

        public static void Add(Saveable saveable)
        {
            _saveables.Add(saveable.SaveKey, saveable);
        }

        public static void Clear()
        {
            _saveables.Clear();
        }

        public static bool Remove(Saveable saveable)
        {
            return _saveables.Remove(saveable.SaveKey);
        }

        public static void Reset()
        {
            foreach (var pair in _saveables)
            {
                pair.Value.OnReset();
            }
        }

        public static void Reload()
        {
            foreach (var pair in _saveables)
            {
                pair.Value.Load();
            }
        }
    }
}
