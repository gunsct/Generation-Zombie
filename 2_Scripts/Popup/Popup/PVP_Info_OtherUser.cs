using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using UnityEngine.UI;
using TMPro;

public class PVP_Info_OtherUser : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Image Portrait;
		public TextMeshProUGUI Lv;
		public Image Nation;
		public Text Name;
		public Image TierIcon;
		public TextMeshProUGUI TierName;
		public TextMeshProUGUI[] Points;//0:리그1:시즌
		public TextMeshProUGUI[] SurvStat;
		public Transform CPNumFont;         //Item_PVP_Main_CP_NumFont
		public Transform CPBucket;
		public Transform CharBuckets;
		public Transform CharPrefabs;       //Item_PVP_Main_EnemyChar
	}
	[SerializeField] SUI m_SUI;
	RES_PVP_USER_DETAIL m_Info;
	int m_RankIdx;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (RES_PVP_USER_DETAIL)aobjValue[0];
		m_RankIdx = (int)aobjValue[1];

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		//상대정보
		TPvPRankTable tdata = TDATA.GeTPvPRankTable(m_RankIdx);
		m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(m_Info.Profile);
		m_SUI.Lv.text = m_Info.LV.ToString();
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(m_Info.Nation);
		m_SUI.Name.text = m_Info.Name;
		m_SUI.TierIcon.sprite = tdata.GetTierIcon();
		m_SUI.TierName.text = string.Format("{0} {1}", tdata.GetRankName(), tdata.GetTierName());
		m_SUI.Points[0].text = string.Format("{0} LP", Utile_Class.CommaValue(m_Info.Point[1]));
		m_SUI.Points[1].text = string.Format("{0} SP", Utile_Class.CommaValue(m_Info.Point[0]));
		//스탯
		for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
			float val = 0f;
			for (int j = 0; j < m_Info.Chars.Count; j++) {
				val += m_Info.Chars[j].Stat[i];
			}
			m_SUI.SurvStat[(int)i].text = Mathf.RoundToInt(val).ToString();
		}
		//전투력
		string cp = m_Info.Power.ToString();
		int length = cp.Length;
		UTILE.Load_Prefab_List(length, m_SUI.CPBucket, m_SUI.CPNumFont);
		for (int i = 0; i < length; i++) {
			Item_PVP_Main_CP_NumFont num = m_SUI.CPBucket.GetChild(i).GetComponent<Item_PVP_Main_CP_NumFont>();
			num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", cp.Substring(i, 1)), "png"), Utile_Class.GetCodeColor("#FFD2D1"));//m_Info.UserNo == USERINFO.m_UID ? Utile_Class.GetCodeColor("#B3ECFF") : 
		}
		//캐릭
		UTILE.Load_Prefab_List(5, m_SUI.CharBuckets, m_SUI.CharPrefabs);
		RES_PVP_CHAR[] chars = new RES_PVP_CHAR[2];

		for (int i = 0; i < 5; i++) {
			chars[0] = m_Info.Chars.Count > i ?  m_Info.Chars[i] : null;
			chars[1] = m_Info.Chars.Count > i + 5 ? m_Info.Chars[i + 5] : null;
			m_SUI.CharBuckets.GetChild(i).GetComponent<Item_PVP_Main_EnemyChar>().SetData(chars, i < 5 - tdata.m_HideMemberCnt);
		}
		base.SetUI();
	}

	/// <summary> 전투력 표기 </summary>
	void SetCP(long _cp, Color _color) {
		// [20230328] ODH 전투력 0일때 숫자 안나옴 셋팅 방식 변경
		string cp = _cp.ToString();
		int length = cp.Length;
		UTILE.Load_Prefab_List(length, m_SUI.CPBucket, m_SUI.CPNumFont);
		for (int i = 0; i < length; i++) {
			Item_PVP_Main_CP_NumFont num = m_SUI.CPBucket.GetChild(i).GetComponent<Item_PVP_Main_CP_NumFont>();
			num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", cp.Substring(i, 1)), "png"), _color);
		}
	}
}
