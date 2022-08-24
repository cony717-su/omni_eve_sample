using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;
using System;

public class StaticItem : RowData
{
    [PrimaryKey(1)]
    public int idx { get; set; }
    
    public string view_idx { get; set; }
    public int rare_level { get; set; }
    public string name { get; set; }
    public int type { get; set; }
    public int category { get; set; }
    public int grade { get; set; }
    public int is_stack { get; set; }
    public int hp { get; set; }
    public int atk { get; set; }
    public int def { get; set; }
    public int agi { get; set; }
    public int cri { get; set; }
    public int enhancement_status_idx { get; set; }
    public int over_limit_status_idx { get; set; }
    public int ignition_tree_idx { get; set; }
    public int sell_price_type { get; set; }
    public int sell_price_count { get; set; }
    public int buy_dungeon_coin { get; set; }
    public int buy_arena_coin { get; set; }
    public int enable_library { get; set; }
    public DateTime library_open_date { get; set; }
    public int buy_spa_coin { get; set; }
    public int add_option { get; set; }
    public int option_1_group_idx { get; set; }
    public int option_2_group_idx { get; set; }
    public int option_3_group_idx { get; set; }
    public int enhancement { get; set; }
    public int new_tag { get; set; }
}

public class StaticItemTable : IScriptableObject
{
    public InnerTable<StaticItem> _DataTable { get; set; }
}