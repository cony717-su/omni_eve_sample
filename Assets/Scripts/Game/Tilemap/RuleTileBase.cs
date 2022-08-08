using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CreateAssetMenu(fileName = "RuleTile", menuName = "ScriptableObjects/RuleTile", order = 1)]
public class RuleTileBase : RuleTile<RuleTileBase.Neighbor> {
    public bool customField;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
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
    [SerializeField]
    public RuleTileBase _object;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Load"))
        {

        }

        if (GUILayout.Button("Save"))
        {

        }
    }
}