using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/*
 * 이전 tab의 history를 저장해야할 경우 사용
 * tab 별로 쌓아둔 page를 저장해놓고
 * 다른 tab에 갔다가 이전 tab으로 돌아올 경우에
 * 저장해준 page stack을 가져와서 그대로 띄워준다
 */
public class PageNavigationManager : IManager<PageNavigationManager>
{
    public PageNavigation PageNav { set; get; }
    
    void Start()
    {
        EventManager.Instance.Init();
        PageNav = new PageNavigation();
        Page startPage = PageNav.Push("PageCharacter");
        
    }
}
