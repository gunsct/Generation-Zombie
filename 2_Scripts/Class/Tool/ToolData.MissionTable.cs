using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TMissionTable : ClassMng
{
	public class TMissionCheck
	{
		/// <summary> 타입 </summary>
		public MissionType m_Type;
		/// <summary> 타입 값 </summary>
		public int[] m_Val = new int[2];
		/// <summary> 개수 </summary>
		public int m_Cnt;
	}

	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Desc;
	/// <summary> 모드 </summary>
	public MissionMode m_Mode;
	/// <summary> 모드 그룹 </summary>
	public int m_ModeGid;
	/// <summary> 연결 미션등 처음 발급 체크용 </summary>
	public bool IsFist;
	/// <summary> 다음 연결 미션</summary>
	public int m_LinkIdx;
	/// <summary> 미션 체크 </summary>
	public List<TMissionCheck> m_Check = new List<TMissionCheck>();
	/// <summary> 그룹 내 해당 퀘스트의 등장 확률 </summary>
	public int m_Prob;
	public PostReward[] m_Rewards = new PostReward[2] { new PostReward(), new PostReward() };
	/// <summary> 해당 퀘스트가 등장하는 스테이지 조건 </summary>
	public int m_StageLock;
	/// <summary> 미션 아이콘 이름</summary>
	public string m_Icon;

	public TMissionTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_Mode = pResult.Get_Enum<MissionMode>();
		m_ModeGid = pResult.Get_Int32();
		IsFist = pResult.Get_Boolean();
		m_LinkIdx = pResult.Get_Int32();

		for (int i = 0; i < 4; i++)
		{
			var type = pResult.Get_Enum<MissionType>();
			var value1 = pResult.Get_Int32();
			var value2 = pResult.Get_Int32();
			var cnt = pResult.Get_Int32();
			if (type == MissionType.None) continue;
			var data = new TMissionCheck();
			data.m_Type = type;
			data.m_Val[0] = value1;
			data.m_Val[1] = value2;
			data.m_Cnt = cnt;
			m_Check.Add(data);
		}
		m_Prob = pResult.Get_Int32();
		m_Rewards[0].Kind = pResult.Get_Enum<RewardKind>();
		m_Rewards[0].Idx = pResult.Get_Int32();
		m_Rewards[0].Cnt = pResult.Get_Int32();
		m_Rewards[0].Grade = pResult.Get_Int32();
		m_Rewards[0].LV = pResult.Get_Int32();
		m_StageLock = pResult.Get_Int32();
		m_Icon = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Icon) && !m_Icon.Contains("/"))
			Debug.LogError($"[ MissionTable ({m_Idx}) ] m_Icon 패스 체크할것");
#endif
		m_Rewards[1].Kind = pResult.Get_Enum<RewardKind>();
		m_Rewards[1].Idx = pResult.Get_Int32();
		m_Rewards[1].Cnt = pResult.Get_Int32();
		m_Rewards[1].Grade = pResult.Get_Int32();
		m_Rewards[1].LV = pResult.Get_Int32();
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), m_Check[0].m_Val[0], m_Check[0].m_Val[1], m_Check[0].m_Cnt);
	}
	public Sprite GetIcon() {
		return UTILE.LoadImg(m_Icon, "png");
	}
}
public class TMissionTableMng : ToolFile
{
	public Dictionary<int, TMissionTable> DIC_Idx = new Dictionary<int, TMissionTable>();
	public List<TMissionTable> Datas = new List<TMissionTable>();

	public TMissionTableMng() : base("Datas/MissionTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TMissionTable data = new TMissionTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// MissionTable
	TMissionTableMng m_Mission = new TMissionTableMng();

	public TMissionTable GetMissionTable(int _idx)
	{
		if (!m_Mission.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Mission.DIC_Idx[_idx];
	}

	public Dictionary<int, TMissionTable> GetAllMissionTable()
	{
		return m_Mission.DIC_Idx;
	}

	public TMissionTable GetRandMissionTable(MissionMode _mode, MissionType _type) {
		List<TMissionTable> datas = m_Mission.Datas.FindAll(o => o.m_Mode == _mode && o.m_Check.Find(c => c.m_Type == _type) != null && o.m_StageLock <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);
		int probsum = datas.Sum(o => o.m_Prob);
		int prob = 0;
		int randprob = UTILE.Get_Random(0, probsum);

		for(int i = 0; i < datas.Count; i++) {
			prob += datas[i].m_Prob;
			if (prob > randprob) return datas[i];
		}
		return null;
	}
}