using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;

[CustomPropertyDrawer(typeof(LayerAttribute))]
public class LayerDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
         property.intValue = EditorGUI.LayerField(position, label, property.intValue);
         EditorGUI.EndProperty();
    }
}
