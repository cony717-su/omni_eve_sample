using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

using MySql.Data;
using MySql.Data.MySqlClient;

using Shiftup.CommonLib.Data;
using Shiftup.CommonLib.Data.Attributes;
using Shiftup.CommonLib.Logger;

using InnerDevToolCommon.Database;
using InnerDevToolCommon.Attributes;

using InnerDevTool.Data.Game;

namespace InnerDevTool.Data
{
    public class GameSql : MysqlDatabase
    {
        public GameSql(DBConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
            this.dbName = this.connectionInfo.GameDatabaseName;
            this.dbHostInfo = this.connectionInfo.GameDatabaseHostInfo;
        }

        public void AddDummyUser(ulong nfguid, string nickname)
        {
            /*string tableName = "";
            string query = "";

            var nfguidParam = new KeyValuePair<string, object>("@nfguid", nfguid);

            tableName = Util.GetShardingName("user_account", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, shortNfguid) VALUES (@nfguid, {1})", tableName, nfguid);
            this.QueryWithNoneReader(query, nfguidParam);

            tableName = Util.GetShardingName("user_session", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, loginTS, crypt, kickout_version, uuid) VALUES (@nfguid, now(), '', 18, '')", tableName);
            this.QueryWithNoneReader(query, nfguidParam);

            tableName = Util.GetShardingName("user_data", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, inventory_max, character_max, enable_push, friend_max, party_max, arena_total_win, arena_total_lose, summon_mileage, arena_mileage, synthesis_mileage, unit_buff_idx, underground_clear_type, underground_open_type, client_diary, tutorial_step, sub_tutorial_step, is_tutorial_summon, is_tutorial_upgrade, collection_score, collection_score_reward, free_levelup_package_reward, paid_levelup_package_reward, exp_boost_end_date, gold_boost_end_date, arena_enemy_refreshTS, raid_boost_updateTS)VALUES (@nfguid, 100, 100, 1, 20, 2, 0, 0, 0, 0, 0, 0, 0, 1, '', 1, '0', 0, 0, 0, 0, 0, 0, now(), now(), now(), now())", tableName);
            this.QueryWithNoneReader(query, nfguidParam);

            tableName = Util.GetShardingName("user_refill", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, stamina, stamina_max, staminaTS, arena_ticket, arena_ticket_max, arena_ticketTS, blue_stamina, blue_stamina_max, blue_staminaTS, chase_stamina, chase_stamina_max, chase_staminaTS, world_boss_serial_ticket, world_boss_serial_ticket_max, world_boss_serial_ticketTS) VALUES (@nfguid, 0, 5, now(), 0, 5, now(), 0, 10, now(), 0, 5, now(), 0, 6, now())", tableName);
            this.QueryWithNoneReader(query, nfguidParam);

            tableName = Util.GetShardingName("user_resource", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, paid_gem, free_gem, paid_blood_gem, free_blood_gem, paid_gold, free_gold, friend_point, onyx, arena_coin, arena_trophy, skin_coin, spa_coin, underground_coin, raid_coin, raid_boost, world_boss_serial_coin, costume_shop_reset) VALUES (@nfguid, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)", tableName);
            this.QueryWithNoneReader(query, nfguidParam);

            var characterManager = new Manager.CharacterManager(nfguid);
            var character = characterManager.InsertCharacterWithDummyUser(10100002, "c001_01", 3);
            characterManager.Commit();

            var partyManager = new Manager.PartyManager(nfguid);
            var firstPartyData = new PartyData();
            var secondPartyData = new PartyData();
            firstPartyData.idx = 1;
            firstPartyData.uid1 = character.uid;
            firstPartyData.leader_uid = character.uid;
            secondPartyData.idx = 2;
            partyManager.InsertParty(firstPartyData);
            partyManager.InsertParty(secondPartyData);

            tableName = Util.GetShardingName("user", nfguid);
            query = String.Format("INSERT INTO {0}(nfguid, nickname, level, exp, share_char, createTS) VALUES (@nfguid, @nickname, 1, 0, @share, now())", tableName);
            this.QueryWithNoneReader(query, nfguidParam, new KeyValuePair<string, object>("@nickname", nickname), new KeyValuePair<string, object>("@share", character.uid));*/
        }

        public void DeleteUser(ulong nfguid)
        {
            var sharding = nfguid % 32;
            var tables = this.cache.GetTables().Where(table => {
                var matches = new Regex(".*_(\\d{1,2})$").Match(table.Name);
                return matches.Groups[1].ToString().Equals(sharding.ToString());
            });
            foreach (var table in tables)
            {
                if (table.FieldInfos.Keys.Contains("nfguid"))
                {
                    this.DeleteTable(table.Name, nfguid);
                }
            }
        }

        public void InitializationDB()
        {
            var tables = this.cache.GetTables();
            foreach (var table in tables)
            {
                TruncateTable(table.Name);
            }
        }
    }

    public class MainSql : MysqlDatabase
    {
        public MainSql(DBConnectionInfo connectionInfo)
            : base(connectionInfo)
        {
            this.dbName = this.connectionInfo.MainDatabaseName;
            this.dbHostInfo = this.connectionInfo.MainDatabaseHostInfo;
        }

        public void AddDummyUser(ulong nfguid, string nickname)
        {
            /*string tableName = "user_nickname";
            string query = String.Format("INSERT INTO {0} VALUES (@nfguid, @nickname_unique, '', 0)", tableName);
            List<MySqlParameter> listParameter = new List<MySqlParameter>();
            listParameter.Add(new MySqlParameter() { ParameterName = "@nfguid", Value = nfguid });
            listParameter.Add(new MySqlParameter() { ParameterName = "@nickname_unique", Value = nickname });
            this.QueryWithNoneReader(query, listParameter);
            listParameter.Clear();*/
        }

        public void DeleteUser(ulong nfguid)
        {
            string[] tableList = new string[] {
                "user_nickname",
                "user_nickname_token",
                "arena_score",
                "arena_score_snapshot",
                "duel_score",
                "duel_score_snapshot",
                "duel_engarde_score",
                "duel_engarde_enemy",
                "special_raid_ranking",
                "special_raid_assistant_ranking",
                "world_boss_serial_daily_damage",
                "world_boss_serial_season_damage",
                "world_boss_serial_season_high_rank"
            };

            foreach (var tableName in tableList)
            {
                DeleteTable(tableName, nfguid);
            }
        }

        public void InitializationDB()
        {
            string[] tableList = new string[] {
                "user_nickname",
                "user_nickname_token",
                "arena_score",
                "arena_score_snapshot",
                "duel_score",
                "duel_score_snapshot",
                "duel_engarde_score",
                "duel_engarde_enemy",
                "special_raid_ranking",
                "special_raid_assistant_ranking",
                "world_boss_serial_daily_damage",
                "world_boss_serial_encounter_list",
                "world_boss_serial_season_damage",
                "world_boss_serial_season_high_rank"
            };

            foreach (var tableName in tableList)
            {
                TruncateTable(tableName);
            }
        }

        public ulong GetNfguid(string key)
        {
            ulong nfguid = 0;
            string tableName = "user_nickname";

            // by nfguid
            if (ulong.TryParse(key, out nfguid))
            {
                var nfguidParam = new KeyValuePair<string, object>("@nfguid", key);
                object valueNickname = QueryWithScalar(String.Format("SELECT nickname_unique FROM {0} WHERE nfguid = @nfguid", tableName), nfguidParam);
                if (valueNickname != null)
                {
                    return nfguid;
                }
            }

            // by nickname
            var nickname = new KeyValuePair<string, object>("@nickname", key);
            object valueNfguid = QueryWithScalar(String.Format("SELECT nfguid FROM {0} WHERE nickname_unique = @nickname", tableName), nickname);

            return Convert.ToUInt64(valueNfguid);
        }

        public string GetShortNfguid(ulong nfguid)
        {
            string tableName = "user_nickname";
            var nfguidParam = new KeyValuePair<string, object>("@nfguid", nfguid);
            object valueShortNfguid = QueryWithScalar(String.Format("SELECT shortNfguid FROM {0} WHERE nfguid = @nfguid", tableName), nfguidParam);
            if (valueShortNfguid != null)
            {
                return valueShortNfguid.ToString();
            }

            return string.Empty;
        }

        public bool IsQA()
        {
            return this.connectionInfo.DatabaseProfileName == "dc_game_QA";
        }
    }
}