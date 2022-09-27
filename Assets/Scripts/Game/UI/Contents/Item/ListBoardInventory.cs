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
        
        #region 아이템 목록 화면을 내비게이션 뷰에 대응시킨다
        if(navigationView != null)
        {
            // 내비게이션 뷰의 첫 뷰로 설정한다
            navigationView.Push(this);
        }
        #endregion

    }
    
    
    #region 아이템 목록 화면을 내비게이션 뷰에 대응시킨다
    // 내비게이션 뷰
    [SerializeField] private NavigationViewController navigationView;

    // 뷰의 타이틀을 반환한다
    //public override string Title { get { return "SHOP"; } }
    #endregion

    #region 아이템 상세 화면으로 옮기는 처리
    // 아이템 상세 화면의 뷰
    //[SerializeField] private ShopDetailViewController detailView;

    // 셀이 선택됐을 때 호출되는 메서드
    public void OnPressCell(ShopItemTableViewCell cell)
    {
        if(navigationView != null)
        {
            // 선택된 셀로부터 아이템 데이터를 가져와서 아이템 상세 화면의 내용을 갱신한다
            //detailView.UpdateContent(tableData[cell.DataIndex]);
            // 아이템 상세 화면으로 옮긴다
            //navigationView.Push(detailView);
        }
    }
    #endregion
}
