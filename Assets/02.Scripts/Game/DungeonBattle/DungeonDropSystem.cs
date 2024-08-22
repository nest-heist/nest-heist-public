using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 드랍 매니저, 드랍로직과, 사용되는 데이터를 관리
/// </summary>
public class DungeonDropSystem
{
    private DataManager _dataManager;
    public List<ServerUserInventoryData> CurrentItemList;
    public List<ServerUserInventoryData> AddItemList;
    public List<ServerUserEggData> TempEggDataList;
    public List<ServerUserInventoryData> CurrentDropTempList;
    public IBattleManager BattleManager;
    private GameObject _player;

    public DungeonDropSystem(IBattleManager battleManager)
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string uid = DataManager.Instance.ServerDataSystem.User.UserId;
        try
        {
            _dataManager = DataManager.Instance;
            if (_dataManager == null) { throw new InvalidOperationException("DataManager instance is null."); }
            BattleManager = battleManager;
            if (BattleManager == null) { throw new ArgumentNullException("BattleManager instance is null."); }
            CurrentItemList = DeepCopyList(_dataManager.ServerDataSystem.UserInventoryList);
            AddItemList = new List<ServerUserInventoryData>();
            TempEggDataList = new List<ServerUserEggData>();
        }
        catch (InvalidOperationException e)
        {
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName, _dataManager);
            Logging.LogError(e.Message);
        }
        catch (ArgumentNullException e)
        {
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, e.Message, e.StackTrace, sceneName, BattleManager);
            Logging.LogError(e.Message);
        }
        catch (Exception ex)
        {
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, ex.Message, ex.StackTrace, sceneName);
            Logging.LogError(ex.Message);
        }
    }

    // 임시 리스트를 초기화하는 메서드
    public void ClearTempLists()
    {
        AddItemList.Clear();
        TempEggDataList.Clear();
    }
    private List<ServerUserInventoryData> DeepCopyList(List<ServerUserInventoryData> originalList)
    {
        List<ServerUserInventoryData> newList = new List<ServerUserInventoryData>();
        foreach (var item in originalList)
        {
            ServerUserInventoryData newItem = new ServerUserInventoryData
            {
                Id = item.Id,
                UserId = item.UserId,
                ItemType = item.ItemType,
                ItemId = item.ItemId,
                Quantity = item.Quantity,
                AcquiredAt = item.AcquiredAt
            };
            newList.Add(newItem);
        }
        return newList;
    }

    public void AddDropItemsToInventory(string monsterId, Vector2 dropPosition)
    {
        if (!_dataManager.ReadOnlyDataSystem.MonsterDropItem.ContainsKey(monsterId))
        {
            Logging.LogError("일치하는 몬스터의 ID가 없음");
            return;
        }

        List<MonsterDropItemData> dropItemList = _dataManager.ReadOnlyDataSystem.MonsterDropItem[monsterId];

        foreach (var dropItem in dropItemList)
        {
            // 랜덤 값과 비교
            if (UnityEngine.Random.value <= dropItem.Probability)
            {
                // 수량을 MinNum과 MaxNum 사이에서 랜덤으로 결정
                int quantity = UnityEngine.Random.Range(dropItem.MinNum, dropItem.MaxNum + 1);

                bool itemExists = false;

                foreach (var inventoryData in CurrentItemList)
                {
                    if (inventoryData.ItemId == dropItem.ItemId)
                    {
                        // 이미 존재하는 아이템이면 수량 증가
                        inventoryData.Quantity += quantity;
                        itemExists = true;
                        break;
                    }
                }

                if (!itemExists)
                {
                    // 존재하지 않으면 새로운 인벤토리 데이터 추가
                    ServerUserInventoryData newInventoryData = new ServerUserInventoryData
                    {
                        // 고유 ID 생성
                        Id = Guid.NewGuid().ToString(),

                        // TODO : 서버데이터에서 가져오기
                        UserId = DataManager.Instance.ServerDataSystem.User.UserId,

                        // TODO : 아이템 타입 반영 로직 변경할 것
                        ItemType = DataManager.Instance.ReadOnlyDataSystem.ItemInfo[dropItem.ItemId].Type,

                        ItemId = dropItem.ItemId,
                        Quantity = quantity,
                        AcquiredAt = ""
                    };
                    CurrentItemList.Add(newInventoryData);
                }

                AddItemList.Add(new ServerUserInventoryData
                {
                    ItemId = dropItem.ItemId,
                    Quantity = quantity,
                });

                int effectCount = quantity;
                if (dropItem.ItemId == "ITE00001")
                {
                    effectCount = Mathf.CeilToInt(quantity / 100f);
                }

                for (int i = 0; i < effectCount; i++)
                {
                    ActivateItemDropEffect(dropItem.ItemId, dropPosition);
                }
            }
        }
    }

    public void EscapeDungeonGetItem()
    {
        _dataManager.ServerDataSystem.UserInventoryList = DeepCopyList(CurrentItemList);
        ClearTempLists();
        CurrentItemList.Clear();
    }

    public async void ItemAndEggDataOnServer()
    {
        Item[] items = new Item[AddItemList.Count];
        if (AddItemList.Count != 0)
        {
            for (int i = 0; i < AddItemList.Count; i++)
            {
                items[i] = new Item
                {
                    ItemId = AddItemList[i].ItemId,
                    Quantity = AddItemList[i].Quantity
                };
            }
        }

        RewardBody rewardBody = new RewardBody
        {
            UserId = DataManager.Instance.ServerDataSystem.User.UserId,
            EggId = TempEggDataList[0].EggId,
            Items = items
        };

        await LoadingManager.Instance.RunWithLoading(async () =>
        {
            await DataManager.Instance.ServerDataSystem.PostReward(
                rewardBody);
        },
        onLoaded: () =>
        {
            EscapeDungeonGetItem();
        },
        onError: (error) =>
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string uid = DataManager.Instance.ServerDataSystem.User.UserId;
            FirebaseManager.Instance.DBSystem.SendErrorLog(uid, error, "Reward API", sceneName, rewardBody);
        });
    }

    private void ActivateItemDropEffect(string itemId, Vector2 dropPosition)
    {
        var dropEffectPrefab = PoolManager.Instance.GetGameObject(itemId);
        if (dropEffectPrefab != null)
        {
            Vector2 randomizedDropPosition = dropPosition + UnityEngine.Random.insideUnitCircle * 1f;
            dropEffectPrefab.transform.position = randomizedDropPosition;
            dropEffectPrefab.transform.rotation = Quaternion.identity;

            var effectInstance = dropEffectPrefab.GetComponent<ItemDropEffect>();
            if (effectInstance != null)
            {
                // 플레이어의 위치를 목표로 설정
                if (_player == null)
                {
                    _player = BattleManager.Player;
                }

                effectInstance.SetTarget(_player.transform);
            }
        }
    }
}