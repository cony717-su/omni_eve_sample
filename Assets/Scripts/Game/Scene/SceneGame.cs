using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneGame : IScene
{
    GameObject _grid;
    protected override void OnInit()
    {
        StaticManager.Instance.Init();
    }

    private void Start()
    {
        Init();
        // Init Grid for a Tilemap
        ResourcesManager.Instance.InstantiateAsyncFromLabel("InitGrid", OnInitGrid);
    }


    private void OnInitGrid(GameObject grid)
    {
        _grid = grid;
    }

    private void OnDestroy()
    {
        
    }
}
