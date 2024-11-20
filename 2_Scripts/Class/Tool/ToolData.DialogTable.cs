using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;

public class TDialogTable : ClassMng
{
	
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 다음 다이얼로그 인덱스  </summary>
	public int m_NextDLIdx;
	/// <summary> 대사에 같이 나올 사운드 </summary>
	public string m_Sound;
	/// <summary> 사운드 볼륨 </summary>
	public float m_SndVolume;
	/// <summary> 카드형태 프리팹 이미지 </summary>
	public string m_CardImg;
	/// <summary> 유닛형태 프리팹 이미지 </summary>
	public string m_UnitImg;
	/// <summary> 이미지 프리팹 애니메이션 트리거 </summary>
	public string m_Ani;
	/// <summary> 대화자 인덱스 </summary>
	public int m_TalkerIdx;
	/// <summary> 대사 형태, Talker 0아니고 Dir None 아닐때만 </summary>
	public DialogTalkType m_TalkEmotion;
	/// <summary> 방향 </summary>
	public DialogTalkDir m_Dir;
	/// <summary> 대사 인덱스 </summary>
	public int m_Desc;
	/// <summary> 선택지 그룹 타입 </summary>
	public SelectGroupType m_SelectGroupType;
	/// <summary> 선택지 그룹 타입 값</summary>
	public int m_SelectGroupValue;
	/// <summary> 선택지 그룹 인덱스 </summary>
	public int m_SelectGID;
	public bool m_TimeAni;

	public TDialogTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_NextDLIdx = pResult.Get_Int32();
		m_Sound = pResult.Get_String();
		m_SndVolume = pResult.Get_Float();
		m_CardImg = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_CardImg) && !m_CardImg.Contains("/"))
			Debug.LogError($"[ DialogTable ({m_Idx}) ] m_CardImg 패스 체크할것");
#endif
		m_UnitImg = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_UnitImg) && !m_UnitImg.Contains("/"))
			Debug.LogError($"[ DialogTable ({m_Idx}) ] m_UnitImg 패스 체크할것");
#endif
		m_Ani = pResult.Get_String();
		m_TalkerIdx = pResult.Get_Int32();
		m_TalkEmotion = pResult.Get_Enum<DialogTalkType>();
		m_Dir = pResult.Get_Enum<DialogTalkDir>();
		m_Desc = pResult.Get_Int32();
		m_SelectGroupType = pResult.Get_Enum<SelectGroupType>();
		m_SelectGroupValue = pResult.Get_Int32();
		m_SelectGID = pResult.Get_Int32();
		m_TimeAni = pResult.Get_Boolean();
	}

	//talk 용
	public Sprite GetCardImg() {
		return UTILE.LoadImg(m_CardImg, "png");
	}
	public Sprite GetUnitImg() {
		return UTILE.LoadImg(m_UnitImg, "png");
	}
	public TTalkerTable GetTalker() {
		return TDATA.GetTalkerTable(m_TalkerIdx);
	}
	//공용
	public string GetDesc() {
		return TDATA.GetString(StringTalbe.Dialog, m_Desc);
	}
	public TDialogTable GetNextDialog()
	{
		if (m_NextDLIdx == 0) return null;
		else return TDATA.GetDialogTable(m_NextDLIdx);
	}
}

public class TDialogTableMng : ToolFile
{
	public Dictionary<int, TDialogTable> DIC_Idx = new Dictionary<int, TDialogTable>();
	public TDialogTableMng() : base("Datas/DialogTable")
	{
	}

	public override void CheckData()
	{

	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TDialogTable data = new TDialogTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DialogTable
	TDialogTableMng m_Dialog = new TDialogTableMng();

	public TDialogTable GetDialogTable(int _idx) {
		if (!m_Dialog.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Dialog.DIC_Idx[_idx];
	}
}
