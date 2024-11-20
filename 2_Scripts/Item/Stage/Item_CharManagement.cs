using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_CharManagement : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public GameObject CharCardPrefab;
		public Transform[] CharObjBucket;
		public TextMeshProUGUI[] CharCnt;
		public RectTransform m_ScrollContent;
		public RectTransform m_Scroll;
		public Item_SortingGroup SortingGroup;
		public Animator Anim;
		public TextMeshProUGUI LvSum;
		public TextMeshProUGUI LvRemain;
		
		public RectTransform TempLoadPanel;
	}
	[SerializeField]
	SUI m_SUI;
	List<Item_CharManageCard> m_AllChar = new List<Item_CharManageCard>();
	List<Item_CharManageCard> m_Gethar = new List<Item_CharManageCard>();
	List<Item_CharManageCard> m_NonGethar = new List<Item_CharManageCard>();

	private void Start() {
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
		StartCoroutine(IE_StartAnimCheck());
	}
	IEnumerator IE_StartAnimCheck() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Select_CharInfo_Menu)) TUTO.Next(this);
		else if (TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.Select_CharInfo_Menu)) TUTO.Next(this);
		//else if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.Select_CharInfo_Menu)) TUTO.Next(m_SUI.SrvMng);
		else if (TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.Select_CharInfo_Menu)) TUTO.Next(this);
	}
	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFCharInfoCard += CharCardRefresh;
		}
	}
	private void OnDestroy()
	{
		if (MainMng.IsValid() && DLGTINFO != null)
		{
			DLGTINFO.f_RFCharInfoCard -= CharCardRefresh;
		}
	}
	/// <summary> Play_Main 씬 킬때 한번 초기화 </summary>
	public void SetData()
	{
		m_SUI.SortingGroup.SetData(SetSort);

		var allchartdatas = TDATA.GetAllCharacterTable();
		var dicmychars = USERINFO.m_Chars.ToDictionary(o => o.m_Idx, o => o);
		for (int i = 0; i < m_AllChar.Count; i++) m_AllChar[i].transform.SetParent(m_SUI.TempLoadPanel);
		UTILE.Load_Prefab_List(allchartdatas.Count, (RectTransform)m_SUI.TempLoadPanel, (RectTransform)m_SUI.CharCardPrefab.transform);

		m_AllChar.Clear();
		m_Gethar.Clear();
		m_NonGethar.Clear();
		//모든 캐릭터와 유저가 보유한 캐릭터 세팅
		for (int i = 0; i < allchartdatas.Count; i++){
			var element = allchartdatas.ElementAt(i);
			int idx = element.Key;
			// 발생하지 않겠지만 혹시 모를 예외처리
			if (m_SUI.TempLoadPanel.childCount < 1) Utile_Class.Instantiate(m_SUI.CharCardPrefab, m_SUI.TempLoadPanel);
			Item_CharManageCard card = m_SUI.TempLoadPanel.GetChild(0).GetComponent<Item_CharManageCard>();
			if (dicmychars.ContainsKey(idx))
			{
				CharInfo info = dicmychars[idx];
				//획득 캐릭터
				card.transform.SetParent(m_SUI.CharObjBucket[0]);
				card.SetData(idx, m_Gethar, Item_CharManageCard.Mode.Management, Click_CB);
				m_Gethar.Add(card);

				List<int> deckidx = new List<int>();
				for (int j = 0; j < 5; j++)
				{
					for (int k = 0; k < USERINFO.m_Deck[j].m_Char.Length; k++)
					{
						if (USERINFO.m_Deck[j].m_Char[k] == info.m_UID)
						{
							deckidx.Add(j);
						}
					}
				}
				card.SetCheckInDeck(deckidx);
			}
			else
			{
				card.transform.SetParent(m_SUI.CharObjBucket[1]);
				card.SetData(idx, m_NonGethar, Item_CharManageCard.Mode.Management, Click_CB);
				card.SetCheckInDeck(null);
				m_NonGethar.Add(card);
			}
			//CharInfo info = USERINFO.GetChar(idx);

			//if (info != null) {//획득 캐릭터
			//	card = Utile_Class.Instantiate(m_SUI.CharCardPrefab, m_SUI.CharObjBucket[0]).GetComponent<Item_CharManageCard>();
			//	card.SetData(idx, m_Gethar, Item_CharManageCard.Mode.Management, Click_CB);
			//	m_Gethar.Add(card);

			//	List<int> deckidx = new List<int>();
			//	for (int j = 0; j < 5; j++) {
			//		for (int k = 0; k < USERINFO.m_Deck[j].m_Char.Length; k++) {
			//			if (USERINFO.m_Deck[j].m_Char[k] == info.m_UID) {
			//				deckidx.Add(j);
			//			}
			//		}
			//	}
			//	card.SetCheckInDeck(deckidx);
			//}
			//else {//미획득 캐릭터
			//	card = Utile_Class.Instantiate(m_SUI.CharCardPrefab, m_SUI.CharObjBucket[1]).GetComponent<Item_CharManageCard>();
			//	card.SetData(idx, m_NonGethar, Item_CharManageCard.Mode.Management, Click_CB);
			//	card.SetCheckInDeck(null);
			//	m_NonGethar.Add(card);
			//}
			m_AllChar.Add(card);
		}
		ScrollRect scroll = m_SUI.m_Scroll.GetComponent<ScrollRect>();
		scroll.verticalNormalizedPosition = 1f;
		SetSort();
		SetCharCount();
		int lvsum = USERINFO.m_Chars.Sum(o => o.m_LV);
		m_SUI.LvSum.text = lvsum.ToString();
		m_SUI.LvRemain.text = string.Format(TDATA.GetString(10079), USERINFO.GetCharLvStatBonusNextLv(lvsum));
		scroll.enabled = !TUTO.IsTutoPlay();
	}
	/// <summary> 캐릭터 카드를 선택했을 때 </summary>
	void Click_CB(Item_CharManageCard _card)
	{
		//선택카드 자동스크롤
		_card.OpenDetail();
		_card.m_State = Item_CharManageCard.State.None;
		AutoScrolling(_card.transform);
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Select_Char_1021)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.Select_Char_1021)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.Select_Char_1024)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.Select_Char_1024)) TUTO.Next();
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////////
	/// UI
	///<summary> 캐릭터 카드 갱신 </summary>
	public void CharCardRefresh() {
		for(int i = 0;i< m_Gethar.Count; i++) {
			Item_CharManageCard card = m_Gethar[i];
			if (card != null) {
				card.SetLvGrade();
				card.SetRankUpAlarm();
				card.SetNewAlarm(card.m_Info != null ? card.m_Info.m_GetAlarm : false);
			}
		}
		
		SetSort();
		int lvsum = USERINFO.m_Chars.Sum(o => o.m_LV);
		m_SUI.LvSum.text = lvsum.ToString();
		m_SUI.LvRemain.text = string.Format(TDATA.GetString(10079), USERINFO.GetCharLvStatBonusNextLv(lvsum));

		m_SUI.m_Scroll.GetComponent<ScrollRect>().enabled = !TUTO.IsTutoPlay();
		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
	}
	/// <summary> 대기 캐릭터 표기 덱에없는 획득 캐릭 / 전체 캐릭 </summary>
	void SetCharCount() {
		m_SUI.CharCnt[0].text = string.Format("{0}/{1}", USERINFO.m_Chars.Count.ToString(), TDATA.GetAllCharacterTable().Count.ToString());
		m_SUI.CharCnt[1].text = string.Format("{0}/{1}", (TDATA.GetAllCharacterTable().Count - USERINFO.m_Chars.Count).ToString(), TDATA.GetAllCharacterTable().Count.ToString());
	}
	public void SetSort() {
		//for (int i = 0; i < m_Gethar.Count; i++) m_Gethar[i].m_Info.GetCombatPower();
		switch (m_SUI.SortingGroup.m_Condition)
		{
		case SortingType.Grade:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				if (befor.m_Info.m_Grade != after.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				if (befor.m_Info.m_GetAlarm != after.m_Info.m_GetAlarm) return after.m_Info.m_GetAlarm.CompareTo(befor.m_Info.m_GetAlarm);
				int beforcp = befor.m_Info.m_CP;
				int aftercp = after.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				if (befor.m_Info.m_LV != after.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		case SortingType.CombatPower:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				int aftercp = after.m_Info.m_CP;
				int beforcp = befor.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				if (after.m_Info.m_GetAlarm != befor.m_Info.m_GetAlarm) return after.m_Info.m_GetAlarm.CompareTo(befor.m_Info.m_GetAlarm);
				if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		case SortingType.Level:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				if (after.m_Info.m_GetAlarm != befor.m_Info.m_GetAlarm) return after.m_Info.m_GetAlarm.CompareTo(befor.m_Info.m_GetAlarm);
				if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				int aftercp = after.m_Info.m_CP;
				int beforcp = befor.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		case SortingType.Men:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				float aftermen = after.m_Info.m_Stat[StatType.Men];
				float beformen = befor.m_Info.m_Stat[StatType.Men];
				if (beformen != aftermen) return aftermen.CompareTo(beformen);
				int aftercp = after.m_Info.m_CP;
				int beforcp = befor.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		case SortingType.Hyg:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				float afterhyg = after.m_Info.m_Stat[StatType.Hyg];
				float beforhyg = befor.m_Info.m_Stat[StatType.Hyg];
				if (beforhyg != afterhyg) return afterhyg.CompareTo(beforhyg);
				int aftercp = after.m_Info.m_CP;
				int beforcp = befor.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		case SortingType.Sat:
			m_Gethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) => {
				float aftersat = after.m_Info.m_Stat[StatType.Sat];
				float beforsat = befor.m_Info.m_Stat[StatType.Sat];
				if (beforsat != aftersat) return aftersat.CompareTo(beforsat);
				int aftercp = after.m_Info.m_CP;
				int beforcp = befor.m_Info.m_CP;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				if (after.m_Info.m_Grade != befor.m_Info.m_Grade) return after.m_Info.m_Grade.CompareTo(befor.m_Info.m_Grade);
				if (after.m_Info.m_LV != befor.m_Info.m_LV) return after.m_Info.m_LV.CompareTo(befor.m_Info.m_LV);
				return befor.m_Idx.CompareTo(after.m_Idx);
			});
			break;
		}

		if (m_SUI.SortingGroup.m_Ascending) m_Gethar.Reverse();

		Item_CharManageCard tutocard = null;
		// 다음 단계인 Select_Char_1024를 위해 앞으로 이동
		if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Select_CharInfo_Menu)
			|| TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.Select_CharInfo_Menu)
			
			|| TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.Select_CharInfo_Menu))//|| TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.Select_CharInfo_Menu)
		{
			// 1024 캐릭터만 앞으로 이동
			var temp = new List<Item_CharManageCard>();
			tutocard = GetCharCard(1024);
			temp.Add(tutocard);
			m_Gethar.Remove(tutocard);
			temp.AddRange(m_Gethar);
			m_Gethar = temp;

		}

		for (int i = 0; i < m_Gethar.Count; i++) {
			m_Gethar[i].transform.SetAsLastSibling();
			//튜토 등으로 갱신됬을 수 있으니 카드들 새로 데이터셋
			m_Gethar[i].SetData(m_Gethar[i].m_Idx, m_Gethar, Item_CharManageCard.Mode.Management, Click_CB);
		}

		m_NonGethar.Sort((Item_CharManageCard befor, Item_CharManageCard after) =>
		{
			int bgrade = befor.m_TData.m_Grade;
			int agrade = after.m_TData.m_Grade;
			if (bgrade != agrade) return agrade.CompareTo(bgrade);
			return befor.m_Idx.CompareTo(after.m_Idx);
		});

		for (int i = 0; i < m_NonGethar.Count; i++) {
			m_NonGethar[i].transform.SetAsLastSibling();
		}
	}
	/// <summary> 카드가 버킷에서 벗어난 경우 즉각 자동으로 이동 </summary>
	public void AutoScrolling(Transform _trans) {
		float posy = 0f;

		float buckettop = m_SUI.m_Scroll.position.y + m_SUI.m_Scroll.rect.height / 2;
		float bucketbottom = m_SUI.m_Scroll.position.y - m_SUI.m_Scroll.rect.height / 2;

		float cardtop = _trans.position.y + _trans.GetComponent<RectTransform>().rect.height / 2 + 68;
		float cardbottom = _trans.position.y - _trans.GetComponent<RectTransform>().rect.height / 2 - 250;

		if (buckettop < cardtop) {//카드 위가 잘릴 경우
			posy = cardtop - buckettop;
			m_SUI.m_ScrollContent.localPosition -= new Vector3(0f, posy, 0f);
		}
		if(bucketbottom > cardbottom) {// 카드 아래가 잘릴 경우
			posy = bucketbottom - cardbottom;
			m_SUI.m_ScrollContent.localPosition += new Vector3(0f, posy, 0f);
		}
	}
	public void ClickBonusStatInfo() {
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.LvBonusList, null);
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 튜토리얼용
	public Item_CharManageCard GetCharCard(int idx)
	{
		return m_AllChar.Find(o => o.m_Info?.m_Idx == idx);
	}
	public void CharScrolling(Transform _trans) {
		StartCoroutine(ScrollingAction(_trans));
	}
	IEnumerator ScrollingAction(Transform _trans) {
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		float to = Mathf.Abs(_trans.localPosition.y - _trans.parent.localPosition.y) - _trans.GetComponent<RectTransform>().rect.height / 2f;
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.m_ScrollContent.localPosition.y, "to", to, "onupdate", "TW_Scrolling", "time", 0.3f));
		yield return new WaitForSeconds(0.3f);
		AutoScrolling(_trans);
	}
	void TW_Scrolling(float _amount) {
		m_SUI.m_ScrollContent.localPosition = new Vector3(m_SUI.m_ScrollContent.localPosition.x, _amount, m_SUI.m_ScrollContent.localPosition.z);
	}
}