using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

public class TUserProfileImageTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;

	/// <summary> 프로필 이미지 </summary>
	public string m_Profile;

	/// <summary> 포트레이트 등장 조건 </summary>
	public PortraitCondition m_Condition;

	/// <summary> 등장 값 </summary>
	public int m_ConditionValue;

	/// <summary> 성별 </summary>
	public GenderType m_Gender;

	public TUserProfileImageTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Profile = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Profile) && !m_Profile.Contains("/"))
			Debug.LogError($"[ UserProfileImageTable ({m_Idx}) ] m_Profile 패스 체크할것");
#endif
		m_Condition = pResult.Get_Enum<PortraitCondition>();
		m_ConditionValue = pResult.Get_Int32();
		m_Gender = pResult.Get_Enum<GenderType>();
	}

	public Sprite GetImage()
	{
		return UTILE.LoadImg(m_Profile, "png");
	}

	public bool IsUse()
	{
		switch(m_Condition)
		{
		case PortraitCondition.GetChar:
			return USERINFO.m_Chars.Find(o => o.m_Idx == m_ConditionValue) != null;
		case PortraitCondition.StageClear:
			return USERINFO.m_Stage[StageContentType.Stage].Idxs.Find(o => o.Pos == 0).Idx >= m_ConditionValue;
		}
		return false;
	}
}

public class TUserProfileImageTableMng : ToolFile
{
	public List<TUserProfileImageTable> Datas = new List<TUserProfileImageTable>();
	public Dictionary<int, TUserProfileImageTable> DIC_Idx = new Dictionary<int, TUserProfileImageTable>();

	public TUserProfileImageTableMng() : base("Datas/UserProfileImageTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TUserProfileImageTable data = new TUserProfileImageTable(pResult);
		Datas.Add(data);
		DIC_Idx.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// UserProfileImageTable
	TUserProfileImageTableMng m_UserProfileImage = new TUserProfileImageTableMng();

	public List<TUserProfileImageTable> GetUserProfileImageList()
	{
		return m_UserProfileImage.Datas.FindAll(o => !o.IsUse());
	}
	public TUserProfileImageTable GetUserProfileImageTable(int _idx) {
		if (!m_UserProfileImage.DIC_Idx.ContainsKey(_idx)) return null;
		return m_UserProfileImage.DIC_Idx[_idx];
	}
	public Sprite GetUserProfileImage(int idx)
	{
		if (!m_UserProfileImage.DIC_Idx.ContainsKey(idx)) return null;
		return m_UserProfileImage.DIC_Idx[idx].GetImage();
	}
	public GenderType GetGender(int _idx) {
		if (!m_UserProfileImage.DIC_Idx.ContainsKey(_idx)) return GenderType.None;
		return m_UserProfileImage.DIC_Idx[_idx].m_Gender;
	}
}