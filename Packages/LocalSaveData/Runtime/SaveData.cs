using System;
using System.Collections.Generic;
using System.Linq;

namespace Seven.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public bool EnableEncryption = true;
        public bool AutoSave = true;

        public Dictionary<Type, IData> DataTypesDictionary { get; } = new Dictionary<Type, IData>();

        public DataString Strings = new DataString();
        public DataBool Bools = new DataBool();
        public DataInt Ints = new DataInt();
        public DataFloat Floats = new DataFloat();
        public DataVector2 Vector2 = new DataVector2();
        public DataVector3 Vector3 = new DataVector3();

        public SaveData()
        {
            AddData(Strings);
            AddData(Bools);
            AddData(Ints);
            AddData(Floats);
            AddData(Vector2);
            AddData(Vector3);
        }

        public bool IsDirty()
        {
            return DataTypesDictionary.Values.Any(data => data.IsDirty);
        }

        private void AddData(IData data)
        {
            DataTypesDictionary.Add(data.Type, data);
        }
    }
}