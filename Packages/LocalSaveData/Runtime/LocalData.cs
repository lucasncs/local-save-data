using System;
using Seven.SaveSystem.Settings;
using UnityEngine;

namespace Seven.SaveSystem
{
    public sealed partial class LocalData
    {
        private static bool _isInitialized;

        private static SaveData _data;

        private static SaveData Data
        {
            get
            {
                if (!_isInitialized)
                    Initialize();

                return _data;
            }
        }

        private static LocalDataSettings Settings => LocalDataSettings.GetInstance();

        // Callbacks
        /// <summary>Give the file path</summary>
        public static event Action<string> OnLoadError;
        public static event Action OnLoadFinish;
        public static event Action OnSaveFinish;

        private static void Initialize()
        {
#if UNITY_EDITOR
            OnSaveFinish += () => Debug.Log("Local Data || Save Finished");
            OnLoadFinish += () => Debug.Log("Local Data || Load Finished");
            OnLoadError += path => Debug.Log($"Local Data || Load Error :: \"{path}\"");
#endif

            _data = new SaveData();
            _isInitialized = true;
            LoadFromFile(Settings.FileName, _data.EnableEncryption);

            if (Application.isPlaying && _data.AutoSave)
            {
                Application.focusChanged += SaveOnFocusChange;
                Application.wantsToQuit += SaveOnQuit;
            }
        }

        private static void SaveOnFocusChange(bool hasFocus)
        {
            if (!hasFocus)
            {
                Save();
            }
        }

        private static bool SaveOnQuit()
        {
            Save();
            return true;
        }
    }
}