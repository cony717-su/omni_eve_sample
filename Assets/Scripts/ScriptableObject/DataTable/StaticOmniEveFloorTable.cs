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
    
}