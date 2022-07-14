using UnityEngine;

public class IManager<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance is null)
                {
                    GameObject newInstance = new GameObject();
                    _instance = newInstance.AddComponent<T>();
                    _instance.name = typeof(T).FullName;
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if(_instance is null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }
}