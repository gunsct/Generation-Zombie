using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class DicInfo_Item_Equip_Btn : SerializableDictionary<Info_Item_Equip.BtnName, Image> { }
public class Info_Item_Equip : Info_Item
{
	public enum BtnName
	{
		UnEquip = 0,
		Change,
		ReMake,
		Upgrade,
		LVUp,
		Delete,
		Mileage,
		Post
	}
#pragma warning disable 0649
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
		public Color[] Colors;
		public UIEffect Char;
		public TextMeshProUGUI Name;
		public SOptionUI Options;
	}

	[System.Serializable]
	public struct SAddOptionUI
	{
		public GameObject[] Active;
		public Item_EquipOption[] Options;
		public GameObject[] LockSlots;
	}

	[System.Serializable]
	public struct SEquipUI
	{
		public Item_InfoStat[] Stat;
		public TextMeshProUGUI CP;
		public SAddOptionUI AddOption;
		public SSpecialUI Special;
		public GameObject BtnGroup;
		public DicInfo_Item_Equip_Btn Btns;
		public CanvasGroup Canvasgroup;
		public GameObject MaxLvAlarm;

		public GameObject[] Lock;
	}
	
	[SerializeField] SEquipUI m_sEqUI;
	[Serializable]
	public struct SShopUI
	{
		public Item_Store_Buy_Button BuyBtn;
	}
	[SerializeField] SShopUI m_SSUI;
	TShopTable m_STData;

	private void Awake() {
		m_sEqUI.Btns[BtnName.Upgrade].gameObject.SetActive(false);
		m_sEqUI.Btns[BtnName.Delete].gameObject.SetActive(false);
		m_sEqUI.Btns[BtnName.Mileage].gameObject.SetActive(false);
		m_sEqUI.Btns[BtnName.Post].gameObject.SetActive(false);
	}
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		if (aobjValue.Length > 3) m_STData = (TShopTable)aobjValue[3];
		base.SetData(pos, popup, cb, aobjValue);
		SetBtn();
	}

	public override void SetUI()
	{
		base.SetUI();
		m_sEqUI.CP.text = Utile_Class.CommaValue(m_Info.m_CP);

		TItemTable tdata = m_Info.m_TData;
		List<ItemStat> stats = tdata.m_Stat;

		// 스텟
		for (int i = 0; i < 2; i++)
		{
			bool Active = i < stats.Count;
			m_sEqUI.Stat[i].gameObject.SetActive(Active);
			if (Active) m_sEqUI.Stat[i].SetData(stats[i].m_Stat, Mathf.RoundToInt(m_Info.GetStat(stats[i].m_Stat)));//stats[i].GetValue(m_Info.m_Lv)
		}

		// 추가 스텟
		for (int i = 0; i < 2; i++) m_sEqUI.AddOption.Active[i].SetActive(true);
		int lockcnt = 0;
		if(m_Info.m_AddStat.Count <= BaseValue.ITEM_OPTION_CNT(m_Info.m_Grade)) {//추가 옵션이 등급수량 이하일 경우 잠금 수
			lockcnt = 2 + BaseValue.ITEM_OPTION_CNT(m_Info.m_Grade) - m_Info.m_AddStat.Count;
		}
		else {//추가 옵션이 등급 수량 초과일 경우 잠금 수
			lockcnt = 2 - (m_Info.m_AddStat.Count - BaseValue.ITEM_OPTION_CNT(m_Info.m_Grade));
		}
		bool firstlock = true;
		for (int i = 0; i < 5; i++) {
			bool Active = i < m_Info.m_AddStat.Count;
			m_sEqUI.AddOption.Options[i].gameObject.SetActive(Active);
			if (Active) m_sEqUI.AddOption.Options[i].SetData(m_Info.m_AddStat[i].m_Stat != StatType.None ? m_Info.m_AddStat[i].ToString() : TDATA.GetString(799));
			bool lockslot = i < lockcnt;
			m_sEqUI.AddOption.LockSlots[i].gameObject.SetActive(lockslot);
			m_sEqUI.AddOption.LockSlots[i].GetComponent<Item_EquipOption>().SetBtn(firstlock);
			if (lockslot && firstlock) {
				firstlock = false;
			}
		}
		// 전용 장비 정보
		TEquipSpecialStat stat = m_Info.m_TSpecialStat;
		for (int i = 0; i < 2; i++) m_sEqUI.Special.Active[i].SetActive(stat != null);
		if (stat != null)
		{
			CharInfo charinfo = USERINFO.GetEquipChar(m_Info.m_Uid);
			TCharacterTable tchar = TDATA.GetCharacterTable(stat.m_Char);
			bool IsEqChar = charinfo != null && (stat.m_Char == 0 || charinfo.m_Idx == stat.m_Char);
			FontStyles style = IsEqChar ? FontStyles.Normal : FontStyles.Underline;
			Color color = m_sEqUI.Special.Colors[IsEqChar ? 1 : 0];
			m_sEqUI.Special.Char.GetComponent<Image>().sprite = tchar.GetPortrait();
			m_sEqUI.Special.Name.text = string.Format(TDATA.GetString(248), tchar.GetCharName());
			m_sEqUI.Special.Name.fontStyle = style;
			m_sEqUI.Special.Name.color = color;

			bool Active = stat.m_Stat.m_Stat != StatType.None;
			m_sEqUI.Special.Options.Active.SetActive(Active);
			if (Active)
			{
				m_sEqUI.Special.Options.Value.text = stat.m_Stat.ToString();
				m_sEqUI.Special.Options.Value.fontStyle = style;
				m_sEqUI.Special.Options.Value.color = color;
			}
		}
		SetLockUI();
	}

	void SetLockUI()
	{
		m_sEqUI.Lock[2].SetActive(USERINFO.GetItem(m_Info.m_Uid) != null);
		m_sEqUI.Lock[0].SetActive(!m_Info.m_Lock);
		m_sEqUI.Lock[1].SetActive(m_Info.m_Lock);
	}

	void SetBtn() {

		// 0:강화, 1:해제, 2 교체, 3 승급, 4 재조합

		// 부모 UI가 어떤것인지 확인해서 버튼 온오프 해주기
		// 강화
		bool IsUpgrade = true;
		UIMng.BtnBG bg = m_Info.IS_LvUP() ? UIMng.BtnBG.Green : UIMng.BtnBG.Not;
		m_sEqUI.Btns[BtnName.LVUp].gameObject.SetActive(IsUpgrade);
		m_sEqUI.Btns[BtnName.LVUp].sprite = POPUP.GetBtnBG(bg);
		//m_sEqUI.Btns[BtnName.LVUp].GetComponent<Button>().interactable = bg != UIMng.BtnBG.Not;
		m_sEqUI.MaxLvAlarm.SetActive(m_Info.m_Lv >= m_Info.m_MaxLV);

		// 해제(장착 해제를 쓸것인지 확인 필요)
		bool IsUnEquip = m_Parent == PopupName.Info_Char && USERINFO.IsUseEquipItem(m_Info.m_Uid);
		m_sEqUI.Btns[BtnName.UnEquip].gameObject.SetActive(IsUnEquip);

		// 교체
		bool IsChange = m_Parent == PopupName.Info_Char && USERINFO.IsUseEquipItem(m_Info.m_Uid);
		m_sEqUI.Btns[BtnName.Change].gameObject.SetActive(IsChange);

		bool IsGradeUp = m_Info.m_Lv >= BaseValue.ITEM_GRADE_MAX_LV(m_Info.m_Grade);
		m_sEqUI.Btns[BtnName.Upgrade].GetComponent<Button>().interactable = IsGradeUp;

		bool IsReassemble = m_Parent != PopupName.Store_Select && m_Parent != PopupName.Info_Char && !USERINFO.IsUseEquipItem(m_Info.m_Uid);
		m_sEqUI.Btns[BtnName.ReMake].gameObject.SetActive(IsReassemble);
		m_sEqUI.Btns[BtnName.ReMake].GetComponent<Button>().interactable = IsReassemble;

		bool IsMileageSelect = m_Parent == PopupName.Store_Select && m_STData != null;
		if (IsMileageSelect) {
			m_sEqUI.Btns[BtnName.Mileage].gameObject.SetActive(true);
			m_SSUI.BuyBtn.SetData(m_STData.m_Idx);
		}

		bool IsPostSelect = m_Parent == PopupName.Store_Select && m_STData == null;
		if (IsPostSelect) {
			m_sEqUI.Btns[BtnName.Post].gameObject.SetActive(true);
		}
	}
	public void ClickUnLockOption(GameObject _obj) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 4)) return;
		if (m_Action != null) return;
		int pos = 0;
		for (int i = 0;i < m_sEqUI.AddOption.Active[1].transform.childCount; i++) {
			GameObject slot = m_sEqUI.AddOption.Active[1].transform.GetChild(i).gameObject;
			if (slot == _obj) break;
			else if (slot.activeSelf && pos < m_Info.m_AddStat.Count) pos++;
		}
		OnOptionResetBtn(pos);
	}

	public void OnOptionResetBtn(int Pos) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 3)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.OptionChange, (result, obj) =>
		{
			if(m_Info.m_AddStat.Count > Pos)m_sEqUI.AddOption.Options[Pos].SetData(m_Info.m_AddStat[Pos].ToString());
			SetUI();
			SetBtn();
		}, m_Info, Pos, Pos >= m_Info.m_AddStat.Count);
	}

	public void OnUnEq()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 1)) return;

		if(!USERINFO.CheckBagSize())
		{
			WEB.StartErrorMsg(EResultCode.ERROR_INVEN_SIZE);
			return;
		}

#if NOT_USE_NET
		// 해당 버튼은 캐릭터 정보창에서만 나올 수 있음
		USERINFO.RemoveEquipUID(m_Info.m_Uid);
		m_ChangeCB?.Invoke(InfoChange.UnEquip, new object[] { this, m_Info });
		MAIN.Save_UserInfo();
		PlayEffSound(SND_IDX.SFX_0005);
		// 연출없이 닫기
		base.Close();
#else
		WEB.SEND_REQ_CHAR_UNEQUIP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			m_ChangeCB?.Invoke(InfoChange.UnEquip, new object[] { this, m_Info });
			PlayEffSound(SND_IDX.SFX_0005);
			base.Close();
		}, m_Info.m_Uid);
#endif
	}

	public void OnLvUP() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 0)) return;
		if (m_Info.m_Lv >= m_Info.m_MaxLV) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, null, TDATA.GetString(963));
			return;
		}
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Focus_EquipUpgrade)) TUTO.Next();
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipLevelUp, (result, obj) =>
		{
			SetUI();
			SetBtn();
			m_ChangeCB?.Invoke(InfoChange.LVUP, new object[] { this, m_Info });
			if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Focus_EquipUpgradeExit)) {
				base.Close(); 
			}
		}, m_Info);
	}

	public void OnChange() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 2)) return;
		// 해당 버튼은 캐릭터 정보창에서만 나올 수 있음
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipChange, (result, obj) =>
		{
			if (result == 1)
			{
				m_ChangeCB?.Invoke(InfoChange.Equip, new object[] { this, m_Info });
				// 연출없이 닫기
				base.Close();
			}
		}, m_Info.m_TData.GetEquipType(), m_Info, USERINFO.GetEquipChar(m_Info.m_Uid));
	}
	/// <summary> 등급 업 </summary>
	public void OnGradeUp() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 2)) return;
		// 해당 버튼은 캐릭터 정보창에서만 나올 수 있음
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipGradeUp, (result, obj) => {
			if (result == 1) {
				SetUI();
				SetBtn();
				m_ChangeCB?.Invoke(InfoChange.Equip, new object[] { this, m_Info });
				// 연출없이 닫기
				base.Close();
			}
		}, m_Info);
	}
	/// <summary> 재조립 </summary>
	public void OnReassembly() {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_Equip, 2)) return;
		// 해당 버튼은 캐릭터 정보창에서만 나올 수 있음
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipReassembly, (result, obj) => {
			if (result == 1) {
				ItemInfo info = obj.GetComponent<EquipReassembly>().m_SelectItem;
				if (info != null) m_Info = info;
				SetUI();
				SetBtn();
				m_ChangeCB?.Invoke(InfoChange.Equip, new object[] { this, m_Info });
				// 연출없이 닫기
				base.Close();
			}
		}, m_Info);
	}

	public void SetLock(bool Lock)
	{
#if NOT_USE_NET
		SetLockUI();
		m_Info.m_Lock = !m_Info.m_Lock;
#else
		WEB.SEND_REQ_ITEM_LOCK((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			SetLockUI();
			m_ChangeCB?.Invoke(InfoChange.Equip, new object[] { this, m_Info });

		}, new long[] { m_Info.m_Uid });
#endif
	}

	/// <summary> 오직 정보만 보여주고 옵션 개조나 하단 버튼류 다 끈다. </summary>
	public override void OnlyInfo() {
		for(int i = 0;i< m_sEqUI.AddOption.Options.Length; i++) {
			m_sEqUI.AddOption.Options[i].SetBtn(false);
		}
		for (int i = 0; i < m_sEqUI.AddOption.LockSlots.Length; i++) {
			m_sEqUI.AddOption.LockSlots[i].GetComponent<Item_EquipOption>().SetBtn(false);
		}
		m_sEqUI.BtnGroup.SetActive(false);
	}
	/// <param name="pos">0 : lvup, 1 : unequip, 2 : equipchange</param>
	public GameObject GetBtn(BtnName pos)
	{
		return m_sEqUI.Btns[pos].gameObject;
	}
	/// <summary> 마일리지 상품 등 구매 콜백 </summary>
	public void Click_Select() {
		Close(m_Info.m_Idx);
	}
}
