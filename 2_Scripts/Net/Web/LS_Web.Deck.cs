using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_DECK : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
	}

	public class RES_DECKINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 적용 스테이지 모드 </summary>
		public int Pos;
		/// <summary> 케릭터 리스트 </summary>
		public long[] CUID;
	}

	public void SEND_REQ_DECK(Action<RES_BASE> action, long UserNo, long UID = 0)
	{
		REQ_DECK _data = new REQ_DECK();
		_data.UserNo = UserNo;
		_data.UID = UserNo;
		SendPost(Protocol.REQ_DECK, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}



	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_DECK_SET
	public class REQ_DECK_SET_LIST : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<REQ_DECK_SET> Decks = new List<REQ_DECK_SET>();
	}

	public class REQ_DECK_SET
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 케릭터 리스트 </summary>
		public long[] CUID;
	}

	public void SEND_REQ_DECK_SET(Action<RES_BASE> action)
	{
		REQ_DECK_SET_LIST _data = new REQ_DECK_SET_LIST();
		_data.UserNo = USERINFO.m_UID;
		for(int i = 0; i < USERINFO.m_Deck.Length; i++)
		{
			DeckInfo deck = USERINFO.m_Deck[i];
			if (!deck.IsChange) continue;
			_data.Decks.Add(deck.Get_REQ());
		}

		if(_data.Decks.Count > 0)
		{
			SendPost(Protocol.REQ_DECK_SET, JsonConvert.SerializeObject(_data), (result, data) => {
				action?.Invoke(ParsResData<RES_BASE>(data));
			});
		}
		else
		{
			action?.Invoke(new RES_BASE());
		}
	}

}
