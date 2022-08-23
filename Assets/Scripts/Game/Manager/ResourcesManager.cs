using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ResourcesManager : IManager<ResourcesManager>
{
    private static Dictionary<string, AsyncOperationHandle> handleDictionary;

    void Start()
    {
        handleDictionary = new Dictionary<string, AsyncOperationHandle>();
    }

    public static void LoadAddressableAsset<T>(string assetName, UnityAction<T> callback)
    {
        Addressables.LoadAssetAsync<T>(assetName).Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load asset at address: {assetName}");
                return;
            }

            handleDictionary.Add(assetName, obj);
            callback(obj.Result);
        };
    }
    public static void OnDestroy(string objName)
    {
        if (!handleDictionary.ContainsKey(objName))
        {
            DebugManager.LogError($"Failed to destroy asset at address: {objName}");
            return;
        }

        var handle = handleDictionary[objName];
        Addressables.Release(handle);
    }

    public static void InstantiateAssetAsync(string assetName, Transform parent = null, 
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
