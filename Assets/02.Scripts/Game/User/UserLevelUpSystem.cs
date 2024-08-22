using System.Collections.Generic;
using UnityEngine;

public class UserLevelUpSystem
{
    public Player Player;

    private ServerUserGameInfoData _userData;

    public int TempPrevLevel = -1;
    public int MaxLevel { get; private set; }

    public bool PlayerDie = false;

    public void Init()
    {
        GetMaxLevel();
    }

    private void GetMaxLevel()
    {
        Dictionary<int, PlayerLevelUpData> levelUpData = DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp;
        MaxLevel = 0;
        foreach (KeyValuePair<int, PlayerLevelUpData> data in levelUpData)
        {
            if (data.Key > MaxLevel)
            {
                MaxLevel = data.Key;
            }
        }
    }

    /// <summary>
    /// 퀘스트 경험치로 레벨업은 보낼 때 계산해서 넣어야 해서 버튼 누르면 계산하도록 한다
    /// 여기는 던전 깼을 때 리워드일 경우에만 사용한다
    /// </summary>
    /// <param name="expCount"></param>
    /// <param name="player"></param>
    public void UpExp(int expCount, Player player = null)
    {
        if (player != null)
        {
            Player = player;
        }

        if (expCount > 0)
        {
            // 플레이어 객체가 없으면 DataManager에서 플레이어 정보를 가져옴
            _userData = Player != null ? Player.UserData : DataManager.Instance.ServerDataSystem.User;
            _userData.Exp += expCount;

            LevelUpCheck();
        }
    }

    private void LevelUpCheck()
    {
        while (true)
        {
            if (_userData.Level == MaxLevel)
            {
                Logging.LogError("UserLevelUpSystem::LevelUpCheck - PlayerLevelUp is null 최고레벨");
                break;
            }

            if (_userData.Exp < DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_userData.Level].SumOfExp)
            {
                break;
            }

            // 이전 레벨 저장
            if (TempPrevLevel == -1)
            {
                TempPrevLevel = _userData.Level;
            }

            // 스텟 안 올려도 괜찮다 (플레이어 스텟 계산은 던전 입장시 계산)
            if (Player != null)
            {
                Player.UpdateLevelStat();
            }

            _userData.Exp -= DataManager.Instance.ReadOnlyDataSystem.PlayerLevelUp[_userData.Level].SumOfExp;
            _userData.Level++;

            Logging.Log($"UserLevelUpSystem::LevelUpCheck {_userData.Level}");

        }
        PostLevelUpToServer();
    }

    private async void PostLevelUpToServer()
    {
        await LoadingManager.Instance.RunWithLoading(async () =>
        {
            await DataManager.Instance.ServerDataSystem.PostLevelUp(new LevelUpBody
            {
                UserId = DataManager.Instance.ServerDataSystem.User.UserId,
                Exp = DataManager.Instance.ServerDataSystem.User.Exp,
                Level = DataManager.Instance.ServerDataSystem.User.Level
            });
        },
        onLoaded: () =>
        {
            TempPrevLevel = -1;
        }
        ,
        onError: (error) =>
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, error, "PostLevelUpToServer", sceneName);
        });
    }


}
