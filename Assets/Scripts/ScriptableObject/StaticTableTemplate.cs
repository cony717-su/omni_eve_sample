using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName ="StaticTableTemplate", menuName ="ScriptableObjects/StaticTableTemplate", order = 1)]
public class StaticTableTemplate : IScriptableObject
{
    [SerializeField]
    string _tableName = "";
    public string TableName
    {
        get => _tableName;
    }

    [Serializable]
    public struct Column
    {
        public bool isPrimaryKey;
        public string type;
        public string property;
    }

    [HideInInspector]
    [SerializeField]
    List<Column> _listColumn = new List<Column>();
    public List<Column> ListColumn
    {
        get => _listColumn;
    }

    public string GetTableName()
    {
        string staticTableName = "";
        foreach (var token in _tableName.ToLower().Split("_"))
        {
            if (token.Length == 0) continue;
            staticTableName += $"{token.Substring(0, 1).ToUpper()}";

            string lowerToken = token.Substring(1, token.Length - 1);
            if (string.IsNullOrEmpty(lowerToken)) continue;
            
            staticTableName += lowerToken;
        }
        return staticTableName;
    }

    string GetNameSpace()
    {
        string nameSpace = "";
        foreach (var column in _listColumn)
        {
            if (string.IsNullOrEmpty(column.property) || string.IsNullOrEmpty(column.type))
            {
                // skip invalid property
                continue;
            }
            
            string name = "";
            if (column.type == nameof(DateTime))
            {
                name = "System";
            }

            // insert name space
            if (!string.IsNullOrEmpty(name) && !nameSpace.Contains($"using {name};"))
            {
                nameSpace += $"using {name};\n";
            }
        }

        return nameSpace;
    }

    public string GetScriptName()
    {
        string tableName = GetTableName();
        if (string.IsNullOrEmpty(tableName))
            return "";
        
        return tableName + "Table";
    }

    string GetProperty()
    {
        string primaryKey = "";
        string property = "";
        
        int order = 0;
        foreach (var column in _listColumn)
        {
            if (string.IsNullOrEmpty(column.property) || string.IsNullOrEmpty(column.type))
            {
                // skip invalid property
                continue;
            }
            
            if (!string.IsNullOrEmpty(primaryKey)) primaryKey += "\n";
            if (!string.IsNullOrEmpty(property)) property += "\n";

            string publicProperty = $"\tpublic {column.type} {column.property}" + " { get; set; }";
            if (column.isPrimaryKey)
            {
                primaryKey += $"\t[PrimaryKey({++order})]\n" + publicProperty;
            }
            else
            {
                property += publicProperty;
            }
        }

        if (string.IsNullOrEmpty(primaryKey))
        {
            DebugManager.LogError($"There is not any primaryKey");
            return "";
        }
        
        return primaryKey + property;
    }

    public void CreateScript()
    {
        string tableName = GetTableName();
        if (string.IsNullOrEmpty(tableName))
        {
            DebugManager.LogError("Invalid TableName");
            return;
        }

        string scriptName = GetScriptName();
        string fileName = $"{StaticManager.DATA_PATH}/{scriptName}.cs";
        if (File.Exists(fileName))
        {
            DebugManager.LogError($"Already Created {fileName}");
            return;
        }

        string templateFileName = StaticManager.DATA_PATH.Replace("/DataTable", "/StaticTableTemplate.txt");
        string template = File.ReadAllText(templateFileName);

        // replace tokens
        template = template.Replace("#NAMESPACE#", GetNameSpace());
        template = template.Replace("#TABLENAME#", tableName);
        template = template.Replace("#SCRIPTNAME#", scriptName);
        template = template.Replace("#PROPERTY#", GetProperty());
        
        File.WriteAllText(fileName, template);
    }
}

[CustomEditor(typeof(StaticTableTemplate))]
public class StaticTableTemplateEditor : Editor
{
    public StaticTableTemplate Script => target as StaticTableTemplate;

    private ReorderableList list;

    private List<string> listColumnType = new List<string>(new []{"int", "string", "DateTime"});
    
    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("_listColumn"),
            true, true, true, true);

        // draw callbacks
        list.drawHeaderCallback = OnDrawHeaderCallback;
        list.drawElementCallback= OnDrawElementCallback;
        list.onAddDropdownCallback = OnAddDropdownCallback;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        string scriptName = Script.GetScriptName();
        if (string.IsNullOrEmpty(scriptName))
        {
            scriptName = "*";
        }

        if (GUILayout.Button($"Create {scriptName}.cs"))
        {
            Script.CreateScript();
        }

        base.OnInspectorGUI();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    void OnDrawHeaderCallback(Rect rect)
    {
        string primaryColumns = "";
        foreach (var column in Script.ListColumn)
        {
            if (column.isPrimaryKey)
            {
                if (!string.IsNullOrEmpty(primaryColumns)) primaryColumns += ", ";
                primaryColumns += $"{column.property}";
            }
        }
        if (!string.IsNullOrEmpty(primaryColumns))
        {
            primaryColumns = $"[PrimaryKey({primaryColumns})]";
        }
        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), primaryColumns);
    }

    void OnDrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2; // 위쪽 패딩

        // isPrimaryKey (check box)
        Rect rectPrimaryKey = new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginChangeCheck();
        bool isPrimaryKey = EditorGUI.Toggle(rectPrimaryKey, element.FindPropertyRelative("isPrimaryKey").boolValue);
        if (EditorGUI.EndChangeCheck())
        {
            element.FindPropertyRelative("isPrimaryKey").boolValue = isPrimaryKey;
        }
                
        // type (combo box)
        Rect rectType = new Rect(rectPrimaryKey.xMax, rect.y, 80, EditorGUIUtility.singleLineHeight);
        EditorGUI.BeginChangeCheck();
        int selectedIndex = listColumnType.FindIndex(x=> x == element.FindPropertyRelative("type").stringValue);
        selectedIndex = EditorGUI.Popup(rectType, selectedIndex, listColumnType.ToArray());
        if (EditorGUI.EndChangeCheck())
        {
            element.FindPropertyRelative("type").stringValue = listColumnType[selectedIndex];
        }
                
        // field (edit box)
        Rect rectField = new Rect(rectType.xMax, rect.y, rect.xMax - rectType.xMax, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rectField, element.FindPropertyRelative("property"), GUIContent.none);
    }
    
    private void OnAddDropdownCallback(Rect rect, ReorderableList list)
    {
        StaticTableTemplate.Column column = new StaticTableTemplate.Column();
        column.isPrimaryKey = false;
        column.type = listColumnType[0];
        column.property = "";
        
        Script.ListColumn.Add(column);
    }
}