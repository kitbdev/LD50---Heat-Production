using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUI.PropertyField(position, property, label, property.isExpanded);
        EditorGUI.EndDisabledGroup();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
    }
}
