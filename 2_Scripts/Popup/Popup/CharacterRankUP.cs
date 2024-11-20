using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterRankUP : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_CharacterCard[] CharCard;
		public TextMeshProUGUI[] CombatPower;
		public TextMeshProUGUI[] LvCount;
		public TextMeshProUGUI[] LvMaxCount;
		public Item_InfoStat[] CrntStats;
		public TextMeshProUGUI[] NextStats;
		public Item_Piece MatGroup;
		public TextMeshProUGUI MatName;
		public TextMeshProUGUI Money;

		public Image BtnBG;
		public Sprite[] BtnBGSprite;
		public TextMeshProUGUI BtnTxt;

		public GameObject[] TutoObj;//0:승급 버튼
	}
	[Serializable]
	public struct SRUI
	{
		public Item_CharacterCard CharCard;
		public Image[] CharCardPre;
		public Image[] Portraits;
		public TextMeshProUGUI[] Lv;
		public TextMeshProUGUI CP;
		public GameObject UnlockGroup;
		public TextMeshProUGUI Unlock;
		public Text UserName;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SRUI m_SURI;
	CharInfo m_Info;
	bool m_IsRankUp;
	float m_PreCP;
	TCharacterGradeTable m_TData { get { return TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade); } }

	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_Info = (CharInfo)aobjValue[0];
		m_PreCP = m_Info.m_CP;

		m_SUI.CharCard[0].SetData(m_Info);
		m_SUI.CharCard[1].SetData(m_Info);
		m_SUI.CharCard[1].SetGrade(m_Info.m_Grade + 1);

		m_SUI.LvCount[0].text = m_Info.m_LV.ToString();
		m_SUI.LvCount[1].text = m_Info.m_LV.ToString();

		var charTable = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade);

		m_SUI.LvMaxCount[0].text = charTable.m_MaxLv.ToString(); 
		m_SUI.LvMaxCount[1].text = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade + 1).m_MaxLv.ToString();

		TW_MaxLvCount(charTable.m_MaxLv);
		TW_CPCount(m_PreCP);

		m_SUI.CombatPower[0].text = m_PreCP.ToString();
		m_SUI.CombatPower[1].text = m_Info.GetCombatPower(m_Info.m_LV, m_Info.m_Grade + 1).ToString();

		List<StatType> stats = new List<StatType>() { StatType.HP, StatType.Atk, StatType.Def, StatType.Heal };
		for (int i = 0;i< stats.Count;i++) {
			m_SUI.CrntStats[i].SetData(stats[i], Mathf.RoundToInt(m_Info.GetStat(stats[i])));
			m_SUI.NextStats[i].text = Mathf.RoundToInt(m_Info.GetStat(stats[i], m_Info.m_LV, m_Info.m_Grade + 1)).ToString();
		}

		m_SUI.MatGroup.SetData(m_TData.m_MatIdx, USERINFO.GetItemCount(m_TData.m_MatIdx), m_TData.m_MatCount);
		m_SUI.MatName.text = TDATA.GetItemTable(m_TData.m_MatIdx).GetName();

		m_SUI.Money.text = Utile_Class.CommaValue(m_TData.m_Money);
		m_SUI.Money.color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, m_TData.m_Money, "#D2533C", "#ffffff");

		m_SUI.BtnBG.sprite = m_SUI.BtnBGSprite[m_Info.IS_CanRankUP() ? 0 : 1];
		m_SUI.BtnTxt.color = m_Info.IS_EnoughMoney() ? Utile_Class.GetCodeColor("#ffffff") : Utile_Class.GetCodeColor("#D2533C");

		TSerumBlockTable tdata = TDATA.GetSerumBlockTableGrade(m_Info.m_LV + 1);
		m_SURI.UnlockGroup.SetActive(tdata != null);
		m_SURI.Unlock.text = tdata == null ? string.Empty : string.Format(TDATA.GetString(1082), tdata.m_Idx);

		SND.PlayEffSound(SND_IDX.SFX_0112);
	}

	void SetResult() {
		PlayVoiceSnd(new List<SND_IDX>() { m_Info.m_TData.m_CommVocIdx[0], m_Info.m_TData.m_CommVocIdx[1], m_Info.m_TData.m_CommVocIdx[2], m_Info.m_TData.m_CommVocIdx[3] });
		m_IsRankUp = false;

		m_SURI.CharCard.SetData(m_Info);
		m_SURI.CharCardPre[0].sprite = BaseValue.CharBG(m_Info.m_Grade - 1);
		m_SURI.CharCardPre[1].sprite = BaseValue.CharFrame(m_Info.m_Grade - 1);
		Sprite originframe = BaseValue.CharOriginFrame(m_Info.m_TData.m_Grade, m_Info.m_Grade - 1);
		m_SURI.CharCardPre[2].sprite = originframe;
		m_SURI.CharCardPre[2].gameObject.SetActive(originframe != null);
		m_SURI.Lv[0].text = m_Info.m_LV.ToString();

		int premaxlv, crntmaxlv;
		premaxlv = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade - 1).m_MaxLv;
		crntmaxlv = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade).m_MaxLv;
		iTween.ValueTo(gameObject, iTween.Hash("from", premaxlv, "to", crntmaxlv, "onupdate", "TW_MaxLvCount", "time", 1f, "delay", 2f));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_PreCP, "to", m_Info.m_CP, "onupdate", "TW_CPCount", "time", 1f, "delay", 2f));

		m_SURI.UserName.text = USERINFO.m_Name;
		for (int i = 0; i < m_SURI.Portraits.Length; i++) {
			m_SURI.Portraits[i].sprite = m_Info.m_TData.GetPortrait();
		}
	}

	void TW_MaxLvCount(float _amount) {
		m_SURI.Lv[1].text = Mathf.RoundToInt(_amount).ToString();
	}
	void TW_CPCount(float _amount) {
		m_SURI.CP.text = Utile_Class.CommaValue(Mathf.RoundToInt(_amount));
	}

	public void ClickBtn() {
		if (m_Action != null) return;
		if (m_Info.IS_CanRankUP()) ClickActiveRankUP();
		else ClickDeactiveRankUp();
	}
	public void ClickDeactiveRankUp() {
		if (m_Action != null) return; 
		PlayCommVoiceSnd(VoiceType.Fail);
		if (!m_Info.IS_EnoughMoney())
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX, true);
		else if (!m_Info.IS_EnoughMat())
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
		else if(!m_Info.IS_EnoughRank())
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(565));
	}
	
	/// <summary> 랭크업 결과창 띄움 </summary>
	public void ClickActiveRankUP() {
		if (m_Action != null) return;
		if (m_IsRankUp) return;
		m_IsRankUp = true;
#if NOT_USE_NET
		m_Info.SetRankUP();
		USERINFO.Check_Mission(MissionType.GradeUp, 0, 0, 1);
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Character_GradeUp_Count);
		USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Character_Grade_Count, m_Info.m_Grade - 1, m_Info.m_Grade);
		USERINFO.m_Collection.Check(CollectionType.Character, m_Info.m_Idx, m_Info.m_Grade);
		m_SUI.Anim.SetTrigger("Upgrade");
		SND.PlayEffSound(SND_IDX.SFX_0113);
		SetResult();
#else
		WEB.SEND_REQ_CHAR_GRADEUP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
				});
				m_IsRankUp = false;
				return;
			}
			m_SUI.Anim.SetTrigger("Upgrade");
			SND.PlayEffSound(SND_IDX.SFX_0113);
			SetResult();
			DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
		}, m_Info);
#endif
	}

	public void ClickResult() {
		if (m_Action != null) return;
		if (m_IsRankUp) return;
		m_IsRankUp = true;
		StartCoroutine(ResultBack());
	}
	IEnumerator ResultBack() {
		var nextcharTable = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade + 1);
		if(nextcharTable != null && m_Info.m_Grade + 1 <= BaseValue.CHAR_MAX_RANK) SetData(PopupPos.POPUPUI, PopupName.CharUpgrade, m_EndCB, new object[] { m_Info });
		else {
			Close(0);
		}
		m_SUI.Anim.SetTrigger("Upgrade_End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_IsRankUp = false;
	}

	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		var nextcharTable = TDATA.GetCharGradeTable(m_Info.m_Idx, m_Info.m_Grade + 1);
		m_SUI.Anim.SetTrigger(nextcharTable == null ? "Max_End" : "End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		base.Close(_result);
	}

	///////튜토용
	public GameObject GetTutoObj(int _idx) {
		return m_SUI.TutoObj[_idx];
	}
}
