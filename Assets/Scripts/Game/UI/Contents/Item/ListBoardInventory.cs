using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ListBoardInventory : ScrollViewController<InventorySlotData>
{
    // Default 슬롯 세팅 뒤에 아이템 정보를 세팅?
    // Start is called before the first frame update
    private int _inventoryCount;
    private List<InventorySlotData> _inventoryList;

    void LoadData()
    {
        for (int i = 0; i < _inventoryCount; ++i)
        {
            InventorySlotData item = new InventorySlotData();
            item.icon = "a107_01";
            item.name = "testItem";
            listData.Add(item);
        }
        
        // 스크롤시킬 내용의 크기를 갱신한다
        UpdateContents();
    }

    protected override void Awake()
    {
        _inventoryList = new List<InventorySlotData>();
        _inventoryCount = 100;
    }
    
    // 리스트 항목에 대응하는 셀의 높이를 반환하는 메서드
    protected override Vector2 GetSlotSize()
    {
        GameObject content = GameObject.Find("Content");
        GridLayoutGroup gridLayoutGroup = content.GetComponent<GridLayoutGroup>();

        return gridLayoutGroup.cellSize;
    }

    protected override int GetContraintSlotCount()
    {
        GameObject content = GameObject.Find("Content");
        GridLayoutGroup gridLayoutGroup = content.GetComponent<GridLayoutGroup>();
        int constraintCount = gridLayoutGroup.constraintCount;

        return constraintCount;
    }

    void Start()
    {
        base.Start();
        LoadData();

    }
}
