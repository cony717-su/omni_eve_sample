using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;

public class StaticOmniEveFloor : RowData
{
    [PrimaryKey(1)]
    public int idx { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public int room_width { get; set; }
    public int room_height { get; set; }
    public int treasure_count_min { get; set; }
    public int treasure_count_max { get; set; }
    public int treasure_trap_prob { get; set; }
    public int treasure_item_prob { get; set; }
    public int treasure_coin_prob { get; set; }
    public int treasure_trap_group_idx { get; set; }
    public int treasure_item_group_idx { get; set; }
    public int treasure_coin_group_idx { get; set; }
    public int mob_count { get; set; }
    public int mob_group_idx { get; set; }
    public int forced_mob_group_idx { get; set; }
    public int shop_equip_count { get; set; }
    public int shop_group_idx { get; set; }
    public int stage_type { get; set; }
}

public class StaticOmniEveFloorTable : IScriptableObject
{
    public InnerTable<StaticOmniEveFloor> _DataTable { get; set; }

    //private void Awake()
    //{
    //    SetFloor();
    //}

    //void SetFloor()
    //{
    //    StaticOmniEveFloor _newFloor = new StaticOmniEveFloor();
    //    _newFloor.idx = 1;
    //    _newFloor.width = 20;
    //    _newFloor.height = 20;
    //    _newFloor.room_width = 2;
    //    _newFloor.room_height = 2;
    //    _newFloor.treasure_count_min = 3;
    //    _newFloor.treasure_count_max = 3;
    //    _newFloor.treasure_trap_prob = 0;
    //    _newFloor.treasure_item_prob = 150;
    //    _newFloor.treasure_coin_prob = 150;
    //    _newFloor.treasure_trap_group_idx = 1;
    //    _newFloor.treasure_item_group_idx = 101;
    //    _newFloor.treasure_coin_group_idx = 1001;
    //    _newFloor.mob_count = 6;
    //    _newFloor.mob_group_idx = 1;
    //    _newFloor.forced_mob_group_idx = 0;
    //    _newFloor.shop_equip_count = 4;
    //    _newFloor.shop_group_idx = 201;
    //    _newFloor.stage_type = 0;

    //    _DataTable.Insert(_newFloor);
    //}

    
}