using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableSpriteLoader : MonoBehaviour
{
    public AssetReferenceSprite newSprite;
    private SpriteRenderer spriteRenderer;
    private AsyncOperationHandle<Sprite> spriteOperation;
    public string newSpriteAddress;
    public bool useAddress;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (useAddress)
        {
            spriteOperation = Addressables.LoadAssetAsync<Sprite>(newSpriteAddress);
            spriteOperation.Completed += SpriteLoaded;
        }
        else
        {
            spriteOperation = newSprite.LoadAssetAsync();
            spriteOperation.Completed += SpriteLoaded;
        }
    }
    private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
                spriteRenderer.sprite = obj.Result;
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
        if (spriteOperation.IsValid())
        {
            Addressables.Release(spriteOperation);
            Debug.Log("Successfully released sprite load operation.");
        }
    }
}
