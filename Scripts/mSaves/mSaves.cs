using System.Collections.Generic;

namespace mFramework.Saves
{
    public static class mSaves
    {
        private static readonly Dictionary<string, Saveable> _saveables;

        static mSaves()
        {
            _saveables = new Dictionary<string, Saveable>();
        }

        public static void Save()
        {
            foreach (var pair in _saveables)
            {
                pair.Value.Save();
            }
        }

        public static void Load()
        {
            foreach (var pair in _saveables)
            {
                pair.Value.Load();
            }
        }

        public static void Add(IEnumerable<Saveable> saveables)
        {
            foreach (var saveable in saveables)
            {
                _saveables.Add(saveable.Key, saveable);
            }
        }

        public static void Add(Saveable saveable)
        {
            _saveables.Add(saveable.Key, saveable);
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
