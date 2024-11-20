using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static LS_Web;

public class EquipReassembly : PopupBase
{
	public enum State
	{
		Idle,
		Reassemble,
		Select,
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_Item_Card[] ItemCards;
		public Item_RewardItem_Card MatCard;
		public TextMeshProUGUI MatCnt;
		public GameObject MatLackFX;
		public TextMeshProUGUI[] GradeName;
		public TextMeshProUGUI[] ItemName;
		public TextMeshProUGUI Level;
		public TextMeshProUGUI[] Money;
		public GameObject ResultPrefab;
		public Transform ResultBucket;
		public Button ReassemblyBtn;
	}
	[SerializeField]
	SUI m_SUI;
	State m_State;
	ItemInfo[] m_ItemInfos = new ItemInfo[2];
	public ItemInfo m_SelectItem = null;
	TItemTable m_MatTData;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_ItemInfos[0] = (ItemInfo)aobjValue[0];
		m_MatTData = TDATA.GetAllItemIdxs().Find(o => o.m_Type == ItemType.ReassemblyMaterial && o.m_Grade == m_ItemInfos[0].m_Grade);
		m_ItemInfos[1] = USERINFO.m_Items.Find(o => o.m_TData == m_MatTData);
		//등급 이름
		RefreshUI();
	}

	void RefreshUI() {
		ItemInfo baseItemInfo = m_ItemInfos[0];
		ItemInfo matItemInfo = m_ItemInfos[1];
		
		m_SUI.ItemCards[0].SetData(baseItemInfo);
		m_SUI.GradeName[0].text = BaseValue.GradeName(baseItemInfo.m_Grade);
		m_SUI.GradeName[0].color = BaseValue.GradeColor(baseItemInfo.m_Grade);
		m_SUI.ItemName[0].text = baseItemInfo.GetName();

		m_SUI.MatCard.SetData(m_MatTData.m_Idx);
		m_SUI.GradeName[1].text = BaseValue.GradeName(matItemInfo != null ? matItemInfo.m_Grade : m_MatTData.m_Grade);
		m_SUI.GradeName[1].color = BaseValue.GradeColor(matItemInfo != null ? matItemInfo.m_Grade : m_MatTData.m_Grade);
		m_SUI.ItemName[1].text = m_MatTData.GetName();
		int stack = m_ItemInfos[1] != null ? m_ItemInfos[1].m_Stack : 0;
		m_SUI.MatCard.GetCntGroup.SetActive(true);
		m_SUI.MatCnt.text = string.Format("<color={0}>{1}</color> / {2}", Utile_Class.GetColorCode(BaseValue.GetUpDownStrColor(stack, 1)), stack, 1);
		m_SUI.MatLackFX.SetActive(stack < 1);
		m_SUI.ItemName[1].color = BaseValue.GetUpDownStrColor(stack, 1, "#D2533C", "#FFFFFF");
		m_SUI.ItemCards[1].SetData(0, baseItemInfo.m_Lv, baseItemInfo.m_Grade);
		m_SUI.GradeName[2].text = BaseValue.GradeName(baseItemInfo.m_Grade);
		m_SUI.GradeName[2].color = BaseValue.GradeColor(baseItemInfo.m_Grade);

		m_SUI.Level.text = string.Format("Lv {0}", m_ItemInfos[0].m_Lv);

		int needmoney = BaseValue.REASSEMBLTY_PRICE * m_ItemInfos[0].m_Lv;
		m_SUI.Money[0].text = USERINFO.m_Money.ToString();
		m_SUI.Money[1].text = needmoney.ToString();
		m_SUI.Money[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, needmoney);

		//m_SUI.ReassemblyBtn.interactable = USERINFO.m_Money >= needmoney && m_ItemInfos[1] != null && m_ItemInfos[1]?.m_Stack > 0;
	}

	/// <summary> 같은 레벨, 등급 랜덤 타입 장비 생성 </summary>
	public void ClickReassembly() {
		if (m_State != State.Idle) return; 
		if (!USERINFO.CheckBagSize()) {
			WEB.StartErrorMsg(EResultCode.ERROR_INVEN_SIZE);
			return;
		}
		if (USERINFO.m_Money < BaseValue.REASSEMBLTY_PRICE * m_ItemInfos[0].m_Lv) {
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		if(m_ItemInfos[1] == null || m_ItemInfos[1]?.m_Stack < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
			return;
		}
		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, TDATA.GetString(322), (result, obj) => {
			if (result == 1) {
				m_State = State.Reassemble;
				PlayEffSound(SND_IDX.SFX_1075);
				for (int i = m_SUI.ResultBucket.childCount - 1; i > -1; i--) {
					Destroy(m_SUI.ResultBucket.GetChild(i).gameObject);
				}
				// 아이템 제거 및 재료 사용
				TReassemblyTable reassemblytable = TDATA.GetReassemblyTable(m_ItemInfos[0].m_TData.m_Type);

#if NOT_USE_NET
				EquipType type = reassemblytable.GetRandEquipType();
				List<TItemTable> itemtables = TDATA.GetAllItemIdxs().FindAll(o => o.GetEquipType() == type && o.m_Grade == m_ItemInfos[0].m_Grade);
				int proptotal = itemtables.Sum(o => o.m_ReassemblyProb);
				for (int i = 0; i < 3; i++) {
					TItemTable itemtable = null;
					int proprand = UTILE.Get_Random(0, proptotal);
					int propsum = 0;
					for(int j = 0; j < itemtables.Count; j++) {
						propsum += itemtables[j].m_ReassemblyProb;
						if(proprand <= propsum) {
							itemtable = itemtables[j];
							break;
						}
					}
					//TItemTable itemtable = itemtables[UTILE.Get_Random(0, itemtables.Count)];

					ItemInfo newitem = new ItemInfo() {
						m_Uid = m_ItemInfos[0].m_Uid,
						m_Idx = itemtable.m_Idx,
						m_Lv = m_ItemInfos[0].m_Lv,
						m_Stack = m_ItemInfos[0].m_Stack,
						m_Exp = 0,
						m_Grade = m_ItemInfos[0].m_Grade,
						m_GetAlarm = true
					};
					for (int j = BaseValue.ITEM_OPTION_CNT(newitem.m_Grade) - 1; j > -1; j--) newitem.SetGradeStat();

					Item_Reassembly_Result reitem = Utile_Class.Instantiate(m_SUI.ResultPrefab, m_SUI.ResultBucket).GetComponent<Item_Reassembly_Result>();
					reitem.SetData(newitem, CB_SelectItem);
				}
				USERINFO.Check_Mission(MissionType.Remake, 0, 0, 1);
				m_SUI.Anim.SetTrigger("Mixing");
#else
				WEB.SEND_REQ_ITEM_REMAKE((res) => {
					if (!res.IsSuccess())
					{
						WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
						WEB.StartErrorMsg(res.result_code);
						return;
					}
					USERINFO.Check_Mission(MissionType.Remake, 0, 0, 1);
					for (int i = 0;i< res.ReMake.Count; i++) {
						ItemInfo newitem = new ItemInfo() {
							m_Uid = res.ReMake[i].UID,
							m_Idx = res.ReMake[i].Idx,
							m_Lv = res.ReMake[i].LV,
							m_Stack = res.ReMake[i].Cnt,
							m_Exp = res.ReMake[i].EXP,
							m_Grade = res.ReMake[i].Grade,
							m_AddStat = res.ReMake[i].AddStat.Select(o => o.GetItemStat()).ToList(),
							m_GetAlarm = true
						};
						//for (int j = BaseValue.ITEM_OPTION_CNT(newitem.m_Grade) - 1; j > -1; j--) newitem.SetGradeStat();

						Item_Reassembly_Result reitem = Utile_Class.Instantiate(m_SUI.ResultPrefab, m_SUI.ResultBucket).GetComponent<Item_Reassembly_Result>();
						reitem.SetData(newitem, CB_SelectItem);
					}
					m_SUI.Anim.SetTrigger("Mixing");
				}, m_ItemInfos[0].m_Uid);
#endif
			}
		});
	}
	public void CB_SelectItem(ItemInfo _info) {
		m_SelectItem = _info;
		for (int i = m_SUI.ResultBucket.childCount - 1; i > -1; i--) {
			m_SUI.ResultBucket.GetChild(i).GetComponent<Item_Reassembly_Result>().NonSelect(m_SelectItem);
		}
	}
	public void ClickReassemblyConfirm() {
		if (m_State != State.Reassemble) return;
		if (m_SelectItem == null) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(560));
			return;
		}
		m_State = State.Select;
		CharInfo charinfo = USERINFO.GetEquipChar(m_ItemInfos[0].m_Uid);
		EquipType type = m_ItemInfos[0].m_TData.GetEquipType();
		List<RES_REWARD_BASE> item = new List<RES_REWARD_BASE>();

#if NOT_USE_NET
		USERINFO.DeleteItem(m_ItemInfos[0].m_Uid);
		if(charinfo != null) charinfo.m_EquipUID[(int)type] = 0;
		USERINFO.InsertItem(m_SelectItem);
		if (charinfo != null && type == m_SelectItem.m_TData.GetEquipType()) {
			charinfo.m_EquipUID[(int)type] = m_SelectItem.m_Uid;
			USERINFO.AddEquipUID(m_SelectItem.m_Uid);
		}
		USERINFO.ChangeMoney(-BaseValue.REASSEMBLTY_PRICE * m_ItemInfos[0].m_Lv);
		USERINFO.DeleteItem(m_ItemInfos[1].m_Uid, 1);
		MAIN.Save_UserInfo();
		
		item.Add(new RES_REWARD_ITEM() {
			Type = Res_RewardType.Item,
			UID = m_SelectItem.m_Uid,
			Idx = m_SelectItem.m_Idx,
			Cnt = 1
		});

		MAIN.SetRewardList(new object[] { item }, () => {
			m_ItemInfos[0] = m_SelectItem;
			RefreshUI();
			Close(1);
		});
#else
		WEB.SEND_REQ_ITEM_REMAKE_SELECT((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}

			item.Add(new RES_REWARD_ITEM() {
				Type = Res_RewardType.Item,
				UID = m_SelectItem.m_Uid,
				Idx = m_SelectItem.m_Idx,
				Cnt = 1
			});
			MAIN.SetRewardList(new object[] { item }, () => {
				m_ItemInfos[0] = m_SelectItem;
				RefreshUI();
				Close(1);
			});
		}, m_ItemInfos[0].m_Uid, m_SelectItem.m_Uid);
#endif

	}
	public void Click_ViewList() {
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EquipReassembly_List, null, m_ItemInfos[0]);
	}
}
