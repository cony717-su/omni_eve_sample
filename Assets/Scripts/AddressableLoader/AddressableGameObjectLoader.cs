using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AddressableGameObjectLoader : MonoBehaviour
{
    public void LoadAsset<T>(string assetName, UnityAction<T> callback)
    {
        ResourcesManager.Instance.LoadAddressableAsset<T>(assetName, callback);
    }
    
    public void LoadGameObject(string objName, UnityAction<GameObject> callback)
    {
        ResourcesManager.Instance.InstantiateAsyncFromLabel(objName, callback);
    }
    
    public void LoadGameObject(string objName, Transform parent, UnityAction<GameObject> callback)
    {
        ResourcesManager.Instance.InstantiateAssetAsync(objName, parent, callback);
    }
    
    public void LoadGameObject(string objName, Transform parent, bool instantiateInWorldSpace, 
        bool trackHandle, UnityAction<GameObject> callback)
    {
        ResourcesManager.Instance.InstantiateAssetAsync(objName, parent, 
            instantiateInWorldSpace, trackHandle,callback);
    }

    public void DestroyHandle(string objName)
    {
        ResourcesManager.Instance.Destroy(objName);
    }
}
