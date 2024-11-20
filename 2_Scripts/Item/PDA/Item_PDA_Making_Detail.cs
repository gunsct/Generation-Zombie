using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Making_Detail : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public Item_RewardItem_Card Item;
		public GameObject[] RewardIcon;
		public Image CharEqIcon;
		public TextMeshProUGUI Name;
		public GameObject[] NeedMatBuckets;
		//public Item_RewardItem_Card[] NeedMats;
		public Item_RewardList_Item[] NeedMats;
		public Item_Adv_CharCountGroup[] NeedMatCnts;
		public Slider Slider;
		public TextMeshProUGUI PriceTxt;
		public TextMeshProUGUI TimeTxt;
		public TextMeshProUGUI CntTxt;
		public Button Btn;
		public Material[] BtnMat;
		public TextMeshProUGUI BtnTxt;
		public Color[] BtnTxtColor;
		public GameObject CountSetGroup;
	}
	[SerializeField]
	SUI m_SUI;
	public int[] m_Cnt = new int[2];
	public Item_Mk_Element_Parent m_MakeElement;

	private void Awake() {
		for (int i = 0; i < m_SUI.NeedMatBuckets.Length; i++) {
			m_SUI.NeedMatBuckets[i].gameObject.SetActive(true);
		}
		//m_SUI.CountSetGroup.SetActive(false);
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_MakeElement = (Item_Mk_Element_Parent)args[0];

		m_SUI.Item.SetData(m_MakeElement.m_Mk_TData.m_ItemIdx);
		bool chareq = m_MakeElement.m_Mk_TData.m_Group == MakingGroup.PrivateEquip;
		m_SUI.RewardIcon[0].SetActive(!chareq);
		m_SUI.RewardIcon[1].SetActive(chareq);
		m_SUI.CharEqIcon.sprite = m_MakeElement.m_Item_TData.GetItemImg();
		m_SUI.Name.text = m_MakeElement.m_Item_TData.GetName();
	  
		m_Cnt[0] = 1;
		m_Cnt[1] = 10;
		m_SUI.Slider.minValue = m_Cnt[0];
		m_SUI.Slider.value = m_Cnt[0];
		m_SUI.Slider.maxValue = m_Cnt[1];
		m_SUI.CountSetGroup.SetActive(m_MakeElement.m_Mk_TData.m_Group != MakingGroup.Equip && m_MakeElement.m_Mk_TData.m_Group != MakingGroup.PrivateEquip);
		RefreshUI();
	}

	public void ChangeMakeCnt() {
		m_Cnt[0] = (int)m_SUI.Slider.value;
		RefreshUI();
	}
	public void MakeCntUpDown(bool _up) {
		m_Cnt[0] = Mathf.Clamp(m_Cnt[0] + (_up ? 1 : -1), 1, m_Cnt[1]);
		m_SUI.Slider.value = m_Cnt[0];
		RefreshUI();
	}
	void RefreshUI() {
		List<TMakingTable.MakeMat> mats = m_MakeElement.m_Mk_TData.m_Mats;
		for (int i = 0; i < m_SUI.NeedMats.Length; i++) {
			if (i < mats.Count) {
				m_SUI.NeedMatCnts[i].SetData(USERINFO.GetItemCount(mats[i].m_Idx), mats[i].m_Count * m_Cnt[0]);
				//m_SUI.NeedMats[i].SetData(mats[i].m_Idx, mats[i].m_Count * m_Cnt[0]);
				RES_REWARD_ITEM item = new RES_REWARD_ITEM() {
					Idx = mats[i].m_Idx,
					Cnt = mats[i].m_Count * m_Cnt[0],
					Type = Res_RewardType.Item
				};
				m_SUI.NeedMats[i].SetData(item, null, false);
				m_SUI.NeedMatBuckets[i].gameObject.SetActive(true);
			}
			else m_SUI.NeedMatBuckets[i].gameObject.SetActive(false);
		}

		bool canmake = m_MakeElement.m_Mk_TData.GetCanMake(m_Cnt[0]);
		m_SUI.Btn.GetComponent<Image>().material = m_SUI.BtnMat[canmake ? 0 : 1];
		m_SUI.BtnTxt.color = m_SUI.BtnTxtColor[canmake ? 0 : 1];

		m_SUI.CntTxt.text = m_Cnt[0].ToString();
		int price = m_MakeElement.m_Mk_TData.m_Dollar * m_Cnt[0];
		m_SUI.PriceTxt.text = Utile_Class.CommaValue(price);
		m_SUI.PriceTxt.color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, price, "#A2321E", "#B1BEA9");
		m_SUI.TimeTxt.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, m_MakeElement.m_Mk_TData.GetTime() * 0.001f * m_Cnt[0]);
	}
	public void ClickBtn() {
		if(!m_MakeElement.m_Mk_TData.IS_EnoughDollar(m_Cnt[0])) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		if (!m_MakeElement.m_Mk_TData.IS_EnoughMat(m_Cnt[0])) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
			return;
		}
		PlayEffSound(SND_IDX.SFX_0122);
#if NOT_USE_NET
		MakeItem(m_MakeElement.m_Mk_TData, m_Cnt[0]);
		m_MakeElement.SetState(TimeContentState.Play);
		USERINFO.Check_Mission(MissionType.Making, 0, 0, 1);
		USERINFO.Check_Mission(MissionType.Making, (int)m_MakeElement.m_Mk_TData.m_Group, 0, 1);
		MAIN.Save_UserInfo();
		string itemname = string.Format("<color=#FFE379>{0}</color>", TDATA.GetItemTable(m_MakeElement.m_Mk_TData.m_ItemIdx).GetName());
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(891), itemname));
		OnClose();
#else
		long premoney = USERINFO.m_Money;
		WEB.SEND_REQ_MAKING_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			var rewardasset = new Dictionary<Res_RewardType, RectTransform>();
			rewardasset.Add(Res_RewardType.Money, (RectTransform)transform);
			SetRewardAssetAni(rewardasset, 1f);
			DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, premoney);
			m_MakeElement.SetState(TimeContentState.Play);
			string itemname = string.Format("<color=#FFE379>{0}</color>", TDATA.GetItemTable(m_MakeElement.m_Mk_TData.m_ItemIdx).GetName());
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(891), itemname));
			OnClose();
		}, m_MakeElement.m_Mk_TData.m_ItemIdx, m_Cnt[0]);
#endif
	}
	/// <summary> 제작 시작, 재료 소모, 타이머 추가 </summary>
	void MakeItem(TMakingTable _table, int _cnt = 1) {
		for (int i = 0; i < _table.m_Mats.Count; i++) {
			USERINFO.DeleteItem(_table.m_Mats[i].m_Idx, _table.m_Mats[i].m_Count * _cnt);
		}
		USERINFO.ChangeMoney(-_table.m_Dollar * _cnt);
		USERINFO.InsertMake(_table.m_ItemIdx, _cnt);
	}
	/// <summary> 재료 수급처 팝업 </summary>
	public void ClickMatGuide(int _pos) {
		List<TMakingTable.MakeMat> mats = m_MakeElement.m_Mk_TData.m_Mats;
		POPUP.ViewItemInfo((result, obj) => { RefreshUI(); }, new object[] { new ItemInfo(mats[_pos].m_Idx), PopupName.NONE, null, mats[_pos].m_Count * m_Cnt[0] });
	}
	/// <summary> 돌아가기 </summary>
	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();

	}
	public override void OnClose() {
		m_CloaseCB?.Invoke(Item_PDA_Making.State.Main, null);
	}
}
