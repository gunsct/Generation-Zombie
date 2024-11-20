using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;
using Newtonsoft.Json;

[System.Serializable] public class DicPassBtn : SerializableDictionary<Item_Store_Tab_Pass.BtnName, Item_Store_Tab_Pass.SActiveBtn> { }
public class Item_Store_Tab_Pass : Item_Store_Tab_Base
{
	public class ViewInfo
	{
		public long Time = 0;
		public int LV = 1;
		public bool IsVIP = false;
		[JsonIgnore] public MissionData Mission;
		[JsonIgnore] public TMissionTable TData;
	}
#pragma warning disable 0649
	public enum BtnName
	{
		AllRecieve = 0,
		BUY_Pass
	}

	public enum AniName
	{
		Normal = 0,
		LvUp,
		End
	}
	[Serializable]
	public struct SActiveBtn
	{
		public GameObject[] Active;
	}
	[Serializable]
	public struct SMissionUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Cnt;
		public Slider Gauge;

		public ScrollRect Scroll;
		public RectTransform ScrollMaxSize;
		public RectTransform Prefab;
		public Transform ContinuePrefab;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Time;
		public SMissionUI Mission;

		public DicPassBtn Btns;
		public GameObject AllRecieveCntGroup;
		public TextMeshProUGUI AllRecieveCntTxt;
	}

	[SerializeField] SUI m_sUI;
	RES_SHOP_ITEM_INFO Pass;
	List<MissionData> Missions;
	List<Item_Pass_Element> Items = new List<Item_Pass_Element>();
	ViewInfo Befor, Now = new ViewInfo();
	int MaxLV;
	int[] m_IdleRewardCnt = new int[2];
	bool IsBuy;
	bool IsAction;
#pragma warning restore 0649
	public override void SetData(Action CB)
	{
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
#if NOT_USE_NET
#else
		WEB.SEND_REQ_MISSIONINFO((res) =>
		{
			CheckMission();
			SetUI();
			InitScroll();
			CB?.Invoke();
			AniName ani = AniName.Normal;
			if (Befor.IsVIP == Now.IsVIP)
			{
				if ((Befor.Time == 0 || Befor.LV < Now.LV) && GetViewMyLV() > 0){
					ani = AniName.LvUp;
				}
			}
			else if (Now.IsVIP) StartCoroutine(VIPOpenCheck());
			StartAnimation(ani);
		}, MissionMode.Pass);
#endif
	}


	void StartAnimation(AniName Ani)
	{
		string name = Ani.ToString();
		if(Ani == AniName.LvUp) {
			DelayPlayFXSND(1.5f, SND_IDX.SFX_0141);
		}
		m_sUI.Ani.ResetTrigger(name);
		m_sUI.Ani.SetTrigger(name);
	}

	void CheckMission(bool Reset = false) {
		Pass = USERINFO.m_ShopInfo.PassInfo[0];
		IsBuy = USERINFO.m_ShopInfo.IsPassBuy();
		m_IdleRewardCnt[0] = m_IdleRewardCnt[1] = 0;

		Missions = USERINFO.m_Mission.Get_Missions(MissionMode.Pass, Pass.Idx);
		Missions.Sort((befor, after) => TDATA.GetMissionTable(befor.Idx).m_LinkIdx.CompareTo(TDATA.GetMissionTable(after.Idx).m_LinkIdx));

		Befor = JsonConvert.DeserializeObject<ViewInfo>(PlayerPrefs.GetString($"SeasonPass_{USERINFO.m_UID}", JsonConvert.SerializeObject(new ViewInfo())));
		if (Befor.Time < Pass.Times[0]) {
			Befor.Time = 0;
			Befor.LV = 1;
			Befor.IsVIP = false;
		}
		Now = new ViewInfo();
		Now.Time = (long)UTILE.Get_ServerTime_Milli();
		// 달성한 레벨 체크
		Now.LV = 1;
		Now.IsVIP = IsBuy;
		for (int i = 0; i < Missions.Count; i++) {
			var mission = Missions[i];
			var tdata = TDATA.GetMissionTable(mission.Idx);
			Now.Mission = mission;
			Now.TData = tdata;
			Now.LV = tdata.m_LinkIdx;
			for (int j = 0; j < mission.State.Length; j++) {
				if (mission.IS_Complete() && mission.State[j] == RewardState.Idle) {
					if(j == 0) m_IdleRewardCnt[j]++;
					else if (j == 1 && Now.IsVIP) m_IdleRewardCnt[j]++;
				}
			}		
			if (!mission.IS_Complete()) break;
		}
		if (Reset) Befor = Now;

		PlayerPrefs.SetString($"SeasonPass_{USERINFO.m_UID}", JsonConvert.SerializeObject(Now));
		PlayerPrefs.Save();
	}

	void SetBtn(BtnName btn, bool Active)
	{
		for(int i = 0; i < m_sUI.Btns[btn].Active.Length; i++) m_sUI.Btns[btn].Active[i].SetActive(Active);
		if(btn == BtnName.AllRecieve && Active) {
			int rewardcnt = m_IdleRewardCnt[0] + m_IdleRewardCnt[1];
			m_sUI.AllRecieveCntGroup.SetActive(rewardcnt > 0);
			if (m_sUI.AllRecieveCntGroup.activeSelf) m_sUI.AllRecieveCntTxt.text = rewardcnt.ToString();
		}
	}

	public int GetViewMyLV()
	{
		return Now.Mission.IS_Complete() && Now.LV == MaxLV ? Now.LV : Now.LV - 1;
	}

	public override void SetUI()
	{
		ResetUI(true);
	}

	void ResetUI(bool Start)
	{
		TimeUI();
		MaxLV = 0;
		// 미션 리스트
		int ItemMax = Items.Count;
		for (int i = Missions.Count - 1, offset = 0; i > -1; i--, offset++)
		{
			Item_Pass_Element obj = null;
			if (offset >= ItemMax)
			{
				obj = Utile_Class.Instantiate(m_sUI.Mission.Prefab.gameObject, m_sUI.Mission.Scroll.content).GetComponent<Item_Pass_Element>();
				Items.Add(obj);
				ItemMax++;
			}
			else obj = Items[offset];

			MissionData befor = i > 0 ? Missions[i - 1] : null;
			var tdata = TDATA.GetMissionTable(Missions[i].Idx);
			obj.SetData(befor, Missions[i], IsBuy, Missions.Count, ItemGet, OnLVUP);
			if (Start)
			{
				// 이전 레벨 전까지는 바로 셋팅
				obj.SetState(tdata.m_LinkIdx > Now.LV ? Item_Pass_Element.AniName.NotOpen : tdata.m_LinkIdx < Now.LV ? Item_Pass_Element.AniName.Opened : Item_Pass_Element.AniName.Now);
				// 잠김 상태셋팅
				obj.SetUnLock(tdata.m_LinkIdx > Befor.LV);
			}

			if (tdata.m_LinkIdx > MaxLV) MaxLV = tdata.m_LinkIdx;
		}
		m_sUI.Mission.ContinuePrefab.SetAsLastSibling();

		if (Missions.Count < ItemMax)
		{
			for (; ItemMax > Missions.Count; ItemMax--) GameObject.Destroy(Items[ItemMax - 1]);
		}

		SetBtn(BtnName.BUY_Pass, !IsBuy);
		SetBtn(BtnName.AllRecieve, m_IdleRewardCnt[0] + m_IdleRewardCnt[1] > 0);

		// 레벨 정보
		m_sUI.LV.text = GetViewMyLV().ToString();

		// 개수는 누적임(0부터 시작으로 보여주려면 주석된것으로 변경해야됨
		//var befor = Missions.Find(o => TDATA.GetMissionTable(o.Idx).m_LinkIdx == Now.LV - 1);
		//int min = befor == null ? 0 : TDATA.GetMissionTable(befor.Idx).m_Cnt;
		int max = Now.TData.m_Check[0].m_Cnt;// Now.TData.m_Cnt - min;
		int now = Now.Mission.GetCnt(0);// Now.Mission.Cnt - min;
		m_sUI.Mission.Gauge.value = max == 0 ? 1 : (float)now / (float)max;
		m_sUI.Mission.Title.text = Now.Mission.m_TData.GetName();
		m_sUI.Mission.Cnt.text = string.Format("{0}/{1}", now, max);
	}

	void InitScroll()
	{
		Canvas.ForceUpdateCanvases();
		m_sUI.Mission.Scroll.content.anchoredPosition = new Vector2(0, GetScrollPosition(Befor.LV));
	}

	float GetScrollPosition(int LV)
	{
		LV--;
		float y = m_sUI.Mission.Prefab.rect.height * LV - (m_sUI.Mission.ScrollMaxSize.rect.height - m_sUI.Mission.Prefab.rect.height) * 0.5f;
		float h = m_sUI.Mission.Scroll.content.rect.height - m_sUI.Mission.ScrollMaxSize.rect.height;

		// y값이 음수기때문에 1에서 빼줘야 자신의 위치
		return Mathf.Clamp(y, 0, h);
	}

	IEnumerator VIPOpenCheck()
	{
		yield return USERINFO.PassVIPOpenAction();
		if (Befor.Time == 0 || Befor.LV < Now.LV) {
			StartAnimation(AniName.LvUp);
			PlayEffSound(SND_IDX.SFX_0143);
		}
		else {
			m_sUI.Mission.Scroll.content.anchoredPosition = new Vector2(0, GetScrollPosition(1));
			StartCoroutine(Element_Action(1, Now.LV));
		}
		yield return new WaitForSeconds(0.05f);
	}

	public void LvUPAniEnd()
	{
		if(Befor.LV != Now.LV) StartCoroutine(Element_Action(Befor.LV, Now.LV));
	}
	IEnumerator Element_Action(int Start, int End)
	{
		float time = 0.1f;
		// 스크롤 이동
		iTween.ValueTo(gameObject, iTween.Hash("from", m_sUI.Mission.Scroll.content.anchoredPosition.y, "to", GetScrollPosition(End), "delay", 0.3f, "time", (End - Start) * time, "onupdate", "MoveScroll", "easetype", "linear"));

		for (int i = Start, offset = Items.Count - i; i <= End; i++, offset--)
		{
			Items[offset].SetUnLock(false, i == End);
			PLAY.PlayEffSound(SND_IDX.SFX_0142);
			yield return new WaitForSeconds(time);
		}
	}

	void MoveScroll(float _amount)
	{
		m_sUI.Mission.Scroll.content.anchoredPosition = new Vector2(0, _amount);
	}

	void MoveNext(MissionData mission)
	{
		float h = m_sUI.Mission.Scroll.content.rect.height - m_sUI.Mission.ScrollMaxSize.rect.height;
		//Vector2 position = m_sUI.Mission.Scroll.content.anchoredPosition;
		//position.y += m_sUI.Mission.Prefab.rect.height;

		//// y값이 음수기때문에 1에서 빼줘야 자신의 위치
		////position.y = Mathf.Clamp(position.y, 0, h);
		//iTween.ValueTo(gameObject, iTween.Hash("from", m_sUI.Mission.Scroll.content.anchoredPosition.y, "to", Mathf.Clamp(position.y, 0, h), "delay", 0.2f, "time", 0.2f, "onupdate", "MoveScroll", "easetype", "linear"));
		////m_sUI.Mission.Scroll.content.anchoredPosition = position;

		// 다음 미션 아이템 찾기
		var next = Items.Find(o => o.GetLV() == mission.m_TData.m_LinkIdx + 1);
		if (next == null) return;
		// 다음 미션이 스크롤 중앙으로 오게 하기
		float hh = m_sUI.Mission.ScrollMaxSize.rect.height * 0.5f;
		RectTransform rtf = (RectTransform)next.transform;
		float y = Mathf.Abs(rtf.anchoredPosition.y) - hh + 110;

		iTween.ValueTo(gameObject, iTween.Hash("from", m_sUI.Mission.Scroll.content.anchoredPosition.y, "to", Mathf.Clamp(y, 0, h), "delay", 0.2f, "time", 0.2f, "onupdate", "MoveScroll", "easetype", "linear"));
	}

	void TimeUI()
	{
		if (Pass == null) return;
		if (Now == null) return;
		long time = (long)((Pass.Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001f);
		m_sUI.Time.text = TimeSpan.FromSeconds(time).ToString(TDATA.GetString(5028));
	}

	private void Update()
	{
		TimeUI(); 
	}

	void ItemGet(Item_Pass_Element item, MissionData mission, int pos)
	{
		if (IsAction) return;
		IsAction = true;
		// Msg_RewardGet_Center
		USERINFO.m_Mission.GetReward(mission, pos, (res) => {
			CheckMission(true);
			ResetUI(true);
			CheckAlarm(Shop.Tab.Pass);
			if (res != null)
			{
				MoveNext(mission);
				PLAY.PlayEffSound(SND_IDX.SFX_0143);
				List<RES_REWARD_BASE> rewards = res.GetRewards();
				if (rewards[0].Type == Res_RewardType.Char || (rewards[0].Type == Res_RewardType.Item && TDATA.GetItemTable(rewards[0].GetIdx()).m_Type == ItemType.CharaterPiece)) {
					var item = rewards.Select(o => {
						if (o.Type == Res_RewardType.Char) {
							RES_REWARD_CHAR info = (RES_REWARD_CHAR)o;
							return new OpenItem() { m_Type = OpenItemType.Character, m_Idx = info.Idx, m_Grade = new int[2] { info.Grade, info.Grade } };
						}
						else {
							RES_REWARD_ITEM info = (RES_REWARD_ITEM)o;
							return new OpenItem() { m_Type = OpenItemType.Item, m_Idx = info.Idx, m_Cnt = info.Cnt, m_Grade = new int[2] { info.Grade, info.Grade } };
						}
					}).ToList();
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharDraw, (result, obj) => {
						IsAction = false; ;
					}, item);
				}
				else {
					POPUP.Set_MsgBox(PopupName.Msg_RewardGet_Center, (result, obj) => {
						IsAction = false;
					}, rewards, Msg_RewardGet_Center.Action.Start_Once);
				}
			}
			else IsAction = false;
		});
	}
	public void OnBuyPass() {
		if (IsAction) return;
		IsAction = true;
		USERINFO.ITEM_BUY(Pass.Idx, 1, (res) =>
		{
			IsAction = false;
			if(res != null && res.IsSuccess())
			{
				CheckMission();
				ResetUI(false);
				PLAY.PlayEffSound(SND_IDX.SFX_0141);
				StartCoroutine(VIPOpenCheck());
			}
		}, true);
	}

	public void OnLVUP(int _lv)
	{
		if (_lv != Now.LV) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PassLvUp_Single, (result, obj) => {
			if (result == 1) {
				CheckMission();
				ResetUI(true);
				AniName ani = AniName.Normal;
				if (Befor.IsVIP == Now.IsVIP)
				{
					if ((Befor.Time == 0 || Befor.LV < Now.LV)) {
						ani = AniName.LvUp;
					}
				}
				else if (Now.IsVIP) StartCoroutine(VIPOpenCheck());
				StartAnimation(ani);
			}
		}, Missions.FindAll(o => TDATA.GetMissionTable(o.Idx).m_LinkIdx == _lv), Now.LV);
	}

	public void ClickAllRecieve() {
		if (m_IdleRewardCnt[0] + m_IdleRewardCnt[1] < 1) return;
		var missions = Missions.FindAll(o => o.IS_Complete() && (o.State[0] != RewardState.Get || o.State[1] != RewardState.Get));
		if (missions.Count < 1) return;
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess()) {
				WEB.SEND_REQ_MISSIONINFO((res2) => {
				});
				return;
			}
			var rewards = res.GetRewards();
			var chars = rewards.FindAll(o => o.Type == Res_RewardType.Char || (o.Type == Res_RewardType.Item && TDATA.GetItemTable(o.GetIdx()).m_Type == ItemType.CharaterPiece));
			rewards.RemoveAll(o=>chars.Contains(o));
			if (chars.Count > 0) {
				var item = chars.Select(o => {
					if (o.Type == Res_RewardType.Char) {
						RES_REWARD_CHAR info = (RES_REWARD_CHAR)o;
						return new OpenItem() { m_Type = OpenItemType.Character, m_Idx = info.Idx, m_Grade = new int[2] { info.Grade, info.Grade } };
					}
					else {
						RES_REWARD_ITEM info = (RES_REWARD_ITEM)o;
						return new OpenItem() { m_Type = OpenItemType.Item, m_Idx = info.Idx, m_Cnt = info.Cnt, m_Grade = new int[2] { info.Grade, info.Grade } };
					}
				}).ToList();
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharDraw, (result, obj) => {
					if (rewards.Count > 0) {
						MAIN.SetRewardList(new object[] { rewards }, () => {
							CheckMission();
							ResetUI(true);
							CheckAlarm(Shop.Tab.Pass);
						});
					}
					else {
						CheckMission();
						ResetUI(true);
						CheckAlarm(Shop.Tab.Pass);
					}
				}, item);
			}
			else {
				MAIN.SetRewardList(new object[] { rewards }, () => {
					CheckMission();
					ResetUI(true);
					CheckAlarm(Shop.Tab.Pass);
				});
			}
		}, missions, -1);
	}
	public void ClickAllRewardInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PassBuy, null, false);
	}
	public override void SetScrollState(bool Active) { }
}
