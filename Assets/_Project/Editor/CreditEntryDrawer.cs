using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CreditEntry))]
public class CreditEntryDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var nameProp = property.FindPropertyRelative("name");
        string entryName = nameProp != null ? nameProp.stringValue : "Unnamed";

        EditorGUI.PropertyField(position, property, new GUIContent(entryName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
