using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public class TAlternativeTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 질문 인덱스 </summary>
	public List<int> m_QstIdx = new List<int>();
	public string m_QstImg;
	[System.Serializable]
	public struct Select
	{
		public int m_Desc;
		public string m_Img;
		public int m_ResultStr;
		public RewardKind m_RewardKind;
		public int m_RewardIdx;
		public string m_ResultImg;
	}
	public Select[] m_SelectTF = new Select[2];

	public TAlternativeTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) {
			int idx = pResult.Get_Int32();
			if(idx != 0)
				m_QstIdx.Add(idx); 
		}
		m_QstImg = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_QstImg) && !m_QstImg.Contains("/")) 
			Debug.LogError($"[ AlternativeTable ({m_Idx}) ] m_QstImg 패스 체크할것");
#endif
		for (int i = 0; i < 2; i++) {
			m_SelectTF[i].m_Desc = pResult.Get_Int32();
			m_SelectTF[i].m_Img = pResult.Get_String();
#if USE_LOG_MANAGER
			if (!string.IsNullOrEmpty(m_SelectTF[i].m_Img) && !m_SelectTF[i].m_Img.Contains("/")) 
				Debug.LogError($"[ AlternativeTable ({m_Idx}) ] m_SelectTF[{i}].m_Img  패스 체크할것");
#endif
			m_SelectTF[i].m_ResultStr = pResult.Get_Int32();
			m_SelectTF[i].m_RewardKind = pResult.Get_Enum<RewardKind>();
			m_SelectTF[i].m_RewardIdx = pResult.Get_Int32();
			m_SelectTF[i].m_ResultImg = pResult.Get_String();
		}
	}

	public string GetQuestion(int _page) {
		if (m_QstIdx.Count - 1 < _page) return null;
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_QstIdx[_page]);
	}
	public Sprite GetQuestionImg() {
		return UTILE.LoadImg(m_QstImg, "png");
	}
	public string GetSelectTF(bool _true) {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_SelectTF[_true == true ? 0 : 1].m_Desc);
	}
	public Sprite GetSelectTFImg(bool _true)
	{
		return UTILE.LoadImg(m_SelectTF[_true == true ? 0 : 1].m_Img, "png");
	}
	public string GetResultTF(bool _true) {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_SelectTF[_true == true ? 0 : 1].m_ResultStr);
	}
	public Sprite GetResultTFImg(bool _true)
	{
		return UTILE.LoadImg(m_SelectTF[_true == true ? 0 : 1].m_ResultImg, "png");
	}
	public int GetRewardValTF(bool _true) {
		return m_SelectTF[_true == true ? 0 : 1].m_RewardIdx;
	}
	public RewardKind GetRewardTypeTF(bool _true) {
		return m_SelectTF[_true == true ? 0 : 1].m_RewardKind;
	}
}

public class TAlternativeTableMng : ToolFile
{
	public Dictionary<int, TAlternativeTable> DIC_Idx = new Dictionary<int, TAlternativeTable>();
	public TAlternativeTableMng() : base("Datas/AlternativeTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TAlternativeTable data = new TAlternativeTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// AlternativeTable
	TAlternativeTableMng m_Alternative = new TAlternativeTableMng();

	public TAlternativeTable GetAlternativeTable(int idx) {
		if (!m_Alternative.DIC_Idx.ContainsKey(idx)) return null;
		return m_Alternative.DIC_Idx[idx];
	}
}

