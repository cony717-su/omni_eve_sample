using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class ResourcesManager: IManager<GameManager>
{
    public static void LoadAddressableAsset<T>(string assetName, UnityAction<T> callback)
    {
        Addressables.LoadAssetAsync<T>(assetName).Completed += obj =>
        {
            if (obj.Status != AsyncOperationStatus.Succeeded)
            {
                DebugManager.LogError($"Failed to load asset at address: {assetName}");
                return;
            }

            callback(obj.Result);
        };
    }
}
