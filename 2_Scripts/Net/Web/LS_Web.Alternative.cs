using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_ALTERNATIVE : REQ_BASE
	{
		public List<REQ_ALTERNATIVE_INFO> AltInfos;
	}
	public class REQ_ALTERNATIVE_INFO : REQ_BASE
	{
		/// <summary> 시드 선택 인덱스 </summary>
		public int Idx;
		/// <summary> 튜토리얼 진행 번호 </summary>
		public bool Select;
	}


	public class RES_ALTERNATIVE : RES_BASE
	{
		public List<RES_CHARINFO> Chars;
		public List<RES_ITEMINFO> Items;
		public List<RES_DECKINFO> Deck;
	}
}
