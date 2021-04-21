/*
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine;

[CustomPropertyDrawer(typeof(ResourceSet))]
public class ResourceDictPropertyDrawer : PropertyDrawer
{
    public const int rowHeight = 20;
    public const int keyWidth = 35;
    public const int valueWidth = 50;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Create property container element.
        var container = new VisualElement();

        SerializedProperty resources = property.FindPropertyRelative("resources");

        SerializedProperty keys = resources.FindPropertyRelative("keys");
        SerializedProperty values = resources.FindPropertyRelative("values");

        int count = resources.FindPropertyRelative("Count").intValue;
        
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        float horizontalStart = position.x;

        
        for (int i = 0; i < count; i++)
        {
            Rect keyRect = new Rect(position.x, position.y + i * rowHeight, keyWidth, position.height);
            Rect valueRect = new Rect(position.x + keyWidth, position.y + i * rowHeight, valueWidth, position.height);
            EditorGUI.PropertyField(keyRect, keys.GetFixedBufferElementAtIndex(i));
            EditorGUI.PropertyField(valueRect, values.GetFixedBufferElementAtIndex(i));
        }

        EditorGUI.EndProperty();
        
    }
    /*
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty resources = property.FindPropertyRelative("resources");

        int count = resources.FindPropertyRelative("keys").fixedBufferSize;
        return count * rowHeight;
    }*/
//}
