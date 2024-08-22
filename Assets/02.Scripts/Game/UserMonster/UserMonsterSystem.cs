using System.Collections.Generic;
using UnityEngine;

public class UserMonsterSystem
{
    public Dictionary<string, UserMonster> UserMonsterList { get; private set; } = new Dictionary<string, UserMonster>();
    private List<ServerUserMonsterData> _serverUserMonsterList { get => DataManager.Instance.ServerDataSystem.UserMonsterDataList; }
    public List<UserMonster> PartyUserMonsterList { get; set; } = new List<UserMonster>();

    public bool AllPartnerMonsterDie = false;

    public void Init()
    {
        // 이벤트 구독
        DataManager.Instance.ServerDataSystem.OnHatchEgg += AddUserMonster;
        DataManager.Instance.ServerDataSystem.OnUpdateParty += (body) => { InitializePartyUserMonsterList(); };

        foreach (var serverMonster in _serverUserMonsterList)
        {
            AddUserMonster(serverMonster);
        }

        InitializePartyUserMonsterList();
    }

    private void InitializePartyUserMonsterList()
    {
        var partnerMonsterIds = DataManager.Instance.ServerDataSystem.User.PartnerMonsterIds;

        if (partnerMonsterIds == null || partnerMonsterIds.Length == 0)
        {
            return;
        }

        PartyUserMonsterList.Clear();
        foreach (var monsterId in partnerMonsterIds)
        {
            if (UserMonsterList.TryGetValue(monsterId, out UserMonster userMonster))
            {
                PartyUserMonsterList.Add(userMonster);
            }
        }
    }

    public void AddUserMonster(ServerUserMonsterData userData)
    {
        // 데이터 메니저 인스턴스 유무 체크, 딕셔너리 유무 체크, 딕셔너리 내 키 유무 체크
        if (DataManager.Instance == null ||
            string.IsNullOrEmpty(userData.MonsterId) ||
            !DataManager.Instance.ReadOnlyDataSystem.MonsterInfo.ContainsKey(userData.MonsterId))
        {
            return;
        }

        // 과정 경과를 체크
        MonsterInfoData monsterInfoData = DataManager.Instance.ReadOnlyDataSystem.MonsterInfo[userData.MonsterId];
        if (monsterInfoData == null)
        {
            return;
        }

        UserMonster newUserMonster = new UserMonster(DataManager.Instance.ReadOnlyDataSystem.MonsterAllStat[userData.MonsterId], userData);
        UserMonsterList[userData.Id] = newUserMonster;
    }

    public void LevelUpMonster(string userMonsterId)
    {
        if (UserMonsterList.ContainsKey(userMonsterId))
        {
            UserMonsterList[userMonsterId].LevelUp();
        }
    }

    public void IncreaseMonsterRank(string userMonsterId)
    {
        if (UserMonsterList.ContainsKey(userMonsterId))
        {
            UserMonsterList[userMonsterId].IncreaseRank();
        }
    }

    public void UpdateMonsterIV(string userMonsterId, int newIV)
    {
        if (UserMonsterList.ContainsKey(userMonsterId))
        {
            UserMonsterList[userMonsterId].UpdateIV(newIV);
        }
    }
    public List<UserMonster> GetSortedMonsters()
    {
        List<UserMonster> sortedMonsters = new List<UserMonster>();

        // 나머지 몬스터 추가
        foreach (var userMonsterPair in UserMonsterList)
        {
            UserMonster userMonster = userMonsterPair.Value;
            sortedMonsters.Add(userMonster);
        }

        return sortedMonsters;
    }

    public void UpdateLevelUpMonsterList(ServerUserMonsterData serverUserMonster, UserMonsterLevelUpBody body)
    {
        UserMonster userMonster = UserMonsterList[serverUserMonster.Id];

        userMonster.UserData.EXP = serverUserMonster.EXP;
        userMonster.UserData.Level = serverUserMonster.Level;

        // 재료 몬스터 제거
        foreach (var expMonsterId in body.expMonsterIds)
        {
            if (UserMonsterList.ContainsKey(expMonsterId))
            {
                UserMonsterList.Remove(expMonsterId);
            }
        }
    }
}
