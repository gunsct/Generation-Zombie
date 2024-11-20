using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[System.Serializable] public class DicInfo_DNA_Btn : SerializableDictionary<Info_DNA.BtnName, GameObject> { }
public class Info_DNA : PopupBase
{
	public enum BtnName
	{
		UnEquip = 0,
		Change,
		Destroy,
		OpChange,
		LVUP,
	}

    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image GradeBG;
		public Item_Inventory_Item Item;
		public TextMeshProUGUI GradeName;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public Transform StatBucket;
		public Transform StatElement;
		public DicInfo_DNA_Btn Btns;
	}
	[SerializeField] SUI m_SUI;
	DNAInfo m_Info;
	CharInfo m_CInfo;
	int m_EquipPos;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (DNAInfo)aobjValue[0];
		if (aobjValue.Length > 1) {
			m_CInfo = (CharInfo)aobjValue[1];
			m_EquipPos = (int)aobjValue[2];
		}
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		m_SUI.GradeBG.sprite = UTILE.LoadImg(string.Format("UI/BG/BG_Popup_DNA_{0}", (int)m_Info.m_TData.m_BGType - 1), "png");
		m_SUI.Item.SetData(m_Info, null);
		m_SUI.GradeName.text = string.Format("{0}{1} DNA", UTILE.Get_RomaNum(m_Info.m_Grade), TDATA.GetString(274));
		m_SUI.Name.text = m_Info.m_TData.GetName();
		m_SUI.Desc.text = m_Info.m_TData.GetDesc();

		var tdata = m_Info.m_TData;
		int DNAMAXOPCnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, BaseValue.MAX_DNA_LV);
		var oplist = TDATA.GetDNALVOps(tdata.m_BGType, tdata.m_Grade);

		UTILE.Load_Prefab_List(DNAMAXOPCnt, m_SUI.StatBucket, m_SUI.StatElement);
		for(int i = 0;i< DNAMAXOPCnt; i++) {
			//스탯 엘리먼트 넣어주기
			Item_Info_DNA_Stat element = m_SUI.StatBucket.GetChild(i).GetComponent<Item_Info_DNA_Stat>();
			element.SetData(oplist[i].m_Lv, m_Info);
		}

		m_SUI.Btns[BtnName.UnEquip].SetActive(m_CInfo != null);
		m_SUI.Btns[BtnName.Change].SetActive(m_CInfo != null);
	}
	public void ClickUnEquip() {
		if (m_Action != null) return;
#if NOT_USE_NET
		m_CInfo.m_EqDNAUID[m_EquipPos] = 0;
		PLAY.PlayEffSound(SND_IDX.SFX_1071);
		Close(0);
#else
		WEB.SEND_REQ_CHAR_DNA_SET((res) => {
			if (!res.IsSuccess()) {
				switch (res.result_code) {
					case EResultCode.ERROR_NOT_FOUND_CHAR:
					case EResultCode.ERROR_NOT_FOUND_DNA:
					case EResultCode.ERROR_POS:
						WEB.SEND_REQ_ALL_INFO(res2 => { });
						return;
					default:
						WEB.StartErrorMsg(res.result_code);
						return;
				}
			}
			PLAY.PlayEffSound(SND_IDX.SFX_1071);
			Close(0);
		}, m_CInfo.m_UID, m_EquipPos, 0);
#endif
	}
	public void ClickChangeEquip() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNASelect, (result, obj) => {
			if(result == 1) Close(result);
		}, m_EquipPos, m_CInfo);
	}
	public void ClickDecomposition() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNADecomposition, (result, obj) => {
			if(result == 1) Close(2);
		}, m_Info);
	}
	public void ClickOptionChange() {
		if (m_Action != null) return;
		var tdata = m_Info.m_TData;
		int opcnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, m_Info.m_Lv);
		if (opcnt < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(955));
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAOptionChange, (result, obj) => {
			SetUI();
		}, m_Info);
	}
	public void ClickLvUp() {
		if (m_Action != null) return;
		if (m_Info.m_Lv >= BaseValue.MAX_DNA_LV) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(963));
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNALvUp, (result, obj) => {
			 SetUI();
		}, m_Info);
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
