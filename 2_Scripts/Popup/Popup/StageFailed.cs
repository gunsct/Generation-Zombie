using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class StageFailed : PopupBase
{
	enum State
	{
		None,
		ReTry,
		GrowthWay,
		GoMain
	}
	[Serializable]
	public struct SRewardUI
	{
		public GameObject Active;
		public Animator Anim;
		public GameObject Prefab;
		public Transform LoadPanel;
		public ScrollRect Scroll;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Desc;
		public GameObject EnergyGroup;
		public TextMeshProUGUI Energy;
		public GameObject ActivePickPanel;
		public Item_CharPick_Percent[] PickChars;
		public SRewardUI Reward;
	}
	[SerializeField] SUI m_SUI;
	StageFailKind m_Kind;
	State m_State = State.None;
	IEnumerator m_Action;
	bool IsStartAction = false, IsReward = false;

	IEnumerator Start() {
		IsStartAction = true;
		m_SUI.Reward.Active.SetActive(false);
		bool check = true;
		List<RES_REWARD_BASE> Rewards = null;
		WEB.SEND_REQ_STAGE_FAIL_REWARD((res) => {
			if (res.IsSuccess()) { 
				if (res.Rewards != null && res.Rewards.Count > 0) Rewards = res.GetRewards();
			}
			check = false;
		}, USERINFO.m_Stage[STAGEINFO.m_StageContentType], STAGEINFO.m_Week, STAGEINFO.m_Pos, STAGEINFO.m_Idx);
		yield return new WaitForSeconds(120f / 60f);
		yield return new WaitWhile(() => check);

		if (Rewards != null && Rewards.Count > 0) yield return RewardAction(Rewards);

		IsStartAction = false;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Kind = (StageFailKind)aobjValue[0];

		base.SetData(pos, popup, cb, aobjValue);

		if (STAGEINFO.m_PlayType != StagePlayType.Stage) return;
	}

	public override void SetUI() {
		base.SetUI();

		m_SUI.EnergyGroup.SetActive(STAGEINFO.m_TStage.m_Energy > 0);
		m_SUI.Energy.text = string.Format("-{0}", STAGEINFO.m_TStage.m_Energy);

		//기존 stringuitable에 66~69는 꺼질듯?
		PlayEffSound(SND_IDX.SFX_0299);
		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) {
			switch (m_Kind) {
				case StageFailKind.Time:
					m_SUI.Desc.text = TDATA.GetString(1402);
					break;
				default://기본은 횟수 제한
					m_SUI.Desc.text = TDATA.GetString(1502);
					break;
			}
		}
		else {
			switch (m_Kind) {
				case StageFailKind.Men:
					m_SUI.Desc.text = TDATA.GetString(UTILE.Get_Random(1102, 1105));
					break;
				case StageFailKind.Hyg:
					m_SUI.Desc.text = TDATA.GetString(UTILE.Get_Random(1302, 1305));
					break;
				case StageFailKind.Sat:
					m_SUI.Desc.text = TDATA.GetString(UTILE.Get_Random(1202, 1205));
					break;
				case StageFailKind.Time:
					m_SUI.Desc.text = TDATA.GetString(1402);
					break;
				case StageFailKind.Turn:
					m_SUI.Desc.text = TDATA.GetString(1502);
					break;
				default://기본은 hp가 다되 죽는거로
					m_SUI.Desc.text = TDATA.GetString(UTILE.Get_Random(1002, 1005));
					break;
			}
		}

		UserPickCharInfo pickinfo = STAGEINFO.GetClearUserPickInfo();
		if (pickinfo == null || pickinfo.Chars.Count < 1)
		{
			m_SUI.ActivePickPanel.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.ActivePickPanel.gameObject.SetActive(true);
			for (int i = 0; i < m_SUI.PickChars.Length; i++)
			{
				if (i < pickinfo.Chars.Count)
				{
					UserPickChar charinfo = pickinfo.Chars[i];
					m_SUI.PickChars[i].gameObject.SetActive(true);
					m_SUI.PickChars[i].SetData(i + 1, charinfo.Idx, ((double)charinfo.Cnt / (double)pickinfo.Total) * 100f);
				} 
				else m_SUI.PickChars[i].gameObject.SetActive(false);
			}
		}
	}

	IEnumerator RewardAction(List<RES_REWARD_BASE> rewards)
	{
		IsReward = true;
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.Reward.LoadPanel, m_SUI.Reward.Prefab.transform);
		Dictionary<Res_RewardType, RectTransform> checkpos = new Dictionary<Res_RewardType, RectTransform>();
		for (int i = 0; i < rewards.Count; i++)
		{
			RES_REWARD_BASE rew = rewards[i];
			Item_RewardList_Item item = m_SUI.Reward.LoadPanel.GetChild(i).GetComponent<Item_RewardList_Item>();
			item.SetData(rew, IsStartAniControll:true);
			item.ShowItem(false);
			switch (rew.Type)
			{
			case Res_RewardType.Money: case Res_RewardType.Exp: case Res_RewardType.Cash:
				if (checkpos.ContainsKey(rew.Type)) break;
				checkpos.Add(rew.Type, (RectTransform)item.transform);
				break;
			}
			//item.gameObject.SetActive(false);
		}
		m_SUI.Reward.Active.SetActive(true);
		yield return new WaitForSeconds(30f / 60f);
		bool ActiveRewardAction = checkpos.Count > 0;

		RewardAssetAni pop = null;
		if(ActiveRewardAction)
			pop = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardAssetAni, (result, obj) => {
				ActiveRewardAction = false;
			}, checkpos).GetComponent<RewardAssetAni>();

		for (int i = 0; i < rewards.Count; i++)
		{
			PlayEffSound(SND_IDX.SFX_0313);
			RES_REWARD_BASE rew = rewards[i];
			Item_RewardList_Item item = m_SUI.Reward.LoadPanel.GetChild(i).GetComponent<Item_RewardList_Item>();
			item.StartAnim();
			if (pop != null) pop.StartAction(rew.Type);
			yield return new WaitForSeconds(0.25f);
		}

		if (pop != null) pop.Dealay_Close(1.75f);

		yield return new WaitWhile(() => ActiveRewardAction);
	}

	public void ClickRewardEnd()
	{
		if (!IsReward) return;
		IsReward = false;
		m_SUI.Reward.Anim.SetTrigger("End");
		StartCoroutine(AniEndCheck(m_SUI.Reward.Anim, () =>
		{
			m_SUI.Reward.Active.SetActive(false);
		}));
	}

	IEnumerator AniEndCheck(Animator ani, Action CB)
	{
		yield return Utile_Class.CheckAniPlay(ani);
		CB?.Invoke();
	}

	public void ClickRestart()
	{
		if (IsStartAction) return;
		if (m_Action != null) return;
		if (m_State != State.None) return;
		m_State = State.ReTry;
		GameObject obh = POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(STAGEINFO.m_TStage.m_Energy > 0 ? 190 : 786), (result, obj) => {
			if (result == 1) {
				if (STAGEINFO.m_TStage.m_Energy > 0 && USERINFO.m_Energy.Cnt < STAGEINFO.m_TStage.m_Energy) {
					POPUP.StartLackPop(BaseValue.ENERGY_IDX, false, (res) => {
						if (res == EResultCode.SUCCESS) {
							STAGEINFO.StageReset((result) => {
								if (result == EResultCode.SUCCESS) Close(1);
							});
						}
					}, TDATA.GetString(782));
					m_State = State.None;
					return;
				}
				STAGEINFO.StageReset((result) => {
					if (result == EResultCode.SUCCESS) Close(1);
				});
			}
			else {
				m_State = State.None;
			}
		}, PriceType.Energy, BaseValue.ENERGY_IDX, STAGEINFO.m_TStage.m_Energy, false).gameObject;
	}
	public void ClickGrowthWay()
	{
		if (IsStartAction) return;
		if (m_Action != null) return;
		if (m_State != State.None) return;
		m_State = State.GrowthWay;
		PlayerPrefs.SetInt($"GrowthWay_{USERINFO.m_UID}", 1);
		PlayerPrefs.Save();
		Close(0);
	}
	public void ClickGoMain()
	{
		if (IsStartAction) return;
		if (m_Action != null) return;
		if (m_State != State.None) return;
		m_State = State.GoMain;
		Close(0);
	}
	public override void Close(int Result = 0)
	{
		if (IsStartAction) return;
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
