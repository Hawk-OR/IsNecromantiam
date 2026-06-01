using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using Unity.Properties;



#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [System.Serializable]
    public class Data
    {
        public TKey key = default(TKey);
        public TValue value = default(TValue);

        public Data()
        {
            key = default(TKey);
            value = default(TValue);
        }

        public Data(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public TValue NumValue(int index) { return data[index].value; }

    [SerializeField]
    private List<Data> data = null;

    public ref List<Data> Get() => ref data;
    public Data Get(int i) => data[i];

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        foreach (Data data in data)
        {
            if (ContainsKey(data.key)) continue;
            Add(data.key, data.value);
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {

    }
}
