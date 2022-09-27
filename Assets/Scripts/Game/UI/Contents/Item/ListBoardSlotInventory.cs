using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotData
{
    public string name;
    public string icon;
}

public class ListBoardSlotInventory : ScrollViewSlot<InventorySlotData>
{
    [SerializeField] private Image iconImage;	// 아이콘을 표시할 이미지
    [SerializeField] private Text nameLabel;	// 아이템 이름을 표시할 텍스트
    
    // 셀의 내용을 갱신하는 메서드를 오버라이트한다
    public override void UpdateContent(InventorySlotData itemData)
    {
        nameLabel.text = itemData.name;
    }

    public void CreateSlot(int idx, Object data)
    {
    }

    public void RefreshSlot(int idx, Object data)
    {
    }
}
