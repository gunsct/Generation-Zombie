using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using static LS_Web;

public class PVP_Revenge : PopupBase
{
	[Serializable]
	public class PVPUserInfo
	{
		public Image Portrait;
		public Image Nation;
		public Text Name;
		public Image TierIcon;
		public TextMeshProUGUI TierName;
		public TextMeshProUGUI LeaguePoint;
		public Transform CPBucket;
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public PVPUserInfo[] UserInfos;
		public Transform CPNumFont;         //Item_PVP_Main_CP_NumFont
		public TextMeshProUGUI[] SurvStat;
		public Transform CharBuckets;
		public Transform CharPrefabs;       //Item_PVP_Main_EnemyChar
		public GameObject[] StealMatGroup;
		public TextMeshProUGUI[] StealMats;
		public TextMeshProUGUI DayPlayCnt;

	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	RES_PVP_USER_BASE[] m_PVPUsers = new RES_PVP_USER_BASE[2];
	RES_PVP_USER_DETAIL m_Target;
	TPvPRankTable m_PRTData;
	Action m_CB;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PVPUsers = (RES_PVP_USER_BASE[])aobjValue[0];
		m_Target = (RES_PVP_USER_DETAIL)aobjValue[1];
		m_PRTData = (TPvPRankTable)aobjValue[2];
		m_CB = (Action)aobjValue[3];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		for (int i = 0; i < 2; i++) {//아마 USERINFO에 PvPInfo가 들어갈듯
			m_SUI.UserInfos[i].Portrait.sprite = TDATA.GetUserProfileImage(m_PVPUsers[i].Profile);
			m_SUI.UserInfos[i].Nation.sprite = BaseValue.GetNationIcon(m_PVPUsers[i].Nation);
			m_SUI.UserInfos[i].Name.text = m_PVPUsers[i].Name;
			if (i == 0) m_PVPUsers[0].Power = USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].GetCombatPower(true);
			SetCP(m_SUI.UserInfos[i], m_PVPUsers[i].Power, i == 0 ? Utile_Class.GetCodeColor("#B3ECFF") : Utile_Class.GetCodeColor("#FFD2D1"));
			m_SUI.UserInfos[i].TierIcon.sprite = m_PRTData.GetTierIcon();
			m_SUI.UserInfos[i].TierName.text = string.Format("{0} {1}", m_PRTData.GetRankName(), m_PRTData.GetTierName());
			m_SUI.UserInfos[i].LeaguePoint.text = string.Format("{0} LP", Utile_Class.CommaValue(m_PVPUsers[i].Point[1]));
		}
		for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
			float val = 0f;
			for (int j = 0; j < m_Target.Chars.Count; j++) {
				val += m_Target.Chars[j].Stat.ContainsKey(i) ? m_Target.Chars[j].Stat[i] : 0;
			}
			m_SUI.SurvStat[(int)i].text = Mathf.RoundToInt(val).ToString();
		}
		//캐릭터 정보
		UTILE.Load_Prefab_List(5, m_SUI.CharBuckets, m_SUI.CharPrefabs);
		RES_PVP_CHAR[] chars = new RES_PVP_CHAR[2];
		for (int i = 0; i < 5; i++) {
			chars[0] = m_Target.Chars[i];
			chars[1] = m_Target.Chars[i + 5];
			m_SUI.CharBuckets.GetChild(i).GetComponent<Item_PVP_Main_EnemyChar>().SetData(chars, i < 5 - m_PRTData.m_HideMemberCnt);
		}
		for (int i = 0; i < 3; i++) {
			TPVP_CampTable tctdata = TDATA.GetTPVP_CampTable(m_Target.CampInfo.BuildLV[0]);
			TPVP_Camp_Storage stdata = TDATA.GetPVP_Camp_Storage(USERINFO.m_CampBuild[CampBuildType.Storage].LV);
			TPVP_Camp_Storage tstdata = TDATA.GetPVP_Camp_Storage(m_Target.CampInfo.BuildLV[1]);
			bool matoff = tstdata.m_StealMat[i] == 0 || stdata.m_SaveMat[i] == 0;
			m_SUI.StealMatGroup[i].SetActive(!matoff);
			m_SUI.StealMats[i * 2].text = m_SUI.StealMats[i * 2 + 1].text = string.Format("{0}~{1}", tstdata.m_StealMat[i], tctdata.m_RatioCnt[i].Cnt);
		}
		m_SUI.DayPlayCnt.text = string.Format("{0}/{1}", Math.Max(0, BaseValue.PVP_DAY_PLAY_COUNT - m_PVPUsers[0].GetDayPlayCnt()), BaseValue.PVP_DAY_PLAY_COUNT);
		base.SetUI();
	}
	/// <summary> 전투력 표기 </summary>
	void SetCP(PVPUserInfo _info, long _cp, Color _color) {
		// [20230328] ODH 전투력 0일때 숫자 안나옴 셋팅 방식 변경
		string cp = _cp.ToString();
		int length = cp.Length;
		UTILE.Load_Prefab_List(length, _info.CPBucket, m_SUI.CPNumFont);
		for (int i = 0; i < length; i++) {
			Item_PVP_Main_CP_NumFont num = _info.CPBucket.GetChild(i).GetComponent<Item_PVP_Main_CP_NumFont>();
			num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", cp.Substring(i, 1)), "png"), _color);
		}
	}
	public void Click_GoBattle() {
		m_CB?.Invoke();
	}
	/// <summary> 덱세팅 진입 </summary>
	public void ClickGoPvPDeck() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 3)) return;
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_DeckSetting, (result, obj) => {
			SetUI();
		});
	}
	//public override void Close(int Result = 0) {
	//	if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
	//	if (m_Action != null) return;
	//	m_Action = CloseAction(Result);
	//	StartCoroutine(m_Action);
	//}
	//IEnumerator CloseAction(int _result) {
	//	m_SUI.Anim.SetTrigger("Close");
	//	yield return new WaitForEndOfFrame();
	//	yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
	//	base.Close(_result);
	//}
}
