using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PageNavigation
{
    // 열려있는 순서대로 스택에 쌓아놓는다
    private Stack<Page> _stkPage;

    private Page _currPage;
    private Page _prevPage;

    public PageNavigation()
    {
        _stkPage = new Stack<Page>();
    }

    public Page Current()
    {
        return _currPage;
    }

    public Page Push(string pageName)
    {
        _prevPage = _currPage;
        
        _currPage = Page.Get(pageName);
        if (!_currPage)
        {
            Debug.Log("_currPage is null");
        }
        _currPage.Show();

        if (_prevPage)
        {
            _prevPage.Hide();
            _stkPage.Push(_prevPage);
        }

        return _currPage;
    }

    public Page Pop()
    {
        _currPage.Hide();
        
        _currPage = _stkPage.Pop();

        if (0 < _stkPage.Count)
        {
            _prevPage = _stkPage.First();
        }
        else
        {
            _prevPage = null;
        }

        _currPage.Show();
        return _currPage;
    }

    public void PopTo(string pageName)
    {
        if (_stkPage.Count < 2)
        {
            Debug.Log("Use Pop()");
            return;
        }
        
        while (0 < _stkPage.Count)
        {
            var pageView = _stkPage.Pop();
            string strPage = pageView.name;

            if (strPage == pageName)
            {
                _currPage.Hide();
                
                _currPage = pageView;
                _currPage.Show();

                _prevPage = _stkPage.First();
                return;
            }
            
            pageView.Hide();
        }
    }

    public void PopToRoot()
    {
        var prevPage = _currPage;
        
        while (0 < _stkPage.Count)
        {
            prevPage.Hide();
            prevPage = _stkPage.Pop();
        }

        _prevPage = null;
        _currPage = prevPage;
        _currPage.Show();
    } 
}
