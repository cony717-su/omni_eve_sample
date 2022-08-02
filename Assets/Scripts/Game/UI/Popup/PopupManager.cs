using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : IManager<PopupManager>
{
    private Queue<Popup> _queuePopup;
    private Popup _currentPopup;
    private static Transform _parent;
    private int _loadPopupCount;
    private bool _isLoadedAll;

    void Start()
    {
        _queuePopup = new Queue<Popup>();
        _parent = GameObject.Find("PopupCanvas").transform;
        Clear();
    }
    
    public void Show(string popupName, params object[] obj)
    {
        ResourcesManager.InstantiateAssetAsync(popupName, _parent,
            true, true,
            (result) =>
            {
                Popup popup = result.GetComponent<Popup>();
                popup.Setup(obj);
                popup.Show();
            });
    }

    // queue popup functions
    public void AddPopupList(List<string> popupList)
    {
        if (popupList.Count <= 0)
        {
            DebugManager.Log("PopupList is empty.");
            return;
        }
        
        _loadPopupCount = popupList.Count;

        foreach (string popupName in popupList)
        {
            AddPopup(popupName);
        }
    }
    private void AddPopup (string popupName)
    {
        ResourcesManager.InstantiateAssetAsync(popupName, _parent,
            true, true,
            (result) =>
            {
                Popup popup = result.GetComponent<Popup>();
                _queuePopup.Enqueue(popup);
                popup.gameObject.SetActive(false);
                OnPopupLoaded();
            });
    }

    private void OnPopupLoaded()
    {
        --_loadPopupCount;

        if (_loadPopupCount <= 0)
        {
            _isLoadedAll = true;
        }

        if (_isLoadedAll)
        {
            Run();
        }
    }

    public void Clear()
    {
        _queuePopup.Clear();
        _currentPopup = null;
        _loadPopupCount = 0;
        _isLoadedAll = false;
    }

    private void Run()
    {
        if (_queuePopup.Count <= 0)
        {
            DebugManager.Log("Queue popup is empty.");
            return;
        }
        
        _currentPopup = _queuePopup.Dequeue();
        _currentPopup.Show();
    }
    
    public void Pop()
    {
        _currentPopup.Hide();
        
        if (_queuePopup.Count <= 0)
        {
            return;
        }
        
        Popup newPopup = _queuePopup.Dequeue();
        _currentPopup = newPopup;
        _currentPopup.Show();
    }
}
