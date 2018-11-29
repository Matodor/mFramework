// ReSharper disable InlineOutVariableDeclaration
using System.Collections.Generic;
using mFramework.Storage;
using UnityEngine;

namespace mFramework.Saves
{
    public static class mSaves
    {
        public static string EncryptPassword { get; set; } = "qQCP#4~[2ss_/7~T";
        public const int SavesVersion = 1;
        public const string SavesVersionKey = "mSaves_version";

        private static readonly Dictionary<string, Saveable> _saveables;

        static mSaves()
        {
            _saveables = new Dictionary<string, Saveable>();
        }

        public static void Save()
        {
            //mStorage.KeyValueStorage.SetValue(SavesVersionKey, SavesVersion);

            foreach (var pair in _saveables)
            {
                pair.Value.Save();
            }
        }

        public static void Load()
        {
            //int savesVersion;
            //if (mStorage.KeyValueStorage.GetValue(SavesVersionKey, out savesVersion))
            //{
            //    Debug.Log($"[mSaves] SavesVersion={savesVersion}");
            //}

            foreach (var pair in _saveables)
            {
                pair.Value.Load();
            }
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

        public static void Remove(Saveable saveable)
        {

        }

        public static void Reset()
        {

        }

        public static void Reload()
        {

        }
    }
}
