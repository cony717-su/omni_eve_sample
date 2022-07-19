using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InnerDevToolCommon.Common;

using InnerDevTool.Data.Main;
using InnerDevTool.Data.Game;

using Shiftup.CommonLib;
using Shiftup.CommonLib.Logger;

using Newtonsoft.Json;

namespace InnerDevTool.Data.Manager
{
    internal class OmniEveManager : UserDataManager
    {
        public UserSingleRow<OmniEveCharacterData> OmniEve { get; set; }

        public OmniEveManager(ulong nfguid)
            : base(nfguid)
        {
        }

        public override void ReadAllData()
        {
            SelectData(this.OmniEve);
        }

        public OmniEveJsonData GetJsonData()
        {
            if (this.OmniEve.Data != null)
            {
                return this.OmniEve.Data.ToJsonData();
            }

            return null;
        }

        public void InsertItem(StaticOmniEveItem item, int count = 1)
        {
            if (this.OmniEve.Data != null)
            {
                var jsonData = GetJsonData();

                OmniEveItemData newItem = new OmniEveItemData(false, item.idx, count);
                jsonData.character.inventory_list.Add(newItem);

                this.OmniEve.Data.data = jsonData.ToSerialize();
            }
        }

        public void OmniEveEquipItem(int selectedIndex)
        {
            if (this.OmniEve.Data != null)
            {
                var jsonData = GetJsonData();
                foreach (var item in jsonData.character.inventory_list.Select((value, index) => new { value, index }))
                {
                    if (selectedIndex == item.index) item.value.is_equipped = true;
                    else item.value.is_equipped = false;
                }

                this.OmniEve.Data.data = jsonData.ToSerialize();
            }
        }

        public void SetOmniEveCharacterData(int floor, int level, int hp, int exp, int turn_count, int coin, int score, int potion_count, int hp_level_up, int atk_level_up, int dex_level_up, int dodge_level_up, int cri_prob_level_up, int cri_factor_level_up, int cri_def_level_up, int def_level_up, int pierce_level_up, int damage_min_level_up, int damage_max_level_up)
        {
            if (this.OmniEve.Data != null)
            {
                var jsonData = GetJsonData();

                jsonData.character.floor = floor;
                jsonData.character.level = level;
                jsonData.character.hp = hp;
                jsonData.character.exp = exp;
                jsonData.character.turn_count = turn_count;
                jsonData.character.coin = coin;
                jsonData.character.score = score;
                jsonData.character.potion_count = potion_count;
                jsonData.character.hp_level_up = hp_level_up;
                jsonData.character.atk_level_up = atk_level_up;
                jsonData.character.dex_level_up = dex_level_up;
                jsonData.character.dodge_level_up = dodge_level_up;
                jsonData.character.cri_prob_level_up = cri_prob_level_up;
                jsonData.character.cri_factor_level_up = cri_factor_level_up;
                jsonData.character.cri_def_level_up = cri_def_level_up;
                jsonData.character.def_level_up = def_level_up;
                jsonData.character.pierce_level_up = pierce_level_up;
                jsonData.character.damage_min_level_up = damage_min_level_up;
                jsonData.character.damage_max_level_up = damage_max_level_up;

                this.OmniEve.Data.data = jsonData.ToSerialize();
            }
        }

        public void SetOmniEveItemData(List<OmniEveItemData> inventoryList)
        {
            if (this.OmniEve.Data != null)
            {
                var jsonData = GetJsonData();

                jsonData.character.inventory_list = inventoryList;

                this.OmniEve.Data.data = jsonData.ToSerialize();
            }
        }
    }
}