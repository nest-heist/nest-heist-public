using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 피로도 시스템
/// 1분마다 피로도 1씩 증가, 구독한 UI에 피로도 변경 알림
/// </summary>
public class FatigueSystem
{
    public ServerUserFatigueData FatigueData { get; private set; }

    private Coroutine _fatigueUpCoroutine;
    public event Action OnFatigueChange;
    private const int FatigueUpSecond = 60;
    private int _tempFatigueUpSecond;
    private bool _isWaitTempSecond = true;

    public void Init()
    {
        FatigueData = DataManager.Instance.ServerDataSystem.UserFatigueData;

        StartFatigueUpCoroutine();
    }

    private void StartFatigueUpCoroutine()
    {
        _tempFatigueUpSecond = FatigueUpSecond - FatigueData.LastFatigueRecoverySeconds;

        if (_fatigueUpCoroutine == null)
        {
            _fatigueUpCoroutine = GameManager.Instance.StartCoroutine(FatigueUpCoroutine());
        }
        else
        {
            GameManager.Instance.StopCoroutine(_fatigueUpCoroutine);
            _fatigueUpCoroutine = GameManager.Instance.StartCoroutine(FatigueUpCoroutine());
        }
    }

    /// <summary>
    /// 피로도 클라에서 1분마다 증가, 서버 업데이트 X
    /// 로비에 가만히 있을 때 UI 업데이트 되야 한다 
    /// </summary>
    /// <returns></returns>
    IEnumerator FatigueUpCoroutine()
    {
        while (true)
        {
            if (_isWaitTempSecond)
            {
                yield return new WaitForSeconds(_tempFatigueUpSecond);
                _isWaitTempSecond = false;
            }
            else
            {
                yield return new WaitForSeconds(FatigueUpSecond);
            }

            if (FatigueData.CurrentFatigue < FatigueData.MaxFatigue)
            {
                FatigueData.CurrentFatigue++;
                OnFatigueChange?.Invoke();
            }
        }
    }

    /// <summary>
    /// 이미 던전 입장할 때 클라 데이터로 체크해서 나올 때는 깎기만 하면 된다. 
    /// </summary>
    /// <param name="needFatigue"></param>
    public async void UpdateFatigueOnServer(int needFatigueAmount)
    {
        await LoadingManager.Instance.RunWithLoading(async () =>
        {
            await DataManager.Instance.ServerDataSystem.UseFatigueAfterDungeon(
                new UpdateFatigueBody
                {
                    UserId = DataManager.Instance.ServerDataSystem.User.UserId,
                    Amount = needFatigueAmount
                }
                );
        },
        onLoaded: () =>
        {
            StartFatigueUpCoroutine();
            _isWaitTempSecond = true;

            OnFatigueChange?.Invoke();

            // Init에서 연결 해놨는데 한번 피로도 소모하고 20 이하로 보이는데 로직에서 체크를 못한다 다시 할당하기
            FatigueData = DataManager.Instance.ServerDataSystem.UserFatigueData;
        },
        onError: (message) =>
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, message, "UpdateFatigueOnServer", sceneName);
        });
    }

}
