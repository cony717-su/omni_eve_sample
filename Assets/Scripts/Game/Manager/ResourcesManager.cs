using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ResourcesManager : IManager<ResourcesManager>
{
    private Dictionary<string, AsyncOperationHandle<GameObject>> _dictObjectHandle = new Dictionary<string, AsyncOperationHandle<GameObject>>();


    public static void LoadAddressableAsset<T>(string assetPath, UnityAction<T> callback)
    {
        Addressables.LoadAssetAsync<T>(assetPath).Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load asset at address: {assetPath}");
                return;
            }

            callback(obj.Result);
        };
    }

    public bool InstantiateAsync(string assetPath, UnityAction<GameObject> callback)
    {
        AsyncOperationHandle<GameObject> handle;
        if (_dictObjectHandle.TryGetValue(assetPath, out handle))
        {
            callback(handle.Result);
            return true;
        }

        handle = Addressables.InstantiateAsync(assetPath);
        handle.Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load asset at address: {assetPath}");
                return;
            }

            callback(obj.Result);
        };
        _dictObjectHandle.Add(assetPath, handle);
        return false;
    }

    public void InstantiateAsyncFromLabel(string labelName, UnityAction<GameObject> callback)
    {
        Addressables.LoadResourceLocationsAsync(labelName).Completed +=
            (handle) =>
            {
                string assetPath = handle.Result[0].ToString();
                InstantiateAsync(assetPath, callback);
            };
    }
}
