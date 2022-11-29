using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileData = TilemapGenerator.TileData;

public class CharacterManager : IManager<CharacterManager>
{
    private int _potionMaxCount = 999;
    private int _levelUpRandomCount = 3;
    private int _dangerHpPercentRatio = 10;

    private int scorePerFloorCount;
    private int scorePerLevelCount;
    private int scorePerItemGradeCount;
    private int scorePerTurnFloor;
    private int scorePerTurnCal;
    private int scorePerCoinCount;
    private int scorePerPotionCount;

    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    MonsterManager monsterManager;

    public class Player : Character
    {
        public int floor { get; set; }
        public int level { get; set; }
        public int exp { get; set; }
        public int expMax { get; set; }
        public int score { get; set; }
        public int turnCount { get; set; }

        public int hpLevelUp { get; set; }
        public int atkLevelUp { get; set; }
        public int dexLevelUp { get; set; }
        public int dodgeLevelUp { get; set; }
        public int criProbLevelUp { get; set; }
        public int criFactorLevelUp { get; set; }
        public int criDefLevelUp { get; set; }
        public int defLevelUp { get; set; }
        public int pierceLevelUp { get; set; }
        public int damageMinLevelUp { get; set; }
        public int damageMaxLevelUp { get; set; }

        public int coin { get; set; }
        public int equippedItem { get; set; }
        public int lastItemUid { get; set; }
        public List<int> inventoryList { get; set; }
        public int inventoryCount { get; set; }
        public int potionCount { get; set; }
        public int trap { get; set; }

        public bool isClear { get; set; }

        public Direction direction { get; set; }

        public void OmniEveSetStatLevelUpValueByType(StatType type, int newValue)
        {
            switch (type)
            {
                case StatType.Hp:
                    hpLevelUp = newValue;
                    return;
                case StatType.Atk:
                    atkLevelUp = newValue;
                    return;
                case StatType.Dex:
                    dexLevelUp = newValue;
                    return;
                case StatType.Dodge:
                    dodgeLevelUp = newValue;
                    return;
                case StatType.CriProb:
                    criProbLevelUp = newValue;
                    return;
                case StatType.CriFactor:
                    criFactorLevelUp = newValue;
                    return;
                case StatType.CriDef:
                    criDefLevelUp = newValue;
                    return;
                case StatType.Def:
                    defLevelUp = newValue;
                    return;
                case StatType.Pierce:
                    pierceLevelUp = newValue;
                    return;
                case StatType.DamageMin:
                    damageMinLevelUp = newValue;
                    return;
                case StatType.DamageMax:
                    damageMaxLevelUp = newValue;
                    return;
                default:
                    return;
            }
        }

        public int OmniEveGetLevelUpStatValueByType(StatType type)
        {
            switch (type)
            {
                case StatType.Hp:
                    return hpLevelUp;
                case StatType.Atk:
                    return atkLevelUp;
                case StatType.Dex:
                    return dexLevelUp;
                case StatType.Dodge:
                    return dodgeLevelUp;
                case StatType.CriProb:
                    return criProbLevelUp;
                case StatType.CriFactor:
                    return criFactorLevelUp;
                case StatType.CriDef:
                    return criDefLevelUp;
                case StatType.Def:
                    return defLevelUp;
                case StatType.Pierce:
                    return pierceLevelUp;
                case StatType.DamageMin:
                    return damageMinLevelUp;
                case StatType.DamageMax:
                    return damageMaxLevelUp;
                default:
                    return -1;
            }
        }

        public void OmniEveAddCoin(int coin)
        {
            this.coin += coin;
            if (this.coin <= 0)
            {
                this.coin = 0;
            }
        }
    }

    public Player player = new();

    public CharacterManager()
    {
        scorePerFloorCount = 1000;
        scorePerLevelCount = 700;
        scorePerItemGradeCount = 2000;
        scorePerTurnFloor = 200;
        scorePerTurnCal = 10;
        scorePerCoinCount = 10;
        scorePerPotionCount = 500;

        player.hpMax = 100;
        player.atk = 10;
        player.dex = 100;
        player.dodge = 100;
        player.criProb = 100;
        player.criFactor = 1000;
        player.criDef = 0;
        player.def = 2;
        player.pierce = 2;
        player.damageMin = 0;
        player.damageMax = 0;

        player.score = 0;
        player.turnCount = 0;
        player.floor = 1;
        player.level = 1;
        player.expMax = 100;
        player.exp = 0;
        player.hp = player.hpMax;

        player.hpLevelUp = 0;
        player.atkLevelUp = 0;
        player.dexLevelUp = 0;
        player.dodgeLevelUp = 0;
        player.criProbLevelUp = 0;
        player.criFactorLevelUp = 0;
        player.defLevelUp = 0;
        player.pierceLevelUp = 0;
        player.damageMinLevelUp = 0;
        player.damageMaxLevelUp = 0;

        player.coin = 0;
        player.potionCount = 0;
        player.equippedItem = 0;
        player.inventoryList = new();
        player.inventoryCount = 0;
        player.lastItemUid = 0;

        player.direction = Character.Direction.Right;

        player.isClear = false;
    }

    public bool OmniEveIsMovable(Vector3Int tilePosition, TilemapGenerator tilemapGenerator)
    {
        if (tilemapGenerator != null)
        {
            TileData t;
            t = tilemapGenerator.OmniEveGetTileDatabyRowColumn((int)tilePosition.x, (int)tilePosition.y);
            // DebugManager.Log($"Moving to {tilePosition}: {t.tileType}");
            switch (t.tileType)
            {
                case TileData.TileType.Floor:
                case TileData.TileType.Passage:
                case TileData.TileType.Stair:
                    return true;
                default:
                    return false;
            }
        }
        return false;
    }

    public void OmniEveSetCharacterTilePosition(Vector3Int tilePosition)
    {
        player.tilePosition = new Vector3Int(tilePosition.x, tilePosition.y, 0);
    }

    public Vector3Int OmniEveGetCharacterTilePosition()
    {
        return player.tilePosition;
    }
}
