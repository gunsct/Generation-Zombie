using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class DNAOptionChange : PopupBase
{
	[Serializable]
	public struct Mat
	{
		public GameObject Group;
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Cnt;         //<color=#58AE4E>200</color> / 200  <color=#FF5151>200</color> / 200
		public GameObject Lack;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_RewardDNA_Card Item;
		public TextMeshProUGUI[] Name;
		public Item_Info_DNA_Stat[] Stat;
		public Transform StatBucket;
		public Transform StatElement;
		public Mat[] Mats;
	}
	[SerializeField] SUI m_SUI;

	DNAInfo m_Info;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (DNAInfo)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		m_SUI.Anim.SetTrigger(m_Info.m_TData.m_BGType.ToString());
		m_SUI.Item.SetData(m_Info.m_Idx, -1, m_Info.m_Lv, m_Info.m_UID);
		m_SUI.Name[0].text = m_SUI.Name[1].text = string.Format("{0} {1} - {2}", BaseValue.GetDNAColorName(m_Info.m_TData.m_BGType), m_Info.m_TData.GetName(), UTILE.Get_RomaNum(m_Info.m_Grade));

		var tdata = m_Info.m_TData;
		int DNAMAXOPCnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, BaseValue.MAX_DNA_LV);
		var oplist = TDATA.GetDNALVOps(tdata.m_BGType, tdata.m_Grade);

		for (int i = 0; i < m_SUI.Stat.Length; i++)
		{
			//스탯 엘리먼트 넣어주기
			m_SUI.Stat[i].SetData(oplist[i].m_Lv, m_Info);
		}

		List<TDNALevelTable.IdxCnt> mats = new List<TDNALevelTable.IdxCnt>();
		mats.AddRange(m_Info.m_TLData.m_TransMats);
		mats.Add(new TDNALevelTable.IdxCnt() { Idx = BaseValue.DOLLAR_IDX, Cnt = m_Info.m_TLData.m_TransDollar });
		for (int i = 0; i < m_SUI.Mats.Length; i++) {
			m_SUI.Mats[i].Group.SetActive(mats.Count > i);
			if (mats.Count > i) {
				RES_REWARD_BASE mat = MAIN.GetRewardData(RewardKind.Item, mats[i].Idx, mats[i].Cnt)[0];
				int getcnt = mats[i].Idx == BaseValue.DOLLAR_IDX ? (int)USERINFO.m_Money : USERINFO.GetItemCount(mats[i].Idx);
				int needcnt = mats[i].Cnt;
				m_SUI.Mats[i].Card.SetData(mat, null, false);
				m_SUI.Mats[i].Card.SetCntActive(false);
				m_SUI.Mats[i].Cnt.text = string.Format("<color={0}>{1}</color> / {2}", getcnt >= needcnt ? "#58AE4E" : "#FF5151", getcnt, needcnt);
				m_SUI.Mats[i].Lack.SetActive(getcnt < needcnt);
			}
		}
	}
	public void ClickOptionChange() {
		if (m_Action != null) return;
		if (m_Info.m_TLData.m_TransDollar > USERINFO.m_Money) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		bool is_mat = true;
		var mats = m_Info.m_TLData.m_TransMats;
		for (int i = 0; i < mats.Count; i++) {
			if (USERINFO.GetItemCount(mats[i].Idx) < mats[i].Cnt) {
				is_mat = false;
				break;
			}
		}
		if (!is_mat) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
			return;
		}
#if NOT_USE_NET
		m_SUI.Anim.SetTrigger("Merge");
		USERINFO.ChangeMoney(-m_Info.m_TLData.m_TransDollar);
		for (int i = 0; i < mats.Count; i++) {
			USERINFO.DeleteItem(mats[i].Idx, mats[i].Cnt);
		}
		m_Info.m_AddStat.Clear();
		for (int i = 0; i < m_Info.m_Lv - 1; i++) {
			TRandomStatTable table = null;
			if (m_Info.m_TLData.m_EssentialStatGrant == 0) {
				table = TDATA.GetPickRandomStat(m_Info.m_TData.m_RandStatGroup);
			}
			else if (m_Info.m_TLData.m_EssentialStatGrant > 0) {
				table = TDATA.GetRandomStatTable(m_Info.m_TLData.m_EssentialStatGrant);
			}
			if (table != null) {
				m_Info.m_AddStat.Add(new ItemStat() {
					m_Stat = table.m_Stat,
					m_Val = table.GetVal()
				});
			}
		}
		MAIN.Save_UserInfo();
		StartCoroutine(m_Action = IE_Result());
#else
		WEB.SEND_REQ_DNA_OPCHANGE((res) => {
			if (res.IsSuccess()) {
				StartCoroutine(m_Action = IE_Result());
			}
		}, m_Info);
#endif
	}
	IEnumerator IE_Result() {
		float frame = 60f;
		m_SUI.Anim.SetTrigger("Merge");
		PlayEffSound(SND_IDX.SFX_1114);
		yield return new WaitForEndOfFrame();

		var tdata = m_Info.m_TData;
		int DNAMAXOPCnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, BaseValue.MAX_DNA_LV);
		var oplist = TDATA.GetDNALVOps(tdata.m_BGType, tdata.m_Grade);
		for (int i = 0; i < m_SUI.Stat.Length; i++) {
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, frame / 330f));
			PlayEffSound(SND_IDX.SFX_1116);
			m_SUI.Stat[i].SetData(oplist[i].m_Lv, m_Info, true);
			frame += 15f;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, frame / 330f));
		yield return new WaitWhile(() => m_SUI.Stat[m_Info.m_AddStat.Count - 1].CheckEndAnim());

		m_SUI.Anim.SetTrigger("MergeToMain");
		SetUI();
		m_Action = null;
	}
	public void ClickViewList() {
		if (m_Action != null) return;
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAList, null, PopupName.DNAOptionChange);
	}
	public void Click_ViewProbList() {
		if (m_Action != null) return;
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAOptionChange_List, null, m_Info.m_TData.m_RandStatGroup);
	}
}
