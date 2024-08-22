using System.Collections.Generic;
using System.Configuration;

public class Define
{
    public enum Sound
    {
        Bgm = 0,
        Effect,
        Speech,
        Max,
    }

    public enum EnvironmentType
    {
        Grassland = 0,
        Desert,
        Beach,
        SnowField,
    }

    public enum AttributeType
    {
        Water = 0,
        Fire,
        Grass,
        Ground,
        Electric,
        Ice,
        Dragon,
        Dark,
        Normal,
    }

    public enum AttackType
    {
        Ranged = 0,
        Melee
    }

    public enum ItemType
    {
        PaidCurrency = 0,
        FreeCurrency,
        Nutrients
    }

    public enum ActionMode
    {
        NonCombat,
        Combat,
        Count
    }

    public enum QuestCategory
    {
        Dungeon = 0,
        Growth,
        Daily,
    }

    public enum SubtaskGroupState
    {
        Inactive,
        Running,
        Complete
    }

    public enum SubtaskState
    {
        Inactive,
        Running,
        Complete
    }

    public enum QuestState
    {
        Inactive,
        Running,
        Complete,
        Cancel,
        WaitingForCompletion
    }



    // 씬 이름
    public const string TestLoginScene = "TestLogin";
    public const string TitleScene = "0_StartScene";
    public const string LobbyScene = "1_LobbyScene";
    public const string DungeonScene = "2_DungeonScene";
    public const string Boss1Stage = "Boss001Scene";
    public const string Boss2Stage = "Boss002Scene";

    //레이어
    public const string EnemyLayer = "Enemy";
    public const string PartnerLayer = "Partner";
    public const string PlayerLayer = "Player";

    // 던전 몬스터 생성 시 고정 값
    public const int DungeonSpawnEnemyIV = 20;
    public const int DungeonSpawnEnemyRank = 1;

    // 재화 아이템 코드
    public const string ItemGold = "ITE00001";
    public const string ItemDia = "ITE00002";

    // 영양제 아이템 코드
    public const string ItemSmallNutrient = "ITE00003";
    public const string ItemMediumNutrient = "ITE00004";
    public const string ItemLargeNutrient = "ITE00005";

    public const string NoEgg = null;

    // 퀘스트에서 Subtask 완료할 때 사용하는 상수
    public const string TargetEgg = "Egg";
    public const string TargetMonster = "Monster";
    public const string TargetDungeon = "Dungeon";
    public const string TargetSmallNutrient = "SmallNutrient";
    public const string TargetMediumNutrient = "MediumNutrient";
    public const string TargetLargeNutrient = "LargeNutrient";
    public const string TargetGold = "Gold";
    public const string TargetLogin = "Login";
    public const string TargetPortal = "Portal";

    // 알
    public const string SlimeEggId = "EGGSLM01";

    // 튜토리얼 
    public const string TutorialEggIncubator = "egg_incubator";
    public const string TutorialDungeon = "dungeon";
    public const string TutorialGeneral = "general_tutorial";

    public const string TutorialScreen1 = "TutorialScreen1";
    public const string TutorialScreen2 = "TutorialScreen2";
    public const string TutorialScreen3 = "TutorialScreen3";

    public static int FindKeyIndex<T>(Dictionary<string, T> dictionary, string keyToFind)
    {
        int index = 0;
        foreach (var key in dictionary.Keys)
        {
            if (key == keyToFind)
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            return email;

        var emailParts = email.Split('@');
        var userName = emailParts[0];
        var domainName = emailParts[1];

        // 사용자명 부분 마스킹
        var maskedUserName = userName.Length > 4
            ? userName.Substring(0, 4) + new string('*', userName.Length - 4)
            : new string('*', userName.Length);

        // 도메인 부분 마스킹
        var domainParts = domainName.Split('.');
        var maskedDomain = domainParts[0].Length > 2
            ? domainParts[0].Substring(0, 2) + new string('*', domainParts[0].Length - 2)
            : new string('*', domainParts[0].Length);

        return $"{maskedUserName}@{maskedDomain}.{domainParts[1]}";
    }
}
