using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LS_Web;

public class Item_PDA_ZombieFarm_CatchedList : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Cnt;
		public Transform Bucket;
		public Transform Element;//Item_ZombieFarm_Catched_Element
		public GameObject DelBtn;
		public GameObject Empty;
	}

	[SerializeField] SUI m_SUI;

	List<ZombieInfo> m_CheckZInfos = new List<ZombieInfo>();
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		SetUI();
	}
	void SetUI() {
		m_CheckZInfos.Clear();
		int zombiecnt = USERINFO.m_NotCageZombie.Count;
		m_SUI.Cnt.text = string.Format(TDATA.GetString(978), zombiecnt, BaseValue.ZOMBIE_KEEP_MAX);
		m_SUI.DelBtn.SetActive(zombiecnt > 0);
		m_SUI.Empty.SetActive(zombiecnt < 1);

		UTILE.Load_Prefab_List(zombiecnt, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< zombiecnt; i++) {
			Item_ZombieFarm_Catched_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_ZombieFarm_Catched_Element>();
			element.SetData(Item_ZombieFarm_Catched_Element.State.Check, USERINFO.m_NotCageZombie[i], SetCheck);
		}
	}
	void SetCheck(bool _check, ZombieInfo _info) {
		if (_check && !m_CheckZInfos.Contains(_info)) m_CheckZInfos.Add(_info);
		else if (!_check && m_CheckZInfos.Contains(_info)) m_CheckZInfos.Remove(_info);
	}
	public void ClickAllDelCheck() {
		m_CheckZInfos = USERINFO.m_NotCageZombie;
		for(int i = 0;i< m_SUI.Bucket.childCount; i++) {
			Item_ZombieFarm_Catched_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_ZombieFarm_Catched_Element>();
			if(!element.Is_Check) element.ClickSet();
		}
	}
	public void ClickDelConfirm() {
		if(m_CheckZInfos.Count < 1) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(986));
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.ZombieDecomposition, (res, obj) => {
			if (res == 1) {
				List<RES_REWARD_BASE> rewards = obj.GetComponent<ZombieDecomposition>().GetReward();
				if (rewards == null) {
					SetUI();
				}
				else MAIN.SetRewardList(new object[] { rewards }, () => {
					SetUI();
				});
			}
		}, m_CheckZInfos);
//		POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(980), TDATA.GetString(981), (result, obj) => {
//			if ((EMsgBtn)result == EMsgBtn.BTN_YES) {
//#if NOT_USE_NET
//#else
//				WEB.SEND_REQ_ZOMBIE_DESTROY((res) => {
//					if (res.IsSuccess()) {
//						if (res.GetRewards() == null) {
//							SetUI();
//						}
//						else MAIN.SetRewardList(new object[] { res.GetRewards() }, ()=> {
//							SetUI();
//						});
//					}
//				}, m_CheckZInfos);
//#endif
//			}
//		});
	}
}
