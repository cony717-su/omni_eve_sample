using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;
using TileData = TilemapGenerator.TileData;

public class MonsterManager : IManager<MonsterManager>
{
    private Dictionary<int, Monster> monsterList;
    private int monsterCount;
    private int activatedBossMonsterId;
    private Dictionary<int, Monster> activatedBossMonsterList;

    CharacterManager characterManager;
    LevelManager levelManager;
    TilemapGenerator tilemapGenerator;

    public class Monster : IScriptableObject
    {
        public int id { get; set; }
        public int groupIdx { get; set; }
        public int idx { get; set; }

        public Vector3 prevTilePosition { get; set; }

        public bool isCharacterInSight;
        public bool isAlreadyAttack;
        public bool isWalking;

        private StaticOmniEveMob _mobData;

        public void Init(int idx)
        {
            _mobData = StaticManager.Instance.Get<StaticOmniEveMob>(idx);
            this.idx = idx;
            
        }

        public bool OmniEveIsCharacterInSight()
        {
            return isCharacterInSight;
        }

        public void OmniEveCheckCharacterInSight(Vector3Int characterTilePos)
        {
            if (isCharacterInSight) return;

            int sight = _mobData.sight;

        }

        public bool OmniEveIsCharacterInAttackRange(Vector3Int characterTilePos)
        {
            int range = _mobData.range;
            return true;
        }

        public bool OmniEveIsAlreadyAttack()
        {
            return isAlreadyAttack;
        }

        public bool OmniEveIsWalking()
        {
            return isWalking;
        }
    }

    public MonsterManager()
    {
        monsterList = new();
        monsterCount = 0;

        activatedBossMonsterId = 0;
        activatedBossMonsterList = new();

        characterManager = gameObject.GetComponent<CharacterManager>();
    }

    private void Update()
    {
        OmniEveCheckMonsterSight();
    }

    public void OmniEveAddMonsterToMonsterList(Monster monster)
    {
        if (monsterList == null)
        {
            monsterList = new();
        }
        monsterList[monster.id] = monster;
    }
        
    public bool OmniEveIsMonsterAllDead()
    {
        if (monsterCount > 0) return false;
        return true;
    }

    public Monster OmniEveGetMonsterByMonsterId(int id)
    {
        return monsterList[id];
    }

    public bool OmniEveIsCharacterInSight(Monster monster)
    {
        return true;
    }

    public void OmniEveCheckMonsterSight()
    {
        Vector3Int tilePos = characterManager.OmniEveGetCharacterTilePosition();
        foreach (KeyValuePair<int, Monster> kvp in monsterList)
        {
            kvp.Value.OmniEveCheckCharacterInSight(tilePos);
        }
    }

    public void GenerateMonster()
    {

    }
}

[CustomEditor(typeof(MonsterManager))]
public class MonsterGenerator : Editor
{
    private MonsterManager monsterManager;

    private void OnEnable()
    {
        monsterManager = FindObjectOfType<MonsterManager>();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Tilemap"))
        {
            // generate
            monsterManager.GenerateMonster();
        }
    }
}
