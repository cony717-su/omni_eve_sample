using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : IManager<PopupManager>
{
    private Queue<Popup> _queuePopup;
    private static Transform parent;

    void Start()
    {
        _queuePopup = new Queue<Popup>();
        parent = GameObject.Find("PopupCanvas").transform;
    }

    public void AddPopup(string popupName)
    {
        ResourcesManager.InstantiateAssetAsync(popupName, parent,
            true, true,
            (result) =>
            {
                Popup popup = result.GetComponent<Popup>();
                _queuePopup.Enqueue(popup);
            });
    }

    public void Show(string popupName)
    {
        // 필요한 초기화 과정을 걸어준 다음에
        // 그 팝업의 show를 걸면서 동시에 리턴
        ResourcesManager.InstantiateAssetAsync(popupName, parent,
            true, true,
            (result) =>
            {
                Popup popup = result.GetComponent<Popup>();
                popup.Show();
            });
    }

    public void Pop()
    {
    }
}
