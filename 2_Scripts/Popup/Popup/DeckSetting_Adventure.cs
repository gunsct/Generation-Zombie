using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class DeckSetting_Adventure : PopupBase
{
	public enum State
	{
		None,
		Hold,
		CardDrag,
		GoStage
	}
	[Serializable]
	public struct SUI
	{
		public Transform PartyGradeGroup;
		public TextMeshProUGUI NeedCharCnt;
		public Color[] NeedCharCntColor;
		public Image FullCharIcon;
		public Sprite[] FullICharSprite;
		public Item_CharDeckCard[] DeckCards;
		[Header("그외")]
		public Transform BGPanel;
		public Transform CloneCardBucket;
		public ScrollRect ScrollReck;
		public Item_SortingGroup SortingGroup;
		public GameObject Empty;
		[Header("생존자 목록")]
		public GameObject CharCardPrefab;
		public Transform CharObjBucket;
		public TextMeshProUGUI CharCnt;
		public RectTransform ScrollContent;
		public RectTransform ScrollTrans;
	}
	[SerializeField]
	SUI m_SUI;
	TAdventureTable m_TData;
	public Dictionary<int, long> m_Chars = new Dictionary<int, long>();
	List<Item_CharManageCard> m_AllChar = new List<Item_CharManageCard>();
	[SerializeField] Item_CharManageCard m_SelectChar;       //드래그드랍 원본 카드
	[SerializeField] Item_CharManageCard m_MoveCloneChar;    //드래그드랍 할 복제 카드
	[SerializeField] Item_CharDeckCard m_SelectDeckChar;

	[SerializeField]
	State m_State = State.None;

	GraphicRaycaster m_GR;
	[SerializeField] float m_HoldTime = 0f;     //드래그드랍 조건 시간

	private void Awake() {
		for(int i = 0; i < m_SUI.DeckCards.Length; i++) {
			m_SUI.DeckCards[i].gameObject.SetActive(false);
		}
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFCharInfoCard += CharCardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard += DeckCharCardRefresh;
		}
	}
	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFCharInfoCard -= CharCardRefresh;
			DLGTINFO.f_RFDeckCharInfoCard -= DeckCharCardRefresh;
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_TData = (TAdventureTable)aobjValue[0];

		Item_MainMenu_Stg_Bg bg = UTILE.LoadPrefab(TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty()).GetBGName(), true, m_SUI.BGPanel).GetComponent<Item_MainMenu_Stg_Bg>();
		//bg.SketchFXOnOff(false);
		bg.MaskOff();
		m_GR = POPUP.GetComponent<GraphicRaycaster>();//캔버스 레이케스트용

		m_SUI.SortingGroup.SetData(SetSort);

		List<long> alreaychar = new List<long>();
		for(int i = 0; i < USERINFO.m_Advs.Count; i++) {
			alreaychar.AddRange(USERINFO.m_Advs[i].m_Chars);
		}

		List<long> chars = (List<long>)aobjValue[1];

		//이미 슬롯에 있는 캐릭터들
		List<CharInfo> inslotchars = new List<CharInfo>();
		for (int i = 0; i < m_TData.m_PartyCount; i++) {
			m_SUI.DeckCards[i].gameObject.SetActive(true);
			if (chars.Count > i) {
				CharInfo info = USERINFO.GetChar(chars[i]);
				m_SUI.DeckCards[i].SetData(info);
				m_Chars.Add(i, info.m_UID);
				inslotchars.Add(info);
			}
			else m_SUI.DeckCards[i].SetData();
		}

		//획득했고 탐험 안나가있는 등급 맞는 캐릭터
		for (int i = 0; i < USERINFO.m_Chars.Count; i++) {
			Item_CharManageCard card = null;
			if (!alreaychar.Contains(USERINFO.m_Chars[i].m_UID) ) {
				card = Utile_Class.Instantiate(m_SUI.CharCardPrefab, m_SUI.CharObjBucket).GetComponent<Item_CharManageCard>();
				card.SetData(USERINFO.m_Chars[i].m_Idx, m_AllChar, Item_CharManageCard.Mode.DeckAdventure, Click_CB);
				card.transform.SetAsFirstSibling();
				card.gameObject.SetActive(!inslotchars.Contains(USERINFO.m_Chars[i]));
				m_AllChar.Add(card);
			}
		}
		SetSort();
		for (int i = m_SUI.PartyGradeGroup.childCount - 1; i > -1; i--) m_SUI.PartyGradeGroup.GetChild(i).gameObject.SetActive(i <= m_TData.m_PartyGrade - 1);
		SetCharCount();
	}
	/// <summary> 덱 구성 바뀔때마다 호출 </summary>
	void SetCharCount() {
		//인원 표시 및 버튼 활성
		int gradecnt = 0;
		for(int i = 0; i < m_Chars.Count; i++) {
			if (USERINFO.GetChar(m_Chars.ElementAt(i).Value).m_Grade >= m_TData.m_PartyGrade) gradecnt++;
		}
		m_SUI.FullCharIcon.sprite = m_SUI.FullICharSprite[gradecnt >= m_TData.m_PartyGradeCount ? 0 : 1];
		m_SUI.NeedCharCnt.color = m_SUI.NeedCharCntColor[gradecnt >= m_TData.m_PartyGradeCount ? 0 : 1];
		m_SUI.NeedCharCnt.text = string.Format("{0}/{1}", gradecnt, m_TData.m_PartyGradeCount);

		int nowcnt = m_AllChar.FindAll(t => t.gameObject.activeSelf == true).Count;
		m_SUI.CharCnt.text = string.Format("{0}/{1}", nowcnt.ToString(), USERINFO.m_Chars.Count.ToString());
		m_SUI.Empty.SetActive(nowcnt < 1);
	}
	void SetSort() {
		//for (int i = 0; i < m_AllChar.Count; i++) m_AllChar[i].m_Info.GetCombatPower();
		switch (m_SUI.SortingGroup.m_Condition) {
			case SortingType.Grade:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.CombatPower:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
			case SortingType.Level:
				m_AllChar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
					if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
					int aftercp = after.m_Info.m_CP;
					int beforcp = befor.m_Info.m_CP;
					if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
					if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
					return befor.m_Idx.CompareTo(after.m_Idx);
				});
				break;
		}
		if (m_SUI.SortingGroup.m_Ascending) m_AllChar.Reverse();

		for (int i = 0; i < m_AllChar.Count; i++) {
			m_AllChar[i].transform.SetAsLastSibling();
		}
	}
	private void Update() {
		if (m_State == State.GoStage) return;
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
					if (results[i].gameObject.name.Equals("Item_CharManageCard_Bucket") && m_SUI.ScrollReck.velocity.magnitude < 30) {
						if (Mathf.Abs(m_SUI.ScrollReck.velocity.y) > 0.1f) continue;
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
						Item_CharDeckCard card = results[i].gameObject.transform.GetComponent<Item_CharDeckCard>();
						if (card == null) continue;

						if (m_SelectDeckChar == card) m_HoldTime += Time.deltaTime;
						else m_HoldTime = 0f;
						m_SelectDeckChar = card;

						if (m_HoldTime > 0.25f) {//1초 이상 누르고 있으면 드래그 가능
							m_State = State.Hold;
							if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Hold, m_SelectChar)) return;
							card.OpenDetail();
						}
						break;
					}
				}
			}
		}
		else if (Input.GetMouseButtonUp(0)) {
			m_SUI.ScrollReck.enabled = true;
			m_HoldTime = 0f;
			m_State = State.None;
			m_SelectChar = null;
		}
	}
	/// <summary> 스크롤 드래그 중 </summary>
	public void HoldReset() {
		m_HoldTime = 0f;
	}

	/// <summary> 캐릭터 카드를 액션 콜백 </summary>
	void Click_CB(Item_CharManageCard _card) {
		if (m_State == State.GoStage) return;
		if (m_SelectChar != _card) return;
		switch (_card.m_State) {
			case Item_CharManageCard.State.Click:
				for (int i = 0; i < m_TData.m_PartyCount; i++) {
					if (m_SUI.DeckCards[i].m_State == Item_CharDeckCard.State.Empty) {
						SND.StopAllVoice();
						SND_IDX vocidx = _card.m_Info.m_TData.GetVoice(TCharacterTable.VoiceType.InDeck);
						PlayVoiceSnd(new List<SND_IDX>() { vocidx });

						//선택한 캐릭터 덱에 추가하고 목록에서 꺼주기
						m_SUI.DeckCards[i].SetData(m_SelectChar.m_Info);
						m_Chars.Add(i, m_SelectChar.m_Info.m_UID);
						m_SelectChar.gameObject.SetActive(false);

						//리스트 수
						SetCharCount();
						AutoScrolling(m_SelectChar.transform);
						m_SelectChar = null;
						break;
					}
				}
				break;
		}
		_card.m_State = Item_CharManageCard.State.None;
	}
   
	/// <summary> 차있는 슬롯 클릭시 덱에서 제거 </summary>
	public void ClickSlot(int _pos) {
		if (m_State == State.GoStage) return;
		if (m_SUI.DeckCards[_pos].m_State == Item_CharDeckCard.State.Set) {//Click
			//리스트에서 켜주고 덱에서 제거
			m_SelectChar = null;
			m_SUI.DeckCards[_pos].SetData();
			m_SUI.DeckCards[_pos].OutSlot();
			//*탐험 덱에 있던거 아래서 켜주기
			m_AllChar.Find(t => t.m_Info.m_UID == m_Chars[_pos]).gameObject.SetActive(true);
			m_Chars.Remove(_pos);
			//리스트 수
			SetCharCount();
		}
	} 
	/// <summary> 카드가 버킷에서 벗어난 경우 즉각 자동으로 이동 </summary>
	void AutoScrolling(Transform _trans) {
		float posy = 0f;

		float buckettop = m_SUI.ScrollTrans.position.y + m_SUI.ScrollTrans.rect.height / 2;
		float bucketbottom = m_SUI.ScrollTrans.position.y - m_SUI.ScrollTrans.rect.height / 2;

		float cardtop = _trans.position.y + _trans.GetComponent<RectTransform>().rect.height / 2 + 68;
		float cardbottom = _trans.position.y - _trans.GetComponent<RectTransform>().rect.height / 2 - 250;

		if (buckettop < cardtop) {//카드 위가 잘릴 경우
			posy = cardtop - buckettop;
			m_SUI.ScrollContent.localPosition -= new Vector3(0f, posy, 0f);
		}
		if (bucketbottom > cardbottom) {// 카드 아래가 잘릴 경우
			posy = bucketbottom - cardbottom;
			m_SUI.ScrollContent.localPosition += new Vector3(0f, posy, 0f);
		}
	}

	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return; 
		if (m_State == State.GoStage) return;
		m_State = State.GoStage;
		MAIN.Save_UserInfo();
		base.Close(Result);
	}

	///<summary> 캐릭터 카드 갱신 </summary>
	public void CharCardRefresh() {
		for (int i = 0; i < m_AllChar.Count; i++) {
			Item_CharManageCard card = m_AllChar[i];
			if (card != null) {
				card.SetLvGrade();
				card.SetRankUpAlarm();
				card.SetSortVal();
			}
		}
		SetSort();
	}
	public void DeckCharCardRefresh() {
		for(int i = 0; i < m_SUI.DeckCards.Length; i++) {
			if(m_SUI.DeckCards[i].m_Info != null) {
				m_SUI.DeckCards[i].Refresh(m_SUI.DeckCards[i].m_Info);
			}
		}
		DLGTINFO.f_RFCharInfoCard?.Invoke();
	}
}