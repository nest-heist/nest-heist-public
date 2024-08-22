using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 던전에 스폰하는 매니저
/// </summary>
public class DungeonSpawnSystem
{
    GameObject _monsterEggPrefab;
    int _padding = 2;
    private int _spawnedMonsterCount;
    private List<GameObject> _spawnedMonsters;
    private bool _isPhaseTwoTriggered = false;
    private IBattleManager _battleManager;

    public DungeonSpawnSystem(IBattleManager battleManager, bool IsDungeonBattle)
    {

        _spawnedMonsters = new List<GameObject>();
        _battleManager = battleManager;

        if(IsDungeonBattle == true)
        {
            for (int i = 0; i < _battleManager.MonsterProbabilityData.Count; i++)
            {
                string monsterId = _battleManager.MonsterProbabilityData[i].MonsterId;
                var monster = ResourceManager.Instance.Load<GameObject>($"Prefabs/Entities/Monster/{monsterId}");
                PoolManager.Instance.AddDynamicObjectToPool(monsterId, monster);
            }
        }
    }

    public GameObject SpawnPartner(Vector3 spawnPos, int index)
    {
        try
        {
            UserMonster partnerMonsterInfo = GameManager.Instance.UserMonsterSystem.UserMonsterList[DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds[index]];

            string monsterId = partnerMonsterInfo.UserData.MonsterId;
            GameObject monsterPrefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/Entities/Monster/{monsterId}");
            GameObject partnerMonster = Object.Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            
            ComponentAttacher.AttachParnterComponent(partnerMonster);
            partnerMonster.GetComponent<BaseMonster>().ComponentInit();
            SetPartnerMonsterStats(partnerMonster, partnerMonsterInfo);
            SetSkill(partnerMonster, partnerMonsterInfo.UserData.SkillId);
            partnerMonster.GetComponent<BaseMonster>().Init();

            return partnerMonster;
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
            return null;
        }
    }

    public GameObject SpawnPlayer(Vector3 spawnPos, GameObject partnerMonster)
    {
        try
        {
            string playerId = DataManager.Instance.ServerDataSystem.User.PlayerId;
            GameObject playerPrefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/Entities/Player/{playerId}");
            GameObject player = Object.Instantiate(playerPrefab, spawnPos, Quaternion.identity);

            _battleManager.VirtualCamera.Follow = player.transform;
            Player playerComponent = player.GetComponent<Player>();
            playerComponent.Partner = partnerMonster;
            playerComponent.StatHandler.SetBaseStat(DataManager.Instance.ReadOnlyDataSystem.PlayerBaseStat[playerId].ReturnStat());
            playerComponent.StatHandler.AddStat(DataManager.Instance.ReadOnlyDataSystem.PlayerLevelStat[playerId].ReturnStat().StatCalculator(StatCalculator.PlayerCalculateLevelStat, DataManager.Instance.ServerDataSystem.User.Level), StatType.Add);

            playerComponent.StatHandler.UpdateStat();
            playerComponent.Health.MonsterId = DataManager.Instance.ServerDataSystem.User.PlayerId;
            return player;
        }
        catch(System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
            return null;
        }
    }

    public List<GameObject> SpawnEnemy(List<RectInt> rooms)
    {
        try
        {
            List<GameObject> enemyMonsterList = new List<GameObject>();
            int spawnNum = _battleManager.InfoData.MonsterSpawnNum;
            float totalProbability = CalculateTotalProbability();
            Dictionary<RectInt, int> roomSpawnCount = CalculateRoomSpawnCounts(rooms, spawnNum);

            foreach (var room in rooms)
            {
                SpawnMonstersInRoom(room, roomSpawnCount[room], totalProbability, enemyMonsterList);
            }
            return enemyMonsterList;
        }
        catch(System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, new Object());
            Logging.LogError(ex.Message);
            return null;
        }
    }

    private void SpawnMonstersInRoom(RectInt room, int spawnCount, float totalProbability, List<GameObject> monsterList)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            string monsterId = SelectMonsterByProbability(totalProbability);
            var monsterPrefab = PoolManager.Instance.GetDynamicGameObject(monsterId);

            Vector2Int roomSpawnPos = CalculateRoomSpawnPosition(room);
            monsterPrefab.transform.position = new Vector2(roomSpawnPos.x, roomSpawnPos.y);
            monsterPrefab.transform.rotation = Quaternion.identity; 
            
            ComponentAttacher.AttachEnemyComponent(monsterPrefab);


            // 에드컴포넌트를 통하여 드랍 컴포넌트를 부착
            monsterPrefab.GetComponent<BaseMonster>().ComponentInit();
            SetEnemyMonsterStats(monsterPrefab, monsterId);
            SetSkill(monsterPrefab, "MSK00001");
            monsterPrefab.GetComponent<BaseMonster>().Init();
            monsterPrefab.GetComponent<MonsterDeathDropChaser>().Initialize(_battleManager.DropSystem, monsterId);
            monsterList.Add(monsterPrefab);
        }
    }

    public void SpawnMonstersInRoomWithEgg(RectInt room, int numberOfMonsters)
    {
        for (int i = 0; i < numberOfMonsters; i++)
        {
            string monsterId = SelectMonsterByProbability(CalculateTotalProbability());
            var monsterPrefab = PoolManager.Instance.GetDynamicGameObject(monsterId);

            // 위치 초기화
            monsterPrefab.transform.position = Vector3.zero;
            monsterPrefab.transform.rotation = Quaternion.identity;

            Vector2Int spawnPos = CalculateRoomSpawnPosition(room);
            monsterPrefab.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);

            ComponentAttacher.AttachEnemyComponent(monsterPrefab);
            monsterPrefab.GetComponent<BaseMonster>().ComponentInit();
            SetEnemyMonsterStats(monsterPrefab, monsterId);
            SetSkill(monsterPrefab, "MSK00001");

            monsterPrefab.GetComponent<BaseMonster>().Init();
            monsterPrefab.GetComponent<BaseMonster>().MonsterUI.HealthBar.Init();
            monsterPrefab.GetComponent<MonsterDeathDropChaser>().Initialize(_battleManager.DropSystem, monsterId);
            //  monster.GetComponent<BaseMonster>().HealthSystem.OnDeathEvent.AddListener(() => HandleMonsterDeath());
            _spawnedMonsters.Add(monsterPrefab);
            _spawnedMonsterCount = _spawnedMonsters.Count;
        }
    }

    private void SetSkill(GameObject monster, string skillId)
    {
        SkillHandler skillHandler = monster.GetComponent<SkillHandler>();
        skillHandler.Init(skillId);
    }

    private void SetPartnerMonsterStats(GameObject monster, UserMonster partnerMonster)
    {
        BaseMonster baseMonster = monster.GetComponent<BaseMonster>();
        baseMonster.StatHandler.SetBaseStat(partnerMonster.BaseStat);

        baseMonster.StatHandler.AddStat(partnerMonster.IVStat.StatCalculator(StatCalculator.CalculateIVStat, partnerMonster.UserData.IV), StatType.Add);
        baseMonster.StatHandler.AddStat(partnerMonster.RankStat.StatCalculator(StatCalculator.CalculateRankStat, partnerMonster.UserData.Rank), StatType.Add);
        baseMonster.StatHandler.AddStat(partnerMonster.LevelStat.StatCalculator(StatCalculator.CalculateLevelStat, partnerMonster.UserData.Level), StatType.Add);

        baseMonster.StatHandler.UpdateStat();
        baseMonster.StatHandler.Init();

        baseMonster.HealthSystem.MonsterId = partnerMonster.UserData.MonsterId;
        baseMonster.HealthSystem.Init();
        #region 
        //BaseMonster baseMonster = monster.GetComponent<BaseMonster>();
        //MonsterStat temp = partnerMonster.BaseStat;
        //temp.MaxHP = 1000000;

        //baseMonster.StatHandler.SetBaseStat(temp);

        //baseMonster.StatHandler.AddStat(partnerMonster.IVStat.StatCalculator(StatCalculator.CalculateIVStat, partnerMonster.UserData.IV), StatType.Add);
        //baseMonster.StatHandler.AddStat(partnerMonster.RankStat.StatCalculator(StatCalculator.CalculateRankStat, partnerMonster.UserData.Rank), StatType.Add);
        //baseMonster.StatHandler.AddStat(partnerMonster.LevelStat.StatCalculator(StatCalculator.CalculateLevelStat, partnerMonster.UserData.Level), StatType.Add);

        //baseMonster.StatHandler.UpdateStat();
        #endregion
    }

    public GameObject SpawnMonsterEgg(List<RectInt> rooms, string dungeonId)
    {
        try
        {
            // 랜덤 Egg생성
            string selectedEggId = SelectRandomEgg(dungeonId);

            // 알 소환 위치
            RectInt randomRoom = rooms[Random.Range(0, rooms.Count)];
            Vector2Int eggSpawnPos = CalculateRoomSpawnPosition(randomRoom);

            if (_monsterEggPrefab == null || _monsterEggPrefab.name != selectedEggId)
            {
                _monsterEggPrefab = ResourceManager.Instance.Load<GameObject>($"Prefabs/Entities/MonsterEgg/{selectedEggId}");
            }
            // Instantiate egg
            GameObject monsterEgg = Object.Instantiate(_monsterEggPrefab, new Vector2(eggSpawnPos.x, eggSpawnPos.y), Quaternion.identity);
            MonsterEgg eggComponent = monsterEgg.GetComponent<MonsterEgg>();
            eggComponent.SetEggId(selectedEggId);

            return monsterEgg;
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, new Object());
            Logging.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 던전 알 데이터에서 랜덤으로 알 선택하기
    /// </summary>
    /// <param name="dungeonId"></param>
    /// <returns></returns>
    private string SelectRandomEgg(string dungeonId)
    {
        List<DungeonEggProbabilityData> eggProbabilities = DataManager.Instance.ReadOnlyDataSystem.DungeonEggProbability[dungeonId];
        float totalProbability = eggProbabilities.Sum(e => e.Probability);
        float randomValue = UnityEngine.Random.Range(0f, totalProbability);

        float sumProbability = 0f;
        foreach (DungeonEggProbabilityData egg in eggProbabilities)
        {
            sumProbability += egg.Probability;
            if (randomValue <= sumProbability)
            {
                return egg.EggId; // 선택된 알의 ID를 반환
            }
        }

        // 리스트에 데이터 없어서 선택된 알이 없을 경우
        return Define.SlimeEggId;
    }

    private void SetEnemyMonsterStats(GameObject monster, string monsterId)
    {
        BaseMonster baseMonster = monster.GetComponent<BaseMonster>();
        MonsterAllStatData allStat = DataManager.Instance.ReadOnlyDataSystem.MonsterAllStat[monsterId];

        try
        {
            baseMonster.StatHandler.SetBaseStat(allStat.BaseStat.ReturnStat());

            baseMonster.StatHandler.AddStat(allStat.IVStat.ReturnStat().StatCalculator(StatCalculator.CalculateIVStat, Define.DungeonSpawnEnemyIV), StatType.Add);
            baseMonster.StatHandler.AddStat(allStat.RankStat.ReturnStat().StatCalculator(StatCalculator.CalculateRankStat, Define.DungeonSpawnEnemyRank), StatType.Add);
            int lv = Random.Range(_battleManager.InfoData.MinLevel, _battleManager.InfoData.MaxLevel);
            baseMonster.StatHandler.AddStat(allStat.LevelStat.ReturnStat().StatCalculator(StatCalculator.CalculateLevelStat, lv), StatType.Add);

            baseMonster.Lv = lv;

            baseMonster.StatHandler.UpdateStat();
            baseMonster.StatHandler.Init();
            baseMonster.HealthSystem.MonsterId = monsterId;
            baseMonster.HealthSystem.Init();
        }
        catch (System.Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, new Object());
            Logging.LogError(ex.Message);
        }
    }

    private float CalculateTotalProbability()
    {
        float totalProbability = 0f;
        _battleManager.MonsterProbabilityData.ForEach(m => { totalProbability += m.Probability; });
        return totalProbability;
    }

    private Dictionary<RectInt, int> CalculateRoomSpawnCounts(List<RectInt> rooms, int spawnNum)
    {
        int totalRoomArea = rooms.Sum(room => room.width * room.height);
        Dictionary<RectInt, int> roomSpawnCount = new Dictionary<RectInt, int>();

        int remainingSpawnNum = spawnNum;
        foreach (var room in rooms)
        {
            int roomArea = room.width * room.height;
            int spawnCountForRoom = Mathf.RoundToInt((float)roomArea / totalRoomArea * spawnNum);
            roomSpawnCount[room] = spawnCountForRoom;
            remainingSpawnNum -= spawnCountForRoom;
        }

        if (remainingSpawnNum > 0)
        {
            var largestRoom = rooms.OrderByDescending(room => room.width * room.height).First();
            roomSpawnCount[largestRoom] += remainingSpawnNum;
        }

        return roomSpawnCount;
    }

    private Vector2Int CalculateRoomSpawnPosition(RectInt room)
    {
        int minX = room.x + _padding;
        int maxX = room.x + room.width - _padding;
        int minY = room.y + _padding;
        int maxY = room.y + room.height - _padding;

        int spawnX = Random.Range(minX, maxX);
        int spawnY = Random.Range(minY, maxY);

        return new Vector2Int(spawnX, spawnY);
    }

    private string SelectMonsterByProbability(float totalProbability)
    {
        float randomPoint = Random.value * totalProbability;
        string selectedMonsterId = _battleManager.MonsterProbabilityData[_battleManager.MonsterProbabilityData.Count - 1].MonsterId;

        foreach (DungeonMonsterProbabilityData monster in _battleManager.MonsterProbabilityData)
        {
            if (randomPoint < monster.Probability)
            {
                return monster.MonsterId;
            }
            else
            {
                randomPoint -= monster.Probability;
            }
        }
        return selectedMonsterId;
    }
}
