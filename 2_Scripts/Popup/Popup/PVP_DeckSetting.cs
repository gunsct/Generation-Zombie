using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class PVP_DeckSetting : PopupBase
{
	public enum State
	{
		None,
		CardDrag,
		Hold,
		Back
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_Tab[] Tabs;

		public GameObject[] TutoObj;//0:진형 탭 1:팀변경버튼 2:지원가탭3:나가기
		public ScrollRect Scroll;
		public Button SaveBtn;
	}
	[Serializable]
	public struct SDUI
	{
		public TextMeshProUGUI[] DeckPosName;
		public Transform CPBucket;
		public Transform CPNumFont;
		public TextMeshProUGUI[] SurvStats;
		public Item_PVP_DeckSetting_Slot[] Slots;
	}
	[Serializable]
	public struct SCUI
	{
		public Image PosIcon;
		public TextMeshProUGUI PosName;
		public Item_SortingGroup SortingGroup;
		public Transform CharBucket;
		public Transform CharElement;               //Item_CharManageCard
		public GameObject Empty;
		public Image DeckPosBtnIcon;
		public TextMeshProUGUI DeckPosBtnName;
		public ScrollRect ScrollReck;
		public RectTransform ScrollTrans;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SDUI m_SDUI;
	[SerializeField] SCUI m_SCUI;

	List<Item_CharManageCard> m_AllChar = new List<Item_CharManageCard>();
	Item_PVP_DeckSetting_Slot m_SelectDeckChar;
	Item_CharManageCard m_SelectChar;

	[SerializeField] PVPDeckPos m_DeckPos = PVPDeckPos.Atk;
	[SerializeField] PVPPosType m_BTPos = PVPPosType.Combat;
	DeckInfo m_DeckInfo { get { return USERINFO.m_Deck[m_DeckPos == PVPDeckPos.Atk ? BaseValue.MAX_DECK_POS_PVP_ATK : BaseValue.MAX_DECK_POS_PVP_DEF]; } }
	long[] m_PreAtkDeck;
	long[] m_PreDefDeck;

	GraphicRaycaster m_GR;
	State m_State = State.None;
	[SerializeField] float m_HoldTime = 0f;				//드래그드랍 조건 시간
	IEnumerator m_Action;

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFCharInfoCard += CardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard += CardRefresh;
		}
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFCharInfoCard -= CardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard -= CardRefresh;
		}
	}
	private void Update() {
		if (m_State == State.Back) return;
		//아무 상태 아닐때 터치하고 있는게 캐릭터 카드일떄
		//잡고있는 캐릭터 카드가 0.25초 이상 지속된 경우 드래그 앤 드랍 상태로 변경
		//드래그앤드랍동안 슬롯 이펙트와, 원본카드 버킷과 복사카드 자체 포지션 이동
		//드랍할때 포지션 체크해서 빈 슬롯이면 슬롯에 채워주기
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		if(Input.touchCount == 1)
#endif
		if (Input.GetMouseButton(0) && POPUP.GetPopup() == this) {
			if (m_State == State.None) {
				List<RaycastResult> results = new List<RaycastResult>();
				PointerEventData ped = new PointerEventData(null);
				ped.position = Input.mousePosition;
				m_GR.Raycast(ped, results);

				for (int i = 0; i < results.Count; i++) {
					if (results[i].gameObject.GetComponent<RayCastIgnore>() != null) break;
					if (results[i].gameObject.name.Equals("Item_CharManageCard_Bucket") && m_SCUI.ScrollReck.velocity.magnitude < 30) {
						if (Mathf.Abs(m_SCUI.ScrollReck.velocity.y) > 0.1f) continue;
						Item_CharManageCard card = results[i].gameObject.transform.parent.GetComponent<Item_CharManageCard>();
						if (card == null) continue;

						if (m_SelectChar == card) m_HoldTime += Time.deltaTime;
						else m_HoldTime = 0f;
						m_SelectChar = card;

						if (m_HoldTime > 0.25f) {//1초 이상 누르고 있으면 드래그 가능
							m_State = State.Hold;
							card.OpenDetail();
							AutoScrolling(m_SelectChar.transform);
							m_SelectChar = null;
						}
						break;
					}
					else {
						Item_PVP_DeckSetting_Slot card = results[i].gameObject.transform.GetComponent<Item_PVP_DeckSetting_Slot>();
						if (card == null) continue;

						if (m_SelectDeckChar == card) m_HoldTime += Time.deltaTime;
						else m_HoldTime = 0f;
						m_SelectDeckChar = card;

						if (m_HoldTime > 0.25f) {//1초 이상 누르고 있으면 드래그 가능
							m_State = State.Hold;
							if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Hold, m_SelectChar)) return;
							card.OpenDetail(m_DeckInfo);
						}
						break;
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(0)) {
			m_SCUI.ScrollReck.enabled = true;
			m_HoldTime = 0f;
			m_State = State.None;
			m_SelectChar = null;
		}
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_GR = POPUP.GetComponent<GraphicRaycaster>();//캔버스 레이케스트용

		m_PreAtkDeck = USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].m_Char.ToArray(); 
		m_PreDefDeck = USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].m_Char.ToArray();

		UTILE.Load_Prefab_List(USERINFO.m_Chars.Count, m_SCUI.CharBucket, m_SCUI.CharElement);
		for(int i = 0;i< USERINFO.m_Chars.Count; i++) {
			Item_CharManageCard element = m_SCUI.CharBucket.GetChild(i).GetComponent<Item_CharManageCard>();
			element.SetData(USERINFO.m_Chars[i].m_Idx, m_AllChar, Item_CharManageCard.Mode.PVP, ClickCharCard);
			m_AllChar.Add(element);
		}
		m_SUI.Tabs[0].SetData(1, TDATA.GetString(10011), ClickBtTap);
		m_SUI.Tabs[0].SetAlram(false);
		m_SUI.Tabs[0].OnClick();
		m_SUI.Tabs[1].SetAlram(false);
		m_SUI.Tabs[1].SetData(2, TDATA.GetString(10012), ClickBtTap);
		m_SCUI.SortingGroup.SetData(SetSort);
		SetSort();

		m_SUI.SaveBtn.interactable = USERINFO.IS_ChangeDeck();

		base.SetData(pos, popup, cb, aobjValue);
		if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.Select_DeckSetBtn)) TUTO.Next();
		StartCoroutine(m_Action = IE_StartActin());
	}
	IEnumerator IE_StartActin() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 0.3f));
		for(int i = 0; i < m_SDUI.Slots.Length; i++) {
			m_SDUI.Slots[i].SetAnim("Start");
		}
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.DeckSet_Action)) TUTO.Next();
		m_Action = null;
	}
	public override void SetUI() {
		//덱슬롯과 캐릭터 목록 UI
		SetDeckInfo();
		//덱 스탯,전투력 UI
		SetDeckCPSurv();
		//공방덱 전환 버튼 UI
		SetDeckPosBtn();
		//어태커서포터 UI
		SetBtPosGroup();

		base.SetUI();
	}
	/// <summary> 전투력 표기 </summary>
	void SetCP(long _cp, Color _color) {

		// [20230328] ODH 전투력 0일때 숫자 안나옴 셋팅 방식 변경
		string cp = _cp.ToString();
		int length = cp.Length;
		UTILE.Load_Prefab_List(length, m_SDUI.CPBucket, m_SDUI.CPNumFont);
		for (int i = 0; i < length; i++)
		{
			Item_PVP_Main_CP_NumFont num = m_SDUI.CPBucket.GetChild(i).GetComponent<Item_PVP_Main_CP_NumFont>();
			num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", cp.Substring(i, 1)), "png"), _color);
		}
		//List<int> nums = new List<int>();
		//int cp = (int)_cp;
		//while (cp != 0) {
		//	nums.Add(cp % 10);
		//	cp = Mathf.FloorToInt(cp / 10);
		//}
		//UTILE.Load_Prefab_List(nums.Count, m_SDUI.CPBucket, m_SDUI.CPNumFont);
		//for (int i = 0; i < nums.Count; i++) {
		//	Item_PVP_Main_CP_NumFont num = m_SDUI.CPBucket.GetChild(nums.Count - 1 - i).GetComponent<Item_PVP_Main_CP_NumFont>();
		//	num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", nums[i]), "png"));
		//}
	}
	/// <summary> 덱 스탯, 전투력 표기 </summary>
	void SetDeckCPSurv() {

		float[] vals = new float[3];//0:men,1:hyg,2:sat,3:cp
		long cp = 0;
		for(int i = 0; i < m_DeckInfo.m_Char.Length; i++) {
			CharInfo cinfo = USERINFO.GetChar(m_DeckInfo.m_Char[i]);
			if (cinfo == null) continue;
			vals[0] += cinfo.GetStat(StatType.Men, 0, 0, 0, 0, true);
			vals[1] += cinfo.GetStat(StatType.Hyg, 0, 0, 0, 0, true);
			vals[2] += cinfo.GetStat(StatType.Sat, 0, 0, 0, 0, true);
			cp += cinfo.m_PVPCP;
		}
		for (int i = 0; i < 3; i++) {
			m_SDUI.SurvStats[i].text = Mathf.RoundToInt(vals[i]).ToString();
		}
		SetCP(cp, m_DeckPos == PVPDeckPos.Atk ? Utile_Class.GetCodeColor("#FFD2D1") : Utile_Class.GetCodeColor("#B3ECFF"));
	}
	/// <summary> 덱 세팅, 덱 슬롯에 있나 없나, 어태커서포터에 따라 변경  </summary>
	void SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode _mode = Item_PVP_DeckSetting_Slot.Mode.Init, int _slotpos = -1) {
		int activecnt = 0;
		//덱 슬롯 갱신
		for (int i = 0; i < m_SDUI.Slots.Length; i++) {
			if(_slotpos == -1 || _slotpos == i)
				m_SDUI.Slots[i].SetData(i, USERINFO.GetChar(m_DeckInfo.m_Char[i]), USERINFO.GetChar(m_DeckInfo.m_Char[i + 5]), m_BTPos, ClickSlot, _mode);
		}
		//캐릭터 목록 갱신
		for(int i = 0; i < m_AllChar.Count; i++) {
			CharInfo cinfo = m_AllChar[i].m_Info;
			bool indeck = m_DeckInfo.IS_InDeck(cinfo.m_Idx);
			bool posequal = m_BTPos == cinfo.m_TData.m_PVPPosType;
			m_AllChar[i].gameObject.SetActive(!indeck && posequal);
			m_AllChar[i].SetData(cinfo);
			if (!indeck && posequal) activecnt++;
		}
		m_SCUI.Empty.SetActive(activecnt < 1);
	}
	/// <summary> 슬롯 선택(빼기) </summary>
	void ClickSlot(int _pos)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_DeckSetting, 0, _pos)) return;

		PlayEffSound(SND_IDX.SFX_0010);
		m_SUI.SaveBtn.interactable = true;
		m_DeckInfo.SetChar(_pos, 0);
		SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.Refresh, _pos > 4 ? _pos - 5 : _pos);
		//덱 스탯,전투력 UI
		SetDeckCPSurv();
	}
	/// <summary> 캐릭터 선택(빈슬롯에 넣기)</summary>
	void ClickCharCard(Item_CharManageCard _card)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_DeckSetting, 1, _card)) return;
		//빈슬롯
		int pos = -1;
		for (int i = 0 ; i < 5; i++) {
			if (m_SDUI.Slots[i].GetNowCInfo() == null) {
				pos = i + (_card.m_Info.m_TData.m_PVPPosType == PVPPosType.Combat ? 0 : 5);
				break;
			}
		}
		if (pos < 0) return;

		PlayEffSound(SND_IDX.SFX_0010);
		m_SUI.SaveBtn.interactable = true;
		m_DeckInfo.SetChar(pos, _card.m_Info.m_UID);
		SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.Refresh, pos > 4 ? pos - 5 : pos);
		//덱 스탯,전투력 UI
		SetDeckCPSurv();
	}
	/// <summary> 어태커, 서포터 스왑 </summary>
	bool ClickBtTap(Item_Tab _tab)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_DeckSetting, 2, _tab.m_Pos)) return false;
		PlayEffSound(SND_IDX.SFX_0011);
		m_SUI.Tabs[(int)m_BTPos - 1].SetActive(false);
		m_BTPos = (PVPPosType)_tab.m_Pos;
		m_SUI.Tabs[(int)m_BTPos - 1].SetActive(true);
		SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.BTChange);
		SetBtPosGroup();
		return true;
	}
	/// <summary> 어태커지원가 유아이 </summary>
	void SetBtPosGroup() {
		m_SCUI.PosIcon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_PVP_Pos_{0}", m_BTPos == PVPPosType.Combat? "Atk" : "Sup"), "png");
		m_SCUI.PosName.text = TDATA.GetString(m_BTPos == PVPPosType.Combat ? 10011 : 10012);
	}
	/// <summary> 공방덱 스왑</summary>
	public void ClickDeckPosSwap()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_DeckSetting, 3)) return;
		PlayEffSound(SND_IDX.SFX_0011);
		m_DeckPos = m_DeckPos == PVPDeckPos.Atk ? PVPDeckPos.Def : PVPDeckPos.Atk;
		SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.Refresh);
		SetDeckCPSurv();
		SetDeckPosBtn();
	}
	/// <summary> 공방덱 스왑 버튼 </summary>
	void SetDeckPosBtn() {
		m_SUI.Anim.SetTrigger("SlotChange");
		m_SUI.Anim.SetTrigger(m_DeckPos == PVPDeckPos.Atk ? "Slot_ATK" : "Slot_DEF");
		m_SCUI.DeckPosBtnIcon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", m_DeckPos == PVPDeckPos.Atk ? 5 : 4), "png");
		m_SCUI.DeckPosBtnName.text = TDATA.GetString(m_DeckPos == PVPDeckPos.Atk ? 10014 : 10013);
		for (int i = 0; i < m_SDUI.DeckPosName.Length; i++) m_SDUI.DeckPosName[i].text = string.Format("{0} {1}", TDATA.GetString(m_DeckPos == PVPDeckPos.Atk ? 10013 : 10014), TDATA.GetString(10015));
	}
	void SetSort() {
		//for (int i = 0; i < m_AllChar.Count; i++) m_AllChar[i].m_Info.GetCombatPower(0,0,true);
		switch (m_SCUI.SortingGroup.m_Condition) {
			case SortingType.Grade:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.CombatPower:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Level:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Men:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					float aftermen = after.m_Info.m_PVPStat[StatType.Men];
					float beformen = befor.m_Info.m_PVPStat[StatType.Men];
					if (beformen != aftermen) return aftermen.CompareTo(beformen);
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Hyg:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					float afterhyg = after.m_Info.m_PVPStat[StatType.Hyg];
					float beforhyg = befor.m_Info.m_PVPStat[StatType.Hyg];
					if (beforhyg != afterhyg) return afterhyg.CompareTo(beforhyg);
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Sat:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					float aftersat = after.m_Info.m_PVPStat[StatType.Sat];
					float beforsat = befor.m_Info.m_PVPStat[StatType.Sat];
					if (beforsat != aftersat) return aftersat.CompareTo(beforsat);
					int aftercp = after.m_Info.m_PVPCP;
					int beforcp = befor.m_Info.m_PVPCP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
		}

		if (m_SCUI.SortingGroup.m_Ascending) m_AllChar.Reverse();
		for (int i = 0; i < m_AllChar.Count; i++) {
			m_AllChar[i].transform.SetAsLastSibling();

			if (m_AllChar[i] != null) {
				m_AllChar[i].SetSortVal(m_SCUI.SortingGroup.m_Condition);
			}
		}
	}
	public void CardRefresh() {
		for (int i = 0; i < m_AllChar.Count; i++) {
			Item_CharManageCard card = m_AllChar[i];
			if (card != null) {
				card.SetLvGrade();
				card.SetRankUpAlarm();
				card.SetSortVal(m_SCUI.SortingGroup.m_Condition);
			}
		}
		//덱슬롯과 캐릭터 목록 UI
		SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.Refresh);
		//덱 스탯,전투력 UI
		SetDeckCPSurv();
	}
	/// <summary> 카드가 버킷에서 벗어난 경우 즉각 자동으로 이동 </summary>
	void AutoScrolling(Transform _trans) {
		float posy = 0f;

		float buckettop = m_SCUI.ScrollTrans.position.y + m_SCUI.ScrollTrans.rect.height / 2;
		float bucketbottom = m_SCUI.ScrollTrans.position.y - m_SCUI.ScrollTrans.rect.height / 2;

		float cardtop = _trans.position.y + _trans.GetComponent<RectTransform>().rect.height / 2 + 68;
		float cardbottom = _trans.position.y - _trans.GetComponent<RectTransform>().rect.height / 2 - 250;

		if (buckettop < cardtop) {//카드 위가 잘릴 경우
			posy = cardtop - buckettop;
			m_SCUI.CharBucket.localPosition -= new Vector3(0f, posy, 0f);
		}
		if (bucketbottom > cardbottom) {// 카드 아래가 잘릴 경우
			posy = bucketbottom - cardbottom;
			m_SCUI.CharBucket.localPosition += new Vector3(0f, posy, 0f);
		}
	}
	public void ClickSave() {
		if (!USERINFO.IS_ChangeDeck()) return;

		if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].IS_FullDeck(true)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10076));
			return;
		}
		if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].IS_FullDeck(true)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10077));
			return;
		}
		PlayEffSound(SND_IDX.SFX_1400);
		WEB.SEND_REQ_DECK_SET((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					WEB.SEND_REQ_DECK((res2) => {
						// 다시 보여주기
						SetUI();
					}, USERINFO.m_UID);
				});
				return;
			}
			for (int i = 0; i < USERINFO.m_Deck.Length; i++) USERINFO.m_Deck[i].IsChange = false;
			m_SUI.SaveBtn.interactable = false;
		});
	}

	public void Click_AutoSet() {
		if (TUTO.IsTutoPlay()) return;
		int atkcnt = 0;
		int sptcnt = 0;
		for(int i = 0; i < USERINFO.m_Chars.Count; i++) {
			TCharacterTable tdata = USERINFO.m_Chars[i].m_TData;
			if (tdata.m_PVPPosType == PVPPosType.Combat) atkcnt++;
			else sptcnt++;
		}
		if(atkcnt < 5 || sptcnt < 5) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(276));
			return;
		}

		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, TDATA.GetString(10883), (res, obj) => {
			if(res == 1) {
				AutoDeckSet(m_DeckPos);
				POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, string.Format(TDATA.GetString(10886), TDATA.GetString(m_DeckPos == PVPDeckPos.Atk ? 10014 : 10013)), (res, obj) => {
					if (res == 1) {
						AutoDeckSet(m_DeckPos == PVPDeckPos.Atk ? PVPDeckPos.Def : PVPDeckPos.Atk);
					}
					//덱 스탯,전투력 UI
					SetDeckInfo(Item_PVP_DeckSetting_Slot.Mode.Refresh);
					SetDeckCPSurv();
					ClickSave();
				});
			}
		});
		
	}
	void AutoDeckSet(PVPDeckPos _pos) {
		DeckInfo dinfo = USERINFO.m_Deck[_pos == PVPDeckPos.Atk ? BaseValue.MAX_DECK_POS_PVP_ATK : BaseValue.MAX_DECK_POS_PVP_DEF];
		for (int i = 0; i < dinfo.m_Char.Length; i++) {
			dinfo.SetChar(i, 0);
		}

		int atkpullcnt = 0;
		int sptpullcnt = 0;
		for (int i = 0; atkpullcnt < 5 || sptpullcnt < 5; i++) {
			CharInfo cinfo = m_AllChar[i].m_Info;
			if (atkpullcnt < 5 && cinfo.m_TData.m_PVPPosType == PVPPosType.Combat) {
				dinfo.SetChar(atkpullcnt, cinfo.m_UID);
				atkpullcnt++;
			}
			else if (sptpullcnt < 5 && cinfo.m_TData.m_PVPPosType == PVPPosType.Supporter) {
				dinfo.SetChar(5 + sptpullcnt, cinfo.m_UID);
				sptpullcnt++;
			}
		}
	}
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (USERINFO.IS_ChangeDeck()) {
			POPUP.Set_MsgBox(PopupName.Msg_YN, "", TDATA.GetString(10078), (res, obj) => {
				if ((EMsgBtn)res == EMsgBtn.BTN_YES) {
					if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].IS_FullDeck(true)) {
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10076));
						return;
					}
					if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].IS_FullDeck(true)) {
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10077));
						return;
					}
					PlayEffSound(SND_IDX.SFX_1400);
					WEB.SEND_REQ_DECK_SET((res) => {
						if (!res.IsSuccess()) {
							WEB.StartErrorMsg(res.result_code, (btn, obj) => {
								WEB.SEND_REQ_DECK((res2) => {
									// 다시 보여주기
									SetUI();
								}, USERINFO.m_UID);
							});
							return;
						}
						for (int i = 0; i < USERINFO.m_Deck.Length; i++) USERINFO.m_Deck[i].IsChange = false;
						base.Close(Result);
					});
				}
				else {
					USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].m_Char = m_PreAtkDeck;
					USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].IsChange = false;
					USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].m_Char = m_PreDefDeck;
					USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].IsChange = false;
					base.Close(Result);
				}
			});
		}
		else base.Close(Result);
	}

	///////튜토용
	/// <summary>0:진형 탭 1:팀변경버튼 2:지원가탭3:나가기 </summary>
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}

	public void ScrollLock(bool _lock)
	{
		m_SUI.Scroll.enabled = !_lock;
	}
}
