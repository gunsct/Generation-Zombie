using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LS_Web;

public class ZombieResInfo : ClassMng
{
	public long UID;
	public long Exp;
	public int Idx;
	public int LV;

	public void SetDATA(ZombieResInfo data)
	{
		UID = data.UID;
		Exp = data.Exp;
		Idx = data.Idx;
		LV = data.LV;
	}
	public int GetLv() {
		TZombieResearchLevelTable table = TDATA.GetZombieResearchLevelTable(Idx, LV);
		while (table.m_Exp > 0 && Exp >= table.m_Exp) {
			Exp -= table.m_Exp;
			LV++;
			table = TDATA.GetZombieResearchLevelTable(Idx, LV);
		}
		return LV;
	}
}


