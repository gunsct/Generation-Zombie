using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

public class DeckInfo : ClassMng
{
	public long m_UID;
	public long[] m_Char = new long[10];
	// 시너지 저장할경우 누적되서 저장이 된다.
	// 플레이 덱이 로드된후 SetSynergy 호출로 변경
	/// <summary> 덱에적용된 시너지 </summary>
	[JsonIgnore] public List<JobType> m_Synergy = new List<JobType>();
	[JsonIgnore] public List<JobType> m_NotSynergy = new List<JobType>();

	[JsonIgnore] public bool IsChange = false;
	public DeckInfo(long _uid = -1) {
		if (_uid == -1)
			m_UID = Utile_Class.GetUniqeID();
		else
			m_UID = _uid;
	}

	public void SetDATA(LS_Web.RES_DECKINFO data)
	{
		m_UID = data.UID;
		int Cnt = data.CUID.Length;
		m_Char = new long[Cnt];
		System.Array.Copy(data.CUID, 0, m_Char, 0, Cnt);
		IsChange = false;
	}

	public LS_Web.REQ_DECK_SET Get_REQ()
	{
		LS_Web.REQ_DECK_SET info = new LS_Web.REQ_DECK_SET();
		info.UID = m_UID;
		int Cnt = m_Char.Length;
		info.CUID = new long[Cnt];
		System.Array.Copy(m_Char, 0, info.CUID, 0, Cnt);
		return info;
	}
	public bool IS_InDeck(int _idx) {
		bool indeck = false;
		for (int i = 0; i < m_Char.Length; i++) {
			if (m_Char[i] == 0) continue;
			CharInfo info = USERINFO.GetChar(m_Char[i]);
			if (info.m_Idx == _idx) {
				indeck = true;
				break;
			}
		}
		return indeck;
	}
	public void SetChar(int _pos, long _uid, List<CharInfo> checkchars = null) {
		IsChange = true;
		m_Char[_pos] = _uid;
		SetSynergy(checkchars);
	}
	public int GetDeckCharCnt() {
		int cnt = 0;
		for(int i = 0; i < m_Char.Length; i++) {
			if (m_Char[i] != 0) cnt++;
		}
		return cnt;
	}
	/// <summary> 현재 덱의 시너지 세팅</summary>
	public void SetSynergy(List<CharInfo> checkchars = null) {
		//튜토리얼용으로 강제로 넣는건 시너지 체크 예외처리 해야함
		if (TUTO.CheckUseCloneDeck() && this != TUTO.m_CloneDeck) return;

		m_Synergy.Clear();
		m_NotSynergy.Clear();

		if (checkchars == null) checkchars = TUTO.CheckUseCloneDeck() ? TUTO.m_CloneChars : USERINFO.m_Chars;
		//시너지 카운팅
		Dictionary<JobType, int> alljob = new Dictionary<JobType, int>();
		for(int i = 0; i < 5; i++) {
			long charuid = 0;
			charuid = m_Char[i];

			if (charuid != 0) {
				CharInfo info = checkchars.Find(o => o.m_UID == charuid);
				List<JobType> jobs = info.m_TData.m_Job;
				for(int j = 0; j < jobs.Count; j++) {
					if (!alljob.ContainsKey(jobs[j])) 
						alljob.Add(jobs[j], 0);
					alljob[jobs[j]]++;
				}
			}
		}

		//시너지 받는거랑 안받는거 나눔
		for (int i = 0; i < alljob.Count; i++)
		{
			KeyValuePair<JobType, int> data = alljob.ElementAt(i);
			JobType job = data.Key;
			if (TDATA.GetSynergyTable(job).IS_CanSynergy(data.Value)) {
				if(!m_Synergy.Contains(job))
					m_Synergy.Add(job);
			}
			else
				if(!m_NotSynergy.Contains(job))
					m_NotSynergy.Add(job);
		}
	}

	public int GetCombatPower(bool _pvp = false, bool _precal = false) {
		int val = 0;
		for(int i = 0; i < m_Char.Length; i++) {
			if (m_Char[i] == 0) continue;
			CharInfo info = USERINFO.GetChar(m_Char[i]);
			val += _precal ? (_pvp ? info.m_PVPCP  : info.m_CP) : info.GetCombatPower(0, 0, _pvp);
		}
		return val;
	}
	public bool IS_FullDeck(bool _pvpdeck) {
		bool full = true;
		for(int i = 0;i < (_pvpdeck ? 10 : 5); i++) {
			if(m_Char[i] == 0) {
				full = false;
				break;
			}
		}
		return full;
	}
}
