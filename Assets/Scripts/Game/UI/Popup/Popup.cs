using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public virtual void Setup(params object[] obj)
    {
    }
    public void Show()
    {
        transform.position = new Vector3(540.0f, 960.0f, 0.0f);
        this.gameObject.SetActive(true);
        OnShow();
    }

    public virtual void OnShow()
    {
    }

    public void Hide()
    {
        OnHide();
        Destroy(this.gameObject);
    }

    public virtual void OnHide()
    {
    }
}
