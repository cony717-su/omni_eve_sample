using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right,
    };

    public enum StatType
    {
        Hp,
        Atk,
        Dex,
        Dodge,
        CriProb,
        CriFactor,
        CriDef,
        Def,
        Pierce,
        DamageMin,
        DamageMax,
    };

    public int hpMax { get; set; }
    public int hp { get;set; }
    public int atk { get; set; }
    public int dex { get; set; }
    public int dodge { get; set; }
    public int criProb { get; set; }
    public int criFactor { get; set; }
    public int criDef { get; set; }
    public int def { get; set; }
    public int pierce { get; set; }

    public int damageMin { get; set; }
    public int damageMax { get; set; }

    public Vector3Int tilePosition { get; set; }

    public int OmniEveGetStatValueByType(StatType type)
    {
        switch (type)
        {
            case StatType.Hp:
                return hp;
            case StatType.Atk:
                return atk;
            case StatType.Dex:
                return dex;
            case StatType.Dodge:
                return dodge;
            case StatType.CriProb:
                return criProb;
            case StatType.CriFactor:
                return criFactor;
            case StatType.CriDef:
                return criDef;
            case StatType.Def:
                return def;
            case StatType.Pierce:
                return pierce;
            case StatType.DamageMin:
                return damageMin;
            case StatType.DamageMax:
                return damageMax;
            default:
                return -1;
        }
    }

    public bool OmniEveIsDead()
    {
        return this.hp <= 0;
    }
}
