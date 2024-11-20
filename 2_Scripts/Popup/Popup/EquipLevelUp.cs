using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class EquipLevelUp : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	public struct SStatUI
	{
		public GameObject[] Active;
		public Item_InfoStat Now;
		public TextMeshProUGUI Next;
		public TextMeshProUGUI Up;
	}
	[System.Serializable]
	public struct SGuageUI
	{
		public TextMeshProUGUI[] LV;
		[ReName("Back", "Front")]
		public Slider[] Guage;
		[ReName("Back", "Front")]
		public Image[] GuageFillImg;
		[ReName("Back", "Front")]
		public GameObject[] GuageFillEff;
		[ReName("+0 Back", "+0 Front", "+1~Back", "+1~Front")]
		public Sprite[] GuageImg;
		public TextMeshProUGUI[] Value;

		public GameObject EffPanel;
		public GameObject Eff;
	}

	[System.Serializable]
	public struct SSelectListUI
	{
		public TextMeshProUGUI Label;
		public ScrollReck_ViewItemController ScrollController;
		public ScrollRect Scroll;
		public GameObject Prefab;
	}

	[System.Serializable]
	public struct SListUI
	{
		public ScrollReck_ViewItemController ScrollController;
		public GameObject Prefab;
	}

	[System.Serializable]
	public struct SUI
	{
		public Item_Inventory_Item Item;

		public TextMeshProUGUI[] CP;
		public Color[] CPColor;
		public SStatUI[] Stat;
		public Color[] StatColor;
		public SGuageUI Guage;

		public TextMeshProUGUI SelectLabel;
		public SSelectListUI SelectList;
		public SListUI List;

		public TextMeshProUGUI[] Price;
		public TextMeshProUGUI Filter;
		public DicSortingUseType Filter_Type;

		public Image LVUpBtn;
		public GameObject LVUpBtnEff;

		public RectTransform LVUpEffLoadPanel;
		[ReName("가상", "1레벨", "2~5레벨", "6~9레벨")]
		public GameObject[] LVUpEffs;

		//튜토용
		public GameObject[] TutoFocus;

		[ReName("-1", "+1")]
		public TextMeshProUGUI[] BtnLabel;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] Animator m_Ani;
	SortingType m_FilterGrade;
	protected ItemInfo m_Info;
	protected PopupName m_Parent;

	// 다음 레벨정보 데이터
	int m_InitLV, m_InitExp;
	long m_Price = 0;
	int m_AddExp = 0;
	int m_AddActionExp = 0;

	IEnumerator m_Action;

	/// <summary> 재료로 사용가능한 아이템 전체 목록 </summary>
	List<ItemInfo> m_AllMatItems = new List<ItemInfo>();
	/// <summary> 판매 등록 아이템 리스트 </summary>
	List<Item_RewardItem_Card> m_Items = new List<Item_RewardItem_Card>();
	/// <summary> 선택 가능한 재료 목록 </summary>
	List<ItemInfo> m_MatItems = new List<ItemInfo>();
	/// <summary> 선택한 아이템 정뵤 (key : UID, value : 개수) </summary>
	Dictionary<ItemInfo, int> m_SelectItems = new Dictionary<ItemInfo, int>();
	Dictionary<long, Item_RewardItem_Card> m_SelectItemObjs = new Dictionary<long, Item_RewardItem_Card>();

#pragma warning restore 0649
#region cloase
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		if (m_Ani == null)
		{
			base.Close(Result);
			return;
		}
		m_Action = StartEndAni(Result);
		StartCoroutine(m_Action);
	}

	IEnumerator StartEndAni(int Result)
	{
		m_Ani.SetTrigger("Close");
		yield return Utile_Class.CheckAniPlay(m_Ani);

		base.Close(Result);
	} 
#endregion

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		PlayEffSound(SND_IDX.SFX_1069);
		m_Info = (ItemInfo)aobjValue[0];
		m_Price = 0;

		m_SUI.List.ScrollController.SetData(0, m_SUI.List.Prefab.transform as RectTransform, SetMatItems);
		m_SUI.SelectList.ScrollController.SetData(0, m_SUI.SelectList.Prefab.transform as RectTransform, SetSelectMatItems);
		SetAllMatList();

		var listcnt = m_SUI.List.ScrollController.GetViewCnt();
		for (int i = 0;i< listcnt; i++) {
			m_Items.Add(m_SUI.List.ScrollController.GetItem<Item_RewardItem_Card>(i));
			var tevent = m_Items[i].gameObject.AddComponent<PointerEventCheck>();
			tevent.SetCB(repeatcb: (obj) => {
				var card = obj.GetComponent<Item_RewardItem_Card>();
				POPUP.ViewItemInfo((result, obj) => {
					SetMatSort();
				}, new object[] { card.m_Item, m_Popup, null }).OnlyInfo();
			});
		}

		m_SUI.BtnLabel[0].text = string.Format(TDATA.GetString(1108), "-1");
		m_SUI.BtnLabel[1].text = string.Format(TDATA.GetString(1108), "+1");

		m_FilterGrade = Enum.Parse<SortingType>(PlayerPrefs.GetString("EquipLevelUpFilterGrade", SortingType.FILTER_Grade_10.ToString()));
		if (m_FilterGrade < SortingType.FILTER_Grade_1 || m_FilterGrade > SortingType.FILTER_Grade_10) m_FilterGrade = SortingType.FILTER_Grade_10;

		InitLV();

		base.SetData(pos, popup, cb, aobjValue);
	}

	public void SetAllMatList()
	{
		m_SelectItems.Clear();
		m_SelectItemObjs.Clear();
		m_AllMatItems = USERINFO.GetBagItems(3, new List<long> { m_Info.m_Uid });
		m_SUI.SelectList.ScrollController.SetItemCount(0);
		m_SUI.List.ScrollController.InitPosition();
	}

	void InitLV() {
		m_InitLV = m_Info.m_Lv;
		m_InitExp = m_Info.m_Exp;
		m_AddExp = m_AddActionExp = 0;


		m_SUI.Guage.Guage[0].gameObject.SetActive(true);
		m_SUI.Guage.Guage[0].value = (float)m_InitExp / (float)m_Info.m_TExpData.m_Exp[0];
	}

	public override void SetUI() {
		iTween.StopByName(gameObject, "GageAction");
		m_AddActionExp = m_AddExp;
		base.SetUI();

		InitSelectMatItem();

		// 장비 정보
		SetItemInfo();

		SetFilterUI();
	}

	void SetItemInfo() {
		TItemTable tdata = m_Info.m_TData;
		m_SUI.Item.SetData(m_Info, null, null, LockMarkMode: Item_RewardItem_Card.LockActiveMode.Normal, m_Info);
		m_SUI.Item.StateChange(Inventory.EState.Normal);

		ItemInvenGroupType group = tdata.GetInvenGroupType();

		// 현재 정보
		m_SUI.CP[0].text = Utile_Class.CommaValue(m_Info.m_CP);
		List<ItemStat> stats = tdata.m_Stat;
		for (int i = 0; i < 2; i++) {
			bool Active = i < stats.Count;
			m_SUI.Stat[i].Active[0].SetActive(Active);
			if (Active) m_SUI.Stat[i].Now.SetData(stats[i].m_Stat, Mathf.RoundToInt(stats[i].GetValue(m_Info.m_Lv)), true);
		}


		int lv;
		int exp;
		var tnextexp = CalcLV(out lv, out exp, m_AddExp);
		SetNextInfo(lv);

		SetGuageUI();
	}

	void SetNextInfo(int NextLV) {
		NextLV = Mathf.Min(NextLV, m_Info.m_MaxLV);

		var bcp = m_Info.m_CP;
		var acp = m_Info.GetCombatPower(NextLV);
		m_SUI.CP[1].text = Utile_Class.CommaValue(acp);
		m_SUI.CP[1].color = m_SUI.CPColor[bcp != acp ? 1 : 0];
		List<ItemStat> stats = m_Info.m_TData.m_Stat;
		for (int i = 0; i < stats.Count; i++)
		{
			var befor = Mathf.RoundToInt(stats[i].GetValue(m_InitLV));
			var after = Mathf.RoundToInt(stats[i].GetValue(NextLV));
			m_SUI.Stat[i].Next.text = Utile_Class.CommaValue(after);
			if (befor != after)
			{
				m_SUI.Stat[i].Next.color = m_SUI.StatColor[1];
				m_SUI.Stat[i].Active[1].SetActive(true);
				m_SUI.Stat[i].Up.text = Utile_Class.CommaValue(after - befor);
			}
			else
			{
				m_SUI.Stat[i].Next.color = m_SUI.StatColor[0];
				m_SUI.Stat[i].Active[1].SetActive(false);
			}

		}

		// 가격

		m_SUI.Price[0].text = Utile_Class.CommaValue(USERINFO.m_Money);
		m_SUI.Price[1].text = string.Format("/ {0}", Utile_Class.CommaValue(m_Price));
		m_SUI.Price[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, m_Price);

		SetLVUpBtnActive(m_SelectItems.Count > 0);
	}

	void SetLVUpBtnActive(bool Active)
	{
		m_SUI.LVUpBtn.sprite = POPUP.GetBtnBG(Active ? UIMng.BtnBG.Green : UIMng.BtnBG.Not);
		m_SUI.LVUpBtn.GetComponent<Button>().interactable = Active;
		m_SUI.LVUpBtnEff.SetActive(m_SUI.LVUpBtn.GetComponent<Button>().interactable);
	}

#region Select Mat Items

	void InitSelectMatItem()
	{
		m_SUI.SelectList.ScrollController.SetItemCount(m_SelectItems.Count);
		////UTILE.Load_Prefab_List(m_SelectItems.Count, m_SUI.SelectList.Scroll.content, m_SUI.SelectList.Prefab.transform);
		SetViewITemList();

		//SetSelectMatItems();
	}

	void SetSelectMatItems(ScrollReck_ViewItemController.RefreshMode mode)
	{
		var keys = m_SelectItems.Keys.ToList();
		var listcnt = m_SUI.SelectList.ScrollController.GetViewCnt();
		int offset = m_SUI.SelectList.ScrollController.GetViewLine() * m_SUI.SelectList.ScrollController.GetOneLineItemCnt();
		m_SelectItemObjs.Clear();
		for (int i = 0 ; i < listcnt; i++, offset++)
		{
			Item_RewardItem_Card item = m_SUI.SelectList.ScrollController.GetItem<Item_RewardItem_Card>(i);
			if (item == null) break;
			if (offset > -1 && offset < keys.Count)
			{
				var info = keys[offset];
				item.SetData(info, ClickSelectMatItem, LockMode: info.m_Lock ? Item_RewardItem_Card.LockActiveMode.Block : Item_RewardItem_Card.LockActiveMode.None);
				item.SetCnt(info.m_Lv, m_SelectItems[info]);
				item.gameObject.SetActive(true);
				m_SelectItemObjs.Add(info.m_Uid, item);
			}
			else item.gameObject.SetActive(false);
		}
		m_SUI.SelectLabel.text = string.Format(TDATA.GetString(1105), m_SelectItems.Sum(o => o.Value));

		if (mode == ScrollReck_ViewItemController.RefreshMode.Add)
		{
			StopCoroutine("MoveScrollEndPosition");
			StartCoroutine("MoveScrollEndPosition");
		}
	}

	void AddSelectItem(ItemInfo item, int add = 1)
	{
		if (m_Action != null) return;
		if (item.m_Lock) return;
		if (add < 0)
		{
			if (!m_SelectItems.ContainsKey(item)) return;
			m_SelectItems[item] += add;
			if (m_SelectItems[item] < 1)
			{
				m_SelectItems.Remove(item);
				//GameObject.Destroy(m_SelectItemObjs[item.m_Uid].gameObject);
				//m_SelectItemObjs.Remove(item.m_Uid);
				m_SUI.SelectList.ScrollController.SetItemCount(m_SelectItems.Count);
				if (m_MatItems.Find(o => o == item) == null) m_MatItems.Add(item);
				SetMatSort();
			}
			else 
			{
				if (m_SelectItemObjs.ContainsKey(item.m_Uid)) m_SelectItemObjs[item.m_Uid].SetCnt(item.m_Lv, m_SelectItems[item]);
				if (item.m_TData.GetEquipType() == EquipType.End && m_MatItems.Find(o => o == item) == null)
				{
					m_MatItems.Add(item);
					SetMatSort();
					m_SUI.List.ScrollController.SetItemCount(m_MatItems.Count);
				}
				else SetMatItems(ScrollReck_ViewItemController.RefreshMode.Normal);
			}

			m_SUI.SelectLabel.text = string.Format(TDATA.GetString(1105), m_SelectItems.Sum(o => o.Value));

			SetExpData();
			return;
		}

		// 증가된 레벨 정보
		int lv = 0, exp = 0;
		m_Info.CalcLV(out lv, out exp, m_AddExp);
		if (lv >= m_Info.m_MaxLV) return;

		if (!m_SelectItems.ContainsKey(item))
		{
			m_SelectItems.Add(item, 0);
			m_SUI.SelectList.ScrollController.SetItemCount(m_SelectItems.Count);
			//m_SelectItemObjs.Add(item.m_Uid, Utile_Class.Instantiate(m_SUI.SelectList.Prefab, m_SUI.SelectList.Scroll.content).GetComponent<Item_RewardItem_Card>());
			//m_SelectItemObjs[item.m_Uid].transform.localScale = Vector3.one * 0.58f;
			//m_SelectItemObjs[item.m_Uid].SetData(item, ClickSelectMatItem);
			//StopCoroutine("MoveScrollEndPosition");
			//StartCoroutine("MoveScrollEndPosition");
		}

		m_SelectItems[item] += add;
		if(m_SelectItemObjs.ContainsKey(item.m_Uid)) m_SelectItemObjs[item.m_Uid].SetCnt(item.m_Lv, m_SelectItems[item]);

		if (item.m_Stack <= m_SelectItems[item])
		{
			m_MatItems.Remove(item);
			SetMatSort();
			m_SUI.List.ScrollController.SetItemCount(m_MatItems.Count);
		}
		else SetMatItems(ScrollReck_ViewItemController.RefreshMode.Normal);

		m_SUI.SelectLabel.text = string.Format(TDATA.GetString(1105), m_SelectItems.Sum(o => o.Value));
		SetExpData();
	}

	IEnumerator MoveScrollEndPosition()
	{
		iTween.StopByName(gameObject, "ScrollMoveEndPosition");
		//m_SUI.SelectList.Scroll.content.ForceUpdateRectTransforms();
		yield return new WaitForEndOfFrame();
		if(m_SUI.SelectList.Scroll.viewport.rect.width < m_SUI.SelectList.Scroll.content.rect.width)
		{
			var x = m_SUI.SelectList.Scroll.viewport.rect.width - m_SUI.SelectList.Scroll.content.rect.width;
			iTween.MoveTo(m_SUI.SelectList.Scroll.content.gameObject, iTween.Hash("position", new Vector3(x, m_SUI.SelectList.Scroll.content.localPosition.y, 0f), "time", 0.5f, "islocal", true, "easetype", "easeOutQuart", "name", "ScrollMoveEndPosition"));
		}
	}

	void ClickSelectMatItem(ItemInfo item)
	{
		if (TUTO.IsTutoPlay()) return;
		AddSelectItem(item, -1);
	}
#endregion

#region NoSelect Mat Items
	void SetViewITemList()
	{
		m_MatItems.Clear();
		m_MatItems.AddRange(m_AllMatItems);
		int MaxGrade = m_FilterGrade - SortingType.FILTER_Grade_1 + 1;
		m_MatItems.RemoveAll(o =>
		{
			// 리스트에서 안보이는것이 아닌 자동 선택에서 제외됨
			//if (o.m_TData.GetEquipType() != EquipType.End && o.m_Grade > MaxGrade) return true;
			if(m_SelectItems.ContainsKey(o))
			{
				if (o.m_TData.GetEquipType() == EquipType.End) return o.m_Stack == m_SelectItems[o];
				else return true;
			}
			return false;
		});
		SetMatSort();

		m_SUI.List.ScrollController.SetItemCount(m_MatItems.Count);
	}

	void SetMatSort()
	{
		m_MatItems.Sort((befor, after) => {
			var btdata = befor.m_TData;
			var atdata = after.m_TData;
			var btype = btdata.GetInvenGroupType();
			var atype = atdata.GetInvenGroupType();
			if (btype != atype) return atype.CompareTo(btype); // 재료가 앞으로
			if (befor.m_Lock != after.m_Lock) return befor.m_Lock.CompareTo(after.m_Lock);			// 잠금상태는 가장 아래로

			if (btdata.m_Grade != atdata.m_Grade) return btdata.m_Grade.CompareTo(atdata.m_Grade);	// 등급 낮순서 앞으로

			if (befor.m_Lv != after.m_Lv) return befor.m_Lv.CompareTo(after.m_Lv);					// 레벨 낮은 순서
			if (befor.m_Idx != after.m_Idx) return befor.m_Idx.CompareTo(after.m_Idx);				// 인덱스 낮은 순서

			return befor.m_Uid.CompareTo(after.m_Uid);  // 획득 순서 (고유번호가 높을수록 나중에 획득)
		});
		// 필터 데이터 제거
		SetMatItems(ScrollReck_ViewItemController.RefreshMode.Normal);
	}

	void SetMatItems(ScrollReck_ViewItemController.RefreshMode mode)
	{
		var listcnt = m_SUI.List.ScrollController.GetViewCnt();
		int offset = m_SUI.List.ScrollController.GetViewLine() * m_SUI.List.ScrollController.GetOneLineItemCnt();
		for (int i = 0; i < listcnt; i++, offset++)
		{
			Item_RewardItem_Card item = m_SUI.List.ScrollController.GetItem<Item_RewardItem_Card>(i);
			if (item == null) break;
			if (offset > -1 && offset < m_MatItems.Count)
			{
				var info = m_MatItems[offset];
				item.SetData(info, ClickMatItem, LockMode: info.m_Lock ? Item_RewardItem_Card.LockActiveMode.Block : Item_RewardItem_Card.LockActiveMode.None);
				item.SetCnt(info.m_Lv, info.m_Stack - (m_SelectItems.ContainsKey(info) ? m_SelectItems[info] : 0));
				item.gameObject.SetActive(true);
			}
			else item.gameObject.SetActive(false);
		}
	}
	bool IsScrollMove()
	{
		return POPUP.GetPopup()?.m_Popup == PopupName.EquipLevelUp;
	}

	void ClickMatItem(ItemInfo item)
	{
		if (TUTO.IsTutoPlay()) return;
		AddSelectItem(item);
		PlayEffSound(SND_IDX.SFX_1055);
	}

#endregion

#region Exp
	void SetExpData()
	{
		var items = m_AllMatItems.FindAll(o => m_SelectItems.ContainsKey(o));
		int calcexp = items.Sum(o => o.GetExp() * m_SelectItems[o]);
		if (m_AddExp < calcexp)
		{
			// 추가 경험치 연출
			var eff = Utile_Class.Instantiate(m_SUI.Guage.Eff, m_SUI.Guage.EffPanel.transform).GetComponent<EF_EqExp>();
			eff.SetData(calcexp - m_AddExp);
		}
		m_AddExp = calcexp;
		float per = 1f;
		per -= USERINFO.GetSkillValue(SkillKind.EquipLevelUpSale);
		switch (m_Info.m_TData.GetEquipType()) {
			case EquipType.Weapon: per -= USERINFO.ResearchValue(ResearchEff.WeaponSale); break;
			case EquipType.Helmet: per -= USERINFO.ResearchValue(ResearchEff.HelmetSale); break;
			case EquipType.Costume: per -= USERINFO.ResearchValue(ResearchEff.CostumeSale); break;
			case EquipType.Shoes: per -= USERINFO.ResearchValue(ResearchEff.ShoesSale); break;
			case EquipType.Accessory: per -= USERINFO.ResearchValue(ResearchEff.AccSale); break;
		}
		m_Price = (long)Mathf.RoundToInt(items.Sum(o => o.GetExpPrice(m_SelectItems[o])) * per);

		int lv;
		int exp;
		var tnextexp = CalcLV(out lv, out exp, m_AddExp);
		SetNextInfo(lv);
		StartExpAction(true, lv);
	}

	// 가상 게이지 연출
	void StartExpAction(bool virtureplay, int LastLV)
	{
		IsVirtureLVUP = virtureplay;
		ActionLastLV = LastLV;
		SetGuageUI();
		PlayEffSound(SND_IDX.SFX_1060);
		//if (Utile_Class.IsPlayiTween(gameObject, "GageAction")) m_AddActionExp = m_AddExp;//게이지 진행중에 m_AddActionExp 오르는게 안끝나있어서 1초 지나기전까지 press가 계속 됨
		iTween.StopByName(gameObject, "GageAction");
		//iTween.ValueTo(gameObject, iTween.Hash("from", (double)m_AddActionExp, "to", (double)m_AddExp, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "SetActionExp", "name", "GageAction"));
		iTween.ValueTo(gameObject, iTween.Hash("from", (double)m_AddActionExp, "to", (double)m_AddExp, "time", 1f, "easetype", "easeOutQuart", "onupdate", "SetActionExp", "name", "GageAction"));
	}

	bool IsVirtureLVUP;
	int ActionLastLV;

	void SetActionExp(float value)
	{
		int blv, bexp;
		CalcLV(out blv, out bexp, m_AddActionExp);
		m_AddActionExp = Mathf.RoundToInt(value);
		SetGuageUI();

		int lv, exp;
		CalcLV(out lv, out exp, m_AddActionExp);
		if (blv < lv && lv == ActionLastLV)
		{
			var eff = m_SUI.LVUpEffs[0];
			if(!IsVirtureLVUP)
			{
				var gap = m_InitLV - ActionLastLV;
				if (gap < 2) eff = m_SUI.LVUpEffs[1];
				else if (gap < 6) eff = m_SUI.LVUpEffs[2];
				else eff = m_SUI.LVUpEffs[3];
				PlayEffSound(SND_IDX.SFX_1061);
			}
			else PlayEffSound(SND_IDX.SFX_0012);
			Utile_Class.Instantiate(eff, m_SUI.LVUpEffLoadPanel);
		}
	}

	public TEquipExpTable CalcLV(out int lv, out int exp, long AddExp)
	{
		var tdata = m_Info.m_TData;
		lv = m_InitLV;
		exp = m_InitExp;
		long temp = exp + AddExp;
		int MaxLV = m_Info.m_MaxLV;
		EquipType type = tdata.GetEquipType();
		if (type == EquipType.End) return null;
		int grade = m_Info.m_Grade;
		TEquipExpTable tExp = TDATA.GetEquipExpTable(type, grade, lv);
		while (tExp != null)
		{
			if (lv == MaxLV) break;
			if (temp < tExp.m_Exp[0]) break;
			lv++;
			temp -= tExp.m_Exp[0];
			tExp = TDATA.GetEquipExpTable(type, grade, lv);
		}
		exp = (int)temp;
		if (lv == MaxLV)
		{
			tExp = TDATA.GetEquipExpTable(type, grade, MaxLV - 1);
			exp = tExp.m_Exp[0];
		}
		return tExp;
	}

	void SetGuageUI()
	{
		int lv;
		int exp;
		var tnextexp = CalcLV(out lv, out exp, m_AddActionExp);

		// 레벨 정보
		if(m_Action != null)
		{
			m_SUI.Guage.LV[0].text = $"<size=80%>Lv. </size>{lv}";
			m_SUI.Guage.LV[1].text = $"<size=80%><color=#aaaaaa>Lv. </color></size>{lv}";
		}
		else
		{
			int addlv = lv - m_InitLV;
			m_SUI.Guage.LV[0].text = $"<size=80%>Lv. </size>{m_InitLV} + {addlv}";
			m_SUI.Guage.LV[1].text = $"<size=80%><color=#aaaaaa>Lv. </color></size>{m_InitLV} <color=#fff100>+ {addlv}</color>";
		}

		if (lv == m_InitLV)
		{
			m_SUI.Guage.Guage[0].value = (float)exp / (float)tnextexp.m_Exp[0];
			m_SUI.Guage.GuageFillEff[0].SetActive(true);
			m_SUI.Guage.GuageFillImg[0].sprite = m_SUI.Guage.GuageImg[0];
			var texp = m_Info.m_TExpData;
			m_SUI.Guage.Guage[1].value = (float)m_Info.m_Exp / (float)texp.m_Exp[0];
			m_SUI.Guage.GuageFillEff[1].SetActive(false);
			m_SUI.Guage.GuageFillImg[1].sprite = m_SUI.Guage.GuageImg[1];
		}
		else
		{
			m_SUI.Guage.Guage[0].value = 1f;
			m_SUI.Guage.GuageFillEff[0].SetActive(false);
			m_SUI.Guage.GuageFillImg[0].sprite = m_SUI.Guage.GuageImg[2];
			m_SUI.Guage.Guage[1].value = (float)exp / (float)tnextexp.m_Exp[0];
			m_SUI.Guage.GuageFillEff[1].SetActive(true);
			m_SUI.Guage.GuageFillImg[1].sprite = m_SUI.Guage.GuageImg[3];
		}

		m_SUI.Guage.Value[0].text = $"{Utile_Class.CommaValue(exp)} / {Utile_Class.CommaValue(tnextexp.m_Exp[0])}";
		m_SUI.Guage.Value[1].text = $"{Utile_Class.CommaValue(exp)} <color=#aaaaaa>/ {Utile_Class.CommaValue(tnextexp.m_Exp[0])}</color>";
	}
#endregion

#region Filter
	void SetFilterUI()
	{
		var grade = string.Format(TDATA.GetString(1107), m_FilterGrade - SortingType.FILTER_Grade_1 + 1);
		m_SUI.Filter.text = string.Format(TDATA.GetString(1106), grade);
	}

	public void OnFilterChange()
	{
		if (TUTO.IsTutoPlay()) return; 
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Filter_EquipLevelUp, (result, obj) =>
		{
			m_FilterGrade = (SortingType)result;
			PlayerPrefs.SetString("EquipLevelUpFilterGrade", m_FilterGrade.ToString());
			PlayerPrefs.Save();
			m_SUI.List.ScrollController.InitPosition();
			SetViewITemList();
			SetFilterUI();
		}, m_FilterGrade, m_SUI.Filter_Type);
	}
#endregion

	void Add_LV(int AddLV)
	{
		// 증가된 레벨 정보
		int lv = 0, exp = 0;
		m_Info.CalcLV(out lv, out exp, m_AddExp);
		if (lv >= m_Info.m_MaxLV) return;
		int CheckLV = AddLV == 0 ? m_Info.m_MaxLV : Mathf.Clamp(lv + AddLV, m_InitLV, m_Info.m_MaxLV);
		int NeedExp = (int)m_Info.GetNeedExp(CheckLV) - m_AddExp;
		long mymoney = (USERINFO.m_Money - m_Price);
		var matitems = m_MatItems.ToArray();
		for (int i = 0; i < matitems.Length; i++)
		{
			if (NeedExp < 1) break;
			ItemInfo info = matitems[i];
			if (info.m_Lock) continue;
			int MaxGrade = m_FilterGrade - SortingType.FILTER_Grade_1 + 1;
			if (info.m_TData.GetEquipType() != EquipType.End && info.m_Grade > MaxGrade) continue;
			int usecnt = m_SelectItems.ContainsKey(info) ? m_SelectItems[info] : 0;
			// 필요 개수 체크
			exp = info.GetExp();
			int needcnt = Utile_Class.NeedCnt(NeedExp, exp);
			int cnt = Math.Min(needcnt, info.m_Stack - usecnt);
			float per = 1f;
			per -= USERINFO.GetSkillValue(SkillKind.EquipLevelUpSale);
			switch (m_Info.m_TData.GetEquipType()) {
				case EquipType.Weapon: per -= USERINFO.ResearchValue(ResearchEff.WeaponSale); break;
				case EquipType.Helmet: per -= USERINFO.ResearchValue(ResearchEff.HelmetSale); break;
				case EquipType.Costume: per -= USERINFO.ResearchValue(ResearchEff.CostumeSale); break;
				case EquipType.Shoes: per -= USERINFO.ResearchValue(ResearchEff.ShoesSale); break;
				case EquipType.Accessory: per -= USERINFO.ResearchValue(ResearchEff.AccSale); break;
			}
			int price = Mathf.RoundToInt(info.GetExpPrice(cnt) * per);
			// 금액 부족
			if (mymoney < price)
			{
				POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
				return;
			}
			AddSelectItem(info, cnt);
			NeedExp -= exp * cnt;
			mymoney -= price;
			PlayEffSound(SND_IDX.SFX_1055);
		}
	}

	void Minus_LV(int MinusLV)
	{
		if (MinusLV > -1) return;
		// 증가된 레벨 정보
		int lv = 0, exp = 0;
		CalcLV(out lv, out exp, m_AddExp);
		if (m_InitLV >= lv)
		{
			for(int i = 0; i < m_SelectItems.Count; i++)
			{
				var info = m_SelectItems.ElementAt(i);
				AddSelectItem(info.Key, info.Value * -1);
			}
			m_SUI.List.ScrollController.InitPosition();
			return;
		}

		//// 해당 레벨까지 재료 제거
		//// 1레벨씩 떨어뜨리면서 제거해준다.
		var tdata = m_Info.m_TData;
		//var texp = TDATA.GetEquipExpTable(tdata.GetEquipType(), m_Info.m_Grade, lv - 1);
		//var needexp = Math.Min(m_AddExp, text.m_Exp[0]);
		//m_Info.CalcLV(out lv, out exp, m_AddExp - needexp);

		//// 경험치 0까지 내려주기
		//needexp += exp;

		var selectitems = m_SelectItems.Keys.ToList();
		var need = exp + 1;
		//selectitems.Sort((befor, after) => {
		//	var btdata = befor.m_TData;
		//	var atdata = after.m_TData;
		//	var btype = btdata.GetInvenGroupType();
		//	var atype = atdata.GetInvenGroupType();
		//	if (btype != atype) return atype.CompareTo(btype); // 재료가 앞으로

		//	if (btdata.m_Grade != atdata.m_Grade) return btdata.m_Grade.CompareTo(atdata.m_Grade); // 등급 낮순서 앞으로

		//	if (befor.m_Lv != after.m_Lv) return befor.m_Lv.CompareTo(after.m_Lv);  // 레벨 낮은 순서
		//	if (befor.m_Idx != after.m_Idx) return befor.m_Idx.CompareTo(after.m_Idx);  // 인덱스 낮은 순서

		//	return befor.m_Uid.CompareTo(after.m_Uid);  // 획득 순서 (고유번호가 높을수록 나중에 획득)
		//});

		for (int i = selectitems.Count - 1; i > -1; i--)
		{
			if (need < 1) break;
			ItemInfo info = selectitems[i];
			var matexp = info.GetExp();
			var needcnt = Utile_Class.NeedCnt(need, matexp);
			int cnt = Math.Min(needcnt, m_SelectItems[info]);
			need -= cnt * matexp;
			AddSelectItem(info, cnt * -1);
		}

		m_SUI.List.ScrollController.InitPosition();
		Minus_LV(MinusLV + 1);
	}

	public void OnAutoSet(int AddLV) {
		if (AddLV > -1) Add_LV(AddLV);
		else Minus_LV(AddLV);
	}

	public void OnLVUP() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.EquipLevelUp, 0)) return;
		if (m_SelectItems.Count < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(560));
			return;
		}
		if (USERINFO.m_Money < m_Price) {
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		if (m_SelectItems.Keys.ToList().Find(o => o.m_TData.GetEquipType() != EquipType.End && (o.m_Lv > 1 || m_Info.m_Grade < o.m_Grade)) != null) {
			POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(252), TDATA.GetString(253), (btn, obj) => {
				if ((EMsgBtn)btn == EMsgBtn.BTN_YES) {
					SEND_REQ_ITEM_LVUP();
				}
			});
			return;
		}
		SEND_REQ_ITEM_LVUP();
	}

	void SEND_REQ_ITEM_LVUP()
	{
		if (m_Action != null) return;
#if NOT_USE_NET
		m_Action = StartLVUpAction();
		StartCoroutine(m_Action);
#else
		List<REQ_USE_ITEM> items = new List<REQ_USE_ITEM>();
		for (int i = m_SelectItems.Count - 1; i > -1; i--)
		{
			var data = m_SelectItems.ElementAt(i);
			items.Add(new REQ_USE_ITEM()
			{
				UID = data.Key.m_Uid,
				Cnt = data.Value
			});
		}

		WEB.SEND_REQ_ITEM_LVUP((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_ALL_INFO((res2) => {
					InitLV();
					SetUI();
				});
				WEB.StartErrorMsg(res.result_code);
				return;
			}

			m_AddExp = (int)res.AddExp;

			m_Action = StartLVUpAction();
			StartCoroutine(m_Action);
		}, m_Info, items);
#endif
	}

	IEnumerator StartLVUpAction() {
		m_SUI.LVUpBtn.GetComponent<Button>().interactable = false;
		iTween.StopByName(gameObject, "GageAction");
#if NOT_USE_NET
		m_AddActionExp = m_AddExp;
		int lv = 0, exp = 0;
		CalcLV(out lv, out exp, m_AddExp);
		USERINFO.Check_Mission(MissionType.EquipLevelUp, 0, 0, lv - m_Info.m_Lv);
		USERINFO.Check_MissionUpDown(MissionType.EquipLevelUp, m_Info.m_Lv, lv, 1);
		if (lv != m_Info.m_Lv) USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Level_Count, m_Info.m_Lv, lv);
		m_Info.m_Lv = lv;
		m_Info.m_Exp = exp;

		for (int i = m_SelectItems.Count - 1; i > -1; i--) {
			var data = m_SelectItems.ElementAt(i);
			data.Key.m_Stack -= data.Value;
			if (data.Key.m_Stack < 1) USERINFO.m_Items.Remove(data.Key);
		}
		USERINFO.ChangeMoney(-m_Price);
#endif
		m_AddActionExp = 0;
		m_Price = 0;
		m_SelectItems.Clear();
		m_SelectItemObjs.Clear();
		UTILE.Load_Prefab_List(m_SelectItems.Count, m_SUI.SelectList.Scroll.content, m_SUI.SelectList.Prefab.transform);

		StartExpAction(false, m_Info.m_Lv);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		m_SUI.LVUpBtn.GetComponent<Button>().interactable = true;
		MAIN.Save_UserInfo();
		SetAllMatList();
		InitLV();
		m_Action = null;
		SetUI();

	}

	///튜토리얼
	/// <summary> 0:강화재료리스트, 1:강화버튼그룹, 2:강화나가기버튼 </summary>
	public GameObject GetTutoFocus(int _pos) {
		return m_SUI.TutoFocus[_pos];
	}
}
