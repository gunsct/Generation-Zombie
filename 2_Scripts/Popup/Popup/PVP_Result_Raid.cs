using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class PVP_Result_Raid : PopupBase
{
	[Serializable]
	public class Mat
	{
		public GameObject Obj;
		public Image Icon;
		public Image Bg;
		public Image GradeBg;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Mat[] Mats;
		public Color[] CntColor;
		public Color[] BGColor;
		public Color[] GradeBGColor;
		public TextMeshProUGUI Msg;     //6237
	}
	[SerializeField] SUI m_SUI;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[0];
		PlayEffSound(SND_IDX.SFX_1360);
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		CampBuildInfo sinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		TPVP_Camp_Storage tdata = TDATA.GetPVP_Camp_Storage(sinfo.LV);
		RES_REWARD_BASE coin = m_Rewards.Find(o => o.Type == Res_RewardType.PVPCoin);
		bool is_full = false;
		if (coin != null) m_Rewards.Remove(coin);
		for (int i = 0; i < m_SUI.Mats.Length; i++) {
			if (m_Rewards.Count - 1 < i) m_SUI.Mats[i].Obj.SetActive(false);
			else {
				m_SUI.Mats[i].Obj.SetActive(true);
				RES_REWARD_MONEY reward = (RES_REWARD_MONEY)m_Rewards[i];
				TItemTable itdata = TDATA.GetItemTable(BaseValue.CAMP_RES_IDX(reward.Type));
				m_SUI.Mats[i].Icon.sprite = itdata.GetItemImg();
				m_SUI.Mats[i].Bg.color = m_SUI.BGColor[i];
				m_SUI.Mats[i].GradeBg.color = m_SUI.GradeBGColor[i];
				m_SUI.Mats[i].Name.text = itdata.GetName();
				if (tdata.m_SaveMat[i] == 0) {
					//m_SUI.Mats[i].Cnt.text = "+0";
					//m_SUI.Mats[i].Cnt.color = m_SUI.CntColor[0];
					m_SUI.Mats[i].Obj.SetActive(false);
				}
				else {
					m_SUI.Mats[i].Obj.SetActive(true);
					m_SUI.Mats[i].Cnt.text = string.Format("+{0}", Utile_Class.CommaValue(reward.Add));
					m_SUI.Mats[i].Cnt.color = m_SUI.CntColor[1];
					if (reward.Now - reward.Befor < reward.Add) is_full = true;
				}
			}
		}
		m_SUI.Msg.text = !is_full ? TDATA.GetString(6237) : string.Format("{0}\n{1}", TDATA.GetString(6237), TDATA.GetString(6255));
		base.SetUI();
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
