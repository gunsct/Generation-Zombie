using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Event_10_Stage : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI[] ItemCnts;
		public Item_RecommendGoodsBanner_Event RecommendGoods;
		public Item_Event_10_Stage_Element[] StgElements;
		public ScrollRect Scroll;
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Event;
	FAEventData_Stage_Minigame m_Minigame;
	int m_NowPos;

	List<FAEventData_Stage> m_StageInfos;
	public List<TStageTable> m_TDatas = new List<TStageTable>();

	public void SetData(MyFAEvent _event) {
		m_Event = _event;


		m_NowPos = (int)m_Event.Values[0];
		m_Minigame = (FAEventData_Stage_Minigame)m_Event.RealData;
		m_SUI.RecommendGoods.SetData(true, m_Minigame.ShopItems);
		m_StageInfos = m_Minigame.StageInfos;
		m_TDatas.Clear();

		for (int i = 0; i < m_SUI.StgElements.Length; i++) {
			if (i < m_StageInfos.Count) {
				m_TDatas.Add(TDATA.GetStageTable(m_StageInfos[i].Idx));
				m_SUI.StgElements[i].SetData(m_StageInfos[i].LV, m_TDatas[i], m_StageInfos[i].Limit, GoDeckSetting, m_NowPos);
			}
		}
		int startpos = m_NowPos;
		if (STAGEINFO.m_PlayType == StagePlayType.Event) {
			startpos = STAGEINFO.m_LV;
		}
		else {
			startpos += m_SUI.StgElements[startpos].IS_Lock ? -1 : 0;
		}
		SetScroll(startpos);

		for (int i = 0; i < m_SUI.ItemCnts.Length; i++) {
			m_SUI.ItemCnts[i].text = USERINFO.GetItemCount(BaseValue.EVENT_10_ITEMIDX[i]).ToString();
		}
	}
	public void Click_RecommendCharacter() {
		if (IS_EvtEnd()) return;
		PlayEffSound(SND_IDX.SFX_3050);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Event_10_RecomSrv, null, m_Minigame.StageCharReward);
	}
	public void GoDeckSetting(int _pos) {
		if (IS_EvtEnd()) return;

		PlayEffSound(SND_IDX.SFX_3002);

		m_NowPos = _pos;
		SetScroll(m_NowPos);
		FAEventData_Stage info = m_StageInfos[m_NowPos - 1];
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting, (result, obj) => {
			if (result == 1) {
				if (IS_EvtEnd()) return;
				PlayEffSound(SND_IDX.SFX_3003);
				PLAY.GetStagePlayCode((result) => {
					if (result != EResultCode.SUCCESS) {
						SetData(m_Event);
						return;
					}
					PLAY.GoEventState(m_Event, info.Idx, info.LV);
				}, StageContentType.Stage, info.Idx, DayOfWeek.Sunday, 0, m_Event.UID);
			}
		}, TDATA.GetStageTable(info.Idx), StageContentType.Stage, DayOfWeek.Sunday, 0, true);//info.LV
	}
	void SetScroll(int _pos) {
		m_SUI.Scroll.horizontalNormalizedPosition = (float)(_pos - 1) / (float)m_SUI.StgElements.Length;
	}
	bool IS_EvtEnd() {
		if (m_Event.GetRemainEndTime() <= 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2036));
			return true;
		}
		else return false;
	}
}
