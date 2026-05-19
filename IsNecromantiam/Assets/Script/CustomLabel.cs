using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CustomLabel : PropertyAttribute
{
    public readonly GUIContent label;

    public CustomLabel(string label) => this.label = new GUIContent(label);
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CustomLabel))]
public class CustomLabelDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CustomLabel newLabel = attribute as CustomLabel;

        if (newLabel != null) label = newLabel.label;

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
