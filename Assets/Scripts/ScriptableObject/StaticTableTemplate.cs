using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName ="StaticTableTemplate", menuName ="ScriptableObjects/StaticTableTemplate", order = 1)]
public class StaticTableTemplate : IScriptableObject
{
    public string tableName;

    [Serializable]
    public struct Column
    {
        public bool isPrimaryKey;
        public string type;
        public string field;
    }

    [SerializeField]
    List<Column> _listColumn = new List<Column>();
    public List<Column> ListColumn
    {
        get => _listColumn;
    }




}

[CustomEditor(typeof(StaticTableTemplate))]
public class StaticTableTemplateEditor : Editor
{
    public StaticTableTemplate Script => target as StaticTableTemplate;

    private ReorderableList list;
    
    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("ListColumn"),
            true, true, true, true);
        
        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2; // 위쪽 패딩
                
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("type"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 100, rect.y, rect.width - 100 - 30, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("field"), GUIContent.none);
            };
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        string filename = Script.tableName;
        if (string.IsNullOrEmpty(filename))
        {
            filename = "*.cs";
        }
        else
        {
            filename += "Table.cs";
        }
        
        if (GUILayout.Button($"Create C# Script: {filename}"))
        {
            
        }
        base.OnInspectorGUI();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}