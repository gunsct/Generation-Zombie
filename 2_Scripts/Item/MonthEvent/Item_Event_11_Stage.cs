using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Event_11_Stage : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI ItemCnts;
		public Item_RecommendGoodsBanner_Event RecommendGoods;
		public Transform Bucket;
		public Transform StgElement;
		public ScrollRect Scroll;
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Event;
	FAEventData_GrowUP m_GrowUp;
	int m_NowPos;

	List<FAEventData_Stage> m_StageInfos;

	IEnumerator m_ElementCor;

	private void OnDisable() {
		if (m_ElementCor != null) StopCoroutine(m_ElementCor);
	}

	public void SetData(MyFAEvent _event) {
		DLGTINFO?.f_RFShellUI.Invoke(USERINFO.m_Energy.Cnt);
		DLGTINFO?.f_RFCashUI.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		m_SUI.ItemCnts.text = USERINFO.GetItemCount(BaseValue.EVENT_11_ITEMIDX).ToString();

		m_Event = _event;


		/// <para> GrowUP => 0 : 진행 스테이지 레벨, 1 : 리롤 카운트, 2 : 칠면조 레벨, 3 : 칠면조 Exp </para>
		m_NowPos = (int)m_Event.Values[0];
		m_GrowUp = (FAEventData_GrowUP)m_Event.RealData;
		m_SUI.RecommendGoods.SetData(true, m_GrowUp.ShopItems);
		m_StageInfos = m_GrowUp.StageInfos;

		UTILE.Load_Prefab_List(m_StageInfos.Count, m_SUI.Bucket, m_SUI.StgElement);
		for (int i = 0; i < m_StageInfos.Count; i++) {
			Item_Event_11_Stage_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_11_Stage_Element>();
			element.SetData(m_StageInfos[i].LV, TDATA.GetStageTable(m_StageInfos[i].Idx), m_StageInfos[i].Limit, GoDeckSetting, m_NowPos);
		}
		int startpos = m_NowPos;
		if (STAGEINFO.m_PlayType == StagePlayType.Event) {
			startpos = STAGEINFO.m_LV;
		}
		else {
			startpos += m_SUI.Bucket.GetChild(startpos).GetComponent<Item_Event_11_Stage_Element>().IS_Lock ? -1 : 0;
		}
		SetScroll(startpos);

		m_SUI.ItemCnts.text = USERINFO.GetItemCount(BaseValue.EVENT_11_ITEMIDX).ToString();

		if (m_ElementCor != null) StopCoroutine(m_ElementCor);
		m_ElementCor = IE_ElementAction();
		StartCoroutine(m_ElementCor);
	}
	IEnumerator IE_ElementAction() {
		for(int i = 0;i< m_StageInfos.Count; i++) {
			yield return new WaitForSeconds(0.1f);
			m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_11_Stage_Element>().SetAnim("Start");
		}
	}
	public void Click_RecommendCharacter() {
		if (IS_EvtEnd()) return;
		PlayEffSound(SND_IDX.SFX_3050);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Event_10_RecomSrv, null, m_GrowUp.StageCharReward);
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
		m_SUI.Scroll.horizontalNormalizedPosition = (float)(_pos - 1) / (float)m_StageInfos.Count;
	}
	bool IS_EvtEnd() {
		if (m_Event.GetRemainEndTime() <= 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2036));
			return true;
		}
		else return false;
	}
}
