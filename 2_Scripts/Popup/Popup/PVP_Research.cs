using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using System.Linq;

public class PVP_Research : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_Tab[] Tabs;
		public Transform Bucket;
		public Transform Element;   //Item_PVP_Research_List
	}
	[SerializeField] SUI m_SUI;
	int m_TabPos;
	ResearchType m_NowType;
	IEnumerator m_Action; //end ani check
	bool Is_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_SUI.Tabs[0].SetData(0, TDATA.GetString(6229), SetTab);
		m_SUI.Tabs[0].SetAlram(false);
		m_SUI.Tabs[0].OnClick();
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(6230), SetTab);
		m_SUI.Tabs[1].SetAlram(false);
		m_SUI.Tabs[2].SetData(2, TDATA.GetString(6231), SetTab);
		m_SUI.Tabs[2].SetAlram(false);
	}
	public override void SetUI() {
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		CheckAlarm();

		base.SetUI();
	}
	bool SetTab(Item_Tab _tab) {
		switch (_tab.m_Pos) {
			case 0: m_NowType = ResearchType.Camp_Attack; break;
			case 1: m_NowType = ResearchType.Camp_Survive; break;
			case 2: m_NowType = ResearchType.Camp_Defense_Steal; break;
		}
		SetResearch(m_NowType);
		m_SUI.Tabs[m_TabPos].SetActive(false);
		m_TabPos = _tab.m_Pos;
		m_SUI.Tabs[m_TabPos].SetActive(true);
		return true;
	}
	void CheckAlarm() {
		m_SUI.Tabs[0].SetAlram(USERINFO.IsCompResearching(ResearchType.Camp_Attack));
		m_SUI.Tabs[1].SetAlram(USERINFO.IsCompResearching(ResearchType.Camp_Survive));
		m_SUI.Tabs[2].SetAlram(USERINFO.IsCompResearching(ResearchType.Camp_Defense_Steal));
	}
	/// <summary>
	/// 연구 세팅
	/// </summary>
	/// <param name="_type"></param>
	void SetResearch(ResearchType _type) {
		Dictionary<int, Dictionary<int, TResearchTable>> tdatas = TDATA.GetResearchTableGroup(_type);
		int cnt = tdatas.SelectMany(o => o.Value.Values).Max(o => o.m_Pos.m_Line);

		UTILE.Load_Prefab_List(cnt, m_SUI.Bucket, m_SUI.Element);
		for(int i = 1;i<= cnt; i++) {
			Item_PVP_Research_List list = m_SUI.Bucket.GetChild(i - 1).GetComponent<Item_PVP_Research_List>();
			list.SetData(_type, i, CB_Research);
		}
	}
	public void CB_Research(ResearchInfo _info, Action _cb) {
		if (Is_Action) return;
		if (_info.m_State == TimeContentState.Idle) {
			List<ResearchType> types = new List<ResearchType>() { ResearchType.Camp_Attack, ResearchType.Camp_Defense_Steal, ResearchType.Camp_Survive };
			ResearchInfo progressinfo = null;
			for (int i = 0; i < types.Count; i++) {
				if (progressinfo == null) progressinfo = USERINFO.IsResearching(types[i]);
				else break;
			}
			//ResearchInfo progressinfo = USERINFO.IsResearching();
			if (progressinfo != null) {
				PlayCommVoiceSnd(VoiceType.Fail);
				if (progressinfo.GetRemainTime() > 0) {
					POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, string.Format(TDATA.GetString(846), progressinfo.m_TData.GetName()), (result, obj) => {
						if (result == 1) {
							if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
								Is_Action = true;
								progressinfo.OnComplete((res) => {
									Is_Action = false;
									if (res.IsSuccess()) {
										PlayEffSound(SND_IDX.SFX_0004);
										SetUI();
										SetResearch(m_NowType);
										POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(847), progressinfo.m_TData.GetName()));
										_cb?.Invoke();
									}
								});
							}
							else {
								POPUP.StartLackPop(BaseValue.CASH_IDX);
							}
						}
					}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Research, progressinfo.GetRemainTime()), false);
				}
				else {
					Is_Action = true;
					progressinfo.OnComplete((res) => {
						if (res.IsSuccess()) {
							PlayEffSound(SND_IDX.SFX_0104);
							WEB.SEND_REQ_RESEARCH_START((res) => {
								Is_Action = false;
								if (res.IsSuccess()) {
									SetUI();
									SetResearch(m_NowType);
									_cb?.Invoke();
								}
								else if (res.result_code == EResultCode.ERROR_CAMP_RES) {
									POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6248));
									Close(2);
								}
							}, _info);
						}
					});
				}
			}
			else {
				Is_Action = true;
				WEB.SEND_REQ_RESEARCH_START((res) => {
					Is_Action = false;
					if (res.IsSuccess()) {
						PlayEffSound(SND_IDX.SFX_0104);
						SetUI();
						SetResearch(m_NowType);
						_cb?.Invoke();
					}
					else if (res.result_code == EResultCode.ERROR_CAMP_RES) {
						POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6248));
						Close(2);
					}
				}, _info);
			}
		}
		else if(_info.m_State == TimeContentState.Play) {
			if (_info.GetRemainTime() > 0f) {
				POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, string.Format(TDATA.GetString(846), _info.m_TData.GetName()), (result, obj) => {
					if (result == 1) {
						if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
							Is_Action = true;
							_info.OnComplete((res) => {
								Is_Action = false;
								if (res.IsSuccess()) {
									PlayEffSound(SND_IDX.SFX_0004);
									SetUI();
									SetResearch(m_NowType);
									POPUP.Set_MsgBox(PopupName.Msg_PVP_Research_LvUp, null, _info);
									//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(847), _info.m_TData.GetName()));
									_cb?.Invoke();
								}
							});
						}
						else {
							POPUP.StartLackPop(BaseValue.CASH_IDX);
						}
					}
				}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Research, _info.GetRemainTime()), false);
			}
			else {
				Is_Action = true;
				_info.OnComplete((res) => {
					Is_Action = false;
					if (res.IsSuccess()) {
						PlayEffSound(SND_IDX.SFX_0004);
						SetUI();
						SetResearch(m_NowType);
						POPUP.Set_MsgBox(PopupName.Msg_PVP_Research_LvUp, null, _info);
						//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(847), _info.m_TData.GetName()));
						_cb?.Invoke();
					}
				});
			}
		}
	}
	public void Click_ViewInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Research_EffectList);
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
