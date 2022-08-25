using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;

public class StaticItemAddOptionGroup : RowData
{
	[PrimaryKey(1)]
	public int idx { get; set; }

	public int option_idx { get; set; }
	public int option_prob { get; set; }
}

public class StaticItemAddOptionGroupTable : IScriptableObject
{
    public InnerTable<StaticItemAddOptionGroup> _DataTable { get; set; }
}