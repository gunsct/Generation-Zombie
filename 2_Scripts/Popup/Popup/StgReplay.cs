using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class StgReplay : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Timer;
		public Item_StgReplay_Stage_Element[] Elements;
		public GameObject CloseBtn;
	}
	[SerializeField] SUI m_SUI;
	UserInfo.Stage m_Stage;
	List<UserInfo.StageIdx> m_StgIdxs = new List<UserInfo.StageIdx>();
	StageContentType m_StageContent = StageContentType.Replay;

	public GameObject GetStgList(int _pos) { return m_SUI.Elements[_pos].gameObject; }
	public GameObject GetCloseBtn { get { return m_SUI.CloseBtn; } }
	//매일 00시 기준 리스트 초기화
	//또는 비용 주고 초기화(초기화권)
	private void Update() {
		m_SUI.Timer.text = UTILE.GetSecToTimeStr((UTILE.Get_ZeroTime() + 86400000 - UTILE.Get_ServerTime_Milli()) * 0.001d);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_0195);
		m_StageContent = (StageContentType)aobjValue[0];
		switch (m_StageContent) {
			case StageContentType.Replay:
				//m_SUI.Anim.SetTrigger("Normal");
				break;
			case StageContentType.ReplayHard:
				//m_SUI.Anim.SetTrigger("Night");
				break;
			case StageContentType.ReplayNight:
				//m_SUI.Anim.SetTrigger("Apo");
				break;
		}
		DLGTINFO?.f_RFShellUI?.Invoke(USERINFO.m_Energy.Cnt);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		SetList();
		DLGTINFO?.f_RFTicketUI?.Invoke(USERINFO.GetItemCount(BaseValue.CLEARTICKET_IDX), USERINFO.GetItemCount(BaseValue.CLEARTICKET_IDX));
	}
	void SetList() {
		m_Stage = USERINFO.m_Stage[m_StageContent];
		m_StgIdxs = m_Stage.Idxs;
		for(int i = 0; i < m_SUI.Elements.Length; i++) {
			if (i < m_StgIdxs.Count) {
				m_SUI.Elements[i].SetData(m_StgIdxs[i], UsePass, GoPlay);
				m_SUI.Elements[i].gameObject.SetActive(true);
			}
			else m_SUI.Elements[i].gameObject.SetActive(false);
		}
	}
	void UsePass(UserInfo.StageIdx _info) {
		TStageTable tdata = TDATA.GetStageTable(_info.Idx);
		if (_info.Clear < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(1025));
			return;
		}
		//소탕권 부족시
		if (USERINFO.GetItemCount(BaseValue.CLEARTICKET_IDX) < 1) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(540));
			return;
		}
		//에너지 부족시
		if (tdata.m_Energy > 0 && USERINFO.m_Energy.Cnt < tdata.m_Energy) {
			POPUP.StartLackPop(BaseValue.ENERGY_IDX, false, (res) => {
				SetUI();
			});
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Pass_Use, (result, obj) => {
			if (result > 0) {
				WEB.SEND_REQ_STAGE_CLEAR_TICKET((res) => {
					if (!res.IsSuccess()) {
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						return;
					}
					USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, tdata.m_Energy * result);
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Pass_Result, (res, obj) => {
						SetUI();
					}, res);
				}, m_Stage, _info.Week, _info.Pos, _info.Idx, result);
			}
		}, (int)m_Stage.GetItemCnt(false), m_Stage.GetItemMax(false), tdata.m_Energy, true);
	}

	void GoPlay(UserInfo.StageIdx _info) {
		TStageTable tdata = TDATA.GetStageTable(_info.Idx);
		if (tdata.m_Energy > 0 && USERINFO.m_Energy.Cnt < tdata.m_Energy) {
			POPUP.StartLackPop(BaseValue.ENERGY_IDX);
			return;
		}

		if (tdata.m_Mode == StageModeType.Training) {
			PLAY.GetStagePlayCode((result) => { PLAY.GoReplay(m_Stage, _info); }, m_StageContent, _info.Idx, _info.Week, _info.Pos);
		}
		else {
			WEB.SEND_REQ_STAGE_CLEAR_TICKET((res) => {
				if (!res.IsSuccess()) {
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					return;
				}
				USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, tdata.m_Energy);
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => { SetUI(); });
			}, m_Stage, _info.Week, _info.Pos, _info.Idx, 1);
			//DLGTINFO?.f_OBJSndOff?.Invoke();
			//SND.StopEff();
			//PlayEffSound(SND_IDX.SFX_0300);
			//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting, (result, obj) => {
			//	if (result == 1) PLAY.GetStagePlayCode((result) => { PLAY.GoReplay(m_Stage, _info); }, m_StageContent, _info.Idx, _info.Week, _info.Pos);
			//}, tdata, m_StageContent, _info.Week, _info.Pos);
		}
	}
	public void Click_Refresh() {
		if (TUTO.IsTutoPlay()) return;
		TShopTable tdata = TDATA.GetShopTable(BaseValue.REPLAY_REFRESH_SHOP_IDX);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(1026)
			, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						USERINFO.ITEM_BUY(BaseValue.REPLAY_REFRESH_SHOP_IDX, 1, (res) => {
							// REPLAY_REFRESH_SHOP_IDX의 구매의경우 팝업만 이용 res는 무조건 null 이발생함
							WEB.SEND_REQ_RESET_REPLAY((res2) =>
							{
								if (res2.IsSuccess())
								{
									m_SUI.Anim.SetTrigger("Refresh");
									SetUI();
								}
							}, m_StageContent);
						});
					}
					else {
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
				}
			}, tdata.m_PriceType, tdata.m_PriceIdx, tdata.GetPrice(), false);
	}
	public override void Close(int Result = 0) {
		if (TUTO.IsTuto(TutoKind.Replay, (int)TutoType_Replay.Focus_CloseBtn)) TUTO.Next();
		base.Close(Result);
	}
}
