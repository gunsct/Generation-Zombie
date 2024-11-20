using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class Stage_StartReward : PopupBase
{
	[Serializable]
	public class DiffNPC
	{
		/// <summary> 대사 인덱스 string - ETC </summary>
		public List<int> m_Speech = new List<int>();
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_Reward_Card[] Cards;
		public RectTransform Block;
		public GameObject BGFX;
		public GameObject Panel;
		public TextMeshPro Speech;
		public GameObject[] DiffBGFX;
		public Transform BGGroup;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] List<DiffNPC> m_DiffNPC = new List<DiffNPC>();
	[SerializeField] List<int> m_SPSpeech = new List<int>();
	Dictionary<Item_Reward_Card, TSelectDropGroupTable> m_Rewards = new Dictionary<Item_Reward_Card, TSelectDropGroupTable>();
	bool m_CanSelect;
	int m_Pos;
	bool Is_Auto;
	private void Awake() {
		for(int i = 0; i < m_SUI.DiffBGFX.Length; i++) {
			m_SUI.DiffBGFX[i].SetActive(false);
		}
	}
	public GameObject GetPanel { get { return m_SUI.Panel; } }

	private void OnDisable()
	{
		// iOS 17이상부터 캔버스에 3D 렌더러가 올라갔을때 텍스트 문제 생김
		var texts = new List<TextMeshPro>(GetComponentsInChildren<TextMeshPro>(true));
		foreach(var alpha in texts)
		{
			Utile_Class.TextMeshProAlphaChange(alpha, 1f);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		int sidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
		int diffidx = BaseValue.GetDiffStgIdx(sidx);// USERINFO.GetDifficulty();
		int prop = UTILE.Get_Random(0, 100);
		DiffNPC diffnpc = m_DiffNPC[diffidx];
		string spc = TDATA.GetString(ToolData.StringTalbe.Etc, diffnpc.m_Speech[UTILE.Get_Random(0, diffnpc.m_Speech.Count)]);
		switch (diffidx) {
			case 0:
				if (sidx < 401)
					m_SUI.Anim.SetTrigger("StgNormal");
				else {
					m_SUI.Anim.SetTrigger(prop > 95 ? "StgNormal_SP" : "StgNormal");
					if(prop > 95) spc = TDATA.GetString(ToolData.StringTalbe.Etc, m_SPSpeech[UTILE.Get_Random(0, m_SPSpeech.Count)]);
				}
				break;

			case 1: m_SUI.Anim.SetTrigger("StgHard"); break;
			case 2: m_SUI.Anim.SetTrigger("StgNightmare"); break;
		}
		m_SUI.Speech.text = spc;

		Vector3 panelpos = Utile_Class.GetWorldPosition(Vector2.zero, -10f);
		panelpos.z = 0;
		transform.position = panelpos;
		m_SUI.Block.localPosition = -transform.localPosition;
		//스테이지가 메인일떄 사이즈랑 위치
		float diff = 0;
		float h = 2560 - diff;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			diff = 230 + 50;
			h = 2560 - diff;
		}
		m_SUI.Block.sizeDelta = new Vector2(m_SUI.Block.sizeDelta.x, h);
		m_SUI.Block.localPosition += new Vector3(0f, diff * 0.5f, 0f);

		RefreshResolutin();

		m_SUI.BGFX.GetComponent<Animator>().SetTrigger("Start");

		m_SUI.DiffBGFX[USERINFO.GetDifficulty()].SetActive(true);

		int rewardgid = (int)aobjValue[0];
		if (aobjValue.Length > 1) m_Pos = (int)aobjValue[1];
		if (aobjValue.Length > 2) Is_Auto = (bool)aobjValue[2];

		StartCoroutine(IE_SetCard(rewardgid));

		PlayEffSound(SND_IDX.SFX_0320);
	}

	void RefreshResolutin() {
		transform.position = Vector3.zero;
		Vector3 scale = Vector3.one / Canvas_Controller.SCALE;
		scale.z = 1;
		transform.localScale = scale;
		m_SUI.BGGroup.localScale = Vector3.one * Mathf.Max(1f, Canvas_Controller.ASPECT / ((float)Screen.width / (float)Screen.height));
	}
	private void Update() {
		RefreshResolutin();

		bool cantouch = false;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) cantouch = STAGE.TouchCheck();
		else if (MAIN.IS_State(MainState.TOWER) && POPUP.GetMainUI().m_Popup == PopupName.Stage) cantouch = TOWER.TouchCheck();
		else if (MAIN.IS_State(MainState.PVP) && POPUP.GetMainUI().m_Popup == PopupName.PVP) cantouch = PVP.TouchCheck();

		if (cantouch && m_CanSelect) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hit = Physics.RaycastAll(ray, Camera.main.farClipPlane);
			for (int i = 0; i < hit.Length; i++) {
				GameObject hitobj = hit[i].transform.gameObject;
				if (!hitobj.activeSelf) continue;
				Item_Reward_Card hitcard = hitobj.GetComponent<Item_Reward_Card>();
				if (hitcard != null) {
					m_CanSelect = false;
					StartCoroutine(SelectReward(hitcard));
				}
			}
		}
	}
	IEnumerator IE_SetCard(int _gid) {
		List<TSelectDropGroupTable> picktables = new List<TSelectDropGroupTable>();
		for (int i = 0; i < 3; i++) {
			TSelectDropGroupTable table = TDATA.GetRandSelectDropGroupTable(_gid, picktables);
			m_SUI.Cards[i].SetData(new StageCardInfo(table.m_Val), StageRewardState.Normal);
			m_Rewards.Add(m_SUI.Cards[i], table);
			picktables.Add(table);
		}

		PlayEffSound(SND_IDX.SFX_0210);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 50f / 92f));

		//if (TUTO.IsTuto(TutoKind.Stage_201, (int)TutoType_Stage_201.StageStart_Loading)) {
		//	TUTO.Next();
		//	yield return new WaitWhile(() => !TUTO.IsEndTuto(TutoKind.Stage_201, (int)TutoType_Stage_201.DL_1161));
		//}
		if (Is_Auto) {
			yield return new WaitForSeconds(1f);
			StartCoroutine(SelectReward(m_SUI.Cards[UTILE.Get_Random(0, 3)]));
		}
		else m_CanSelect = true;
	}
	IEnumerator SelectReward(Item_Reward_Card _card) {
		PlayEffSound(SND_IDX.SFX_0321);
		TSelectDropGroupTable droptable = m_Rewards[_card];
		TStageCardTable cardtable = TDATA.GetStageCardTable(droptable.m_Val);

		m_SUI.BGFX.GetComponent<Animator>().SetTrigger("End");

		int pos = 0;
		for(int i = 0; i < m_Rewards.Count; i++) {
			if(m_Rewards.ElementAt(i).Key == _card) {
				pos = i;
				break;
			}
		}
		switch (pos) {
			case 0: m_SUI.Anim.SetTrigger(cardtable.m_Type == StageCardType.Gamble ? "Use_L" : (m_Pos == 0 ? "End_L" : "End_L")); break;
			case 1: m_SUI.Anim.SetTrigger(cardtable.m_Type == StageCardType.Gamble ? "Use_C" : (m_Pos == 0 ? "End_C" : "End_C")); break;
			case 2: m_SUI.Anim.SetTrigger(cardtable.m_Type == StageCardType.Gamble ? "Use_R" : (m_Pos == 0 ? "End_R" : "End_R")); break;
		}

		float value;
		switch (cardtable.m_Type) {
			/// <summary> HP 회복 </summary>
			case StageCardType.RecoveryHp:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.HP, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.HP, Mathf.RoundToInt(value));
				break;
			case StageCardType.RecoveryHpPer:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				break;
			/// <summary> 포만감 회복 </summary>
			case StageCardType.RecoverySat:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Sat, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				break;
			/// <summary> 정신력 회복 </summary>
			case StageCardType.RecoveryMen:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Men, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Men, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Men, Mathf.RoundToInt(value));
				break;
			/// <summary> 청결도 회복 </summary>
			case StageCardType.RecoveryHyg:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Hyg, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Hyg, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Hyg, Mathf.RoundToInt(value));
				break;
			/// <summary> 포만감 회복 </summary>
			case StageCardType.PerRecoverySat:
				value = STAGE_USERINFO.GetMaxStat(StatType.Sat) * cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Sat, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				break;
			/// <summary> 정신력 회복 </summary>
			case StageCardType.PerRecoveryMen:
				value = STAGE_USERINFO.GetMaxStat(StatType.Men) * cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Men, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Men, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Men, Mathf.RoundToInt(value));
				break;
			/// <summary> 청결도 회복 </summary>
			case StageCardType.PerRecoveryHyg:
				value = STAGE_USERINFO.GetMaxStat(StatType.Hyg) * cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(_card.transform, StatType.Hyg, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Hyg, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.AddStat(StatType.Hyg, Mathf.RoundToInt(value));
				break;
			case StageCardType.RecoveryAP:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_RecoveryAP(Mathf.RoundToInt(value));
				break;
			case StageCardType.LimitTurnUp:
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_LimitTurnUp(Mathf.RoundToInt(value));
				break;
			case StageCardType.AddRerollCount:
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_AddRerollCount(cardtable.m_Value1);
				break;
		}

		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));

		if (cardtable.IS_BuffCard()) {
			if (cardtable.m_Type == StageCardType.Synergy) {
				value = cardtable.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) STAGE.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(value));
			}
			else {
				if (MAIN.IS_State(MainState.STAGE)) STAGE.SetBuff(EStageBuffKind.Stage, cardtable.m_Idx);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.SetBuff(EStageBuffKind.Stage, cardtable.m_Idx);
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.SetBuff(EStageBuffKind.Stage, cardtable.m_Idx);
				else if (MAIN.IS_State(MainState.PVP)) PVP.SetBuff((UserPos)m_Pos, cardtable.m_Idx);
				//레벨업은 스텟 리셋
				switch (cardtable.m_Type) {
					case StageCardType.LevelUp: 
						STAGE_USERINFO.StatReset(); 
						break;
					case StageCardType.ConDeadly:
						if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.SetAllEnemyChange(Mathf.RoundToInt(cardtable.m_Value1));
						break;
					case StageCardType.NoHpBar:
						if (MAIN.IS_State(MainState.STAGE)) POPUP.GetMainUI().GetComponent<Main_Stage>().GetHpObj.SetActive(!STAGE_USERINFO.ISBuff(StageCardType.NoHpBar));
						break;
				}
			}
		}
		switch (cardtable.m_Type) {
			case StageCardType.Material:
				STAGE?.AddMaterial((StageMaterialType)Mathf.RoundToInt(cardtable.m_Value1), Mathf.RoundToInt(cardtable.m_Value2)); break;
			case StageCardType.Sniping:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.Sniping, 1); break;
			case StageCardType.Shotgun:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.ShotGun, 1); break;
			case StageCardType.MachineGun:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.GatlingGun, 1); break;
			case StageCardType.AirStrike:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.AirStrike, 1); break;
			case StageCardType.ShockBomb:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.ShockBomb, 1); break;
			case StageCardType.Dynamite:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.Dynamite, 1); break;
			case StageCardType.Grenade:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.Grenade, 1); break;
			case StageCardType.LightStick:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.LightStick, 1); break;
			case StageCardType.FlashLight:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.FlashLight, 1); break;
			case StageCardType.StarShell:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.Flare, 1); break;
			case StageCardType.ThrowExtin:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.FireSpray, 1); break;
			case StageCardType.PowderExtin:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.FireExtinguisher, 1); break;
			case StageCardType.PowderBomb:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.PowderBomb, 1); break;
			case StageCardType.FireBomb:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.FireBomb, 1); break;
			case StageCardType.FireGun:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.FireGun, 1); break;
			case StageCardType.NapalmBomb:
				STAGE?.m_MainUI?.GetMakeUtileCard(StageMaterialType.NapalmBomb, 1); break;
			case StageCardType.Gamble:
				TGambleCardTable gambletable = TDATA.GetGambleCardTable(_card.m_Info.m_GambleIdx);
				float randprop = UTILE.Get_Random(0f, 1f); float? synergySD = STAGE_USERINFO.GetSynergeValue(JobType.Swindler, 1);
				if (synergySD != null) {
					randprop = Mathf.Clamp(randprop - (float)synergySD, 0f, 1f);
					STAGE_USERINFO.ActivateSynergy(JobType.Swindler);
					Utile_Class.DebugLog_Value("Swindler 도박 확률 증가");
				}
				GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Gamble, null, gambletable, randprop).gameObject;

				yield return new WaitWhile(() => popup != null);

				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				else if (MAIN.IS_State(MainState.TOWER)) yield return TOWER.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				else if (MAIN.IS_State(MainState.BATTLE)) BATTLE.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				break;
		}

		Close(0);
	}
}
