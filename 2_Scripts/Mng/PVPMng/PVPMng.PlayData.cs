using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static LS_Web;
using static PVPInfo;

public partial class PVPMng : ObjMng
{
	public enum Result
	{
		None,
		WIN,
		SurvStat,	//생존 스텟 중 하나라도 0이 되면 패배, 동시 0되도 패배
		Turn,       //0이 될 때까지 승패가 결정되지 않으면 패배
		Surrender,	//항복
		Out			//클라종료시 패매
	}
	public class PlayUser
	{
		public float[,] m_Stat = new float[(int)StatType.Max, 2];
		public Dictionary<ResearchEff, float> m_Research = new Dictionary<ResearchEff, float>();
		public Item_PVP_Char[] m_Chars = new Item_PVP_Char[10];
		public int m_KillCnt = 0;
		//pvpchar 만들어서 여기에 포지션별 뭐 charinfo[10] 이렇게 오면 앞에 5자리는 전투원 이런식으로 규칙잡아
		//역할에 맞는 스탯 계산 자체 hp등 감소랑 상태나 버프같은거 가지고 있고
		//play에서 전체 루틴돌리기
		public int GetStat(StatType _stat, int _pos = 0) {
			return Mathf.RoundToInt(m_Stat[(int)_stat, _pos]);
		}
		public float GetResearch(ResearchEff _fx) {
			if (!m_Research.ContainsKey(_fx)) return 0f;
			return m_Research[_fx];
		}
	}
	public static int MAPCNT = 1;
	public static int MAXTURN = 7;
	public int Turn;
	public PlayUser[] m_PlayUser = new PlayUser[(int)UserPos.Max] { new PlayUser(), new PlayUser() };
	public Dictionary<UserPos, Dictionary<StageCardType, float>> m_SelectReward = new Dictionary<UserPos, Dictionary<StageCardType, float>>();

	/// <summary> 20캐릭터 기본 speed 순으로 재보정 </summary>
	public void SetCharSpeedRevision() {
		List<RES_PVP_CHAR> allchar = new List<RES_PVP_CHAR>();
		for (UserPos i = UserPos.My; i < UserPos.Max; i++) {
			allchar.AddRange(PVPINFO.Users[(int)i].m_Info.Chars);
		}
		allchar.Sort((RES_PVP_CHAR _b, RES_PVP_CHAR _a) => {
			return _a.Stat[StatType.Speed].CompareTo(_b.Stat[StatType.Speed]);
		});
		for(int i = 0; i < allchar.Count; i++) {
			allchar[i].Stat[StatType.Speed] = TDATA.GeTPVPSpeedRevision(i).m_RevisedSpeed;
		}
	}
	public void SetPlayData()
	{
		Turn = MAXTURN;
		for(UserPos i = UserPos.My; i < UserPos.Max; i++)
		{
			PVPUser user = PVPINFO.Users[(int)i];
			PlayUser data = m_PlayUser[(int)i];
			data.m_Stat = SetStat(user);
			data.m_Research = user.m_Info.Research;
			for (int j = 0; j < 10; j++) {
				data.m_Chars[j] = GetChar(i, j);
				data.m_Chars[j].SetData(i, user.m_Info.Chars[j]);
			}
			//data.Buff.Clear();
		}
	}
	public void SetRefreshChar() {
		for (int i = 0; i < 2; i++) {
			PlayUser data = m_PlayUser[i];
			for (int j = 0; j < 10; j++) {
				data.m_Chars[j].Refresh();
			}
		}
	}
	public float[,] SetStat(PVPUser _user)
	{
		float[,] stat = new float[(int)StatType.Max, 2];
		for (StatType i = StatType.Men; i < StatType.Max; i++) {
			if(_user.Stats.ContainsKey(i)) stat[(int)i, 0] = stat[(int)i, 1] = _user.Stats[i]; 
		}
		return stat;
	}
	public void SetBuff(UserPos _userpos, int _idx) {
		TStageCardTable data = TDATA.GetStageCardTable(_idx);
		if (!m_SelectReward.ContainsKey(_userpos)) m_SelectReward.Add(_userpos, new Dictionary<StageCardType, float>());
		if (!m_SelectReward[_userpos].ContainsKey(data.m_Type)) m_SelectReward[_userpos].Add(data.m_Type, 0f);
		m_SelectReward[_userpos][data.m_Type] += data.m_Value1;
	}
	public float GetBuff(UserPos _userpos, StageCardType _type) {
		return !m_SelectReward.ContainsKey(_userpos) ? 0f : (m_SelectReward[_userpos].ContainsKey(_type) ? m_SelectReward[_userpos][_type] : 0f);
	}
	public float GetNowStat(UserPos _pos, StatType _stat) {
		return m_PlayUser[(int)_pos].m_Stat[(int)_stat, 0];
	}
	public float GetMaxStat(UserPos _pos, StatType _stat) {
		return m_PlayUser[(int)_pos].m_Stat[(int)_stat, 1];
	}
}
