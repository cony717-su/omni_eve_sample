using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressableSpriteAtlasLoader : MonoBehaviour
{
    public AssetReferenceT<SpriteAtlas> newAtlas;
    public string spriteAtlasAddress;
    public string atlasedSpriteName;
    public bool useAddress;
    private SpriteRenderer spriteRenderer;
    private AsyncOperationHandle<SpriteAtlas> atlasOperation;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAddress)
        {
            atlasOperation = Addressables.LoadAssetAsync<SpriteAtlas>(spriteAtlasAddress);
            atlasOperation.Completed += SpriteAtlasLoaded;
        }
        else
        {
            atlasOperation = newAtlas.LoadAssetAsync();
            atlasOperation.Completed += SpriteAtlasLoaded;
        }
    }
    private void SpriteAtlasLoaded(AsyncOperationHandle<SpriteAtlas> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                spriteRenderer.sprite = obj.Result.GetSprite(atlasedSpriteName);
                break;
            case AsyncOperationStatus.Failed:
                Debug.LogError("Sprite load failed.");
                break;
            default:
                // case AsyncOperationStatus.None:
                break;
        }
    }
    void OnDestroy()
    {
        if (atlasOperation.IsValid())
        {
            Addressables.Release(atlasOperation);
            Debug.Log("Successfully released atlas load operation.");
        }
    }
}
