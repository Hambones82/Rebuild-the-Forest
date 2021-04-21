using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(KeyPress))]
public class KeyCodeToInputActionTypePropertyDrawer : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect keyCodeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect keyPressTypeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

        SerializedProperty keyCodeProperty = property.FindPropertyRelative("keyCode");
        SerializedProperty keyPressTypeProperty = property.FindPropertyRelative("keyPressType");
        //draw keycode thing
        //draw action thing

        KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), keyCodeProperty.enumNames[keyCodeProperty.enumValueIndex]);


        keyCode = InputControls.KeyCodeField(keyCodeRect, keyCode);
        EditorGUI.PropertyField(keyPressTypeRect, keyPressTypeProperty, new GUIContent("U/D/H? "));

        string keyCodeName = keyCode.ToString();
        int propertyIndex = Array.IndexOf(keyCodeProperty.enumNames, keyCodeName);
        keyCodeProperty.enumValueIndex = propertyIndex;
        
        EditorGUI.EndProperty();
    }
	
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }

}
