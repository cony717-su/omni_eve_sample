using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;

public class StaticCharacterSkin : RowData
{
    [PrimaryKey(1)]
    public int idx { get; set; }
    
    public int type { get; set; }
    public int sub_type { get; set; }
    public string view_idx { get; set; }
    public int is_voice { get; set; }
    public string name { get; set; }
}

public class StaticCharacterSkinTable : IScriptableObject
{
    public InnerTable<StaticOmniEveFloor> _DataTable { get; set; }
}