using UnityEngine;

/// <summary>
/// 안바뀌는 SO 데이터 
/// 임시로 텍스트 데이터만 넣어둠
/// </summary>
[CreateAssetMenu(fileName = "TextDataSO", menuName = "ScriptableObjects/TextDataSO")]
public class TextDataSO : ScriptableObject
{
    public string Id;
    public string Kor;
    public string Eng;
}
