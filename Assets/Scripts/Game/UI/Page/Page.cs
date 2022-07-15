using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }

    public virtual void Setup(params object[] obj)
    {
    }
    
    public static Page Get(string pageName)
    {
        var pageObj = GameObject.Find(pageName);
        
        // Hide 처리되어 비활성화된 경우
        if (!pageObj)
        {
            pageObj = GameObject.Find("MasterCanvas").transform.Find(pageName).gameObject;
        }
        
        Page page = pageObj.GetComponent<Page>();
        return page;
    } 
    
    public void Show()
    {
        this.gameObject.SetActive(true);
        OnShow();
    }
    
    public virtual void OnShow()
    {
    }
    
    public void Hide()
    {
        OnHide();
        this.gameObject.SetActive(false);
    }

    public virtual void OnHide()
    {
    }
}
