using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class Tag
{
    [SerializeField] private string name;

    public string Name
    {
        get => name;
        set => name = value;
    }

    public Tag(string name)
    {
        this.name = name;
    }

    public bool IsTagged => !string.IsNullOrEmpty(name) && name != "Untagged";

    public static bool operator ==(Tag tag, string tagName) => tag.name == tagName;
    public static bool operator ==(string name, Tag tag) => name == tag.name;

    public static bool operator !=(Tag tag, string tagName) => tag.name != tagName;
    public static bool operator !=(string name, Tag tag) => tag.name != name;

    public bool Equals(Tag other) => name == other.name;
    public override bool Equals(object obj) => obj is Tag other && Equals(other);

    public override int GetHashCode() => (name != null ? name.GetHashCode() : 0);

    public override string ToString() => name;

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Tag))]
    private class MyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative("name");

            var tag = EditorGUI.TagField(position, label, prop.stringValue);

            prop.stringValue = tag;
        }
    }
#endif
}
