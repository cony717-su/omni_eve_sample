using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ContentsIcon : IconSlot
{
    public void SetContentsIcon(ContentsObject contents, string iconName)
    {
        ContentsObjectType contentsType = contents.ContentsType;
        switch (contentsType)
        {
            case ContentsObjectType.Character:
                SetContentsIconCharacter(iconName, contents);
                break;
            case ContentsObjectType.Item:
                SetContentsIconItem(iconName, contents);
                break;
        }
    }

    public void SetContentsIconCharacter(string iconName, ContentsObject contents)
    {
        int idx = contents.ContentsIdx;
        //var data = StaticManager.Instance.Get<StaticCharacterSkin>(idx);
        //string viewIdx = data.view_idx;
        
        string tempImg = "davi";
        SetIconSlot(iconName, tempImg);
        AttributeType tempAttribute = AttributeType.Light;
        SetIconFrameColor(iconName, tempAttribute);
    }

    public void SetContentsIconItem(string iconName, ContentsObject contents)
    {
        int idx = contents.ContentsIdx;
        //var data = StaticManager.Instance.Get<StaticItem>(idx);
        //string viewIdx = data.view_idx;
        
        string tempImg = "a107_01";
        SetIconSlot(iconName, tempImg);
    }

}
