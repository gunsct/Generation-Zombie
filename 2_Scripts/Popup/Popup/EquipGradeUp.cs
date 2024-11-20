using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class EquipGradeUp : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Animator MatCardAnim;
		public Item_Item_Card[] ItemCards;
		public TextMeshProUGUI[] GradeName;
		public TextMeshProUGUI NeedGradeTxt;
		public TextMeshProUGUI[] ItemName;
		public TextMeshProUGUI[] MaxLevel;
		public TextMeshProUGUI[] Money;
		public GameObject[] BottomGroups;
		public GameObject MatSelectGroup;
		public Transform MatBucket;
		public GameObject MatPrefab;
		public GameObject MatBtn;
		public Button UpGradeBtn;
	}
	[SerializeField]
	SUI m_SUI;
	ItemInfo[] m_ItemInfos = new ItemInfo[2];
	Item_ItemSquare_SingleCheck m_SelectMat;

	private void Awake() {
		m_SUI.ItemCards[1].gameObject.SetActive(false);
		m_SUI.MatSelectGroup.SetActive(false);
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_ItemInfos[0] = (ItemInfo)aobjValue[0];

		//등급 이름
		RefreshUI();
	}

	void RefreshUI() {
		m_SUI.ItemCards[0].SetData(m_ItemInfos[0]);
		bool islastgrade = m_ItemInfos[0].m_Grade == BaseValue.MAX_ITEM_GRADE;
		m_SUI.BottomGroups[0].gameObject.SetActive(!islastgrade && !m_SUI.MatSelectGroup.activeSelf);
		m_SUI.BottomGroups[1].gameObject.SetActive(!islastgrade && !m_SUI.MatSelectGroup.activeSelf);


		m_SUI.ItemCards[1].gameObject.SetActive(m_ItemInfos[1] != null);
		m_SUI.GradeName[1].gameObject.SetActive(m_ItemInfos[1] != null);
		m_SUI.ItemName[1].gameObject.SetActive(m_ItemInfos[1] != null);
		m_SUI.MatBtn.SetActive(m_ItemInfos[0].m_Grade < BaseValue.MAX_ITEM_GRADE && m_ItemInfos[1] == null);
		if (m_ItemInfos[1] != null) {
			m_SUI.ItemCards[1].SetData(m_ItemInfos[1]);
			m_SUI.GradeName[1].text = BaseValue.GradeName(m_ItemInfos[1].m_Grade);
			m_SUI.GradeName[1].color = BaseValue.GradeColor(m_ItemInfos[1].m_Grade);
			m_SUI.ItemName[1].text = m_ItemInfos[1].m_TData.GetName();
		}
		else {
			m_SUI.NeedGradeTxt.gameObject.SetActive(m_ItemInfos[0].m_Grade < BaseValue.MAX_ITEM_GRADE);
			m_SUI.NeedGradeTxt.text = string.Format(TDATA.GetString(315), BaseValue.GradeColorCode(m_ItemInfos[0].m_Grade), BaseValue.GradeName(m_ItemInfos[0].m_Grade));
		}
		m_SUI.GradeName[0].text = BaseValue.GradeName(m_ItemInfos[0].m_Grade);
		m_SUI.GradeName[0].color = BaseValue.GradeColor(m_ItemInfos[0].m_Grade);
		m_SUI.ItemName[0].text = m_ItemInfos[0].m_TData.GetName();

		int grade = m_ItemInfos[0].m_Grade;
		if (!islastgrade) {
			m_SUI.ItemCards[2].SetData(m_ItemInfos[0].m_Idx, m_ItemInfos[0].m_Lv, grade + 1);
			m_SUI.GradeName[2].text = BaseValue.GradeName(grade + 1);
			m_SUI.GradeName[2].color = BaseValue.GradeColor(grade + 1);
			m_SUI.MaxLevel[0].text = BaseValue.ITEM_GRADE_MAX_LV(grade).ToString();
			m_SUI.MaxLevel[1].text = BaseValue.ITEM_GRADE_MAX_LV(grade + 1).ToString();
			m_SUI.Money[0].text = USERINFO.m_Money.ToString();
			m_SUI.Money[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, BaseValue.ITEM_GRADEUP_MONEY(grade));
			m_SUI.Money[1].text = BaseValue.ITEM_GRADEUP_MONEY(grade).ToString();
			m_SUI.UpGradeBtn.interactable = USERINFO.m_Money >= BaseValue.ITEM_GRADEUP_MONEY(grade) && m_ItemInfos[1] != null;
		}
	}

	/// <summary> 재료장비 고를 창 띄움 </summary>
	public void ClickMatEquip() {
		m_SUI.BottomGroups[0].gameObject.SetActive(false);
		m_SUI.BottomGroups[1].gameObject.SetActive(false);
		m_SUI.NeedGradeTxt.gameObject.SetActive(false);

		for (int i = m_SUI.MatBucket.childCount - 1;i> -1; i--) {
			Destroy(m_SUI.MatBucket.GetChild(i).gameObject);
		}

		List<ItemInfo> items = new List<ItemInfo>(
			USERINFO.m_Items.FindAll(
			o => {
				if (m_ItemInfos[0] != null && o.m_Uid == m_ItemInfos[0].m_Uid) return false;
				if (o.m_TData.GetEquipType() != m_ItemInfos[0].m_TData.GetEquipType()) return false;
				if (o.m_Lv != m_ItemInfos[0].m_Lv) return false;
				if (USERINFO.GetEquipChar(o.m_Uid) != null) return false;
				return o.m_Grade == m_ItemInfos[0].m_Grade;
			}));
		//for (int i = 0; i < items.Count; i++) items[i].GetCombatPower();
		items.Sort((befor, after) => {
			// 일단 전투력순으로만 소팅
			return after.m_CP.CompareTo(befor.m_CP);
		});

		for(int i = 0; i < items.Count; i++) {
			Item_ItemSquare_SingleCheck item = Utile_Class.Instantiate(m_SUI.MatPrefab, m_SUI.MatBucket).GetComponent<Item_ItemSquare_SingleCheck>();
			item.SetData(items[i], (result) => { CB_MatSelect(result); });
			item.SetCheck(m_ItemInfos[1] == items[i]);
		}
		m_SUI.MatSelectGroup.SetActive(true);
	}

	public void CB_MatSelect(Item_ItemSquare_SingleCheck _item) {
		if (m_SelectMat != null) m_SelectMat.SetCheck(false);
		m_SUI.MatCardAnim.SetTrigger("Loop");
		m_SelectMat = _item; 
		m_SelectMat.SetCheck(true);
		m_ItemInfos[1] = m_SelectMat.m_Info;
		RefreshUI();
	}
	public void ClickSelectConfirm() {
		m_SUI.MatCardAnim.SetTrigger("Set");
		m_SUI.NeedGradeTxt.gameObject.SetActive(true);
		m_SUI.MatSelectGroup.SetActive(false);
		RefreshUI();
	}
	/// <summary> 승급 </summary>
	public void ClickGradeUp() {
#if NOT_USE_NET
		USERINFO.ChangeMoney(-BaseValue.ITEM_GRADEUP_MONEY(m_ItemInfos[0].m_Grade));
		m_ItemInfos[0].m_Grade++;
		m_ItemInfos[0].SetGradeStat();
		USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Grade_Count, m_ItemInfos[0].m_Grade - 1, m_ItemInfos[0].m_Grade);
		USERINFO.m_Collection.Check(CollectionType.Equip, m_ItemInfos[0].m_Idx, m_ItemInfos[0].m_Grade);

		List<RES_REWARD_BASE> item = new List<RES_REWARD_BASE>();
		item.Add(new RES_REWARD_ITEM() {
			Type = Res_RewardType.Item,
			UID = m_ItemInfos[0].m_Uid,
			Idx = m_ItemInfos[0].m_Idx,
			Cnt = 1
		});

		USERINFO.DeleteItem(m_ItemInfos[1].m_Uid);
		MAIN.Save_UserInfo();

		StartCoroutine(GradeUpAction(item));
#else
		WEB.SEND_REQ_ITEM_UPGRADE((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}

			List<RES_REWARD_BASE> items = new List<RES_REWARD_BASE>();
			items.Add(new RES_REWARD_ITEM() {
				Type = Res_RewardType.Item,
				UID = m_ItemInfos[0].m_Uid,
				Idx = m_ItemInfos[0].m_Idx,
				Cnt = 1
			});

			StartCoroutine(GradeUpAction(items));
		}, m_ItemInfos[0], m_ItemInfos[1]);
#endif
	}

	IEnumerator GradeUpAction(List<RES_REWARD_BASE> _items) {
		//연출
		m_SUI.Anim.SetTrigger("Mixing");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 73f/96f));

		MAIN.SetRewardList(new object[] { _items }, () => { Close(0); });
		//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList_Equip, (result, obj) => {
		//	//RefreshUI();
		//	Close(0);
		//}, _items);
	}
}
