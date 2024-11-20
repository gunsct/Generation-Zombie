using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class RewardList_Equip : PopupBase
{
	[System.Serializable]
	public struct SOptionUI
	{
		public GameObject Active;
		public TextMeshProUGUI Value;
	}

	[System.Serializable]
	public struct SSpecialUI
	{
		public GameObject[] Active;
		public UIEffect Char;
		public TextMeshProUGUI Name;
		public SOptionUI Options;
	}
	[System.Serializable]
	public struct SEquipUI
	{
		public GameObject StatGroupActive;
		public GameObject[] StatActive;
		public Item_InfoStat[] Stat;
		public TextMeshProUGUI CP;
		public SOptionUI[] Options;
		public SSpecialUI Special;
	}

	[System.Serializable]
	public struct SOneUI
	{
		public GameObject Active;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Grade;
		public TextMeshProUGUI Name;
	}

	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public GameObject CloseBtn;
		public SOneUI OneItem;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SEquipUI m_sEqUI;
	IEnumerator m_PlayAction;
	List<ItemInfo> m_Info = new List<ItemInfo>();
	List<RES_REWARD_ITEM> m_Item = new List<RES_REWARD_ITEM>();
	int m_MaxGrade;
	private void Start() {
	}

	/// <summary> aobjValue 0 : 제작된 아이템 정보, 1 : 제작된 수량, 2 : 제작된 등급(소모품은 모두 일반등급이라 제작시 내부 등급 필요) </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		List<RES_REWARD_BASE> items = (List<RES_REWARD_BASE>)aobjValue[0];
		m_MaxGrade = (int)aobjValue[1];

		PlayEffSound(SND_IDX.SFX_0310);
		for (int i = 0;i< items.Count; i++) {
			m_Item.Add((RES_REWARD_ITEM)items[i]);
		}

		for (int i = 0; i < m_Item.Count; i++) {
			m_Info.Add(USERINFO.GetItem(m_Item[i].UID));
		}

		m_PlayAction = StartAniCheck();
		StartCoroutine(m_PlayAction);

		base.SetData(pos, popup, cb, aobjValue);
	}
	
	void SetItemsUI(int _pos) {
		PLAY.PlayEffSound(SND_IDX.SFX_0143);
		ItemInfo itemInfo = m_Info[_pos];
		TItemTable tdata = itemInfo.m_TData;

		int grade = itemInfo.m_Grade;
		
		List<ItemStat> stats = tdata.m_Stat;

		if (m_Item[_pos].result_code == EResultCode.SUCCESS_POST) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));

		//아이템 카드와 이름
		m_SUI.OneItem.Active.SetActive(true);
		m_SUI.OneItem.Item.SetData(m_Item[_pos]);
		m_SUI.OneItem.Item.SetGoodFrame(m_MaxGrade == grade);
		m_SUI.OneItem.Grade.text =  itemInfo.GetGradeGroupName();
		m_SUI.OneItem.Grade.color = BaseValue.GradeColor(grade);
		m_SUI.OneItem.Name.text = tdata.GetName();

		//전투력 표기
		m_sEqUI.CP.text = Utile_Class.CommaValue(m_Info[_pos].m_CP);

		// 스텟
		for (int i = 0; i < 2; i++) {
			bool Active = i < stats.Count;
			m_sEqUI.Stat[i].gameObject.SetActive(Active);
			if (Active) m_sEqUI.Stat[i].SetData(stats[i].m_Stat, Mathf.RoundToInt(itemInfo.GetStat(stats[i].m_Stat)));//Mathf.RoundToInt(stats[i].GetValue(m_Info[_pos].m_Lv))
		}

		// 추가 스텟
		m_sEqUI.StatActive[0].SetActive(m_Info[_pos].m_AddStat.Count > 0);
		m_sEqUI.StatActive[1].SetActive(m_Info[_pos].m_AddStat.Count > 0);
		for (int i = 0; i < 3; i++) {
			bool Active = i < m_Info[_pos].m_AddStat.Count;
			m_sEqUI.Options[i].Active.SetActive(Active);
			if (Active) m_sEqUI.Options[i].Value.text = m_Info[_pos].m_AddStat[i].ToString();
		}

		// 전용 장비 정보
		TEquipSpecialStat stat = m_Info[_pos].m_TSpecialStat;
		for (int i = 0; i < 2; i++) m_sEqUI.Special.Active[i].SetActive(stat != null);
		if (stat != null)
		{
			CharInfo charinfo = USERINFO.GetEquipChar(m_Info[_pos].m_Uid);
			TCharacterTable tchar = TDATA.GetCharacterTable(stat.m_Char);
			bool IsEqChar = charinfo != null && (stat.m_Char == 0 || charinfo.m_Idx == stat.m_Char);
			FontStyles style = FontStyles.Normal;
			m_sEqUI.Special.Char.GetComponent<Image>().sprite = tchar.GetPortrait();
			m_sEqUI.Special.Name.text = string.Format(TDATA.GetString(248), tchar.GetCharName());
			m_sEqUI.Special.Name.fontStyle = style;

			bool Active = stat.m_Stat.m_Stat != StatType.None;
			m_sEqUI.Special.Options.Active.SetActive(Active);
			if (Active)
			{
				m_sEqUI.Special.Options.Value.text = stat.m_Stat.ToString();
				m_sEqUI.Special.Options.Value.fontStyle = style;
			}
		}

		m_sEqUI.StatGroupActive.SetActive(m_Info[_pos].m_AddStat.Count > 0 || tdata.m_Value != 0);

		m_Info.RemoveAt(_pos);
	}

	IEnumerator StartAniCheck() {
		m_SUI.CloseBtn.SetActive(false);
		SetItemsUI(m_Info.Count - 1);

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_SUI.CloseBtn.SetActive(true);
		m_PlayAction = null;
	}

	public void ClickExit() {
		Close(0);
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_PlayAction != null) return;

		if (m_Info.Count > 0) {
			m_PlayAction = StartAniCheck();
			StartCoroutine(m_PlayAction);
		}
		else {
			m_SUI.CloseBtn.SetActive(false);
			m_PlayAction = CloseAni(Result);
			StartCoroutine(m_PlayAction);
		}
	}

	public IEnumerator CloseAni(int Result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_PlayAction = null;
		base.Close(Result);
	}
}
