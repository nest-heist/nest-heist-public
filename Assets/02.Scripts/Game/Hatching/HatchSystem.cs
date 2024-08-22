using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부화기 리스트 관리
/// 인벤토리 영양제, 몬스터 리스트, 알 리스트에 접근해서 부화기에 넣어주는 역할 
/// </summary>
public class HatchSystem
{
    // 서버 데이터에 있는 거 그냥 갖고오기
    public List<ServerUserMonsterData> UserMonsterDataList { get => DataManager.Instance.ServerDataSystem.UserMonsterDataList; }
    public List<ServerUserEggData> UserEggDataList { get => DataManager.Instance.ServerDataSystem.UserEggDataList; }
    public List<ServerUserInventoryData> UserInventoryList { get => DataManager.Instance.ServerDataSystem.UserInventoryList; }
    public List<ServerIncubatorData> UserIncubatorList { get => DataManager.Instance.ServerDataSystem.UserIncubatorList; }

    public Dictionary<string, Incubator> IncubatorDic { get; private set; } = new Dictionary<string, Incubator>();

    public string IncubatorId { get; private set; }

    public Incubator CurrentIncubator { get => IncubatorDic[IncubatorId]; }

    public event Action OnRemainSecondUpdated;

    private Coroutine _countSecondCoroutine;

    public void Init()
    {
        foreach (ServerIncubatorData incubator in UserIncubatorList)
        {
            SetIncubator(incubator);
        }

        if (_countSecondCoroutine == null)
        {
            _countSecondCoroutine = GameManager.Instance.StartCoroutine(StartRemainSecondCoroutine());
        }
    }

    /// <summary>
    /// 여러 부화기 동시에 다 돌려야 해서 못 멈춘다 반복해서 1초마다 계속 실행된다
    /// </summary>
    private IEnumerator StartRemainSecondCoroutine()
    {
        while (true)
        {
            foreach (Incubator incubator in IncubatorDic.Values)
            {
                if (incubator.ServerData.UserEggId != Define.NoEgg && incubator.ServerData.RemainingTime > 0)
                {
                    incubator.ServerData.RemainingTime--;
                    OnRemainSecondUpdated?.Invoke();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 부화기 시작 누른 다음에 부화기 새로 만들어서 데이터 저장하기
    /// </summary>
    /// <param name="serverIncubatordata"></param>
    public void SetIncubator(ServerIncubatorData serverIncubatordata)
    {
        Incubator incubator = new Incubator(serverIncubatordata);
        IncubatorDic[serverIncubatordata.Id] = incubator;

        IncubatorId = serverIncubatordata.Id;
    }

    /// <summary>
    /// UI에서 알 선택하기
    /// </summary>
    /// <param name="selectIdx"></param>
    public void SelectedEgg(ServerUserEggData egg)
    {
        CurrentIncubator.ClientIData.TempEgg = egg;
    }

    /// <summary>
    /// 인벤토리에서 영양제 빼기
    /// 선택한 알 알 인벤토리에서 삭제
    /// </summary>
    /// <returns></returns>
    public void StartHatching(int smallNutrientCount, int mediumNutrientCount, int largeNutrientCount)
    {
        try
        {
            // --- 서버 처리 부분 클라에서도 처리 인벤토리, 알 인벤토리 ---
            ServerUserInventoryData small = UserInventoryList.Find(x => x.ItemId == Define.ItemSmallNutrient);
            ServerUserInventoryData medium = UserInventoryList.Find(x => x.ItemId == Define.ItemMediumNutrient);
            ServerUserInventoryData large = UserInventoryList.Find(x => x.ItemId == Define.ItemLargeNutrient);

            if(smallNutrientCount > 0 && small != null)
            {
                small.Quantity -= smallNutrientCount;
            }
            if(mediumNutrientCount > 0 && medium != null)
            {
                medium.Quantity -= mediumNutrientCount;
            }
            if(largeNutrientCount > 0 && large != null)
            {
                large.Quantity -= largeNutrientCount;
            }

            string UserEggId = CurrentIncubator.ServerData.UserEggId;
            ServerUserEggData egg = UserEggDataList.Find(x => x.Id == UserEggId);

            // 선택한 알 삭제
            if (egg == null)
            {
                return;
            }
            UserEggDataList.Remove(egg);
        }
        catch (Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }

    public void EndHatching(string incubatorId)
    {
        try
        {
            Incubator incubator = IncubatorDic[incubatorId];
            incubator.Reset();
        }
        catch (Exception ex)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }
}
