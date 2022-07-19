using System;
using System.Collections.Generic;
using System.Drawing;

using Shiftup.CommonLib.Data.Attributes;

using InnerDevToolCommon.Data;
using InnerDevToolCommon.Attributes;

using Newtonsoft.Json;

namespace InnerDevTool.Data.Game
{
    public class OmniEveItemData
    {
        //private Image icon = null;

        public bool is_equipped;
        public int count;
        public int item_idx;

        public OmniEveItemData(bool is_equipped, int item_idx, int count = 1)
        {
            this.is_equipped = is_equipped;
            this.count = count;
            this.item_idx = item_idx;
        }

        /*public Image Icon
        {
            get
            {
                if (this.icon == null)
                {
                    var staticOmniEveItem = DBLookup.Instance.StaticData.StaticOmniEveItemTable.Get(item_idx);
                    if (staticOmniEveItem != null)
                    {
                        string viewIdx = staticOmniEveItem.view_idx;
                        this.icon = ItemIconHelper.Instance.GetIcon(viewIdx);
                    }
                }

                return this.icon;
            }
        }*/

        public bool ShouldSerializeicon() { return false; }
        public bool ShouldSerializeIcon() { return false; }
    }

    public class OmniEveCharData
    {
        public int floor;
        public int level;
        public int hp;
        public int exp;

        public int turn_count;
        public int coin;
        public int score;
        public int potion_count;

        public int hp_level_up;
        public int atk_level_up;
        public int dex_level_up;
        public int dodge_level_up;
        public int cri_prob_level_up;
        public int cri_factor_level_up;
        public int cri_def_level_up;
        public int def_level_up;
        public int pierce_level_up;

        public int damage_min_level_up;
        public int damage_max_level_up;

        public List<OmniEveItemData> inventory_list;

        public OmniEveCharData() : this(1, 1, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, new List<OmniEveItemData>())
        {

        }

        public OmniEveCharData(int floor, int level, int hp, int exp, int turn_count, int coin, int score, int potion_count, int hp_level_up, int atk_level_up, int dex_level_up, int dodge_level_up, int cri_prob_level_up, int cri_factor_level_up, int cri_def_level_up, int def_level_up, int pierce_level_up, int damage_min_level_up, int damage_max_level_up, List<OmniEveItemData> inventory_list)
        {
            this.floor = floor;
            this.level = level;
            this.hp = hp;
            this.exp = exp;

            this.turn_count = turn_count;
            this.coin = coin;
            this.score = score;
            this.potion_count = potion_count;

            this.hp_level_up = hp_level_up;
            this.atk_level_up = atk_level_up;
            this.dex_level_up = dex_level_up;
            this.dodge_level_up = dodge_level_up;
            this.cri_prob_level_up = cri_prob_level_up;
            this.cri_factor_level_up = cri_factor_level_up;
            this.cri_def_level_up = cri_def_level_up;
            this.def_level_up = def_level_up;
            this.pierce_level_up = pierce_level_up;

            this.damage_min_level_up = damage_min_level_up;
            this.damage_max_level_up = damage_max_level_up;

            this.inventory_list = inventory_list;
        }

    }

    public class OmniEveJsonData
    {
        public OmniEveCharData character = new OmniEveCharData();

        public string ToSerialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class OmniEveCharacterData : ShardingRowData
    {
        public string data { get; set; }

        public OmniEveJsonData ToJsonData()
        {
            var jsonString = this.data.Replace("'", "");
            var convertData = JsonConvert.DeserializeObject<OmniEveJsonData>(jsonString);
            return convertData;
        }
    }
}
