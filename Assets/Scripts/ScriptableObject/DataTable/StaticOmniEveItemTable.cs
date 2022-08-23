using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;
public class StaticOmniEveItem : RowData
{
    [PrimaryKey(1)]
    public int idx { get; set; }

    public string view_idx { get; set; }
    public string name { get; set; }
    public int category { get; set; }
    public int grade { get; set; }
    public int sell_price { get; set; }
    public int buy_price { get; set; }
    public int damage_min { get; set; }
    public int damage_max { get; set; }
    public int hp { get; set; }
    public int atk { get; set; }
    public int dex { get; set; }
    public int pierce { get; set; }
    public int def { get; set; }
    public int cri_factor { get; set; }
    public int cri_prob { get; set; }
    public int cri_def { get; set; }
    public int dodge { get; set; }
}

public class StaticOmniEveItemTable : IScriptableObject
{
    public InnerTable<StaticOmniEveItem> _DataTable { get; set; }
}