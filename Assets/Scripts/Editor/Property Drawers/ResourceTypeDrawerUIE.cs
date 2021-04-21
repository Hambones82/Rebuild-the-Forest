using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(ResourceType))]
public class ResourceTypeDrawerUIE : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int linecount = 3;
        return EditorGUIUtility.singleLineHeight * linecount + EditorGUIUtility.standardVerticalSpacing * (linecount - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        var categoryRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        var contentRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);

        SerializedProperty categoryProperty = property.FindPropertyRelative("resourceSelection");
        int categoryIntValue = categoryProperty.enumValueIndex;
        
        int buildingSelectionValue = (int)ResourceCategory.Building;
        int rawResourceSelectionValue = (int)ResourceCategory.RawResource;

        SerializedProperty rawResourceProperty = property.FindPropertyRelative("rawResourceType");
        SerializedProperty buildingTypeProperty = property.FindPropertyRelative("buildingType");

        SerializedProperty contentProperty = null;

        if (categoryIntValue == buildingSelectionValue)
        {
            contentProperty = buildingTypeProperty;
        }
        else if (categoryIntValue == rawResourceSelectionValue)
        {
            contentProperty = rawResourceProperty;
        }
        else
        {
            Debug.Log("Error: resource type selection enum has an invalid value");
        }

        EditorGUI.PropertyField(categoryRect, categoryProperty, GUIContent.none);
        EditorGUI.PropertyField(contentRect, contentProperty, GUIContent.none);

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    /*
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement container = new VisualElement();
        
        SerializedProperty categoryProperty = property.FindPropertyRelative("resourceSelection");
        int categoryIntValue = categoryProperty.enumValueIndex;

        PropertyField categoryField = new PropertyField(categoryProperty);

        int buildingSelectionValue = (int)ResourceCategory.Building;
        int rawResourceSelectionValue = (int)ResourceCategory.RawResource;
        
        PropertyField rawResourceField = new PropertyField(property.FindPropertyRelative("rawResourceType"));
        PropertyField buildingTypeField = new PropertyField(property.FindPropertyRelative("buildingType"));

        container.Add(categoryField);
        
        if(categoryIntValue == buildingSelectionValue)
        {
            container.Add(buildingTypeField);
        }
        else if(categoryIntValue == rawResourceSelectionValue)
        {
            container.Add(rawResourceField);
        }
        else
        {
            Debug.Log("Error: resource type selection enum has an invalid value");
        }
        

        return container;

    }
    */

}

