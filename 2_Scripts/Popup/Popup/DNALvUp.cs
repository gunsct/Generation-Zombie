using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class DNALvUp : PopupBase
{
	[Serializable]
	public struct Mat
	{
		public GameObject Group;
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Name;		//색상 이름 DNA - 로마등급
		public TextMeshProUGUI Cnt;         //<color=#58AE4E>200</color> / 200  <color=#FF5151>200</color> / 200
		public GameObject Lack;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_RewardDNA_Card Item;
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI[] Lv;
		public TextMeshProUGUI[] OptionTxt;
		public TextMeshProUGUI[] Dollar;
		public Mat[] Mats;
	}
	[Serializable]
	public struct SRUI
	{
		public GameObject ResultPanel;
		public Item_RewardDNA_Card DNA;
		public TextMeshProUGUI[] Lv;
		public Item_Info_DNA_Stat[] Stats;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SRUI m_SRUI;

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
		m_SUI.Desc.text = m_Info.m_TData.GetDesc();

		var tdata = m_Info.m_TData;
		var tNowLV = TDATA.GetDNALevelTable(tdata.m_BGType, tdata.m_Grade, m_Info.m_Lv);
		var tNextLV = TDATA.GetDNALevelTable(tdata.m_BGType, tdata.m_Grade, m_Info.m_Lv + 1);

		m_SUI.Lv[0].text = string.Format("<size=80%>Lv. </size>{0}", m_Info.m_Lv);
		m_SUI.Lv[1].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", m_Info.m_Lv);
		m_SUI.Lv[2].text = string.Format("<size=80%>Lv. </size>{0}", m_Info.m_Lv + 1);
		m_SUI.Lv[3].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", m_Info.m_Lv + 1);
		m_SUI.OptionTxt[0].text = string.Format(TDATA.GetString(962), TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, m_Info.m_Lv));
		m_SUI.OptionTxt[1].text = m_SUI.OptionTxt[2].text = string.Format(TDATA.GetString(962), TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, m_Info.m_Lv+1));

		var mats = m_Info.m_TLData.m_LvUpMats;
		for (int i = 0; i < m_SUI.Mats.Length; i++) {
			m_SUI.Mats[i].Group.SetActive(mats.Count > i);
			if (mats.Count > i) {
				RES_REWARD_BASE mat = MAIN.GetRewardData(RewardKind.Item, mats[i].Idx, mats[i].Cnt)[0];
				int getcnt = USERINFO.GetItemCount(mats[i].Idx);
				int needcnt = mats[i].Cnt;
				m_SUI.Mats[i].Card.SetData(mat, null, false);
				m_SUI.Mats[i].Card.SetCntActive(false);
				m_SUI.Mats[i].Name.text = TDATA.GetItemTable(mat.GetIdx()).GetName();//#58AE4E, #FF5151
				m_SUI.Mats[i].Cnt.text = string.Format("<color={0}>{1}</color> / {2}", getcnt >= needcnt ? "#58AE4E" : "#FF5151", getcnt, needcnt);
				m_SUI.Mats[i].Lack.SetActive(getcnt < needcnt);
			}
		}

		int dollar = m_Info.m_TLData.m_LvUpDollar;
		m_SUI.Dollar[0].text = Utile_Class.CommaValue(USERINFO.m_Money);
		m_SUI.Dollar[1].text = Utile_Class.CommaValue(dollar);
		m_SUI.Dollar[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, dollar);
	}
	public void ClickLvUp() {
		if (m_Action != null) return;
		if(m_Info.m_TLData.m_LvUpDollar > USERINFO.m_Money) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		bool is_mat = true;
		var mats = m_Info.m_TLData.m_LvUpMats;
		for (int i = 0;i< mats.Count; i++) {
			if(USERINFO.GetItemCount(mats[i].Idx) < mats[i].Cnt) {
				is_mat = false;
				break;
			}
		}
		if (!is_mat) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
			return;
		}

		DNAInfo preinfo = new DNAInfo() {
			m_Idx = m_Info.m_Idx,
			m_UID = m_Info.m_UID,
			m_AddStat = new List<ItemStat>(),
			m_Lv = m_Info.m_Lv
		};
		preinfo.m_AddStat.AddRange(m_Info.m_AddStat);

#if NOT_USE_NET
		USERINFO.ChangeMoney(-m_Info.m_TLData.m_LvUpDollar);
		for (int i = 0; i < mats.Count; i++) {
			USERINFO.DeleteItem(mats[i].Idx, mats[i].Cnt);
		}
		m_Info.m_Lv++;
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
		MAIN.Save_UserInfo();
		StartCoroutine(m_Action = IE_Result(preinfo));
#else
		WEB.SEND_REQ_DNA_UPGRADE((res) => {
			if (res.IsSuccess()) {
				StartCoroutine(m_Action = IE_Result(m_Info));
			}
		}, m_Info);
#endif
	}
	IEnumerator IE_Result(DNAInfo _preinfo) {
		m_SUI.Anim.SetTrigger("Merge");
		PlayEffSound(SND_IDX.SFX_1113);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => !m_SRUI.ResultPanel.activeSelf);

		var tdata = m_Info.m_TData;
		int DNAMAXOPCnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, BaseValue.MAX_DNA_LV);
		var oplist = TDATA.GetDNALVOps(tdata.m_BGType, tdata.m_Grade);

		for(int i = 0, lv = _preinfo.m_Lv - 1; i < 3; i++, lv++)
		{
			int opcnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, lv);
			m_SRUI.Stats[i].gameObject.SetActive(lv <= BaseValue.MAX_DNA_LV && opcnt > 0);
			if (m_SRUI.Stats[i].gameObject.activeSelf)
			{
				if (lv >= _preinfo.m_Lv) m_SRUI.Stats[i].SetNone(lv, _preinfo);
				else m_SRUI.Stats[i].SetData(lv, _preinfo);
			}
		}

		RES_REWARD_DNA reward = new RES_REWARD_DNA() {
			UID = m_Info.m_UID,
			Idx = m_Info.m_Idx,
			Grade = m_Info.m_Grade,
			Lv = m_Info.m_Lv,
			Type = Res_RewardType.DNA
		};
		m_SRUI.DNA.SetData(reward.Idx, 0, reward.Lv - 1, reward.UID);

		m_SRUI.Lv[0].text = string.Format("<size=80%>Lv. </size>{0}", m_Info.m_Lv - 1);
		m_SRUI.Lv[1].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", m_Info.m_Lv - 1);
		m_SRUI.Lv[2].text = string.Format("<size=80%>Lv. </size>{0}", m_Info.m_Lv);
		m_SRUI.Lv[3].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", m_Info.m_Lv);


		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 110f / 205f));
		PlayEffSound(SND_IDX.SFX_1114);

		m_SRUI.DNA.SetData(reward.Idx, 0, reward.Lv, reward.UID);

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 200f / 205f));
		PlayEffSound(SND_IDX.SFX_1116);

		for (int i = 0, lv = _preinfo.m_Lv - 1; i < 3; i++, lv++)
		{
			int opcnt = TDATA.GetDNALVOpCnt(tdata.m_BGType, tdata.m_Grade, lv);
			m_SRUI.Stats[i].gameObject.SetActive(lv <= BaseValue.MAX_DNA_LV && opcnt > 0);
			if (m_SRUI.Stats[i].gameObject.activeSelf) m_SRUI.Stats[i].SetData(lv, _preinfo, lv == _preinfo.m_Lv);
		}

		m_Action = null;
	}
	public void ClickViewList() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAList, null, PopupName.DNALvUp);
	}
	public void ClickConfirm() {
		if (m_Info.m_Lv >= BaseValue.MAX_DNA_LV) Close(0);
		else {
			StartCoroutine(IE_ToMain());
		}
	}
	IEnumerator IE_ToMain() {
		m_SUI.Anim.SetTrigger("MergeToMain");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		SetUI();
	}
}
