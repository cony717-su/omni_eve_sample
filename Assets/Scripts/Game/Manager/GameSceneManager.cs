using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class GameSceneManager : IManager<GameSceneManager>
{
    enum SceneState
    {
        SCENE_START,
        MOVE_TO_SCENE_LOADER,
        MOVE_TO_ACTIVE_SCENE,
        READY_TO_LOAD,
    }

    private const string LOADING_SCENE_NAME = "SceneLoader";
    private ReactiveProperty<SceneState> _SceneStateProperty = new ReactiveProperty<SceneState>(SceneState.SCENE_START);

    private string _TargetSceneName;
    private string _ActiveSceneName;
    private SceneInstance _LoadedScene;

    public void MoveToScene(string targetSceneName)
    {
        _TargetSceneName = targetSceneName;
        _ActiveSceneName = SceneManager.GetActiveScene().name;
        _SceneStateProperty.Value = SceneState.MOVE_TO_SCENE_LOADER;
    }

    private void Start()
    {
        _SceneStateProperty.Subscribe(NextSceneState);
    }

    private void OnDestroy()
    {
        _SceneStateProperty.Dispose();
    }

    void NextSceneState(SceneState state)
    {
        DebugManager.Log(state.ToString());
        switch (state)
        {
            case SceneState.MOVE_TO_SCENE_LOADER:
                LoadScene(LOADING_SCENE_NAME, SceneState.MOVE_TO_ACTIVE_SCENE);
                break;
            case SceneState.MOVE_TO_ACTIVE_SCENE:
                LoadScene(_TargetSceneName, SceneState.READY_TO_LOAD);
                break;
            case SceneState.READY_TO_LOAD:
                DebugManager.Log();
                break;
        }
    }

    void LoadScene(string sceneName, SceneState nextState)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            DebugManager.Log("Address To Add not set.");
            return;
        }
        Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single).Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load scene at address: {_TargetSceneName}");
                return;
            }
            _LoadedScene = obj.Result;
            _SceneStateProperty.Value = nextState;
        };
    }

}