using Shiftup.CommonLib.Data.Attributes;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Common;
public class StaticOmniEveMob : RowData
{
    [PrimaryKey(1)]
    public int group_idx { get; set; }

    [PrimaryKey(2)]
    public int idx { get; set; }

    public int is_boss { get; set; }
    public string view_idx { get; set; }
    public string atk_effet { get; set; }
    public int level { get; set; }
    public int hp { get; set; }
    public int atk { get; set; }
    public int def { get; set; }
    public int dex { get; set; }
    public int damage_min { get; set; }
    public int damage_max { get; set; }
    public int cri_prob { get; set; }
    public int cri_factor { get; set; }
    public int cri_def { get; set; }
    public int dodge { get; set; }
    public int exp { get; set; }
    public int coin { get; set; }
    public int prob { get; set; }
    public int sight { get; set; }
    public int range { get; set; }
    public int pierce { get; set; }
    public int score { get; set; }
}
public class StaticOmniEveMobTable : IScriptableObject
{
    public InnerTable<StaticOmniEveMob> _DataTable { get; set; }
}