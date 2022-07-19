using System;
using System.Collections.Generic;
using System.Drawing;

using Shiftup.CommonLib.Data.Attributes;

using InnerDevToolCommon;
using InnerDevToolCommon.Data;
using InnerDevToolCommon.Attributes;

namespace InnerDevTool.Data.Main
{
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
}