using System;

/// <summary>
/// CSV : ReadOnly 모든 데이터 로드해서 딕셔너리로 만들어주기
/// Server : 서버에서 데이터 받아오기
/// Json : 로컬에 데이터 저장하고 불러오기
/// </summary>
public class DataManager : Singleton<DataManager>
{
    public ReadOnlyDataSystem ReadOnlyDataSystem { get; private set; }
    public ServerDataSystem ServerDataSystem { get; private set; }
    public LocalDataSystem LocalDataSystem { get; private set; }

    protected override void Awake()
    {
        _isDontDestroyOnLoad = true;
        base.Awake();

        ReadOnlyDataSystem = new ReadOnlyDataSystem();
        ServerDataSystem = new ServerDataSystem();
        LocalDataSystem = new LocalDataSystem();

        ReadOnlyDataSystem.Init();
        ServerDataSystem.Init();
        LocalDataSystem.Init();
    }
}
