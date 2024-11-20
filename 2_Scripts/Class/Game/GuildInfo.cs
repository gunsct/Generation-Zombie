using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class GuildInfo_Base : ClassMng
{
	public enum AlramMode
	{
		/// <summary> ���� </summary>
		None = 0,
		/// <summary> �⼮ </summary>
		Attendance,
		/// <summary> ���� ���� </summary>
		ResStart,
		/// <summary> ���� �Ϸ� </summary>
		ResEnd,
		/// <summary> ���� </summary>
		KickGuild,
		/// <summary> ���ο� ���� ��û�� ���� ��� </summary>
		NewJoin,
		/// <summary> ������ �±� </summary>
		Mastar,
		/// <summary> ���� �˸� </summary>
		Spare_Master,
	}
	/// <summary> ������ȣ </summary>
	public long UID;
	/// <summary> ���� �ڵ� </summary>
	public string Name;
	/// <summary> ���� �ڵ� </summary>
	public string Nation;
	/// <summary> �λ� </summary>
	public string Intro;
	/// <summary> �������� </summary>
	public string Notice;
	/// <summary> ��� ��ũ </summary>
	public int Icon;
	/// <summary> ���� ����ġ(�� �⿩��) </summary>
	public long TotalExp = 0;
	/// <summary> ���Թ�� </summary>
	public GuildJoinType JoinType = GuildJoinType.Auto;
	/// <summary> ���� ���� </summary>
	public int JoinLV = 1;
	/// <summary> ������ ���� �ε��� </summary>
	public int ResIdx;
	/// <summary> ���� ����ġ </summary>
	public long ResExp;

	public int _UserCnt;
	public int MaxUserCnt;
	public long TPower;
	public virtual int MemberCnt { get { return _UserCnt; } }
	public void SetData(RES_GUILDINFO_SIMPLE info)
	{
		UID = info.UID;
		Name = info.Name;
		Nation = info.Nation;
		Intro = info.Intro;
		Notice = info.Notice;
		Icon = info.Icon;
		TotalExp = info.Exp;
		JoinType = info.JoinType;
		JoinLV = info.JoinLV;
		ResIdx = info.ResIdx;
		ResExp = info.ResExp;
		_UserCnt = info.UserCnt;
		MaxUserCnt = info.MaxUserCnt;
		TPower = info.TPower;
	}

	public void Calc_Exp(out int LV, out long Exp)
	{
		if (UID == 0)
		{
			LV = 0;
			Exp = 0;
			return;
		}
		TDATA.GetGuild_LV(TotalExp, out LV, out Exp);
	}

	public Sprite GetGuilMark()
	{
		return TDATA.GetGuideMark(Icon);
	}

}

public class GuildKickInfo : ClassMng
{
	public long UID;
	public string Name;

	public void Save()
	{
		UID = USERINFO.m_Guild.UID;
		Name = USERINFO.m_Guild.Name;
		PlayerPrefs.SetString($"MYGUILD_KICK_INFO_{USERINFO.m_UID}", JsonConvert.SerializeObject(this));
		PlayerPrefs.Save();
	}

	public void Load()
	{
		var data = JsonConvert.DeserializeObject<GuildKickInfo>(PlayerPrefs.GetString($"MYGUILD_KICK_INFO_{USERINFO.m_UID}", "{}"));
		UID = data.UID;
		Name = data.Name;
	}

	public void CheckKick(bool IsCenterMsg = false)
	{
		if(IsKick())
		{
			if (IsCenterMsg) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(6103), Name));
			else POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, string.Format(TDATA.GetString(6103), Name));
			Save();
		}
	}

	public bool IsKick()
	{
		Load();
		return UID != 0 && USERINFO.m_Guild.UID == 0;
	}
}

public class GuildShop
{
	public long GUID;
	public long CTime = 0;
	public List<int> Idxs = new List<int>();

	[JsonIgnore] public bool IsChange;

	public void Clear()
	{
		IsChange = true;
		CTime = 0;
		Idxs.Clear();
	}
	public void Save()
	{
		if (!IsChange) return;
		PlayerPrefs.SetString($"MYGUILD_SHOP_{MainMng.Instance.USERINFO.m_UID}", JsonConvert.SerializeObject(this));
		PlayerPrefs.Save();
	}

	public static GuildShop Load()
	{
		return JsonConvert.DeserializeObject<GuildShop>(PlayerPrefs.GetString($"MYGUILD_SHOP_{MainMng.Instance.USERINFO.m_UID}", JsonConvert.SerializeObject(new GuildShop())));
	}

	public long GetEndTime()
	{
		var utile = MainMng.Instance.UTILE;
		return (long)utile.Get_ZeroTime(CTime) + 24 * 60 * 60 * 1000L;
	}
}

public class GuildInfo : GuildInfo_Base
{
	public override int MemberCnt { get { return Users.Count < 1 ? _UserCnt : Users.Count; } }

	public RES_GUILD_USER Master;
	public RES_GUILD_USER MyInfo;

	/// <summary> ���� </summary>
	public List<RES_GUILD_USER> Users = new List<RES_GUILD_USER>();
	/// <summary> �Ϸ�� ���� ����(���� ������������ ���� �Ϸ� �˸��� ������ ��)</summary>
	public List<int> EndRes = new List<int>();
	/// <summary> ��� ������</summary>
	public List<RES_GUILD_ITEM> Items = new List<RES_GUILD_ITEM>();


	/// <summary> ��û ���� </summary>
	public List<RES_GUILD_REQUSER> ReqUsers = new List<RES_GUILD_REQUSER>();

	/// <summary> ���� ������ �ʿ��� ��Ȳ </summary>
	[JsonIgnore] public bool IsReLoad = false;

	[JsonIgnore] public int ResStep { get { return TDATA.GetGuild_ResStep(USERINFO.m_Guild.EndRes); } }

	public void LoadGuild(Action EndCB, long UID, bool LoadMember, bool LoadEndRes, bool LoadItems )
	{
		int InfoMode = (int)GuildInfoMode.Base;
		if (LoadMember) InfoMode |= (int)GuildInfoMode.Member;
		if (LoadEndRes) InfoMode |= (int)GuildInfoMode.EndRes;
		if (LoadItems) InfoMode |= (int)GuildInfoMode.Items;
		WEB.SEND_REQ_GUILD((res) => {
			if (res.IsSuccess())
			{
				if(UID == 0)
				{
					USERINFO.m_GCoin = res.m_GCoin;
					USERINFO.m_GRTime = res.m_GRTime;
				}
				var befor = USERINFO.m_Guild.UID;
				// �ڽ��� ������� �������ֱ�
				SetData(res, LoadMember, LoadEndRes, LoadItems);
				// ������ ��������� �޶����ٸ� ���Թ̼� üũ
				if(befor > 0 && befor != USERINFO.m_Guild.UID) USERINFO.Check_Mission(MissionType.Guild, 0, 0, 1);
			}
			EndCB?.Invoke();
		}, InfoMode, UID);
		IsReLoad = false;
	}

	public GuildGrade MyGrade()
	{
		return UID != 0 && MyInfo != null ? MyInfo.Grade : GuildGrade.End;
	}

	public void GradeChange(long target, GuildGrade grade)
	{
		GradeChange(Users.Find(o => o.UserNo == target), grade);
	}
	public void GradeChange(RES_GUILD_USER target, GuildGrade grade)
	{
		if (target == null) return;
		if (grade == GuildGrade.Master)
		{
			Master.Grade = GuildGrade.Normal;
			Master = target;
		}
		target.Grade = grade;
		if(target.UserNo == USERINFO.m_UID) SaveMyGrade();
	}

	public void LoadGuildReqJoin(Action EndCB)
	{
		// ��û ����Ʈ �ޱ�
		WEB.SEND_REQ_GUILD_REQUSER_LIST((res2) => {
			if (res2.IsSuccess()) SetData(res2.Users);
			EndCB?.Invoke();
		}, UID);
	}

	public void SetData(RES_GUILDINFO info, bool LoadMember = true, bool LoadEndRes = true, bool LoadItems = true)
	{
		base.SetData(info);
		if (LoadMember) SetData(info.Users);
		if (LoadEndRes) SetData(info.EndRes);
		if (LoadItems) SetData(info.Items);
	}

	public void SetData(List<RES_GUILD_USER> info)
	{
		Users = info;
		Master = info.Find(o => o.Grade == GuildGrade.Master);
		MyInfo = info.Find(o => o.UserNo == USERINFO.m_UID);
	}
	public void SetData(List<int> info)
	{
		EndRes = info;
		if(UID == USERINFO.m_Guild.UID) USERINFO.EnergyCheck();
	}
	public void SetData(List<RES_GUILD_ITEM> info)
	{
		Items = info;
	}
	public void SetData(List<RES_GUILD_REQUSER> info)
	{
		ReqUsers = info;
	}

	public void SaveLV()
	{
		int LV;
		long Exp;
		Calc_Exp(out LV, out Exp);
		PlayerPrefs.SetInt($"MYGUILD_LV_{USERINFO.m_UID}", LV);
		PlayerPrefs.Save();
	}

	public bool IsUserKickState()
	{
		// TUDO : ���̵� PVP �߰��� �Ⱓ���� �߹� �Ұ� ����
		return true;
	}

	public AlramMode GetAlramMode()
	{
		if (IsAlram(AlramMode.KickGuild)) return AlramMode.KickGuild;
		// �⼮ üũ
		else if (IsAlram(AlramMode.Attendance)) return AlramMode.Attendance;
		// ���� ���� üũ
		else if(USERINFO.m_GuildKickCheck.UID == 0 && UID != 0)
		{
			SaveLV();
			USERINFO.m_GuildKickCheck.Save();
			Set_AlramOff();
			SaveMyGrade();
			SaveSpareCheck();
			return AlramMode.None;
		}
		// ���� ���� ���� ��Ȯ�� ��
		else if (IsAlram(AlramMode.ResStart)) return AlramMode.ResStart;
		// ���� �Ϸ� ��Ȯ�� ��
		else if (IsAlram(AlramMode.ResEnd)) return AlramMode.ResEnd;
		else if (IsAlram(AlramMode.NewJoin)) return AlramMode.NewJoin;
		return AlramMode.None;
	}

	public bool IsAlram(AlramMode mode)
	{
		switch(mode)
		{
		case AlramMode.Attendance:	return MyInfo != null && !UTILE.IsSameDay(MyInfo.ATime);
		case AlramMode.ResStart:	return PlayerPrefs.GetInt($"MYGUILD_RES_{USERINFO.m_UID}", 0) != ResIdx;
		case AlramMode.ResEnd:		return PlayerPrefs.GetInt($"MYGUILD_END_RES_CNT_{USERINFO.m_UID}", 0) < EndRes.Count;
		case AlramMode.KickGuild:	return MyInfo == null && USERINFO.m_GuildKickCheck.IsKick();
		case AlramMode.NewJoin:
			if (MyInfo != null && MyInfo.Grade == GuildGrade.Master)
			{
				// ���ο� ���� ��û�� ���� ���
				var values = PlayerPrefs.GetString($"MYGUILD_REQ_USERS_{USERINFO.m_UID}", "").Split('|');
				for (int i = 0; i < values.Length; i++)
				{
					if (values[i].Length < 1) continue;
					if (!ReqUsers.Exists(o => o.UserNo == long.Parse(values[i]))) return true;
				}
			}
			break;
		case AlramMode.Mastar:
			var mygrade = PlayerPrefs.GetInt($"MYGUILD_MYGRADE_{USERINFO.m_UID}", (int)GuildGrade.End);
			return mygrade < (int)GuildGrade.Master && MyInfo.Grade == GuildGrade.Master;
		case AlramMode.Spare_Master:
			var check = PlayerPrefs.GetInt($"MYGUILD_SPARE_CHECK_{USERINFO.m_UID}", (int)GuildGrade.End);
			if (check == (int)GuildGrade.Spare_Master) return false;
			return MyGrade() == GuildGrade.Spare_Master;
		}
		return false;
	}

	public void Set_AlramOff()
	{
		// Ż�𶧴� ������ �������־����
		if (UID == 0)
		{
			PlayerPrefs.SetInt($"MYGUILD_RES_{USERINFO.m_UID}", 0);
			PlayerPrefs.GetInt($"MYGUILD_END_RES_CNT_{USERINFO.m_UID}", 0);
			PlayerPrefs.SetString($"MYGUILD_REQ_USERS_{USERINFO.m_UID}", "");
		}
		else
		{
			PlayerPrefs.SetInt($"MYGUILD_RES_{USERINFO.m_UID}", ResIdx);
			PlayerPrefs.SetInt($"MYGUILD_END_RES_CNT_{USERINFO.m_UID}", EndRes.Count);
			PlayerPrefs.SetString($"MYGUILD_REQ_USERS_{USERINFO.m_UID}", string.Join("|", ReqUsers.Select(o => o.UserNo).ToArray()));
		}
		PlayerPrefs.Save();
	}

	public void SaveSpareCheck()
	{
		PlayerPrefs.SetInt($"MYGUILD_SPARE_CHECK_{USERINFO.m_UID}", MyInfo == null ? (int)GuildGrade.End : (int)MyInfo.Grade);
		PlayerPrefs.Save();
	}
	public void SaveMyGrade()
	{
		PlayerPrefs.SetInt($"MYGUILD_MYGRADE_{USERINFO.m_UID}", MyInfo == null ? (int)GuildGrade.End : (int)MyInfo.Grade);
		PlayerPrefs.Save();
	}

	public void Set_Alram_ReqUser_Off()
	{
		if (MyInfo.Grade == GuildGrade.Master) PlayerPrefs.SetString($"MYGUILD_REQ_USERS_{USERINFO.m_UID}", string.Join("|", ReqUsers.Select(o => o.UserNo).ToArray()));
		else PlayerPrefs.SetString($"MYGUILD_REQ_USERS_{USERINFO.m_UID}", "");
		PlayerPrefs.Save();
	}

	public void Set_Alram_Res_End_Off()
	{
		PlayerPrefs.SetInt($"MYGUILD_END_RES_CNT_{USERINFO.m_UID}", EndRes.Count);
		PlayerPrefs.Save();
	}

	public void Set_Alram_Res_Off()
	{
		PlayerPrefs.SetInt($"MYGUILD_RES_{USERINFO.m_UID}", ResIdx);
		PlayerPrefs.Save();
	}

	public List<TShopTable> GetMyShopList()
	{
		var data = GuildShop.Load();
		var mygrade = USERINFO.m_Guild.MyGrade();
		if (data.GUID != UID || !UTILE.IsSameDay(data.CTime))
		{
			data.Clear();
			// �̸� �־�ΰ� ������ ������ ����Ʈ�� ������
			// �� ��Ȳ�� ���� ����Ʈ�� �ٽ� �����ߴ� �����ϸ� �޶����� ������ ����
			AddShopData(data, ShopGroup.Guild_member);
			//AddShopData(data, ShopGroup.Guild_normal_Char);
			//AddShopData(data, ShopGroup.Guild_normal_DNA);
			AddShopData(data, ShopGroup.Guild_master);
			data.GUID = UID;
			data.CTime = (long)UTILE.Get_ServerTime_Milli();
			data.Save();
		}

		var list = data.Idxs.Select(o => TDATA.GetShopTable(o)).ToList();
		if (mygrade == GuildGrade.End) list.RemoveAll(o => o.m_Group == ShopGroup.Guild_member);
		if (mygrade != GuildGrade.Master) list.RemoveAll(o => o.m_Group == ShopGroup.Guild_master);
		return list;
	}

	void AddShopData(GuildShop indata, ShopGroup group)
	{
		indata.IsChange = true;
		var tdatas = TDATA.GetGroupShopTable(group);
		List<TShopTable> picklist = new List<TShopTable>();
		// ��� �̰��� �����϶��� 0���� ����Ʈ��
		if (UID == 0) tdatas = tdatas.FindAll(o => o.m_Level < 1);
		else {
			int[] limitcnt = new int[2];
			switch (group) {
				case ShopGroup.Guild_master:
					limitcnt[0] = 5;
					limitcnt[1] = 12;
					break;
				case ShopGroup.Guild_member:
					limitcnt[0] = 0;
					limitcnt[1] = 5;
					break;
					// ������ �ش��ϴ� ��ǰ 3���� �����
					// ���� ������ �ִ� ��ǰ ���� Ȯ��
			}
			var temp = tdatas.FindAll(o => {
				var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(b => b.Idx == o.m_Idx);
				if (buyinfo != null && buyinfo.Cnt > 0) return true;
				return false;
			});

			if (tdatas.Count > 0 && temp.Count < limitcnt[1] - limitcnt[0]) {
				tdatas.RemoveAll(o => temp.Exists(t => t.m_Idx == o.m_Idx));
				picklist.AddRange(temp);

				for (int i = limitcnt[0]; i < limitcnt[1]; i++) {
					if (temp.Exists(o => o.m_Level == i)) continue;
					List<TShopTable> groups = tdatas.FindAll(o => o.m_Level == i && o.m_NoOrProb > 0);
					int probsum = groups.Sum(o => o.m_NoOrProb);
					int prob = UTILE.Get_Random(0, probsum);
					int preprob = 0;
					for (int j = 0; j < groups.Count; j++) {
						preprob += groups[j].m_NoOrProb;
						if (preprob >= prob) {
							picklist.Add(groups[j]);
							break;
						}
					}
				}
			}
		}
		if (picklist.Count > 0) indata.Idxs.AddRange(picklist.Select(o => o.m_Idx));
	}
}


