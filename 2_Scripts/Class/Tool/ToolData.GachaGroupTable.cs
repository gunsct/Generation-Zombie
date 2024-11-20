using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LS_Web;

public class TGachaGroupTable : ClassMng
{
	/// <summary> 그룹 호출용 Index </summary>
	public int m_GIdx;
	/// <summary> Reward타입 (Character, Item)  </summary>
	public RewardKind MRewardKind;
	/// <summary> Type에 따른 Index </summary>
	public int m_RewardIdx;
	/// <summary> 지급 될 수량 </summary>
	public int m_RewardCount;
	/// <summary> 지급 될 등급 </summary>
	public int m_RewardGrade;
	/// <summary> 확률 값( 상대 확률) </summary>
	public int m_Prob;

	public TGachaGroupTable(CSV_Result pResult) {
		m_GIdx = pResult.Get_Int32();
		MRewardKind = pResult.Get_Enum<RewardKind>();
		m_RewardIdx = pResult.Get_Int32();
		m_RewardCount = pResult.Get_Int32();
		m_RewardGrade = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
	}
}

public class GachaGroup
{
	public List<int> m_Probs = new List<int>();
	public int m_TotalProb = 0;
	public List<TGachaGroupTable> m_List = new List<TGachaGroupTable>();

	public void Add(TGachaGroupTable item) {
		m_TotalProb += item.m_Prob;
		m_Probs.Add(m_TotalProb);
		m_List.Add(item);
	}
}


public class TGachaGroupTableMng : ToolFile
{
	public Dictionary<int, GachaGroup> DIC_GID = new Dictionary<int, GachaGroup>();

	public TGachaGroupTableMng() : base("Datas/GachaGroupTable")
	{
	}

	public override void DataInit()
	{
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGachaGroupTable data = new TGachaGroupTable(pResult);
		if (!DIC_GID.ContainsKey(data.m_GIdx))
		{
			DIC_GID.Add(data.m_GIdx, new GachaGroup());
		}
		DIC_GID[data.m_GIdx].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GachaGroupTable
	TGachaGroupTableMng m_GachaGroup = new TGachaGroupTableMng();


	public void LoadGachaGroupTable() {
		byte[] pData = UTILE.GetData("Datas/GachaGroupTable", ".bytes");
		if (pData != null) {
			Utile_Class.Decode(pData, pData.Length, 0);
			SetGachaGroupTable(UTILE.Get_CsvResult(Encoding.UTF8.GetString(pData)));
		}
	}
	/// <summary> 테스트 안해봄 추후 싱크로드가 필요하면 셋팅할것 </summary>
	public IEnumerator LoadGachaGroupTableAsync()
	{
		byte[] pData = null;
		yield return UTILE.GetData_Async("Datas/GachaGroupTable", ".bytes", true, (data) => { pData = data; });
		if (pData == null) yield break;

		Utile_Class.Decode(pData, pData.Length, 0);

		SetGachaGroupTable(UTILE.Get_CsvResult(Encoding.UTF8.GetString(pData)));

		yield return new WaitForEndOfFrame();
	}

	void SetGachaGroupTable(CSV_Result pResult) {
		int size = pResult.Get_LineSize();
		for (int i = 0; i < size; i++, pResult.next()) {
		}
	}
	/// <summary> 가챠 그룹 </summary>
	public GachaGroup GetGachaGroup(int _gid) {
		if (!m_GachaGroup.DIC_GID.ContainsKey(_gid)) return null;
		return m_GachaGroup.DIC_GID[_gid];
	}
	
	/// <summary> 가챠 그룹 1,2,3에서 확률로 뽑힌 그룹</summary>
	public TGachaGroupTable GetGachaGroupTable(int _gid) {
		if (!m_GachaGroup.DIC_GID.ContainsKey(_gid))
		{
			Utile_Class.DebugLog($"Cant Find GachaGroupTable gid {_gid}");
			return null;
		}
		GachaGroup group = m_GachaGroup.DIC_GID[_gid];

		int prob = UTILE.Get_Random(0, group.m_TotalProb);
		int preprob = 0;
		for (int i = 0; i < group.m_Probs.Count; i++) {
			if (preprob <= prob && group.m_Probs[i] > prob) return group.m_List[i];
			preprob = group.m_Probs[i];
		}
		return null;
	}
	
	public class GachaData
	{
		public int result_code;
		/// <summary> Reward타입 (Character, Item)  </summary>
		public RewardKind Kind;
		/// <summary> Type에 따른 Index </summary>
		public int Idx;
		/// <summary> 지급 될 수량 </summary>
		public int Cnt;
		/// <summary> 지급 될 등급 </summary>
		public int Grade;
	}
	
	public List<RES_REWARD_BASE> GetGachaItem(TItemTable itemTable, bool _insert = true)
	{
		int gid = itemTable.m_Value;
		switch(itemTable.m_Type)
		{
			case ItemType.AllBox:
				return GetGachaItem_All(gid, _insert);
			case ItemType.RandomBox:
				return GetGachaItem_Random(gid, _insert);
			default:
				return null;
		}
	}
	public List<RES_REWARD_BASE> GetGachaItemList(TItemTable itemTable, bool _char4piece = true) {
		int gid = itemTable.m_Value;
		switch (itemTable.m_Type) {
			case ItemType.AllBox:
			case ItemType.RandomBox:
				return GetGachaItem_All(gid, false, _char4piece);
			default:
				return null;
		}
	}
	List<RES_REWARD_BASE> GetGachaItem_Random(int gid, bool _insert)
	{
		TGachaGroupTable item = GetGachaGroupTable(gid);
		
		List<GachaData> datalist = new List<GachaData>();
		datalist.Add(new GachaData() { Kind = item.MRewardKind, Idx = item.m_RewardIdx, Cnt = item.m_RewardCount, Grade = item.m_RewardGrade });
		return InsertGachaItem(datalist, _insert);
	}

	/// <summary>
	/// 가챠 그룹의 보상을 모두 불러옴
	/// </summary>
	/// <param name="gid"></param>
	/// <param name="_insert">네트워크 없이 할때 클라에서 보상 받는 경우 사용</param>
	/// <param name="_viewoverlapchar"> 순수 보상 목록만 필요할 경우, 보유캐릭터나 조각변환이나 캐릭터 레벨 미표기용 </param>
	/// <returns></returns>
	public List<RES_REWARD_BASE> GetGachaItem_All(int gid, bool _insert, bool _viewoverlapchar = false)
	{
		List<GachaData> datalist = new List<GachaData>();

		GachaGroup group = GetGachaGroup(gid);
		for(int j = 0; j < group.m_List.Count; j++)
		{
			TGachaGroupTable item = group.m_List[j];
			if (item.m_Prob == 0) continue;
			datalist.Add(new GachaData() { Kind = item.MRewardKind, Idx = item.m_RewardIdx, Cnt = item.m_RewardCount, Grade = item.m_RewardGrade });
		}
		return InsertGachaItem(datalist, _insert, _viewoverlapchar);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="datalist"></param>
	/// <param name="_insert"></param>
	/// <param name="_onlyview"> 순수 보상 목록만 필요할 경우, 보유캐릭터나 조각변환이나 캐릭터 레벨 미표기용 </param>
	/// <returns></returns>
	List<RES_REWARD_BASE> InsertGachaItem(List<GachaData> datalist, bool _insert, bool _onlyview = false)
	{
		List<RES_REWARD_BASE> re = new List<RES_REWARD_BASE>();
		// 중복 캐릭터 체크
		// 데이터 검토(보유및 중복 캐릭터만 검사하여 피스로 전환 해준다.)
		for (int i = 0; i < datalist.Count; i++)
		{
			GachaData data = datalist[i];
			if (data.Kind != RewardKind.Character) continue;
			CharInfo mychar = USERINFO.m_Chars.Find(o => o.m_Idx == data.Idx);
			if (data.Grade < 1) data.Grade = TDATA.GetCharacterTable(data.Idx).m_Grade;
			if (mychar == null) continue;
			if (data.Grade < mychar.m_Grade && !_onlyview)
			{
				// 피스 전환
				data.Kind = RewardKind.Item;
				data.Idx = mychar.m_TData.m_PieceIdx;
				data.Cnt = BaseValue.STAR_OVERLAP(data.Grade);
				data.result_code = EResultCode.SUCCESS_REWARD_PIECE;
			}
		}

		// 데이터 적용
		for (int i = 0; i < datalist.Count; i++)
		{
			GachaData data = datalist[i];
			switch (data.Kind)
			{
			case RewardKind.Character:
				CharInfo charinfo = USERINFO.m_Chars.Find(o => o.m_Idx == data.Idx);
				if (charinfo == null || _onlyview)
				{
					if (_insert) charinfo = USERINFO.InsertChar(data.Idx, data.Grade);
					else charinfo = new CharInfo(data.Idx, 0, data.Grade);
					RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
					rchar.SetData(charinfo);
					re.Add(rchar);
				}
				else
				{
					if (_insert) charinfo.m_Grade = data.Grade;
					RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
					rchar.Type = Res_RewardType.Char;
					rchar.SetData(charinfo);
					re.Add(rchar);
				}
				break;
			case RewardKind.Item:
				var tdata = GetItemTable(data.Idx);
				RES_REWARD_MONEY rmoney;
				RES_REWARD_ITEM ritem;
				switch (tdata.m_Type)
				{
				case ItemType.Dollar:
					rmoney = new RES_REWARD_MONEY();
					rmoney.Type = Res_RewardType.Money;
					re.Add(rmoney);
					rmoney.Befor = USERINFO.m_Money;
					if (_insert) USERINFO.InsertItem(data.Idx, data.Cnt);
					rmoney.Now = USERINFO.m_Money;
					rmoney.Add = _insert ? (int)(rmoney.Now - rmoney.Befor) : data.Cnt;
					break;
				case ItemType.Cash:
					rmoney = new RES_REWARD_MONEY();
					rmoney.Type = Res_RewardType.Cash;
					re.Add(rmoney);
					rmoney.Befor = USERINFO.m_Cash;
					if (_insert) USERINFO.InsertItem(data.Idx, data.Cnt);
					rmoney.Now = USERINFO.m_Cash;
					rmoney.Add = _insert ? (int)(rmoney.Now - rmoney.Befor) : data.Cnt;
							break;
				case ItemType.Energy:
					rmoney = new RES_REWARD_MONEY();
					rmoney.Type = Res_RewardType.Energy;
					re.Add(rmoney);
					rmoney.Befor = USERINFO.m_Energy.Cnt;
					if (_insert) USERINFO.InsertItem(data.Idx, data.Cnt);
					rmoney.Now = USERINFO.m_Energy.Cnt;
					rmoney.Add = _insert ? (int)(rmoney.Now - rmoney.Befor) : data.Cnt;
					rmoney.STime = (long)USERINFO.m_Energy.STime;
					break;
				case ItemType.InvenPlus:
					rmoney = new RES_REWARD_MONEY();
					rmoney.Type = Res_RewardType.Inven;
					re.Add(rmoney);
					rmoney.Befor = USERINFO.m_InvenSize;
					if (_insert) USERINFO.InsertItem(data.Idx, data.Cnt);
					rmoney.Now = USERINFO.m_InvenSize;
					rmoney.Add = _insert ? (int)(rmoney.Now - rmoney.Befor) : data.Cnt;
					break;
				default:
					ritem = new RES_REWARD_ITEM();
					ritem.Type = Res_RewardType.Item;
					if (_insert) ritem.UID = USERINFO.InsertItem(data.Idx, data.Cnt).m_Uid;
					ritem.Idx = data.Idx;
					ritem.Cnt = data.Cnt;
					re.Add(ritem);
					break;
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = null;
				if (_insert) zombieInfo = USERINFO.InsertZombie(data.Idx);
				else zombieInfo = new ZombieInfo(data.Idx);
				RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
				zombie.UID = zombieInfo.m_UID;
				zombie.Idx = zombieInfo.m_Idx;
				zombie.Grade = zombieInfo.m_Grade;
				re.Add(zombie);
				break;
			case RewardKind.DNA:
				TDnaTable dnaTable = TDATA.GetDnaTable(data.Idx);
				DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
				if (_insert) USERINFO.m_DNAs.Add(dnaInfo);
				RES_REWARD_DNA dna = new RES_REWARD_DNA();
				dna.UID = dnaInfo.m_UID;
				dna.Idx = dnaInfo.m_Idx;
				dna.Grade = dnaInfo.m_Grade;
				re.Add(dna);
				break;
			}
		}
		return re;
	}
}

