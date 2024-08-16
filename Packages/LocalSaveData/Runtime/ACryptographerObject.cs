using UnityEngine;

namespace Seven.SaveSystem
{
    public abstract class ACryptographerObject : ScriptableObject, ICryptographer
    {
        public abstract string Decrypt(byte[] soup, string key);

        public abstract byte[] Encrypt(string original, string key);
    }
}