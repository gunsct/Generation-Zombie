using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_PDA_Making_Main : Item_PDA_Base
{
	public enum Page
	{
		Equip,
		CharacterEquip,
		ResearchMat,
		EquipMat,
		Max
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Animator[] ScrollAnim;//0:기존, 1:전용장비
		public Transform Bucket;
		public GameObject ElementPrefabs;
		//등급 추가해야함 브실골
		public TextMeshProUGUI GradeTxt;
		public Animator[] PageBtnAnims;
		public TextMeshProUGUI PageName;
		public Material[] AllGetBtnMat;
		public Image AllGetBtn;
		public TextMeshProUGUI AllGetBtnTxt;
		public ScrollRect Scroll;
		public Transform ScrollContent;
		public GameObject[] Alarm;
		public GameObject LvUpMsg;

		public GameObject[] ViewGroup;//0:기존 스크롤, 1:전용장비 스크롤, 2:전용 장비 진행
		public TextMeshProUGUI CharEqTitle;
		public GameObject CharEqPrefab;//Item_Mk_Element_Char
		public Transform CharEqBucket;
		public Image[] GradeTapImg;
		public TextMeshProUGUI[] GradeTapTxt;
		public Color[] GradeTapTxtColor;
		public Item_CharEquipProgress CharEqProgress;
		public GameObject[] CharEquipPanels;
		public TextMeshProUGUI CharEquipLock;
	}
	[Serializable]
	public struct SCEPUI
	{
		public Image Portrait;
		public TextMeshProUGUI Name;
		public GameObject ElementPrefab;
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SCEPUI m_SCEPUI;
	Page m_Page = Page.Equip;
	bool Is_Change;
	List<Item_Mk_Element> m_AllElement = new List<Item_Mk_Element>();
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	List<RES_REWARD_BASE> m_NotEquipRewards = new List<RES_REWARD_BASE>();
	List<RES_REWARD_BASE> m_EquipRewards = new List<RES_REWARD_BASE>();
	List<Item_Mk_Element_Char> m_AllCharEqElement = new List<Item_Mk_Element_Char>();
	List<Item_Mk_CharEquip> m_CharEqRewards = new List<Item_Mk_CharEquip>();

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_SUI.GradeTxt.text = string.Format(TDATA.GetString(1028), Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp)));

		ChangePage((int)m_Page, true);

		AllGetBtnSet();

		SetAlarm();

		if (TUTO.IsTuto(TutoKind.Making, (int)TutoType_Making.ViewMaking)) TUTO.Next(this);

		StartCoroutine(IE_MakeTimer());
	}
	void SetAlarm() {
		for(int i = 1; i< m_SUI.Alarm.Length + 1; i++) {
			m_SUI.Alarm[i - 1].SetActive(USERINFO.IsCompMaking((MakingGroup)i));
		}
	}
	void InitScrollPosition() {
		m_SUI.Scroll.verticalNormalizedPosition = 1;
		m_SUI.Scroll.velocity = Vector2.zero;
		m_SUI.Scroll.StopMovement();
	}
	public void ChangeTab(int _pos) {
		if (m_Page == (Page)_pos) return;
	//	if (Is_Change) return;
		Is_Change = true;
		PlayEffSound(SND_IDX.SFX_0121);
		ChangePage(_pos);
		StartCoroutine(ChangeAction());
	}
	/// <summary> 아이템 항목 페이지 변경</summary>
	public void ChangePage(int _pos, bool _init = false) {
		m_SUI.PageBtnAnims[(int)m_Page].SetTrigger("Off");
		m_Page = (Page)_pos;
		m_SUI.PageBtnAnims[(int)m_Page].SetTrigger(_init ? "On" : "OnStart");

		switch (m_Page) {
			case Page.Equip: m_SUI.PageName.text = TDATA.GetString(112); break;
			case Page.CharacterEquip:
				int lv = Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp));
				m_SUI.CharEqTitle.text = string.Format("{0} Lv.{1}", TDATA.GetString(281), lv); break;
			case Page.ResearchMat: m_SUI.PageName.text = TDATA.GetString(114); break;
			case Page.EquipMat: m_SUI.PageName.text = TDATA.GetString(115); break;
		}

		InitScrollPosition();
		SetRecipe();
		SetAlarm();
		AllGetBtnSet();
	}
	IEnumerator ChangeAction() {
		m_SUI.Anim.SetTrigger("SideMove");
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Is_Change = false;
	}
	void AllGetBtnSet() {
		bool canget = false;
		if (m_Page == Page.CharacterEquip) {
			MakingInfo chareqinfo = USERINFO.m_Making.Find(o => o.m_TData.m_Group == MakingGroup.PrivateEquip && o.m_State == TimeContentState.Play);
			canget = chareqinfo != null && chareqinfo.IS_Complete();
		}
		else canget = m_AllElement.Find((t) => t.m_Info != null && t.m_Info.IS_Complete()) != null;

		m_SUI.AllGetBtn.material = m_SUI.AllGetBtnMat[canget ? 0 : 1];
		m_SUI.AllGetBtn.GetComponent<Button>().interactable = canget;
		m_SUI.AllGetBtnTxt.color = canget ? Utile_Class.GetCodeColor("#E0D499") : Utile_Class.GetCodeColor("#A4B999");
	}

	bool IsGroup(Page page, MakingGroup gropup)
	{
		switch (page)
		{
		case Page.Equip: return gropup == MakingGroup.Equip;
		case Page.CharacterEquip: return gropup == MakingGroup.PrivateEquip;
		case Page.ResearchMat: return gropup == MakingGroup.ResearchMaterial;
		case Page.EquipMat: return gropup == MakingGroup.MakingMaterial;
		}
		return false;
	}

	/// <summary> 제작서들 제작 가능 여부와 등급 순위로 정렬 </summary>
	void RecipeListUp() {
		//테이블의 모든 제작가능한 아이템만 리스트업
		List<TMakingTable> canrecipes = new List<TMakingTable>();
		List<TMakingTable> alltdata = TDATA.GetAllMakingTable();
		int LV = Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp));
		canrecipes.AddRange(alltdata.FindAll(o => o.m_LV <= LV && IsGroup(m_Page, o.m_Group)));

		// 우선순위 1. 제작 완료됨 → 2. 제작 가능 → 3. 제작 불가능(재료 부족) → 4. 제작 진행 중 → 5. 제작 불가능(레벨 부족)
		canrecipes.Sort((TMakingTable _a, TMakingTable _b) => {
			int astate = GetSortState(_a);
			int bstate = GetSortState(_b);
			if (astate != bstate) return astate.CompareTo(bstate);
			// Point 높은 순
			return _b.m_Point.CompareTo(_a.m_Point);
		});

		//카드들 삭제 후 리스트 대로 생성
		for (int i = m_AllElement.Count - 1; i >= 0; i--) Destroy(m_AllElement[i].gameObject);
		m_AllElement.Clear();

		for (int i = 0; i < canrecipes.Count; i++) {
			Item_Mk_Element element = Utile_Class.Instantiate(m_SUI.ElementPrefabs, m_SUI.Bucket).GetComponent<Item_Mk_Element>();
			element.SetData(canrecipes[i], SetPlayCB);
			element.transform.SetAsLastSibling();
			m_AllElement.Add(element);
		}

		m_SUI.LvUpMsg.SetActive(canrecipes.Count < 1);
	}
	void CharEqListUp() {
		//전용장비 캐릭터 리스트업
		m_AllCharEqElement.Clear();
		int lv = Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp));
		m_SUI.CharEqTitle.text = string.Format("{0} Lv.{1}",TDATA.GetString(281), lv);
		List<TMakingTable> tables = TDATA.GetAllMakingTable().FindAll(o => o.m_Group == MakingGroup.PrivateEquip && o.m_LV == lv);
		tables.Sort((TMakingTable _after, TMakingTable _before) => {
			return _before.GetCanMake().CompareTo(_after.GetCanMake());
		});
		UTILE.Load_Prefab_List(tables.Count, m_SUI.CharEqBucket, m_SUI.CharEqPrefab.transform);

		for (int i = 0; i < m_SUI.CharEqBucket.childCount; i++) {
			Item_Mk_Element_Char element = m_SUI.CharEqBucket.GetChild(i).GetComponent<Item_Mk_Element_Char>();
			element.SetData(tables[i], SetPlayCB);
			m_AllCharEqElement.Add(element);
		}
		SetCharEqGrade(0);
	}
	/// <summary> 소팅용 번호 만들기 </summary>
	/// <returns>
	/// <para>1 : 제작 완료됨</para>
	/// <para>2 : 제작 가능 </para>
	/// <para>3 : 제작 불가능(재료 부족) </para>
	/// <para>4 : 제작 진행 중 </para>
	/// <para>5 : 제작 불가능(레벨 부족) </para>
	/// </returns>
	int GetSortState(TMakingTable data) {
		MakingInfo ainfo = USERINFO.m_Making.Find(t => t.m_Idx == data.m_ItemIdx);
		if (ainfo != null) {
			if (ainfo.IS_Complete()) return 1;
			else return 4;
		}

		if (data.GetCanMake()) return 2;
		else if (USERINFO.m_MakeLV < data.m_LV) return 5;
		return 3;
	}
	public void SetPlayCB(Item_Mk_Element_Parent _element) {
		switch (_element.m_State)
		{
		case TimeContentState.Idle:
			if (TUTO.TouchCheckLock(TutoTouchCheckType.Making, 2, null, _element.m_Mk_TData.m_ItemIdx)) return;
			m_CloaseCB?.Invoke(Item_PDA_Making.State.Detail, new object[] { _element });
			break;
		case TimeContentState.Play:
#if NOT_USE_NET
			if (_element.m_Info.IS_Complete()) {
				PlayCommVoiceSnd(VoiceType.Success);
				SetReward(_element.m_Info);
				
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Making_Count);
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Weapon_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() == ItemInvenGroupType.Equipment && TDATA.GetItemTable(o.GetIdx()).GetEquipType() == EquipType.Weapon));
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Armor_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() == ItemInvenGroupType.Equipment && TDATA.GetItemTable(o.GetIdx()).GetEquipType() != EquipType.Weapon));
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Etc_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() != ItemInvenGroupType.Equipment));

				if (m_Rewards.Count > 0) {
					MAIN.SetRewardList(new object[] { m_Rewards }, () => {
						m_Rewards.Clear(); 
					});
				}

				SetRecipe();
				SetAlarm();
				AllGetBtnSet();
				MAIN.Save_UserInfo();
			}
			else {
				POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(215), (result, obj) =>
				{
					if (result == 1) {
						_element.m_Info.PastComplete();
						SetPlayCB(_element);
					}
				}, PriceType.Cash, (int)USERINFO.m_Cash, BaseValue.GetTimePrice(ContentType.Making, _element.m_Info.GetRemainTime()));
			}
#else
			List<RES_REWARD_BASE> equips = MAIN.GetRewardData(RewardKind.Item, _element.m_Info.m_TData.m_ItemIdx, 1, true).FindAll(o => o.Type == Res_RewardType.Item && TDATA.GetItemTable(o.GetIdx()).GetEquipType() != EquipType.End);
			int maxgrade = equips == null || equips.Count < 1 ? 1 : equips.Select(o => o.GetGrade()).Max();

			if (_element.m_Info.IS_Complete())
			{
				PlayCommVoiceSnd(VoiceType.Success);
				WEB.SEND_REQ_MAKING_END((res) =>
				{
					if (!res.IsSuccess())
					{
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						return;
					}

					SetRecipe();
					AllGetBtnSet();
					SetAlarm();
				
					if (res.Rewards == null) return;
					m_Rewards.AddRange(res.GetRewards());

					if(m_Rewards.Count > 0) {
						MAIN.SetRewardList(new object[] { m_Rewards }, () => {
							m_Rewards.Clear();
						}, maxgrade);
					}
				}, new List<long>() { _element.m_Info.m_UID }, false);
			}
			else{
				POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(215), (result, obj) =>
				{
					if (result == 1) {
						if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
							WEB.SEND_REQ_MAKING_END((res) =>
							{
								if (!res.IsSuccess()) {
									WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
									return;
								}

								SetRecipe();
								AllGetBtnSet();
								SetAlarm();

								if (res.Rewards == null) return;
								m_Rewards.AddRange(res.GetRewards());

								if (m_Rewards.Count > 0) {
									MAIN.SetRewardList(new object[] { m_Rewards }, () => {
										m_Rewards.Clear();
									}, maxgrade);
								}
							}, new List<long>() { _element.m_Info.m_UID }, true);
						}
						else {
							POPUP.StartLackPop(BaseValue.CASH_IDX);
						}
						
					}
					//SetPlayCB(_element);
				}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Making, _element.m_Info.GetRemainTime()), false);
			}
#endif
				break;
		}
	}

	void SetRecipe() {
		if (m_Page != Page.CharacterEquip) {
			m_SUI.ViewGroup[0].SetActive(true);
			m_SUI.ViewGroup[1].SetActive(false);
			m_SUI.ViewGroup[2].SetActive(false);
			RecipeListUp();
		}
		else {
			m_SUI.ViewGroup[0].SetActive(false);
			m_SUI.LvUpMsg.SetActive(false);
			MakingInfo chareqinfo = USERINFO.m_Making.Find(o => o.m_TData.m_Group == MakingGroup.PrivateEquip && o.m_State == TimeContentState.Play);
			if (chareqinfo != null) {//진행중인 생산이 있으면
				m_SUI.ViewGroup[1].SetActive(false);
				m_SUI.ViewGroup[2].SetActive(true);
				m_SUI.CharEqProgress.SetData(chareqinfo, SetPlayCB);
			}
			else {
				m_SUI.ViewGroup[1].SetActive(true);
				m_SUI.ViewGroup[2].SetActive(false);
				int idx = BaseValue.CHAREQUIP_MAKING_OPEN;
				bool open = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= idx;
				m_SUI.CharEquipPanels[0].SetActive(open);
				m_SUI.CharEquipPanels[1].SetActive(open);
				m_SUI.CharEquipLock.gameObject.SetActive(!open);
				m_SUI.CharEquipLock.text = string.Format(TDATA.GetString(273), idx / 100, idx % 100);
				if (open) CharEqListUp();
			}
		}
	}
	void SetReward(MakingInfo _info) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Making, 1, _info, 0)) return;
		//아이템 획득, 제작 리스트에서 삭제, 서버 동기화
		if (_info.m_TData.m_Group != MakingGroup.PrivateEquip) {
			Item_Mk_Element_Parent card = m_AllElement.Find(t => t.m_Info == _info);
			if (card == null) return;
		}

		PlayEffSound(SND_IDX.SFX_0123);

		int itemIdx = _info.m_TData.m_ItemIdx;
		// 추후 서버에서 받은 데이터를 그대로 표시하도록 하기위해 만들어줌
		
		var itemTable = TDATA.GetItemTable(itemIdx);
		if(itemTable.m_Type == ItemType.RandomBox || itemTable.m_Type == ItemType.AllBox) {  //박스면 바로 까서 주기
			for (int i = 0; i < _info.m_Count; i++) {
				List<RES_REWARD_BASE> resreward = TDATA.GetGachaItem(itemTable);
				m_Rewards.AddRange(resreward);
			}
		}
		else {
			var item = USERINFO.InsertItem(itemIdx, _info.m_Count);
			RES_REWARD_ITEM resitem = new RES_REWARD_ITEM() {
				Type = Res_RewardType.Item,
				UID = item.m_Uid,
				Idx = itemIdx,
				Cnt = _info.m_Count,
			};
			m_Rewards.Add(resitem);
		}

		USERINFO.DeleteMake(_info);
	}
	public void ClickAllGet() {
		if (m_Page != Page.CharacterEquip) {
			List<Item_Mk_Element> compmake = m_AllElement.FindAll((t) => t.m_Info != null && t.m_Info.IS_Complete());
#if NOT_USE_NET
		for(int i = 0; i < compmake.Count; i++) {
			SetReward(compmake[i].m_Info);
		}
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Making_Count, 0, compmake.Count);
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Weapon_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() == ItemInvenGroupType.Equipment && TDATA.GetItemTable(o.GetIdx()).GetEquipType() == EquipType.Weapon));
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Armor_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() == ItemInvenGroupType.Equipment && TDATA.GetItemTable(o.GetIdx()).GetEquipType() != EquipType.Weapon));
		USERINFO.m_Achieve.Check_Achieve(AchieveType.Etc_Making_Count, 0, m_Rewards.Count(o => TDATA.GetItemTable(o.GetIdx()).GetInvenGroupType() != ItemInvenGroupType.Equipment));

		if(m_Rewards.Count > 0) {
			MAIN.SetRewardList(new object[] { m_Rewards }, () => {
				m_Rewards.Clear();
			});
		}

		RecipeListUp();
		AllGetBtnSet();
		SetAlarm();
		MAIN.Save_UserInfo();
#else
			WEB.SEND_REQ_MAKING_END((res) => {
				if (!res.IsSuccess()) {
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					return;
				}

				RecipeListUp();
				AllGetBtnSet();
				SetAlarm();

				if (res.Rewards == null) return;
				m_Rewards.AddRange(res.GetRewards());
				if (m_Rewards.Count > 0) {
					MAIN.SetRewardList(new object[] { m_Rewards }, () => {
						m_Rewards.Clear();
					});
				}
			}, compmake.Select(o => o.m_Info.m_UID).ToList(), false);
#endif
		}
		else {
			MakingInfo chareqinfo = USERINFO.m_Making.Find(o => o.m_TData.m_Group == MakingGroup.PrivateEquip && o.m_State == TimeContentState.Play);
			if (chareqinfo != null && chareqinfo.IS_Complete()) SetPlayCB(new Item_Mk_Element_Parent() { m_Mk_TData = chareqinfo.m_TData });
		}
	}

	public void ClickGradeInfo() {
		m_CloaseCB?.Invoke(Item_PDA_Making.State.GradeInfo, null);
	}

	public override void OnClose() {
		m_CloaseCB?.Invoke(Item_PDA_Making.State.End, null);
	}

	/// <summary> 제작중인 아이템들 남은 시간 갱신 </summary>
	IEnumerator IE_MakeTimer() {
		int sec = (int)UTILE.Get_Time();
		for (int i = 0; i < m_AllElement.Count; i++) {
			m_AllElement[i].RefreshTimer();
		}
		m_SUI.CharEqProgress.RefreshTimer();
		AllGetBtnSet();

		yield return new WaitUntil(() => sec < (int)UTILE.Get_Time());//실제 초단위가 바뀔때

		StartCoroutine(IE_MakeTimer());
	}

	/// <summary> 전용 장비 생산 가이드 팝업 </summary>
	public void Click_CharEqHelp() {
		m_CloaseCB?.Invoke(Item_PDA_Making.State.CharEqInfo, null);
	}
	public void SetCharEqGrade(int _grade) {
		for(int i = 0;i< m_AllCharEqElement.Count; i++) {
			m_AllCharEqElement[i].gameObject.SetActive(_grade == 0 || m_AllCharEqElement[i].GetGrade() == _grade);
		}
		for (int i = 0; i < m_SUI.GradeTapImg.Length; i++) {
			m_SUI.GradeTapImg[i].color  = new Color(m_SUI.GradeTapImg[i].color.r, m_SUI.GradeTapImg[i].color.g, m_SUI.GradeTapImg[i].color.b, i == _grade ? 1 : 0);
			m_SUI.GradeTapTxt[i].color = m_SUI.GradeTapTxtColor[i != _grade ? 0 : 1];
		}
	}
}
