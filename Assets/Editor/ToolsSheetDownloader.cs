using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.IO;
using Unity.EditorCoroutines.Editor;

/// <summary>
/// 구글 시트 다운로드 해주는 버튼 만들기
/// 코드에 링크, 파일저장이름 등 다 넣어놨다.
/// </summary>
public class ToolsSheetDownloader : EditorWindow
{
    const string _address = "1xl9jddCO_E7jJRnTK6p5W1e1Pan1KHGriuUBt3L8NtQ";
    const string _sheetPath = "Assets/Resources/CSV";
    const string _format = "csv";

    struct SheetInfo
    {
        public string Name;
        public string Id;
    }

    SheetInfo[] _sheets = new SheetInfo[]
    {
        new SheetInfo { Name = "DungeonEggProbabilityData", Id = "995584907" },
        new SheetInfo { Name = "DungeonInfoData", Id = "826857452" },
        new SheetInfo { Name = "DungeonMonsterProbabilityData", Id = "240791968" },
        new SheetInfo { Name = "EggInfoData", Id = "841109990" },
        new SheetInfo { Name = "ItemInfoData", Id = "882486295" },
        new SheetInfo { Name = "MonsterBaseStatData", Id = "1426118155" },
        new SheetInfo { Name = "MonsterDropItemData", Id = "1627816704" },
        new SheetInfo { Name = "MonsterInfoData", Id = "722984681" },
        new SheetInfo { Name = "MonsterIVStatData", Id = "41174343" },
        new SheetInfo { Name = "MonsterLevelStatData", Id = "2038452145" },
        new SheetInfo { Name = "MonsterLevelUpData", Id = "2079524054" },
        new SheetInfo { Name = "MonsterRankStatData", Id = "802174856" },
        new SheetInfo { Name = "MonsterSkillInfoData", Id = "1203934138" },
        new SheetInfo { Name = "PlayerBaseStatData", Id = "1816530482" },
        new SheetInfo { Name = "PlayerInfoData", Id = "887560366" },
        new SheetInfo { Name = "PlayerLevelStatData", Id = "1577930560" },
        new SheetInfo { Name = "PlayerLevelUpData", Id = "158876595" },
        new SheetInfo { Name = "SubtaskInfoData", Id = "1638859933" },
        new SheetInfo { Name = "RewardInfoData", Id = "517265494" },
        new SheetInfo { Name = "QuestInfoData", Id = "135978167" },
    };

    private int _pendingDownloads = 0;

    /// <summary>
    /// 툴에서 메뉴 창으로 띄우기
    /// </summary>
    [MenuItem("Tools/Sheet Downloader")]
    public static void ShowWindow()
    {
        GetWindow<ToolsSheetDownloader>("Sheet Downloader");
    }

    /// <summary>
    /// 다운로드 버튼
    /// </summary>
    private void OnGUI()
    {
        GUILayout.Label("구글 시트에서 CSV 파일 다운받기", EditorStyles.boldLabel);

        if (GUILayout.Button("전부 다운받기"))
        {
            DownloadAllCSVFiles();
        }

        // 일단 하나씩 받는 버튼도 있는데 잘 안쓴다.
        foreach (SheetInfo sheet in _sheets)
        {
            if (GUILayout.Button($"한 페이지만 받기 - {sheet.Name}"))
            {
                _pendingDownloads = 1; 
                EditorCoroutineUtility.StartCoroutineOwnerless(Download(sheet.Id, sheet.Name));
            }
        }
    }

    /// <summary>
    /// 시트 다 다운받기 - 이것만 사용해도 다 받아진다.
    /// </summary>
    private void DownloadAllCSVFiles()
    {
        _pendingDownloads = _sheets.Length;
        foreach (SheetInfo sheet in _sheets)
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(Download(sheet.Id, sheet.Name));
        }
    }

    /// <summary>
    /// 링크로 오픈된 구글 시트에서 각 링크, 페이지 gid로 찾아서 csv로 다운로드
    /// </summary>
    /// <param name="sheetId"></param>
    /// <param name="saveFileName"></param>
    /// <param name="sheetPath"></param>
    /// <param name="address"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    private IEnumerator Download(string sheetId, string saveFileName, string sheetPath = _sheetPath, string address = _address, string format = _format)
    {
        var url = $"https://docs.google.com/spreadsheets/d/{address}/export?format={format}&gid={sheetId}";
        using (var www = UnityWebRequest.Get(url))
        {
            Debug.Log("Start Downloading...");

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download {saveFileName}: {www.error}");
                _pendingDownloads--;
                yield break;
            }

            var fileUrl = $"{_sheetPath}/{saveFileName}.{format}";
            File.WriteAllText(fileUrl, www.downloadHandler.text + "\n");

            Debug.Log("Download Complete.");
        }

        _pendingDownloads--;

        if (_pendingDownloads <= 0)
        {
            EditorUtility.DisplayDialog("Download Complete", "All CSV files have been downloaded successfully.", "OK");
        }
    }
}
