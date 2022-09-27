using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

// handleDictionary는 유지하고
// AddressableAssetSprite/AtlasLoader 형식으로 컴포넌트화 시켜서
// 로딩이 필요한 곳에 넣어주고 핸들 생성과 제거를 해주면 좋을듯
public class ResourcesManager : IManager<ResourcesManager>
{
    private static Dictionary<string, AsyncOperationHandle> handleDictionary;

    void Start()
    {
        handleDictionary = new Dictionary<string, AsyncOperationHandle>();
    }

    public void LoadAddressableAsset<T>(string assetName, UnityAction<T> callback)
    {
        Addressables.LoadAssetAsync<T>(assetName).Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load asset at address: {assetName}");
                return;
            }

            //handleDictionary.Add(assetName, obj);
            callback(obj.Result);
        };
    }
    public void Destroy(string objName)
    {
        if (!handleDictionary.ContainsKey(objName))
        {
            DebugManager.LogError($"Failed to destroy asset at address: {objName}");
            return;
        }
    
        var handle = handleDictionary[objName];
        Addressables.Release(handle);
    }

    public void InstantiateAssetAsync(string assetName, Transform parent = null, 
        bool instantiateInWorldSpace = false, bool trackHandle = true, 
        UnityAction<GameObject> callback = null)
    {
        Addressables.InstantiateAsync(assetName, parent, instantiateInWorldSpace, 
            trackHandle).Completed += (obj) =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to instantiate asset at address: {assetName}");
                return;
            }

            if (!handleDictionary.ContainsKey(assetName))
            {
                handleDictionary.Add(assetName, obj);
            }

            if (callback != null)
            {
                callback(obj.Result);
            }
        };
    } 
    public void InstantiateAssetAsync(string assetName, Transform parent = null,
        UnityAction<GameObject> callback = null)
    {
        Addressables.InstantiateAsync(assetName, parent).Completed += (obj) =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to instantiate asset at address: {assetName}");
                return;
            }

            if (!handleDictionary.ContainsKey(assetName))
            {
                handleDictionary.Add(assetName, obj);
            }

            if (callback != null)
            {
                callback(obj.Result);
            }
        };
    }
    
    public void InstantiateAsyncFromLabel(string labelName, UnityAction<GameObject> callback)
    {
        Addressables.LoadResourceLocationsAsync(labelName).Completed +=
            (handle) =>
            {
                string assetPath = handle.Result[0].ToString();
                InstantiateAssetAsync(assetPath, null, false, true, callback);
            };
    }
}
