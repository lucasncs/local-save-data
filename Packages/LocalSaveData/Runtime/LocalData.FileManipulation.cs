using System.IO;
using UnityEngine;

namespace Seven.SaveSystem
{
    public sealed partial class LocalData
    {
        public static void Save(bool encrypt)
        {
            if (Data.IsDirty())
                SaveToFile(Settings.FileName, encrypt);
        }

        public static void Save()
        {
            Save(Data.EnableEncryption);
        }

        public static void Load(bool encrypt)
        {
            LoadFromFile(Settings.FileName, encrypt);
        }

        public static void Load()
        {
            Load(Data.EnableEncryption);
        }

        public static void DeleteFile()
        {
            string fileName = Data.EnableEncryption
                ? Settings.FileName + Settings.EncryptionSymbol
                : Settings.FileName;
            
            string filePath = Path.Combine(Settings.FilePath, fileName, Settings.FileExtension);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static void SaveToFile(string fileName, bool encrypt)
        {
            if (fileName.Contains(Settings.EncryptionSymbol))
            {
                fileName = fileName.Replace(Settings.EncryptionSymbol, "");
            }

            string fullName = fileName + Settings.FileExtension;
            string fullCryptoName = fileName + Settings.EncryptionSymbol + Settings.FileExtension;
            string filePath = Settings.FilePath + fullName;
            string cryptoFilePath = Settings.FilePath + fullCryptoName;

            string json = JsonUtility.ToJson(Data);

            if (encrypt)
            {
                if (File.Exists(filePath)) // Delete old file
                {
                    File.Delete(filePath);
                }

                byte[] encryptedData = Settings.Cryptographer.Encrypt(json, Settings.EncryptionKey);
                File.WriteAllBytes(cryptoFilePath, encryptedData);
                OnSaveFinish?.Invoke();
            }
            else
            {
                if (File.Exists(cryptoFilePath)) // Delete old encrypted file
                {
                    File.Delete(cryptoFilePath);
                }

                File.WriteAllText(filePath, json);
                OnSaveFinish?.Invoke();
            }
        }

        private static void LoadFromFile(string fileName, bool encrypt)
        {
            if (fileName.Contains(Settings.EncryptionSymbol))
            {
                fileName = fileName.Replace(Settings.EncryptionSymbol, "");
            }

            string fullName = fileName + Settings.FileExtension;
            string fullCryptoName = fileName + Settings.EncryptionSymbol + Settings.FileExtension;
            string filePath = Settings.FilePath + fullName;
            string cryptoFilePath = Settings.FilePath + fullCryptoName;

            string json;
            bool normalFileExists = File.Exists(filePath);
            bool cryptoFileExists = File.Exists(cryptoFilePath);

            if (normalFileExists) // Not encrypted file exists
            {
                Data.EnableEncryption = false;
                try
                {
                    json = File.ReadAllText(filePath);
                    JsonUtility.FromJsonOverwrite(json, Data);

                    if (encrypt) // Save loaded file as encrypted
                    {
                        Data.EnableEncryption = true;
                        SaveToFile(fileName, true);
                    }
                }
                catch // The file is damaged or encrypted
                {
                    json = Decrypt(filePath); // Try to decrypt
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        JsonUtility.FromJsonOverwrite(json, Data);
                        OnLoadFinish?.Invoke();
                    }
                    else // File is damaged
                        OnLoadError?.Invoke(filePath);
                }
            }

            if (cryptoFileExists) // File is encrypted
            {
                if (normalFileExists && !encrypt) // We don't need encrypted file - delete it
                {
                    File.Delete(cryptoFilePath);
                }
                else
                {
                    Data.EnableEncryption = true;
                    json = Decrypt(cryptoFilePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        JsonUtility.FromJsonOverwrite(json, Data);
                        OnLoadFinish?.Invoke();
                    }
                    else // File is marked as encrypted, but decryption has been failed
                    {
                        try
                        {
                            json = File.ReadAllText(cryptoFilePath); // Try to read it straightforward
                            JsonUtility.FromJsonOverwrite(json, Data);
                            OnLoadFinish?.Invoke();
                        }
                        catch // File is damaged?
                        {
                            OnLoadError?.Invoke(filePath);
                        }
                    }
                }
            }

            if (!normalFileExists && !cryptoFileExists) // No file found, create new empty one
            {
                DeleteAll();
                SaveToFile(fileName, encrypt);
            }
        }

        private static string Decrypt(string filePath)
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            return Settings.Cryptographer.Decrypt(encryptedData, Settings.EncryptionKey);
        }
    }
}