using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_MainMenu_StgMain : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Animator WindowAnim;
		public GameObject DiffGroup;
		public Image[] ChapterNums;
		public Animator GunBtnAnim;
		public GameObject StgElementPrefab;
		public Transform ElementBucket;
		public TextMeshProUGUI NeedEnergy;

		public GameObject DifficultyGroup;
		//public GameObject DiffRewardDecoGroup;
		//public Item_RewardList_Item RewardCard;
		public GameObject MarkBeware;
		public GameObject MarkDanger;

		//public Item_StgDiffBtn[] StgDiffBtns;
		public Image BG;
		public GameObject[] StartBtnFX;
		public GameObject SupplyBox;
		public GameObject Replay;
		public Image StartBlack;
	}
	[SerializeField]
	SUI m_SUI;
	Coroutine m_WindowCor;
	IEnumerator m_StagePageDragAction;
	IEnumerator m_DiffChangeAction;
	float m_StagePageMove;
	List<Item_StgMain_CptElement> m_StagePages = new List<Item_StgMain_CptElement>();
	int m_NowStgNum;
	public Animator GetAnim { get { return m_SUI.Anim; } }

	private void Awake() {
		m_SUI.StartBlack.color = new Color(0f, 0f, 0f, 1f);
	}
	public Item_StgDiffBtn GetDiffBtn(StageDifficultyType _diff) {
		//for(int i = 0; i < m_SUI.StgDiffBtns.Length; i++) {
		//	if (m_SUI.StgDiffBtns[i].m_DiffType == _diff) return m_SUI.StgDiffBtns[i];
		//}
		return null;
	}
	public GameObject GetReplayBtn() {
		return m_SUI.Replay;
	}

	private void OnEnable() {
		if (m_WindowCor != null) StopCoroutine(m_WindowCor);
		m_WindowCor = StartCoroutine(WindowLoop());
	}
	IEnumerator WindowLoop(List<int> _pos = null) {
		List<int> pos = _pos;
		if(pos == null || pos.Count < 1) pos = new List<int>() { 1, 2, 3, 4, 5 };
		int num = pos[UTILE.Get_Random(0, pos.Count)];
		pos.Remove(num);
		m_SUI.WindowAnim.SetTrigger(string.Format("Loop{0}", num));

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.WindowAnim));
		m_WindowCor = StartCoroutine(WindowLoop(pos));
	}
	public void SetData(int _idx, bool _diffchange = false) {
		int chapnum = _idx / 100;
		int chapnum10 = chapnum / 10;
		int chapnum1 = chapnum % 10;
		m_SUI.ChapterNums[0].gameObject.SetActive(chapnum10 > 0);
		m_SUI.ChapterNums[0].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum10), "png");
		m_SUI.ChapterNums[1].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum1), "png");
		m_SUI.ChapterNums[2].gameObject.SetActive(chapnum10 > 0);
		m_SUI.ChapterNums[2].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum10), "png");
		m_SUI.ChapterNums[3].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum1), "png");

		SetStagePage(_idx);
		m_NowStgNum = _idx % 100;

		DLGTINFO?.f_RFShellUI?.Invoke(USERINFO.m_Energy.Cnt); 
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);

		m_SUI.NeedEnergy.text = string.Format("{0}", TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx).m_Energy, USERINFO.GetDifficulty());

		//for (int i = 0; i < 2; i++) {
		//	m_SUI.StgDiffBtns[i].SetData(SetStageDifficulty);
		//}
		StartCoroutine(SetBGAnim());

		StartCoroutine(SetStageInfoAnim());

		m_SUI.SupplyBox.SetActive(USERINFO.GetStoreSupplyBoxCheck() && USERINFO.CheckContentUnLock(ContentType.Store) && TUTO.IsEndTuto(TutoKind.ShopSupplyBox));
		bool canreplay = false;
		int diff = USERINFO.GetDifficulty();
		if (diff == 0 && BaseValue.REPLAY_OPEN <= USERINFO.m_Stage[StageContentType.Stage].Idxs[diff].Idx) canreplay = true;
		else if (diff == 1 && BaseValue.REPLAY_HARD_OPEN <= USERINFO.m_Stage[StageContentType.Stage].Idxs[diff].Idx) canreplay = true;
		else if (diff == 2 && BaseValue.REPLAY_NIGHT_OPEN <= USERINFO.m_Stage[StageContentType.Stage].Idxs[diff].Idx) canreplay = true;
		m_SUI.Replay.SetActive(canreplay);

		//시작 버튼 상태
		LastStage(USERINFO.m_Stage[StageContentType.Stage].IS_LastStage());
	}

	IEnumerator SetBGAnim() {
		yield return new WaitForEndOfFrame();
		switch ((StageDifficultyType)USERINFO.GetDifficulty()) {
			case StageDifficultyType.Normal: m_SUI.Anim.SetTrigger("Normal"); break;
			case StageDifficultyType.Hard: m_SUI.Anim.SetTrigger("Hard"); break;
			case StageDifficultyType.Nightmare: m_SUI.Anim.SetTrigger("Nightmare"); break;
		}
	}
	public void SetStartBtnFx(bool _on) {
		for (int i = 0; i < m_SUI.StartBtnFX.Length; i++) {
			m_SUI.StartBtnFX[i].SetActive(_on);
		}
	}
	public void StagePageDrag() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_StagePageDrag)) return;
		if (!Utile_Class.IsPlayiTween(gameObject) && m_StagePageMove != 0f) {
			TW_ElementBucketPosX(Mathf.Clamp(m_SUI.ElementBucket.localPosition.x + (Input.mousePosition.x - m_StagePageMove), -190f * (m_StagePages.Count - 1), 0f));
			m_StagePageMove = Input.mousePosition.x;
		}
	}
	public void StagePageDragStart() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_StagePageDrag)) return;
		if (Utile_Class.IsPlayiTween(gameObject)) return;
		if (m_StagePageDragAction != null) {
			StopCoroutine(m_StagePageDragAction);
			m_StagePageDragAction = null;
		}
		if(!Utile_Class.IsPlayiTween(gameObject)) m_StagePageMove = Input.mousePosition.x;
	}
	public void StagePageDraglEnd() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_StagePageDrag)) return;
		if (m_StagePageDragAction != null) {
			StopCoroutine(m_StagePageDragAction);
		}
		m_StagePageDragAction = StagePageDragInit();
		StartCoroutine(m_StagePageDragAction);
		m_StagePageMove = 0f;
	}
	IEnumerator StagePageDragInit() {
		yield return new WaitForSeconds(3f);
		if (POPUP.IS_PopupUI()) {
			yield return new WaitWhile(() => POPUP.IS_PopupUI());
			yield return new WaitForSeconds(3f);
		}
		float x = -190f * ((USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx % 100) - 1);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.ElementBucket.localPosition.x, "to", x, "onupdate", "TW_ElementBucketPosX", "time", 0.5f, "name", "StagePageDrag"));
	}
	//스테이지 표시
	void SetStagePage(int _idx) {
		int chapter = _idx / 100;
		int stage = _idx % 100;//다음 스테이지 챕터가 바뀔때까지

		UserInfo.StageIdx userstgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		List<TStageTable> lists = TDATA.GetChapterStages(chapter, USERINFO.GetDifficulty());
		m_StagePages.Clear();
		UTILE.Load_Prefab_List(lists.Count, (RectTransform)m_SUI.ElementBucket, (RectTransform)m_SUI.StgElementPrefab.transform);
		for(int i = 0; i < lists.Count; i++)
		{
			TStageTable tdata = lists[i];
			Item_StgMain_CptElement element = m_SUI.ElementBucket.GetChild(i).GetComponent<Item_StgMain_CptElement>();
			element.transform.localPosition = new Vector3(190 * i, 0f, 0f);
			element.SetData(tdata.m_Idx, _idx, userstgidx.ChapterReward != 0);
			m_StagePages.Add(element);
		}
		TW_ElementBucketPosX(-190f * ((userstgidx.Idx % 100 == 1 || userstgidx.ChapterReward == 0 ? 0 : -1) + (_idx - 1) % 100));//

		TStageTable stageTable = TDATA.GetStageTable(_idx, USERINFO.GetDifficulty());
		m_SUI.BG.sprite = stageTable.GetChapterBG();
		//m_SUI.RewardCard.gameObject.SetActive(false);
		switch (stageTable.m_Difficulty)
		{
			case 0: // 기본
				m_SUI.DifficultyGroup.SetActive(false);
				m_SUI.MarkBeware.SetActive(false);
				m_SUI.MarkDanger.SetActive(false);
				//m_SUI.RewardCard.gameObject.SetActive(false);
				//m_SUI.DiffRewardDecoGroup.SetActive(false);
				break;
			case 1: // 어려움
				m_SUI.DifficultyGroup.SetActive(true);
				m_SUI.MarkBeware.SetActive(true);
				m_SUI.MarkDanger.SetActive(false);
				//m_SUI.RewardCard.gameObject.SetActive(stageTable.m_ClearReward.Count > 0);
				//m_SUI.DiffRewardDecoGroup.SetActive(stageTable.m_ClearReward.Count > 0);
				break;
			case 2: // 매우어려움
				m_SUI.DifficultyGroup.SetActive(true);
				m_SUI.MarkBeware.SetActive(false);
				m_SUI.MarkDanger.SetActive(true);
				//m_SUI.RewardCard.gameObject.SetActive(stageTable.m_ClearReward.Count > 0);
				//m_SUI.DiffRewardDecoGroup.SetActive(stageTable.m_ClearReward.Count > 0);
				break;
		}

		//if (stageTable.m_ClearReward.Count > 0)
		//{
		//	RES_REWARD_BASE res = new RES_REWARD_BASE();
		//	switch (stageTable.m_ClearReward[0].m_Kind) {
		//		case RewardKind.None:
		//		case RewardKind.Event:
		//			m_SUI.RewardCard.gameObject.SetActive(false);
		//			return;
		//		case RewardKind.Character:
		//			CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == stageTable.m_ClearReward[0].m_Idx);
		//			if (charinfo != null) {
		//				res = new RES_REWARD_ITEM() {
		//					Type = Res_RewardType.Item,
		//					Idx = charinfo.m_TData.m_PieceIdx,
		//					Cnt = TDATA.GetItemTable(charinfo.m_TData.m_PieceIdx).GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
		//					result_code = EResultCode.SUCCESS_REWARD_PIECE
		//				};
		//			}
		//			else {
		//				CharInfo charInfo = new CharInfo(stageTable.m_ClearReward[0].m_Idx);
		//				RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
		//				rchar.SetData(charInfo);
		//				res = rchar;
		//			}
		//			break;
		//		case RewardKind.Item:
		//			if (TDATA.GetItemTable(stageTable.m_ClearReward[0].m_Idx).m_Type == ItemType.RandomBox) {//박스는 바로 까서 주기
		//				TItemTable itemTable = TDATA.GetItemTable(stageTable.m_ClearReward[0].m_Idx);
		//				RES_REWARD_ITEM item = new RES_REWARD_ITEM();
		//				item.Type = Res_RewardType.Item;
		//				item.Idx = itemTable.m_Idx;
		//				item.Cnt = 1;
		//				res = item;
		//			}
		//			else {
		//				TItemTable tdata = TDATA.GetItemTable(stageTable.m_ClearReward[0].m_Idx);
		//				RES_REWARD_MONEY rmoney;
		//				RES_REWARD_ITEM ritem;
		//				switch (tdata.m_Type) {
		//					case ItemType.Dollar:
		//						rmoney = new RES_REWARD_MONEY();
		//						rmoney.Type = Res_RewardType.Money;
		//						rmoney.Befor = USERINFO.m_Money - stageTable.m_ClearReward[0].m_Count;
		//						rmoney.Now = USERINFO.m_Money;
		//						rmoney.Add = stageTable.m_ClearReward[0].m_Count;
		//						res = rmoney;
		//						break;
		//					case ItemType.Cash:
		//						rmoney = new RES_REWARD_MONEY();
		//						rmoney.Type = Res_RewardType.Cash;
		//						rmoney.Befor = USERINFO.m_Cash - stageTable.m_ClearReward[0].m_Count;
		//						rmoney.Now = USERINFO.m_Cash;
		//						rmoney.Add = stageTable.m_ClearReward[0].m_Count;
		//						res = rmoney;
		//						break;
		//					case ItemType.Energy:
		//						rmoney = new RES_REWARD_MONEY();
		//						rmoney.Type = Res_RewardType.Energy;
		//						rmoney.Befor = USERINFO.m_Energy.Cnt - stageTable.m_ClearReward[0].m_Count;
		//						rmoney.Now = USERINFO.m_Energy.Cnt;
		//						rmoney.Add = stageTable.m_ClearReward[0].m_Count;
		//						rmoney.STime = (long)USERINFO.m_Energy.STime;
		//						res = rmoney;
		//						break;
		//					case ItemType.InvenPlus:
		//						rmoney = new RES_REWARD_MONEY();
		//						rmoney.Type = Res_RewardType.Inven;
		//						rmoney.Befor = USERINFO.m_InvenSize - stageTable.m_ClearReward[0].m_Count;
		//						rmoney.Now = USERINFO.m_InvenSize;
		//						rmoney.Add = stageTable.m_ClearReward[0].m_Count;
		//						res = rmoney;
		//						break;
		//					default:
		//						ritem = new RES_REWARD_ITEM();
		//						ritem.Type = Res_RewardType.Item;
		//						ritem.Idx = stageTable.m_ClearReward[0].m_Idx;
		//						ritem.Cnt = stageTable.m_ClearReward[0].m_Count;
		//						res = ritem;
		//						break;
		//				}
		//				break;
		//			}
		//			break;
		//		case RewardKind.Zombie:
		//			ZombieInfo zombieInfo = new ZombieInfo(stageTable.m_ClearReward[0].m_Idx);
		//			RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
		//			zombie.UID = zombieInfo.m_UID;
		//			zombie.Idx = zombieInfo.m_Idx;
		//			zombie.Grade = zombieInfo.m_Grade;
		//			res = zombie;
		//			break;
		//		case RewardKind.DNA:
		//			DNAInfo dnaInfo = new DNAInfo(stageTable.m_ClearReward[0].m_Idx);
		//			RES_REWARD_DNA dna = new RES_REWARD_DNA();
		//			dna.UID = dnaInfo.m_UID;
		//			dna.Idx = dnaInfo.m_Idx;
		//			dna.Grade = dnaInfo.m_Grade;
		//			res = dna;
		//			break;
		//	}
		//	m_SUI.RewardCard.SetData(res);
		//}
	}
	public void SetStageChage(int _crntidx, bool _change = false) {
		if (_change) {
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.ElementBucket.localPosition.x, "to", m_SUI.ElementBucket.localPosition.x - 190f, "onupdate", "TW_ElementBucketPosX", "time", 1f));
		}
		bool recieve = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].ChapterReward == 0;

		int pos = _crntidx % 100 - 1;
		if(pos - 1 > -1) m_StagePages[pos - 1].SetAnim(recieve ? Item_StgMain_CptElement.State.RecieveReward : Item_StgMain_CptElement.State.Set);
		m_StagePages[pos].SetAnim(recieve ? Item_StgMain_CptElement.State.NowStg : Item_StgMain_CptElement.State.NotClear);
	}

	public void TW_ElementBucketPosX(float _amount) {
		m_SUI.ElementBucket.localPosition = new Vector3(_amount, m_SUI.ElementBucket.localPosition.y, m_SUI.ElementBucket.localPosition.z);
	}
	public void SetStagePagePos(float _amount) {
		TW_ElementBucketPosX(m_SUI.ElementBucket.localPosition.x + _amount);
	}
	public void SetChapterChange(int _idx) {
		SetData(_idx);
	}

	IEnumerator SetStageInfoAnim() {
		yield return new WaitForEndOfFrame();

		TStageTable table = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
		int diff = USERINFO.GetDifficulty();
		int gimmick = 0;//0:없음, 1:화재, 2:어둠, 3:공습

		if (table.Is_Dark) gimmick = 2;
		else {
			for (int i = 0; i < table.m_PlayType.Count; i++) {
				if (table.m_PlayType[i].m_Type == PlayType.FireSpread) {
					gimmick = 1;
					break;
				}
				else if (table.m_PlayType[i].m_Type == PlayType.FieldAirstrike) {
					gimmick = 3;
					break;
				}
			}
		}

		switch (diff) {
			case 0:
				switch (gimmick) {
					case 0:
						m_SUI.Anim.SetTrigger("On");
						m_SUI.Anim.SetTrigger("Default");
						m_SUI.Anim.SetTrigger("NoGimmik");
						break;
					case 1:
						m_SUI.Anim.SetTrigger("Off");
						m_SUI.Anim.SetTrigger("Default");
						m_SUI.Anim.SetTrigger("Fire");
						break;
					case 2:
						m_SUI.Anim.SetTrigger("Flick");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("Dark");
						break;
					case 3:
						m_SUI.Anim.SetTrigger("On");
						m_SUI.Anim.SetTrigger("Break");
						m_SUI.Anim.SetTrigger("AirStrike");
						break;
				}
				break;
			case 1:
				switch (gimmick) {
					case 0:
						m_SUI.Anim.SetTrigger("On");
						m_SUI.Anim.SetTrigger("Default");
						m_SUI.Anim.SetTrigger("NoGimmik");
						break;
					case 1:
						m_SUI.Anim.SetTrigger("Off");
						m_SUI.Anim.SetTrigger("Default");
						m_SUI.Anim.SetTrigger("Fire");
						break;
					case 2:
						m_SUI.Anim.SetTrigger("Flick");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("Dark");
						break;
					case 3:
						m_SUI.Anim.SetTrigger("On");
						m_SUI.Anim.SetTrigger("Break");
						m_SUI.Anim.SetTrigger("AirStrike");
						break;
				}
				break;
			case 2:
				switch (gimmick) {
					case 0:
						m_SUI.Anim.SetTrigger("Off");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("NoGimmik");
						break;
					case 1:
						m_SUI.Anim.SetTrigger("Off");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("Fire");
						break;
					case 2:
						m_SUI.Anim.SetTrigger("Flick");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("Dark");
						break;
					case 3:
						m_SUI.Anim.SetTrigger("On");
						m_SUI.Anim.SetTrigger("Black");
						m_SUI.Anim.SetTrigger("AirStrike");
						break;
				}
				break;
		}
	}
	public void SkipAni()
	{
		Utile_Class.AniSkip(m_SUI.Anim);
	}

	public void Idle(bool _endstg = false) {
		Utile_Class.AniResetAllTriggers(m_SUI.Anim);
		m_SUI.Anim.CrossFade(_endstg ? "StartLast" : "Start", 0f, 0, 1f);
		LastStage(_endstg);
	}
	public void InStage() {
		m_SUI.Anim.SetTrigger("StageIn");
		PlayEffSound(SND_IDX.SFX_0007);
	}
	public void InMenu(bool _endstg = false)
	{
		Utile_Class.AniResetAllTriggers(m_SUI.Anim);
		m_SUI.Anim.SetTrigger(_endstg ? "StartLast" : "Start");
		LastStage(_endstg);
	}
	public void TabMenu() {
		m_SUI.Anim.SetTrigger("Tab");
	}

	public void SetStageDifficulty(int _diff) {
		if (m_DiffChangeAction != null) return;
		PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", _diff);
		m_DiffChangeAction = IE_DiffChange();
		StartCoroutine(m_DiffChangeAction);
	}
	IEnumerator IE_DiffChange() {
		m_SUI.Anim.SetTrigger("DiffChange");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 40f / 152f));

		StageDifficultyType diff = (StageDifficultyType)USERINFO.GetDifficulty();
		//난이도 변경시 배경음도 변경
		PLAY.SetBGSND();
		switch (diff) {
			case StageDifficultyType.Normal: m_SUI.Anim.SetTrigger("Normal"); break;
			case StageDifficultyType.Hard: m_SUI.Anim.SetTrigger("Hard"); break;
			case StageDifficultyType.Nightmare: m_SUI.Anim.SetTrigger("Nightmare"); break;
		}

		//for (int i = 0; i < 2; i++) {
		//	m_SUI.StgDiffBtns[i].SetData(SetStageDifficulty);
		//}

		SetData(USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)diff].Idx, true);
		//switch(diff)
		//{
		//case StageDifficultyType.Hard:
		//	if (TUTO.IsTuto(TutoKind.Hard_Stage, (int)TutoType_Hard.Select_Hard)) TUTO.Next();
		//	break;
		//case StageDifficultyType.Nightmare:
		//	if (TUTO.IsTuto(TutoKind.Nightmare_Stage, (int)TutoType_Nightmare.Select_Nightmare)) TUTO.Next();
		//	break;
		//}
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 120f / 152f));
		m_DiffChangeAction = null;
	}
	public void SetAddEventAnim(bool _start, Action _cb = null) {
		StartCoroutine(AddEventAction(_start, _cb));
	}
	IEnumerator AddEventAction(bool _start, Action _cb) {
		if (_start) {
			POPUP.GetMainUI().GetComponent<Main_Play>().SetAnim("Out");
			m_SUI.Anim.SetTrigger("SuddenEvent_Start");
			m_SUI.WindowAnim.SetTrigger("SuddenEvent");

			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

			_cb?.Invoke();
			m_SUI.Anim.SetTrigger("SuddenEvent_In");
		}
		else {
			m_SUI.Anim.SetTrigger("SuddenEvent_End");

			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

			POPUP.GetMainUI().GetComponent<Main_Play>().SetAnim("In");
			_cb?.Invoke();
		}

	}
	void LastStage(bool _endstg) {
		m_SUI.GunBtnAnim.SetTrigger(_endstg ? "Not" : "Loop");
	}

	public GameObject GetStageDiffBtn(StageDifficultyType type)
	{
		//for(int i = m_SUI.StgDiffBtns.Length-1; i > -1; i--)
		//{
		//	if (m_SUI.StgDiffBtns[i].m_DiffType == type) return m_SUI.StgDiffBtns[i].gameObject;
		//}
		return null;
	}
	public void Click_SupplyBox() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 8)) return;
		((Main_Play)POPUP.GetMainUI()).ClickMenuButton((int)MainMenuType.Shop);
		Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>(); 
		shop.SetScrollState(false);
		shop.StartPos(true, ShopGroup.SupplyBox, false, () => { shop.SetScrollState(true); });
	}
	public void Click_Replay() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 10)) return;
		StageContentType content = StageContentType.Replay;
		switch (USERINFO.GetDifficulty()) {
			case 0:
				content = StageContentType.Replay;
				break;
			case 1:
				content = StageContentType.ReplayHard;
				break;
			case 2:
				content = StageContentType.ReplayNight;
				break;
		}
		WEB.SEND_REQ_STAGE((res) => {
			if (res.IsSuccess()) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StgReplay, null, content);
				if(TUTO.IsTuto(TutoKind.Replay, (int)TutoType_Replay.Focus_ReplayBtn)) TUTO.Next();
			}
		}, USERINFO.m_Stage[content].UID);
	}
}
