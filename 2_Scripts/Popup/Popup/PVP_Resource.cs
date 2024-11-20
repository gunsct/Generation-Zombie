using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;

public class PVP_Resource : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Lv;
		public Item_PVP_Resource_Element[] Mats;
	}
	[SerializeField] SUI m_SUI;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	IEnumerator m_Action; //end ani check
	bool Is_Action;
	CampBuildInfo m_CampBuildInfo { get { return USERINFO.m_CampBuild[CampBuildType.Resource]; } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_SUI.Lv.text = string.Format("{0} <size=80%>Lv.{1}</size>", TDATA.GetString(6206), m_CampBuildInfo.LV);
		for(int i = 0; i < 3; i++) {
			m_SUI.Mats[i].SetData(m_CampBuildInfo, i, SetMake, GetReward);
		}
		base.SetUI();
	}
	/// <summary> 제작 시작 0:junk, 1:Cultivate. 2:chemical</summary>
	public void SetMake(int _pos) {
		if (Is_Action) return;
		Is_Action = true;
		WEB.SEND_REQ_CAMP_RES_START((res) => {
			SetUI();
			Is_Action = false;
		}, _pos);
	}
	/// <summary> 제작 보상 0:junk, 1:Cultivate. 2:chemical</summary>
	public void GetReward(int _pos) {
		if (Is_Action) return;
		//획득량 연구 포함해서 계산
		CampBuildInfo sinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		TPVP_Camp_Storage stdata = TDATA.GetPVP_Camp_Storage(sinfo.LV);
		TPVP_Camp_Resource rtdata = TDATA.GetPVP_Camp_Resource(m_CampBuildInfo.LV);

		if (m_CampBuildInfo.IS_MakeComplete(_pos)) {
			Is_Action = true;
			WEB.SEND_REQ_CAMP_RES_END((res) => {
				Is_Action = false;
				if (res.IsSuccess()) {
					PlayEffSound(SND_IDX.SFX_0004);
					SetUI();
					if (res.Rewards == null) return;
					m_Rewards.AddRange(res.GetRewards());

					if (m_Rewards.Count > 0) {
						MAIN.SetRewardList(new object[] { m_Rewards }, () => {
							m_Rewards.Clear();
						});
					}
				}
				else if(res.result_code == EResultCode.ERROR_BUILD_RES_MAX) {
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Storage_Lack, (res, obj) => {
					}, new List<int>() { _pos }, 1);
				}
			}, _pos);
		}
		else {
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(6253), (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						WEB.SEND_REQ_CAMP_RES_END((res) => {
							if (res.IsSuccess()) {
								PlayEffSound(SND_IDX.SFX_0004);
								SetUI();
								if (res.Rewards == null) return;
								m_Rewards.AddRange(res.GetRewards());

								if (m_Rewards.Count > 0) {
									MAIN.SetRewardList(new object[] { m_Rewards }, () => {
										m_Rewards.Clear();
									});
								}
							}
							else if (res.result_code == EResultCode.ERROR_BUILD_RES_MAX) {
								POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Storage_Lack, (res, obj) => {
								}, new List<int>() { _pos }, 1);
							}
						}, _pos);
					}
					else {
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
				}
			}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.PVPCampResource, m_CampBuildInfo.GetRemainMakeTime(_pos)));
		}
	}
	void GetReward() {

	}
	/// <summary> 시설 강화 </summary>
	public void Click_Upgrade() {
		if (Is_Action) return;
		//최대 레벨 예외처리
		if (m_CampBuildInfo.LV >= TDATA.GetTPVP_Camp_NodeLevelGroup(CampBuildType.Resource).Count) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(963));
			return;
		}
		//생산중인게 있으면 예외처리
		if(m_CampBuildInfo.Values[0] != 0 || m_CampBuildInfo.Values[1] != 0 || m_CampBuildInfo.Values[2] != 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6252));
			return;
		}
		Is_Action = true;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			Is_Action = false;
			if (res.IsSuccess()) {
				PlayEffSound(SND_IDX.SFX_0104);
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Base_Upgrade, (res, obj) => {
					if (res == 2) Close(2);
					else SetUI();
				}, CampBuildType.Resource);
			}
		});
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
