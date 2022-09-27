using UnityEngine;

[RequireComponent(typeof(RectTransform))]	// RectTransform 컴포넌트가 필수이다
public class ViewController : MonoBehaviour
{
	// Rect Transform 컴포넌트를 캐시한다
	private RectTransform _cachedRectTransform;
	public RectTransform CachedRectTransform
	{
		get {
			if(_cachedRectTransform == null)
				{ _cachedRectTransform = GetComponent<RectTransform>(); }
			return _cachedRectTransform;
		}
	}
}
