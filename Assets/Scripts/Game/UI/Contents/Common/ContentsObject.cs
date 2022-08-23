using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentsObject
{
    public ContentsObjectType ContentsType { get; set; }
    public int ContentsIdx { get; set; }
    public int ConstnetsCount { get; set; }

    public ContentsObject(ContentsObjectType contentsType, int idx, int count = 0)
    {
        ContentsType = contentsType;
        ContentsIdx = idx;
        ConstnetsCount = count;
    }
}
