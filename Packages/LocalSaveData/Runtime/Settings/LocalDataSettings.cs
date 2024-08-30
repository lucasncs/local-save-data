using UnityEngine;

namespace Seven.SaveSystem.Settings
{
    internal class LocalDataSettings : ScriptableObject
    {
        private static LocalDataSettings _instance;

        [Header("File")]
        [SerializeField] private string _fileName = "PlayerData";
        [SerializeField] private string _fileExtension = ".save";
        [SerializeField] private string _filePath;

        [Header("Encryption")]
        [SerializeField] private string _encryptionKey = "t1w6(Dx-:lsW_db^xN(%fCOw8qpK1=|,"; // must be 32 chars
        [SerializeField] private string _encryptionSymbol = "#";
        [SerializeField] private ACryptographerObject _cryptographer;

        public string FileName => _fileName;
        public string FileExtension => _fileExtension;
        public string FilePath => System.IO.Path.Combine(Application.persistentDataPath, _filePath);
        public string EncryptionKey => _encryptionKey;
        public string EncryptionSymbol => _encryptionSymbol;
        public ICryptographer Cryptographer => _cryptographer;

        private void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this;
            }

            if (_cryptographer == null)
            {
                _cryptographer = GetDefaultCryptographer();
            }
        }

        private static AesCrypto GetDefaultCryptographer()
        {
#if UNITY_EDITOR
            foreach (string assetGuid in UnityEditor.AssetDatabase.FindAssets($"t:{typeof(AesCrypto).FullName}"))
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assetGuid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<AesCrypto>(assetPath);
                if (asset != null) return asset;
            }
#endif
            var aesCrypto = FindObjectOfType<AesCrypto>();
            if (aesCrypto == null)
            {
                aesCrypto = CreateInstance<AesCrypto>();
            }

            return aesCrypto;
        }

        public static LocalDataSettings GetInstance()
        {
            if (_instance) return _instance;
#if UNITY_EDITOR
            _instance = Editor.LocalDataSettingsProvider.GetOrCreateSettings();
#else
            _instance = FindObjectOfType<LocalDataSettings>();
#endif
            return _instance;
        }
    }
}