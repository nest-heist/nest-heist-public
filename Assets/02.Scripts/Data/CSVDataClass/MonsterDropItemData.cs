
/// <summary>
/// 몬스터 드랍 아이템
/// 아이템마다 개별 확률 적용하기 돈은 무조건, 영양제는 확률로
/// </summary>
public class MonsterDropItemData
{
	// 몬스터의 아이디
	public string MonsterId;

	// 아이템 아이디
	public string ItemId;

	// 최소 개수
	public int MinNum;

	// 최대 개수
	public int MaxNum;

	// 아이템 드랍 확률
	public float Probability;
}
