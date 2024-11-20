using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using Coffee.UIEffects;

public class Item_DailyMission : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Timer;   //서버시간에 맞춰 초기화
		public Item_DailyMission_Element[] MissionElements;
		public TextMeshProUGUI ChangeMissionTxt;    //미션 변경 가능 횟수
		public GameObject[] ChangeMissionTxtObj;
		public Image ChangeMissionBG;
		public UIEffect ChangeMissionFX;
		public GameObject ChangeMissionMark;
		public RectTransform RefreshBtn;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] Vector3[] m_RefreshPos;
	List<MissionData> m_Quests = new List<MissionData>();
	MissionData m_SPQuest;
	int m_BuyCnt;
	NewNDailyMission m_Parent;

	private void Update() {

		m_SUI.Timer.text = UTILE.GetSecToTimeStr(0, 86400 - UTILE.Get_ServerTime() % 86400);
	}
	//public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
	//	base.SetData(pos, popup, cb, aobjValue);
	//}
	public void SetData(NewNDailyMission _parent) {
		m_Parent = _parent;
		SetUI();
	}

	public void SetRefeshBtnPosition(int pos)
	{
		m_SUI.RefreshBtn.anchoredPosition = m_RefreshPos[pos];
	}

	void SetUI() {
		m_Parent.CheckAlarm();
		//상점에서 새로고침 구매한 횟수 체크
		CheckRefreshBuy();

#if NOT_USE_NET
		//일퀘 하루 지난거 삭제(클라용)
		USERINFO.DailyQuestTimeCheck();
#endif
		MissionList();
		//미션이 00시 지나 없을 경우
		if (m_Quests.Count < 1) {
#if NOT_USE_NET
			List<TMissionTable> picktables = new List<TMissionTable>();
			TMissionTable sptable = new List<TMissionTable>(TDATA.GetAllMissionTable().Values).Find(o => o.m_Mode == MissionMode.DailyQuest && o.m_ModeGid == (int)UTILE.GetServerDayofWeek());
			picktables.Add(sptable);
			for(int i = 0; i < sptable.m_Check[0].m_Cnt; i++) {
				picktables.Add(PickMission(picktables));
			}
			for(int i = 0;i< picktables.Count; i++) {
				MissionData info = new MissionData(picktables[i].m_Idx);
				USERINFO.SetMission(info);
			}
			MissionList();
#endif
		}
		MissionData spm = m_Quests.Find(o => o.m_TData.m_Mode == MissionMode.DailyQuest);
		List<MissionData> nm = m_Quests.FindAll(o => o.m_TData.m_Mode == MissionMode.Day);
		MissionInfo.Sort(nm);
		//미션 엘리먼트 세팅
		for (int i = 0; i < m_SUI.MissionElements.Length; i++) {
			if (i > spm.m_TData.m_Check[0].m_Cnt) m_SUI.MissionElements[i].gameObject.SetActive(false);
			else {
				MissionData md = i == 0 ? spm : nm[i - 1];
				m_SUI.MissionElements[i].SetData(md, Click_GetReward, Click_GoQuest, Click_Refresh, m_BuyCnt);
			}
		}
	}

	void CheckRefreshBuy() {
		//미션 새로고침 횟수
		RES_SHOP_USER_BUY_INFO[] buyinfos = new RES_SHOP_USER_BUY_INFO[4];
		for (int i = 0; i < 4; i++) {
			buyinfos[i] = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.MISSIONRFRESH_SHOP_IDX[i] && UTILE.IsSameDay(o.UTime));
			if(buyinfos[i] != null && buyinfos[i].Cnt > 0) {
				m_BuyCnt = i + 1;
			}
		}

		if (m_BuyCnt < 4) {
			m_SUI.ChangeMissionTxt.text = string.Format(TDATA.GetString(739), 4 - m_BuyCnt);
			m_SUI.ChangeMissionMark.SetActive(false);
			m_SUI.ChangeMissionTxtObj[0].SetActive(true);
			m_SUI.ChangeMissionTxtObj[1].SetActive(true);
			m_SUI.ChangeMissionTxtObj[2].SetActive(false);
			m_SUI.ChangeMissionBG.color = Utile_Class.GetCodeColor("#FFFFFF");
			m_SUI.ChangeMissionFX.enabled = false;
		}
		else {
			m_SUI.ChangeMissionMark.SetActive(true);
			m_SUI.ChangeMissionTxtObj[0].SetActive(false);
			m_SUI.ChangeMissionTxtObj[1].SetActive(false);
			m_SUI.ChangeMissionTxtObj[2].SetActive(true);
			m_SUI.ChangeMissionBG.color = Utile_Class.GetCodeColor("#D95757 ");
			m_SUI.ChangeMissionFX.enabled = true;
		}
	}
	void MissionList() {
		//퀘스트 목록
		m_Quests.Clear();
		if (m_Quests == null) m_Quests = new List<MissionData>();
		List<MissionData> sps = USERINFO.m_Mission.Get_Missions(MissionMode.DailyQuest);
		if (sps != null && sps.Count > 0) m_SPQuest = sps[0];
		if (m_SPQuest != null) m_Quests.Add(m_SPQuest);
		m_Quests.AddRange(USERINFO.m_Mission.Get_Missions(MissionMode.Day));
	}
	/// <summary> 미션타입중 3가지 타입 뽑아서 그중 하나씩 총 3개 + 특별퀘 1개<returns></returns>
	TMissionTable PickMission(List<TMissionTable> _ignores) {
		TMissionTable table = null;
		List<MissionType> modes = new List<MissionType>();
		for (MissionType i = MissionType.StageClear;i < MissionType.Max; i++) {
			if (_ignores != null && _ignores?.Find(o => o.m_Check.Find(c => c.m_Type == i) != null) != null) continue;
			modes.Add(i);
		}
		MissionType sctmode = MissionType.None;
		//MissionMode mode = MissionMode.None;
		while(table == null) {
			sctmode = modes[UTILE.Get_Random(0, modes.Count)];
			table = TDATA.GetRandMissionTable(MissionMode.Day, sctmode);
		}

		return table;
	}
	/// <summary> 전체 완료 보상 받기 </summary>
	public void Click_GetSPReward() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, 5)) return;
		Click_GetReward(m_Quests.Find(o=>o.m_TData.m_Mode == MissionMode.DailyQuest));
	}
	/// <summary> 보상 받기 <summary>
	public void Click_GetReward(MissionData _info) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, 3)) return;
		m_Parent.Click_GetReward(_info, ()=> {
			SetUI();
		},()=> {
			SetUI();
		});
	}
	/// <summary> 해당 미션으로 바로 가기 </summary>
	public void Click_GoQuest(MissionData _info) {
		m_Parent.Click_GoQuest(_info);
	}
	
	public void Click_Refresh(Item_DailyMission_Element _element) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, 7)) return;
		if (m_BuyCnt > 3) return;
		//새로고침은 하루에 오로직 딱 한번!
#if NOT_USE_NET
		USERINFO.ITEM_BUY(BaseValue.MISSIONRFRESH_SHOP_IDX[m_BuyCnt], 1, (res) => {
			RES_SHOP_USER_BUY_INFO info = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.MISSIONRFRESH_SHOP_IDX[m_BuyCnt]);
			if (info == null) {
				USERINFO.m_ShopInfo.BUYs.Add(new RES_SHOP_USER_BUY_INFO() {
					Idx = BaseValue.MISSIONRFRESH_SHOP_IDX[m_BuyCnt],
					UTime = (long)UTILE.Get_ServerTime_Milli(),
					Cnt = 1
				});
			}
			else {
				info.UTime = (long)UTILE.Get_ServerTime_Milli();
				info.Cnt++;
			}
			List<TMissionTable> crnts = new List<TMissionTable>();
			for (int i = 0; i < m_Quests.Count; i++) {
				crnts.Add(m_Quests[i].m_TData);
			}
			USERINFO.m_Mission.DelData(_element.GetInfo);
			USERINFO.m_Mission.SetData(new MissionData(PickMission(crnts).m_Idx));
			MAIN.Save_UserInfo();
			SetUI();
			_element.SetAnim("Change");
		}, true, null, string.Format(TDATA.GetString(774), BaseValue.MISSIONRFRESH_SHOP_IDX.Length - m_BuyCnt));
#else
		TShopTable tdata = TDATA.GetShopTable(BaseValue.MISSIONRFRESH_SHOP_IDX[m_BuyCnt]);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, string.Format(TDATA.GetString(774), BaseValue.MISSIONRFRESH_SHOP_IDX.Length - m_BuyCnt), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					WEB.SEND_REQ_MISSION_RESET((res) => {
						PlayEffSound(SND_IDX.SFX_0012);
						PlayEffSound(SND_IDX.SFX_0110);
						SetUI();
						_element.SetAnim("Change");
					}, tdata.m_Idx, _element.GetInfo);
				}
				else {
					POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
				}
			}
		}, tdata.m_PriceType, BaseValue.DOLLAR_IDX, tdata.GetPrice(), false);
#endif
	}
}
