using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    protected Register register;

    public enum VisibleState
    {
        Appearing,
        Appeared,
        Disappearing,
        Disappeared,
    }

    void Awake()
    {
        register = new Register();
        OnAwake();
    }

    public virtual void OnAwake()
    {
        
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
        transform.position = new Vector3(540.0f, 960.0f, 0.0f); 
        this.gameObject.SetActive(true);
        OnShow();
        register.RegisterEvent(this);
    }
    
    public virtual void OnShow()
    {
    }
    
    public void Hide()
    {
        OnHide();
        this.gameObject.SetActive(false);
        register.DeRegisterEvent(this);
    }

    public virtual void OnHide()
    {
    }
    
    
}
