using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class DungeonSwitchingSystem
{
    private int _currentPartnerIndex;
    private string[] _partnerMonsterIds;
    private Dictionary<string, float> _monsterHealths = new Dictionary<string, float>();
    private HashSet<string> _deadMonsters = new HashSet<string>();
    private List<MonsterHealthSystem> _partnerMonstersHealthSystems = new List<MonsterHealthSystem>();
    private List<BaseMonster> _partnerMonsterBaseMonsterComponents = new List<BaseMonster>();
    private MonsterHealthSystem _currentPartnerMonsterHealthSystem;
    private IScene _dungeonScene;
    private IBattleManager _battleManager;

    public DungeonSwitchingSystem(IBattleManager dungeonBattleManager)
    {
        _battleManager = dungeonBattleManager;
        _partnerMonsterIds = DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds;
        _currentPartnerIndex = 0;
    }

    public void Init(IScene dungeonScene)
    {
        foreach (var partnerMonster in _battleManager.AllPartnerMonster)
        {
            _partnerMonsterBaseMonsterComponents.Add(partnerMonster.GetComponent<BaseMonster>());
        }

        foreach (var baseMonster in _partnerMonsterBaseMonsterComponents)
        {
            var healthSystem = baseMonster.HealthSystem;
            _partnerMonstersHealthSystems.Add(healthSystem);
            healthSystem.OnDeathEvent.AddListener(() => OnMonsterDeath(GetCurrentPartnerId()));
        }
        _dungeonScene = dungeonScene;
    }

    private void OnMonsterDeath(string monsterId)
    {
        _deadMonsters.Add(monsterId);
        int monsterIndex = Array.IndexOf(_partnerMonsterIds, monsterId);
        if (monsterIndex >= 0)
        {
            _dungeonScene.RemovePartyMonsterButtonClickListener(monsterIndex);
        }

        TrySwitchToNextPartner();
    }

    private void TrySwitchToNextPartner()
    {
        for (int i = 0; i < _partnerMonsterIds.Length; i++)
        {
            int nextIndex = (_currentPartnerIndex + i) % _partnerMonsterIds.Length;
            if (!_deadMonsters.Contains(_partnerMonsterIds[nextIndex]))
            {
                _dungeonScene.OnChangePartyMonster(nextIndex);
                return;
            }
        }
        
        GameManager.Instance.UserMonsterSystem.AllPartnerMonsterDie = true;
        UIManager.Instance.InstanciateUIPopup<UIPopup_DungeonRetryOrExit>();
    }

    public string GetCurrentPartnerId()
    {
        try
        {
            if (_partnerMonsterIds.Length == 0)
            {
                throw new InvalidOperationException("파트너 몬스터 배열이 없음");
            }
        }
        catch (InvalidOperationException ex) 
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName, _partnerMonsterIds);
            Logging.LogError(ex.Message);
        }
        

        return _partnerMonsterIds[_currentPartnerIndex];
    }

    public void SwitchPartnerMonster(int index)
    {
        if (_partnerMonsterIds.Length > index)
        {
            string newSummonMonsterId = _partnerMonsterIds[index];

            // 이미 죽은 몬스터인지 확인
            if (_deadMonsters.Contains(newSummonMonsterId))
            {
                return;
            }
            // 모든 몬스터가 죽었는지 확인
            if (index >= _partnerMonsterIds.Length)
            {
                return;
            }

            // 현재 파트너 몬스터의 체력을 저장
            string currentMonsterId = GetCurrentPartnerId();
            BaseMonster currentMonsterBase = _partnerMonsterBaseMonsterComponents[_currentPartnerIndex];
            if (currentMonsterBase != null)
            {
                _monsterHealths[currentMonsterId] = currentMonsterBase.HealthSystem.Current;
                _battleManager.AllPartnerMonster[_currentPartnerIndex].SetActive(false);
            }

            _currentPartnerIndex = index;
            _battleManager.SpawnPartnerMonster(index);
            _currentPartnerMonsterHealthSystem = _partnerMonstersHealthSystems[index];
        }
        else
        {
            Logging.LogError("index가 유효하지 않음");
        }
    }

    public float GetSavedHealth(string monsterId)
    {
        return _monsterHealths.ContainsKey(monsterId) ? _monsterHealths[monsterId] : 1f;
    }
}