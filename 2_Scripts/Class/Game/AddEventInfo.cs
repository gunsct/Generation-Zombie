using System.Collections.Generic;
public class AddEventInfo
{
	public class ItemState
	{
		/// <summary> 상점 아이템 인덱스 </summary>
		public int Idx;
		/// <summary> 구매한 횟수 </summary>
		public int BuyCnt;
	}
	/// <summary> 인덱스 </summary>
	public int Idx = 0;
	/// <summary> 블랙마켓 상품 </summary>
	public List<ItemState> Items = new List<ItemState>();
}