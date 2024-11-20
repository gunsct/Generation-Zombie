using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionChange : PopupBase
{
#pragma warning disable 0649
	public enum ListType
	{
		Now,
		NotNow,
		All
	}
	[System.Serializable]
	public struct SUI
	{
		public Item_Item_Card Item;
		public TextMeshProUGUI Price;
		public Transform OptionPanel;
		public Animator ActionAni;
		public GameObject SkipPanel;
		public TextMeshProUGUI ConfirmBtnTxt;
	}
	[Serializable]
	public struct SMUI
	{
		public GameObject[] UnLockGroup;
		public GameObject[] LockGroup;
		public TextMeshProUGUI LockProp;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SMUI m_SMUI;
	[SerializeField] Animator m_Ani;
	TextMeshProUGUI[] m_Lists;
	ItemInfo m_Info;
	int m_Pos;
	public bool m_Lock;
	ItemStat m_Stat;
	IEnumerator m_Action;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Info = (ItemInfo)aobjValue[0];
		m_Pos = (int)aobjValue[1];
		if(aobjValue.Length > 2) m_Lock = (bool)aobjValue[2];
		if(!m_Lock) m_Stat = m_Info.m_AddStat[m_Pos];
		m_Lists = m_SUI.OptionPanel.GetComponentsInChildren<TextMeshProUGUI>();
		SetList(ListType.Now);
		SetList(ListType.NotNow);
		
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		m_SUI.SkipPanel.SetActive(false);
		m_SUI.Item.SetData(m_Info);

		if (m_Lock) {
			TRandomStatTable table = TDATA.GetPickRandomStat(m_Info.m_TData.m_RandStatGroup);
			m_Lists[0].text = TDATA.GetStatString(table.m_Stat, table.GetVal());
			m_SMUI.LockProp.text = string.Format("{0}%", BaseValue.EQUIP_OPTIONSLOT_OPENPROB(m_Pos) * 100);
		}
		else m_Lists[0].text = TDATA.GetStatString(m_Stat);

		int Price = m_Lock ? TDATA.GetShopTable(BaseValue.Get_Grade_Shop_Idx_EqOp(m_Info.m_Grade, true)).GetPrice() : TDATA.GetShopTable(BaseValue.Get_Grade_Shop_Idx_EqOp(m_Info.m_Grade, false)).GetPrice();
		m_SUI.Price.text = string.Format("{0}/{1}", Utile_Class.CommaValue(USERINFO.m_Cash), Utile_Class.CommaValue(Price));
		m_SUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, Price);
		m_SUI.ConfirmBtnTxt.text = m_Lock ? TDATA.GetString(3001) : TDATA.GetString(247);

		for (int i = 0; i < m_SMUI.UnLockGroup.Length; i++) {
			m_SMUI.UnLockGroup[i].SetActive(!m_Lock);
		}
		for (int i = 0; i < m_SMUI.LockGroup.Length; i++) {
			m_SMUI.LockGroup[i].SetActive(m_Lock);
		}
	}

	void SetList(ListType _type) {
		switch (_type) {
			case ListType.Now: m_Lists[m_Lists.Length - 1].text = m_Lock ? TDATA.GetString(43) : TDATA.GetStatString(m_Stat); break;
			default:
				for (int i = m_Lists.Length - (_type == ListType.All ? 1 : 2); i > 0; i--) {
					TRandomStatTable table = TDATA.GetPickRandomStat(m_Info.m_TData.m_RandStatGroup);
					m_Lists[i].text = TDATA.GetStatString(table.m_Stat, table.GetVal());
				}
				break;
		}
	}

	public void OnChange()
	{
		if (m_Action != null) return;

		int Price = TDATA.GetShopTable(BaseValue.Get_Grade_Shop_Idx_EqOp(m_Info.m_Grade, false)).GetPrice();
		if (USERINFO.m_Cash < Price) {
			POPUP.StartLackPop(BaseValue.CASH_IDX);
			return;
		}

		for (int i = 0; i < m_SMUI.UnLockGroup.Length; i++) {
			m_SMUI.UnLockGroup[i].SetActive(true);
		}
		for (int i = 0; i < m_SMUI.LockGroup.Length; i++) {
			m_SMUI.LockGroup[i].SetActive(false);
		}

		if (m_Lock) {
#if NOT_USE_NET
			//FireBase-Analytics
			MAIN.GoldToothStatistics(GoldToothContentsType.ItemSlotBuy, m_Info.m_Idx);

			USERINFO.GetCash(-Price);
			MAIN.Save_UserInfo();

			float rand = UTILE.Get_Random(0f, 1f);
			if (rand < BaseValue.EQUIP_OPTIONSLOT_OPENPROB(m_Pos)) {//성공,잠금일때 해제하면서 새로운 스탯 추가
				m_Lock = !m_Lock;
				TRandomStatTable table = TDATA.GetPickRandomStat(m_Info.m_TData.m_RandStatGroup);
				m_Info.m_AddStat.Add(new ItemStat() { m_Stat = table.m_Stat, m_Val = table.GetVal() });
				m_Stat = m_Info.m_AddStat[m_Pos];
				SetList(ListType.All);
				m_Action = OptionChangeAction(() => {
				});
				StartCoroutine(m_Action);
				return;
			}
			else {//실패 연출 후 끄기
				m_Info.DeleteLastAddOption();
				if (m_Pos > 0) m_Pos--;
				SetList(ListType.All);
				m_Action = OptionChangeAction(() => {
					Close(0);
				});
				StartCoroutine(m_Action);
				return;
			}
#else
			WEB.SEND_REQ_ITEM_OPOPEN((res) => {
				if (!res.IsSuccess())
				{
					WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
					WEB.StartErrorMsg(res.result_code);
					return;
				}
				//FireBase-Analytics
				MAIN.GoldToothStatistics(GoldToothContentsType.ItemSlotBuy, m_Info.m_Idx);
				if (res.IsSuc) {
					m_Lock = !m_Lock;
					m_Stat = m_Info.m_AddStat[res.Pos];
					SetList(ListType.All);
					m_Action = OptionChangeAction(() => {
					});
					StartCoroutine(m_Action);
					return;
				}
				else {
					if (res.Pos > 0) m_Pos--;
					SetList(ListType.All);
					m_Action = OptionChangeAction(() => {
						Close(0);
					});
					StartCoroutine(m_Action);
					return;
				}
			}, m_Info.m_Uid);
#endif
		}
		else {
#if NOT_USE_NET
			m_Stat = m_Info.m_AddStat[m_Pos];
			TRandomStatTable table = TDATA.GetPickRandomStat(m_Info.m_TData.m_RandStatGroup);
			if (table == null) return;

			//FireBase-Analytics
			MAIN.GoldToothStatistics(GoldToothContentsType.ItemRemodeling, m_Info.m_Idx);

			m_Info.m_AddStat[m_Pos].m_Stat = table.m_Stat;
			m_Info.m_AddStat[m_Pos].m_Val = table.GetVal();
			USERINFO.GetCash(-Price);
			MAIN.Save_UserInfo();
			SetList(ListType.NotNow);

			// 연출 시작
			m_Action = OptionChangeAction();
			StartCoroutine(m_Action);
#else
			WEB.SEND_REQ_ITEM_OPCHANGE((res) => {
				if (!res.IsSuccess())
				{
					WEB.StartErrorMsg(res.result_code);
					WEB.SEND_REQ_ALL_INFO((res2) => { SetUI(); });
					return;
				}
				
				//FireBase-Analytics
				MAIN.GoldToothStatistics(GoldToothContentsType.ItemRemodeling, m_Info.m_Idx);

				m_Stat = m_Info.m_AddStat[res.Pos];
				SetList(ListType.NotNow);

				// 연출 시작
				m_Action = OptionChangeAction();
				StartCoroutine(m_Action);
			}, m_Info.m_Uid, m_Pos);
#endif
		}
	}

	IEnumerator OptionChangeAction(Action _cb = null)
	{
		m_SUI.SkipPanel.SetActive(true);
		m_SUI.ActionAni.SetTrigger("Roll");

		PlayEffSound(SND_IDX.SFX_1050);

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(()=> Utile_Class.IsAniPlay(m_SUI.ActionAni, 50f / 295f));

		SetList(ListType.Now);

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.ActionAni, 242f / 295f));

		PlayEffSound(SND_IDX.SFX_1051);

		yield return Utile_Class.CheckAniPlay(m_SUI.ActionAni);

		m_SUI.SkipPanel.SetActive(false);
		SetUI();
		// normal로 넘어갈때 일단 멈춰두었으므로 풀어준다.
		m_SUI.ActionAni.SetTrigger("Normal");
		m_Action = null;

		_cb?.Invoke();
	}

	public void OnActionSkip() {
		SND.StopEff();
		Utile_Class.AniSkip(m_SUI.ActionAni, 224f);
	}
	public void ClickViewProb() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.OptionChange_List, null, m_Info.m_TData.m_RandStatGroup);
	}
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
}
