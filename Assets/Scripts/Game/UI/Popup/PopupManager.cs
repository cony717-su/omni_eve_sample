using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PopupManager : IManager<PopupManager>
{
    private List<Popup> _queuePopup;
    private Popup _currentPopup;
    private static Transform _parent;
    private int _loadPopupCount;
    private List<string> _popupNameList;

    void Start()
    {
        _queuePopup = new List<Popup>();
        _parent = GameObject.Find("PopupCanvas").transform;
        _popupNameList = new List<string>();
        Clear();
    }
    
    public void Show(string popupName, params object[] obj)
    {
        ResourcesManager.Instance.InstantiateAssetAsync(popupName, _parent,
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

        _popupNameList = popupList;
        _loadPopupCount = _popupNameList.Count;

        Clear();
        foreach (string popupName in popupList)
        {
            AddPopup(popupName);
        }
    }
    private void AddPopup (string popupName)
    {
        ResourcesManager.Instance.InstantiateAssetAsync(popupName, _parent,
            true, true,
            (result) =>
            {
                Popup popup = result.GetComponent<Popup>();
                popup.gameObject.SetActive(false);
                _queuePopup.Add(popup);
                OnPopupLoaded();
            });
    }

    private void OnPopupLoaded()
    {
        --_loadPopupCount;

        if (0 < _loadPopupCount)
        {
            return;
        }

        _queuePopup.Sort((x, y) =>
        {
            string leftName = x.GetType().ToString();
            string rightName = y.GetType().ToString();
            int leftIndex = _popupNameList.IndexOf(leftName);
            int rightIndex = _popupNameList.IndexOf(rightName);
                
            if (leftIndex < rightIndex)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        });
        
        Run();
    }

    public void Clear()
    {
        _queuePopup.Clear();
        _currentPopup = null;
    }

    private void Run()
    {
        if (_queuePopup.Count <= 0)
        {
            DebugManager.Log("Queue popup is empty.");
            return;
        }

        _currentPopup = _queuePopup.First();
        _queuePopup.RemoveAt(0);
        _currentPopup.Show();
    }
    
    public void Pop()
    {
        _currentPopup.Hide();
        
        if (_queuePopup.Count <= 0)
        {
            return;
        }
        
        Popup newPopup = _queuePopup.First();
        _queuePopup.RemoveAt(0);
        
        _currentPopup = newPopup;
        _currentPopup.Show();
    }
}
