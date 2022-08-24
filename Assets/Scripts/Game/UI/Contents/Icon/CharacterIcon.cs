using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIcon : ContentsIcon
{
    void Start()
    {
        ContentsObject content = new ContentsObject(ContentsObjectType.Character, 10100004, 1);
        SetContentsIcon(content, "icon_slot_01");
    }
}
