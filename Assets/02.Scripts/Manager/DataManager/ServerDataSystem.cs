using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class ServerDataSystem
{
    public ApiService API { get; private set; }
    private RequestQueueSystem _requestQueueSystem = new RequestQueueSystem();


    public ServerUserGameInfoData User;
    public List<ServerUserMonsterData> UserMonsterDataList;
    public List<ServerUserEggData> UserEggDataList;
    public List<ServerUserInventoryData> UserInventoryList;
    public List<ServerUserDungeonProgressData> UserDungeonProgressList;
    public List<ServerIncubatorData> UserIncubatorList;
    public ServerUserFatigueData UserFatigueData;
    public List<ServerCompletedQuestData> CompletedQuestDataList;
    public List<ServerSubtaskData> SubtaskList;
    public List<ServerWinnerData> WinnerList;

    private bool _isAlreadyInit = false;

    public event Action<UpdatePartnerMonsterBody> OnUpdatePartnerMonster;
    public event Action<UserMonsterLevelUpBody> OnUserMonsterLevelUp;
    public event Action<ServerIncubatorData> OnStartHatching;
    public event Action<ServerUserMonsterData> OnHatchEgg;
    public event Action<ServerUserDungeonProgressData> OnUpdateDungeonStage;
    public event Action<ServerUserGameInfoData> OnUpdateParty;
    public event Action<ServerUserFatigueData> OnUseFatigue;
    public event Action OnHatchWin;
    public event Action OnGetWinners;

    public void Init()
    {
        API = new ApiService();
    }

    public async Task InitData()
    {
        try
        {
            if (!_isAlreadyInit)
            {
                _isAlreadyInit = true;
                await API.PostData<object, object>($"/user/sign-in", new object());
                User = await API.GetData<ServerUserGameInfoData>($"/user-game-info/{"UID0000001"}");
                WinnerList = await API.GetData<List<ServerWinnerData>>($"/user/winners");
                UserMonsterDataList = await API.GetData<List<ServerUserMonsterData>>($"/user-monster/user/{User.UserId}");
                UserInventoryList = await API.GetData<List<ServerUserInventoryData>>($"/user-inventory/{User.UserId}");
                UserEggDataList = await API.GetData<List<ServerUserEggData>>($"/user-egg/{User.UserId}");
                UserIncubatorList = await API.GetData<List<ServerIncubatorData>>($"/user-incubator/{User.UserId}");
                UserDungeonProgressList = await API.GetData<List<ServerUserDungeonProgressData>>($"/user-dungeon-progress/{User.UserId}");
                UserFatigueData = await API.GetData<ServerUserFatigueData>($"/user-game-info/current-fatigue/{User.UserId}");
                CompletedQuestDataList = await API.GetData<List<ServerCompletedQuestData>>($"/quest/completed/{User.UserId}");
                SubtaskList = await API.GetData<List<ServerSubtaskData>>($"/quest/task/{User.UserId}");
            }
            else
            {
                await Task.CompletedTask;
            }
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"LoadAllData Error : {e.Message}", e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    public async Task GetWinners()
    {
        try
        {
            await API.GetData<List<ServerWinnerData>>($"/user/winners",
            onSuccess: (obj) =>
            {
                WinnerList = obj;
                OnGetWinners?.Invoke();
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"/user/winners Error : {e.Message}", e.StackTrace, sceneName);
            Logging.LogError(e.Message);
        }
    }

    public async Task UpdatePartnerMonster(UpdatePartnerMonsterBody body)
    {
        try
        {
            await API.PostData<object, UpdatePartnerMonsterBody>("/user-game-info/update-partner-monster", body, onSuccess: (obj) =>
            {
                User.PartnerMonsterId = body.partnerMonsterId;
                OnUpdatePartnerMonster?.Invoke(body);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"UpdatePartnerMonster Error : {e.Message}", e.StackTrace, sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    public async Task UserMonsterLevelUp(UserMonsterLevelUpBody body)
    {
        try
        {
            await API.PostData<UserMonsterLevelUpBody, ServerUserMonsterData>("/user-monster/level-up", body,
            onSuccess: (obj) =>
            {
                GameManager.Instance.UserMonsterSystem.LevelUpMonster(obj.Id);
                GameManager.Instance.UserMonsterSystem.UpdateLevelUpMonsterList(obj, body);
                OnUserMonsterLevelUp?.Invoke(body);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"UserMonsterLevelUp Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    public async Task StartHatchEgg(StartHatchingBody body)
    {
        try
        {
            await API.PostData<StartHatchingBody, ServerIncubatorData>("/user-incubator/start-hatching", body,
            onSuccess: (obj) =>
            {
                GameManager.Instance.HatchingSystem.SetIncubator(obj);

                OnStartHatching?.Invoke(obj);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception ex)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"StartHatchEgg Error {ex.Message}", ex.StackTrace, sceneName, body);
            Logging.LogError(ex.Message);
        }
    }

    public async Task EndHatchEgg(HatchEggBody body)
    {
        try
        {
            await API.PostData<HatchEggBody, ServerUserMonsterAndWinData>("/user-incubator/hatch-egg", body,
            onSuccess: (obj) =>
            {
                OnHatchEgg?.Invoke(obj);

                if (obj.IsWin)
                {
                    OnHatchWin?.Invoke();
                }
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, $"EndHatchEgg Error {e.Message}", e.StackTrace, sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    public async Task UpdateDungeonStage(UpdateStageBody body)
    {
        try
        {
            await API.PostData<UpdateStageBody, ServerUserDungeonProgressData>("/user-dungeon-progress/update-stage", body,
            onSuccess: (obj) =>
            {
                OnUpdateDungeonStage?.Invoke(obj);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"UpdateDungeonStage Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    public async Task UpdateParty(UpdatePartyBody body)
    {
        try
        {
            await API.PostData<UpdatePartyBody, ServerUserGameInfoData>("/user-game-info/update-party", body,
            onSuccess: (obj) =>
            {
                User.PartnerMonsterIds = obj.PartnerMonsterIds;
                OnUpdateParty?.Invoke(obj);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"UpdateParty Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    public async Task UseFatigueAfterDungeon(UpdateFatigueBody body)
    {
        try
        {
            await API.PostData<UpdateFatigueBody, ServerUserFatigueData>("/user-game-info/use-fatigue", body,
            onSuccess: (obj) =>
            {
                UserFatigueData = obj;

                OnUseFatigue?.Invoke(obj);
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"UseFatigueAfterDungeon Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }

    }

    public void UpdateSubtaskCurrenSuccessValue(UpdateValueBody body)
    {
        try
        {
            _requestQueueSystem.EnqueueRequest(async () =>
            {
                await API.PostData<UpdateValueBody, ServerSubtaskData>("/quest/task/update-value", body,
                onSuccess: (obj) =>
                {
                    ServerSubtaskData data = SubtaskList.Find(x => x.Id == obj.Id);
                    if (data == null)
                    {
                        SubtaskList.Add(obj);
                    }

                    data = obj;
                },
                onFailure: (message) =>
                {
                    throw new Exception(message);
                });
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"UpdateSubTaskCurrentSuccessValue Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    /*/// <summary>
    /// 버튼 눌리면 퀘스트 완료 서버에 보내기
    /// 사용 X, 퀘스트 + 리워드 + 경험치로 통합
    /// </summary>
    /// <returns></returns>
    public async Task CompleteQuest(AddCompletedBody body)
    {
        await API.PostData<AddCompletedBody, ServerCompletedQuestData>("/quest/add-completed", body,
        onSuccess: (obj) =>
        {
            CompletedQuestDataList.Add(obj);
        },
        onFailure: (message) =>
        {
            Debug.LogError(message);
        });
    }*/

    /// <summary>
    /// 아이템, 알 얻은 결과물 모아서 서버에 보내기
    /// 던전 리워드에서 사용
    /// </summary>
    /// <returns></returns>
    public async Task PostReward(RewardBody body)
    {
        try
        {
            await API.PostData<RewardBody, AddedServerUserEggData>("/user/reward", body,
            onSuccess: (obj) =>
            {
                if (obj != null)
                {
                    UserEggDataList.Add(obj.AddedEgg as ServerUserEggData);
                }
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"PostReward Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }


    /// <summary>
    /// 레벨업 경험치 서버에 보내기
    /// 던전 리워드에서 사용가능
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task PostLevelUp(LevelUpBody body)
    {
        try
        {
            await API.PostData<LevelUpBody, ServerUserData>("/user-game-info/level-up", body,
            onSuccess: (obj) =>
            {

            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"PostLevelUp Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }

    /// <summary>
    /// 퀘스트 완료 + 리워드 + 경험치 서버에 보내기
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task PostCompletedAndReward(CompletedAndRewardBody body)
    {
        try
        {
            await API.PostData<CompletedAndRewardBody, ServerAddedReward>("/quest/completed-and-reward", body,
            onSuccess: (obj) =>
            {
                if (obj.AddedEgg != null)
                {
                    ServerUserEggData egg = new ServerUserEggData
                    {
                        Id = obj.AddedEgg.Id,
                        UserId = obj.AddedEgg.UserId,
                        EggId = obj.AddedEgg.EggId,
                        AcquiredAt = obj.AddedEgg.AcquiredAt
                    };

                    UserEggDataList.Add(egg);
                }
            },
            onFailure: (message) =>
            {
                throw new Exception(message);
            });
        }
        catch (Exception e)
        {
            string sceneName = "ServerDataSystem";
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, $"PostCompletedAndReward Error {e.StackTrace}", sceneName, body);
            Logging.LogError(e.Message);
        }
    }
}

