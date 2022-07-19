using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TilemapGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[CustomEditor(typeof(TilemapGenerator))]
public class TilemapGeneratorEditor : Editor
{
    private TilemapGenerator generator;

    private void OnEnable()
    {
        generator = FindObjectOfType<TilemapGenerator>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Tilemap"))
        {
            // generate
        }
    }
}