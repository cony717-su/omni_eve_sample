using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class ContentsName : MonoBehaviour
{
    public string GetContentsName(ContentsObject contents)
    {
        ContentsObjectType contentsType = contents.ContentsType;
        string contentsName = "";

        switch (contentsType)
        {
            case ContentsObjectType.Character:
                contentsName = GetContentsNameCharacter(contents);
                break;
            case ContentsObjectType.Item:
                contentsName = GetContentsNameItem(contents);
                break;
        }

        return contentsName;
    }

    public string GetContentsNameCharacter(ContentsObject contents)
    {
        int idx = contents.ContentsIdx;
        return GetCharacterIdxName(idx);
    }

    public string GetContentsNameItem(ContentsObject contents)
    {
        int idx = contents.ContentsIdx;
        return GetItemIdxName(idx);
    }

    public string GetCharacterIdxName(int idx)
    {
        var data = StaticManager.Instance.Get<StaticCharacterSkin>(idx);
        string viewIdx = data.view_idx;
        return Util.Util.GetLocaleText("CHARACTER_NAME_" + viewIdx);
    }

    public string GetItemIdxName(int idx)
    {
        var data = StaticManager.Instance.Get<StaticItem>(idx);
        string viewIdx = data.view_idx;
        return Util.Util.GetLocaleText("ITEM_NAME_" + viewIdx);
    }
}
