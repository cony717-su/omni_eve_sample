using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditorInternal;

using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

[CreateAssetMenu(fileName = "RuleTile", menuName = "ScriptableObjects/RuleTile", order = 1)]

public class RuleTileBase : RuleTile<RuleTileBase.Neighbor> {
    [SerializeField] private string tileName = "None";
    [SerializeField] private string spriteAtlasAddress;

    public AssetReferenceAtlasedSprite sprite;
    public AssetReferenceT<SpriteAtlas> spriteAtlas;
    
    public string TileName
    {
        get => this.tileName;
        set => this.tileName = value;
    }
    
    public string SpriteAtlasAddress
    {
        get => this.spriteAtlasAddress;
        set => this.spriteAtlasAddress = value;
    }

    public class TilingRule : RuleTile.TilingRule
    {
    
    }
    
    public class Neighbor : RuleTile.TilingRule.Neighbor 
    {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) 
    {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
    }
}


[CustomEditor(typeof(RuleTileBase))]
public class RuleTileBaseEditor : RuleTileEditor
{
    private ReorderableList.AddDropdownCallbackDelegate _onAddDropdownCallback;

    private int selectedIndex = 1;
    
    private RuleTileBase Tile
    {
        get => target as RuleTileBase;
    }

    private void OnAddDropdownCallback(Rect rect, ReorderableList list)
    {
        if (string.IsNullOrEmpty(Tile.SpriteAtlasAddress))
        {
            DebugManager.LogError("Set Sprite Atlas");
            return;
        }
        _onAddDropdownCallback(rect, list);
    }

    private void OnDrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
    {
        RuleTile.TilingRule rule = tile.m_TilingRules[index];
        DebugManager.Log(($"OnDrawElementCallback {index}"));
    }
    
    private System.Type GetGenericTypeFromAssetReference(AssetReference assetReferenceObject)
    {
        var type = assetReferenceObject?.GetType();
        while (type != null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AssetReferenceT<>))
                return type.GenericTypeArguments[0];
            type = type.BaseType;
        }

        return null;
    }
    
    public override void SpriteOnGUI(Rect rect, RuleTile.TilingRuleOutput tilingRule)
    {
        tilingRule.m_Sprites[0] = EditorGUI.ObjectField(rect, tilingRule.m_Sprites[0], typeof(Sprite), false) as Sprite;
    }
    
    
    private object GetValue(string fieldName)
    {
        FieldInfo field = typeof(RuleTileEditor).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return field.GetValue(this);
    }
    
    private void SetValue(string fieldName, object value)
    {
        FieldInfo field = typeof(RuleTileEditor).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(this, value);
    }
    
    private void SetData()
    {
        if (Tile.m_DefaultGameObject)
        {
            Tile.TileName = Tile.m_DefaultGameObject.name;
        }
        else
        {
            Tile.TileName = "";
        }

        Tile.SpriteAtlasAddress = AssetDatabase.GUIDToAssetPath(Tile.spriteAtlas.AssetGUID);
    }
    
    public override void OnEnable()
    {
        base.OnEnable();
        
        ReorderableList list = GetValue("m_ReorderableList") as ReorderableList;
        _onAddDropdownCallback = list.onAddDropdownCallback; 
        list.onAddDropdownCallback = OnAddDropdownCallback;
        
        list.drawElementCallback += OnDrawElementCallback;

        SetData();
    }
    
    public override void OnInspectorGUI()
    {
        SetData();
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        
        // todo
        var subAssets = new List<Object>();
        subAssets.Add(null);
        var assetPath = Tile.SpriteAtlasAddress;

        var repr = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
        if (repr.Any())
        {
            var subtype = GetGenericTypeFromAssetReference(Tile.spriteAtlas);
            if (subtype != null)
                repr = repr.Where(o => subtype.IsInstanceOfType(o)).OrderBy(s => s.name).ToArray();
        }
        
        var mainType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        if (mainType == typeof(SpriteAtlas))
        {
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            var sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            subAssets.AddRange(sprites.OrderBy(s => s.name));
        }
        


        //selectedIndex = EditorGUILayout.Popup(selectedIndex, subAssets.ToArray());
        //EditorGUILayout.ObjectField(Style.mainAssetLabel, currentMainSpriteLibraryAsset, typeof(SpriteLibraryAsset), false) as SpriteLibraryAsset;
        //EditorGUILayout.IntField(intProperty.displayName, intProperty.intValue);
        
        
        if (GUILayout.Button("Load"))
        {

        }

        if (GUILayout.Button("Save"))
        {

        }
    }
}