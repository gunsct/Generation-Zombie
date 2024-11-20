using System.Collections.Generic;
using UnityEngine;
using static LS_Web;
using Newtonsoft.Json;

public class PVPInfo : ClassMng
{

	public class PVPUser
	{
		public RES_PVP_USER_DETAIL m_Info;
		public int Pos;
		public Dictionary<StatType, float> Stats = new Dictionary<StatType, float>();
	}
	public PVPUser[] Users = new PVPUser[(int)UserPos.Max];
	public RES_PVP_GROUP m_Group;
	[JsonIgnore] public int m_CounterIdx = 0;
	public void SetUser(UserPos pos, RES_PVP_USER_DETAIL _info)
	{
		Users[(int)pos] = new PVPUser() { m_Info = _info };
		for (StatType i = StatType.Men; i < StatType.Max; i++) {
			for (int j = 0; j < 10; j++) {
				if (!Users[(int)pos].Stats.ContainsKey(i)) Users[(int)pos].Stats.Add(i, 0f);
				if (j > 4 && i == StatType.HP) continue;
				float val = Users[(int)pos].m_Info.Chars[j].Stat.ContainsKey(i) ? Users[(int)pos].m_Info.Chars[j].Stat[i] : 0f;
				if (i == StatType.HP) val = Mathf.RoundToInt(val);
				Users[(int)pos].Stats[i] += val;
			}
		}
	}
}
