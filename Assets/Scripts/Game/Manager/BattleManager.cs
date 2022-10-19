using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : IManager<BattleManager>
{
    private int _defaultAccuracyRate = 800;

    void Start()
    {
        
    }

    private bool IsInProbability(int prob)
    {
        int value = Random.Range(0, 1000);
        if (value <= prob)
        {
            return true;
        }
        return false;
    }

    public int OmniEveCalculateAccuracyRate(int attackerLevel, int targetLevel, int attackerDex, int targetDodge)
    {
        return _defaultAccuracyRate + (attackerLevel - targetLevel) * 10 + attackerDex - targetDodge;
    }
}
