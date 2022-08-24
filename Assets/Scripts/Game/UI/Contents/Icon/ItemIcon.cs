using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIcon : ContentsIcon
{
    void Start()
    {
        ContentsObject contentItem = new ContentsObject(ContentsObjectType.Item, 1010035, 1);
        SetContentsIcon(contentItem, "icon_slot_02");
    }
}
