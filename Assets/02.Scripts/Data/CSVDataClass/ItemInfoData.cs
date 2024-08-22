using static Define;

public class ItemInfoData
{
	// 아이템의 아이디
	public string Id;

    // PaidCurrency = 0, FreeCurrency = 1, Nutrients = 2
    // 아이템 타입 말고 ItemId로 찾기
	// 파싱에서 바꿔야 해서 냅둔다 
    public ItemType Type;

	// 아이템의 이름
	public string Name;

	// 아이템의 설명
	public string Desc;

	// 값
	// 하나 소모 시 몬스터 레벨 업 게이지 채워주는 양 (타입에 대해 올려주는 양 통합적 관리)
	public int Value;
}
