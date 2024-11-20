using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Stage_Make : ObjMng
{
	public enum State { 
		None,
		Get,
		Return,
		Use
	}
	class ReadyGet
	{
		public StageMaterialType m_Type;
		public int m_Cnt;
		public Vector3 m_SPos = Vector3.zero;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public HorizontalLayoutGroup HorizontalGroup;
		public GameObject CardLockPrefab;
		public Transform LockBucket;
		public GameObject CardPrefab;
		public Transform CardBucket;
		public Transform ReturnBucket;
		public Transform OutPosObj;
		public Vector3 InitPos;
		public Vector2 AddReturnPos;
		public GameObject[] Panels;
		public GameObject InfoBtn;
	}
	[SerializeField]
	SUI m_SUI;
	[SerializeField]
	State m_State = State.None;
	[SerializeField]
	Vector3[] OutPos = new Vector3[3];
	//제작대에 있는 카드들
	//StageMakingTable, StageMaterialTable로 정렬
	List<Item_Stage_MakeCard> m_Cards = new List<Item_Stage_MakeCard>();
	//획득 대기중인 재료
	List<ReadyGet> m_ReadyGet = new List<ReadyGet>();
	List<TStageMakingTable> m_CanMakes = new List<TStageMakingTable>();
	int m_LockCnt;
	int[] m_CopyHadMat;
	List<float> m_MergeFailChance = new List<float>();

	readonly float GET_TIME = 0.35f;
	readonly float OUT_TIME = 1.3f;
	readonly float OUT_DELAY = 0.2f;
	readonly float MERGY_TIME = 0.25f;
	readonly float NEXT_ACTION_DELAY = 0.35f;
	readonly float MERGY_MOVE_TIME = 0.25f;
	public int GetCardCnt { get { return m_Cards.Count; } }
	public int GetMatCardCnt { get { return m_Cards.FindAll(o => (int)o.m_MatType <= (int)StageMaterialType.DefaultMat).Count; } }
	public int GetEmptyCnt { get { return BaseValue.STAGE_MAKE_GETMAX - m_LockCnt - m_Cards.Count; } }
	public GameObject GetInfoBtn { get { return m_SUI.InfoBtn; } }
	public GameObject GetFirstMakeCard { get { return m_Cards[0].gameObject; } }
	public GameObject GetMakeCard(StageMaterialType _type) {
		return m_Cards.Find(o => o.m_MatType == _type).gameObject;
	}
	public State GetState() {
		return m_State;
	}
	private void Update() {
		if (m_State == State.None && m_ReadyGet.Count > 0) {
			m_State = State.Get;
			StartCoroutine(GetAction());
		}
	}
	public void SetData() {
#if STAGE_TEST
		int stgidx = 0;
		if (STAGEINFO.m_StageContentType != StageContentType.Stage) {
			TModeTable mtdata = TDATA.GetModeTable(STAGEINFO.m_Idx);
			stgidx = mtdata == null ? 0 : mtdata.m_StageLimit;
			if(stgidx == 0) stgidx = PlayerPrefs.GetInt($"TestStageClearIdx_{USERINFO.m_UID}");
		}
		else {
			stgidx = STAGEINFO.m_Idx;
		}
#else
		int stgidx = 0;
		if (STAGEINFO.m_PlayType == StagePlayType.Stage)
			stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx;
		else
			stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
#endif
		m_CanMakes = TDATA.GetStageMakingList().FindAll(t => t.m_Condition.m_Type == StageMakingConditionType.Stage && t.m_Condition.m_Value  - 1 < stgidx);
		LockInit();
	}
	public void LockInit() {
		int prelockcnt = m_LockCnt;
		int max = BaseValue.STAGE_MAKE_GETMAX - Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown));
		m_LockCnt = Mathf.Clamp(max - (STAGEINFO.m_TStage.m_MakingCnt + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MergeSlotCount))), 0, max) + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown));
		UTILE.Load_Prefab_List(m_LockCnt, m_SUI.LockBucket, m_SUI.CardLockPrefab.transform);
		//for(int i = 0; i < Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown)); i++) {
		//	m_SUI.LockBucket.GetChild(i).GetComponent<Item_Stage_MakeCard>().SetBlock();
		//}
		float cardwidth = m_SUI.CardPrefab.GetComponent<RectTransform>().rect.width * m_SUI.CardPrefab.transform.localScale.x + m_SUI.HorizontalGroup.spacing;
		m_SUI.LockBucket.localPosition = new Vector3(-m_SUI.CardBucket.GetComponent<RectTransform>().rect.width * 0.5f + m_SUI.HorizontalGroup.padding.left + (BaseValue.STAGE_MAKE_GETMAX - m_LockCnt) * cardwidth, m_SUI.LockBucket.localPosition.y, 0f);

		BackDiscard(m_Cards.Count - (BaseValue.STAGE_MAKE_GETMAX - m_LockCnt));
	}
	void CardClickCB(Item_Stage_MakeCard _makecard) {
		// 메인 화면 연출중에는 눌리면 안됨
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageMaking, _makecard)) return;
		if (_makecard.m_TData == null)
		{//기본 재료카드 버리기
			if (!MAIN.IS_State(MainState.STAGE)) return;
			if (POPUP.IS_PopupUI() && POPUP.GetPopup().m_Popup != PopupName.Stage_Reward) return;
			if (POPUP.IS_MsgUI()) return;

			m_Cards.Remove(_makecard);
			STAGEINFO.m_Materials[(int)_makecard.m_MatType]--;
			_makecard.transform.SetParent(m_SUI.ReturnBucket);
			_makecard.StartRemoveAction();
		}
		else
		{//유틸 카드 사용
			if (STAGE.IS_SelectAction_Pause()) return;
			if (STAGE.IS_SelectAction()) return;

			STAGE.StartStageMakingCardAction(_makecard.m_TData.m_CardIdx, ()=> {
				m_Cards.Remove(_makecard);
				if (!IS_Last(_makecard.m_MatType))
					STAGEINFO.m_Materials[(int)_makecard.m_MatType]--;
				_makecard.Return(0f);
			});

			if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_Sniping_Merge)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Focus_Sniping_Merge)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Focus_Merge_Medicine)) TUTO.Next();
			else if(TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.Focus_Merge_Sniping)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Focus_Merge_ShockBomb)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Focus_Merge_Bread)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_403, (int)TutoType_Stage_403.Focus_Merge_Bread)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Focus_MergeLight)) TUTO.Next();
		}
	}
	public bool RandDiscard(int _prob, int _cnt) {
		if (UTILE.Get_Random(0, 1000) > _prob) return false;

		List<int> idxs = new List<int>();
		for(int i = 0;i< STAGEINFO.m_Materials.Length; i++) {
			if (STAGEINFO.m_Materials[i] >= _cnt) idxs.Add(i); 
		}
		if (idxs.Count == 0) return false;

		int randidx = idxs[UTILE.Get_Random(0, idxs.Count)];
		Item_Stage_MakeCard card = m_Cards.Find((t) => t.m_MatType == (StageMaterialType)randidx);
		m_Cards.Remove(card);
		STAGEINFO.m_Materials[randidx] = Mathf.Max(0, STAGEINFO.m_Materials[randidx] - _cnt);


		Vector3[] path = card.GetOutPath(card.transform.position);
		iTween.RotateAdd(card.gameObject, iTween.Hash("z", path[0].x > path[2].x ? 750f : -750f, "time", OUT_TIME, "easetype", "easeOutQuint"));
		card.SetFade(OUT_TIME - 0.1f, 0f);
		iTween.MoveTo(card.gameObject, iTween.Hash("path", path, "time", OUT_TIME, "easetype", "easeOutQuint"));
		card.Return(NEXT_ACTION_DELAY);
		return true;
	}
	void BackDiscard(int _cnt) {
		if (_cnt < 1) return;
		if (STAGEINFO.m_Materials.Length == 0) return;
		List<int> idxs = new List<int>();
		int limit = 0;
		for (int i = STAGEINFO.m_Materials.Length - 1; i > -1; i--) {
			if (STAGEINFO.m_Materials[i] >= 1) {
				idxs.Add(i);
				limit += STAGEINFO.m_Materials[i];
			}
		}
		if (limit == 0) return;
		int cnt = Mathf.Min(_cnt, limit);
		for (int i = cnt - 1, step = idxs.Count - 1; i > -1; i--) {
			int discardidx = idxs[step];
			Item_Stage_MakeCard card = m_Cards.Find((t) => t.m_MatType == (StageMaterialType)discardidx);
			m_Cards.Remove(card);
			STAGEINFO.m_Materials[discardidx] = Mathf.Max(0, STAGEINFO.m_Materials[discardidx] - 1);
			if (STAGEINFO.m_Materials[discardidx] == 0) step--;

			Vector3[] path = card.GetOutPath(card.transform.position);
			iTween.RotateAdd(card.gameObject, iTween.Hash("z", path[0].x > path[2].x ? 750f : -750f, "time", OUT_TIME, "delay", NEXT_ACTION_DELAY * i, "easetype", "easeOutQuint"));
			card.SetFade(OUT_TIME - 0.1f, NEXT_ACTION_DELAY * i);
			iTween.MoveTo(card.gameObject, iTween.Hash("path", path, "time", OUT_TIME, "delay", NEXT_ACTION_DELAY * i, "easetype", "easeOutQuint"));
			card.Return(NEXT_ACTION_DELAY * (i + 1));
		}
	}
	/// <summary> 가진것중 랜덤으로 복사 </summary>
	public void RandCopy(int _cnt = 1) {
		Item_Stage_MakeCard card = m_Cards[UTILE.Get_Random(0, m_Cards.Count)];
		if ((int)card.m_MatType <= (int)StageMaterialType.DefaultMat) GetMat(card.m_MatType, _cnt, Vector3.zero);
		else GetUtile(card.m_MatType, _cnt, Vector3.zero);
	}
	/// <summary> 재료 획득 </summary>
	public void GetMat(StageMaterialType _type, int _cnt, Vector3 _spos) {
		m_ReadyGet.Add(new ReadyGet() { m_Type = _type, m_Cnt = _cnt, m_SPos = _spos });
		//if (m_State != State.None) return;
		//m_State = State.Get;
		//StartCoroutine(GetAction());
	}
	/// <summary> 유틸 획득 </summary>
	public void GetUtile(StageMaterialType _type, int _cnt, Vector3 _spos) {
		StartCoroutine(GetUtileAction(_type, _cnt, _spos));
	}
	/// <summary> 외부에서 줄 수 있느거라 제작 제한 스테이지 없음 </summary>
	IEnumerator GetUtileAction(StageMaterialType _type, int _cnt, Vector3 _spos) {
		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);

		Vector2 startpos = Utile_Class.GetCanvasPosition(_spos);
		int getcnt = BaseValue.STAGE_MAKE_GETMAX - m_LockCnt - m_Cards.Count;
		int addcnt = Math.Min(getcnt, _cnt);
		for (int i = 0; i < _cnt; i++) {
			if (i < addcnt) {//추가
				Item_Stage_MakeCard card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.CardBucket).GetComponent<Item_Stage_MakeCard>();
				card.SetData(_type, TDATA.GetStageMakingList().Find(t => t.m_MatType == _type), CardClickCB);
				if (!IS_Last(_type)) STAGEINFO.m_Materials[(int)_type]++;
				m_Cards.Add(card);

				//연출
				SetSort();
				SortApply();
				card.BucketOff();
				yield return new WaitForEndOfFrame();//솔팅해서 자리 세팅 한프레임 기다림
				card.MoveBucket(startpos, Vector3.zero, GET_TIME);
				PlayEffSound(SND_IDX.SFX_0003);
				yield return new WaitForSeconds(GET_TIME);
				SetCopyHadMat();
				MergeCntCheck();
				yield return MergeAction();
			}
		}
	}
	/// <summary> 추가 가능한 수만큼 추가 액션과 오버 수량 튕겨내기 </summary>
	IEnumerator GetAction() {
		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);

		//if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.GetMat_Delay)) TUTO.Next();
		m_SUI.Anim.SetTrigger("Get");
		ReadyGet get = m_ReadyGet[0];
		Vector2 startpos = Utile_Class.GetCanvasPosition(get.m_SPos);
		int getcnt = BaseValue.STAGE_MAKE_GETMAX - m_LockCnt - m_Cards.Count;
		int addcnt = Math.Min(getcnt, get.m_Cnt);
		List<Item_Stage_MakeCard> addcard = new List<Item_Stage_MakeCard>();
		List<Item_Stage_MakeCard> outcard = new List<Item_Stage_MakeCard>();

		for (int i = 0; i < get.m_Cnt; i++) {
			if(i < addcnt) {//추가
				Item_Stage_MakeCard card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.CardBucket).GetComponent<Item_Stage_MakeCard>();
				card.name = get.m_Type.ToString();
				card.SetData(get.m_Type, null, CardClickCB);
				m_Cards.Add(card);
				STAGEINFO.InsertMaterial(get.m_Type, 1);
				card.gameObject.SetActive(false);
				addcard.Add(card);
			}
			else {//반환, 하나 생성해서 튕겨나가는 애니 후 삭제
				Item_Stage_MakeCard card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.ReturnBucket).GetComponent<Item_Stage_MakeCard>();
				card.name = get.m_Type.ToString();
				card.SetData(get.m_Type, null);
				card.transform.position = startpos;
				card.gameObject.SetActive(false);
				outcard.Add(card);
			}
		}

		//가용 수량만큼 생성해서 슬롯에 넣어 정렬하고 
		m_ReadyGet.Remove(get);

		SetCopyHadMat();
		MergeCntCheck();

		for (int i = 0;i< addcard.Count; i++) {
			addcard[i].gameObject.SetActive(true);
			//연출
			SetSort();
			SortApply();
			addcard[i].BucketOff();
			yield return new WaitForEndOfFrame();//솔팅해서 자리 세팅 한프레임 기다림
			addcard[i].MoveBucket(startpos, Vector3.zero, GET_TIME);
			PlayEffSound(SND_IDX.SFX_0003);
			yield return new WaitForSeconds(GET_TIME);
		}
		for(int i = 0; i < outcard.Count; i++) {
			outcard[i].gameObject.SetActive(true);
			OutPos[0] = outcard[i].transform.position;
			OutPos[1] = GetCardPos(outcard[i].m_MatType) + m_SUI.AddReturnPos;
			OutPos[2] = m_SUI.OutPosObj.position;
			iTween.MoveTo(outcard[i].gameObject, iTween.Hash("path", OutPos, "time", OUT_TIME, "easetype", "easeOutCubic"));
			outcard[i].Return(OUT_TIME);

			//float gettime = GET_TIME;
			//float outtime = 0.4f;
			//m_SUI.Anim.SetTrigger("Glow_Discard");
			//outcard[i].gameObject.SetActive(true);
			//Vector3[] outpos = outcard[i].GetOutPath(GetCardPos(outcard[i].m_MatType) + m_SUI.AddReturnPos, 0.5f);
			//iTween.MoveTo(outcard[i].gameObject, iTween.Hash("path", new Vector3[] { outcard[i].transform.position, GetCardPos(outcard[i].m_MatType) + m_SUI.AddReturnPos }, "time", gettime, "easetype", "easeOutCubic"));
			//iTween.MoveTo(outcard[i].gameObject, iTween.Hash("path", outpos, "time", outtime, "delay", gettime, "easetype", "linear"));
			//outcard[i].Return(gettime + outtime);
			//outcard[i].SetDelayAnim(gettime + outtime - 0.35f, "Discard");

			yield return new WaitForSeconds(OUT_DELAY);
		}

		yield return new WaitForSeconds(NEXT_ACTION_DELAY);

		yield return MergeAction();
		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Delay_Merge)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Delay_Merge)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Delay_Merge)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Delay_Merge)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Delay_Merge)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Delay_Merge)) TUTO.Next();
		m_State = State.None;
	}
	void SetCopyHadMat() {
		m_CopyHadMat = new int[STAGEINFO.m_Materials.Length];
		for (int i = 0;i< STAGEINFO.m_Materials.Length; i++) {
			m_CopyHadMat[i] = STAGEINFO.m_Materials[i];
		}
	}

	void MergeCntCheck() {
		float? synergyDT = STAGE_USERINFO.GetSynergeValue(JobType.Detective, 1);
		float? synergyMF = STAGE_USERINFO.GetSynergeValue(JobType.Mafia, 1);
		float? synergyHT = STAGE_USERINFO.GetSynergeValue(JobType.Hunter, 1);
		float? synergyPM = STAGE_USERINFO.GetSynergeValue(JobType.Pharmacist, 1);
		float? synergyAT = STAGE_USERINFO.GetSynergeValue(JobType.Artist, 1);
		float? synergyVT = STAGE_USERINFO.GetSynergeValue(JobType.Volunteer, 0);
		//제작법은 조건에 따라 거름
		int madecnt = 0;
		for (int i = 0; i < m_CanMakes.Count; i++) {//제작테이블 전부 조회
			bool canmerge = true;
			for (int j = 0; j < m_CanMakes[i].m_Materal.Count; j++) {//재료 조회하여 머지 가능여부 판단
				if (!canmerge) break;
				int need = m_CanMakes[i].m_Materal[j].m_Cnt;
				StageMaterialType type = m_CanMakes[i].m_Materal[j].m_Type;
				//시너지
				if (synergyDT != null && m_CanMakes[i].m_MatType == StageMaterialType.Sniping) {
					if (m_CopyHadMat[(int)StageMaterialType.Bullet] < synergyDT || synergyDT < 1) canmerge = false;
					else canmerge = true;
					break;
				}
				else if (synergyMF != null && m_CanMakes[i].m_MatType == StageMaterialType.ShockBomb) {
					if (m_CopyHadMat[(int)StageMaterialType.GunPowder] < synergyMF || synergyMF < 1) canmerge = false;
					else canmerge = true;
					break;
				}
				else if (synergyHT != null && m_CanMakes[i].m_MatType == StageMaterialType.Bread) {
					if (m_CopyHadMat[(int)StageMaterialType.Food] < synergyHT || synergyHT < 1) canmerge = false;
					else canmerge = true;
					break;
				}
				else if (synergyPM != null && m_CanMakes[i].m_MatType == StageMaterialType.MedBottle) {
					if (m_CopyHadMat[(int)StageMaterialType.Medicine] < synergyPM || synergyPM < 1) canmerge = false;
					else canmerge = true;
					break;
				}
				else if (synergyAT != null && m_CanMakes[i].m_MatType == StageMaterialType.Candle) {
					if (m_CopyHadMat[(int)StageMaterialType.Herb] < synergyAT || synergyAT < 1) canmerge = false;
					else canmerge = true;
					break;
				}
				else if (type > StageMaterialType.DefaultMat && m_CopyHadMat[(int)type] < need) {
					canmerge = false;
					break;
				}
				else if (type <= StageMaterialType.DefaultMat && m_CopyHadMat[(int)type] < need + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MoreMaterial))) {
					canmerge = false;
					break;
				}
				canmerge = true;
			}
			if (canmerge) {//머지 가능
				//재료와 카드 머지연출과 함께 제거
				for (int j = 0; j < m_CanMakes[i].m_Materal.Count; j++) {
					int cnt = 0;
					if (synergyDT != null && m_CanMakes[i].m_MatType == StageMaterialType.Sniping)
						cnt = Mathf.RoundToInt((float)synergyDT);
					else if (synergyMF != null && m_CanMakes[i].m_MatType == StageMaterialType.ShockBomb)
						cnt = Mathf.RoundToInt((float)synergyMF);
					else if (synergyHT != null && m_CanMakes[i].m_MatType == StageMaterialType.Bread)
						cnt = Mathf.RoundToInt((float)synergyHT);
					else if (synergyPM != null && m_CanMakes[i].m_MatType == StageMaterialType.MedBottle)
						cnt = Mathf.RoundToInt((float)synergyPM);
					else if (synergyAT != null && m_CanMakes[i].m_MatType == StageMaterialType.Candle)
						cnt = Mathf.RoundToInt((float)synergyAT);
					else {
						StageMaterialType type = m_CanMakes[i].m_Materal[j].m_Type;
						if (type <= StageMaterialType.DefaultMat) {
							cnt = m_CanMakes[i].m_Materal[j].m_Cnt + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MoreMaterial));
						}
						else {
							cnt = m_CanMakes[i].m_Materal[j].m_Cnt;
						}
					}
						 
					for (int k = cnt - 1; k > -1; k--) {
						m_CopyHadMat[(int)m_CanMakes[i].m_Materal[j].m_Type]--;
					}
				}

				//디버프에 MergeFailChance 있으면 N%확률로 제작 실패
				float rand = UTILE.Get_Random(0f, 1f);
				m_MergeFailChance.Add(rand);
				if (!STAGE_USERINFO.ISBuff(StageCardType.MergeFailChance) ||
					(STAGE_USERINFO.ISBuff(StageCardType.MergeFailChance) && rand >= STAGE_USERINFO.GetBuffValue(StageCardType.MergeFailChance))) {
					//최종 완제만 None이고 유틸이지만 재료일 경우도 있으니 재료 수 증가
					if (!IS_Last(m_CanMakes[i].m_MatType))
						m_CopyHadMat[(int)m_CanMakes[i].m_MatType]++;

					madecnt++;

					//아무 제작이나 해도 체크
					STAGE.m_Check.Check(StageCheckType.AnyMaking, m_CanMakes[i].m_CardIdx);
					//if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.FREE_Play_1) && STAGE.m_Check.GetClearCnt(0) == 2) TUTO.Next();
					STAGE.CheckEnd(StageCheckType.AnyMaking);
				}
				else {
					//조합 실패 연출
					madecnt++;
				}

			}
		}

		if (madecnt > 0) MergeCntCheck();
	}
	/// <summary> 제작대 카드 세팅,  </summary>
	IEnumerator MergeAction() {
		//yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);

		HorizontalLayoutGroup group = m_SUI.CardBucket.GetComponent<HorizontalLayoutGroup>();
		float? synergyDT = STAGE_USERINFO.GetSynergeValue(JobType.Detective, 1);
		float? synergyMF = STAGE_USERINFO.GetSynergeValue(JobType.Mafia, 1);
		float? synergyHT = STAGE_USERINFO.GetSynergeValue(JobType.Hunter, 1);
		float? synergyPM = STAGE_USERINFO.GetSynergeValue(JobType.Pharmacist, 1);
		float? synergyAT = STAGE_USERINFO.GetSynergeValue(JobType.Artist, 1);
		float? synergyVT = STAGE_USERINFO.GetSynergeValue(JobType.Volunteer, 0);
		//제작법은 조건에 따라 거름
		int madecnt = 0;
		for (int i = 0;i < m_CanMakes.Count; i++) {//제작테이블 전부 조회
			bool canmerge = true;
			for(int j = 0;j< m_CanMakes[i].m_Materal.Count; j++) {//재료 조회하여 머지 가능여부 판단
				if (!canmerge) break;
				int need = m_CanMakes[i].m_Materal[j].m_Cnt;
				StageMaterialType type = m_CanMakes[i].m_Materal[j].m_Type;
				//시너지
				if (synergyDT != null && m_CanMakes[i].m_MatType == StageMaterialType.Sniping) {
					if (STAGEINFO.m_Materials[(int)StageMaterialType.Bullet] < synergyDT || synergyDT < 1) {
						canmerge = false;
					}
					else {
						canmerge = true;
						STAGE_USERINFO.ActivateSynergy(JobType.Detective);
						Utile_Class.DebugLog_Value("Detective 총알 2개로 저격 제작");
					}
					break;
				}
				else if (synergyMF != null && m_CanMakes[i].m_MatType == StageMaterialType.ShockBomb) {
					if (STAGEINFO.m_Materials[(int)StageMaterialType.GunPowder] < synergyMF || synergyMF < 1) {
						canmerge = false;
					}
					else {
						canmerge = true;
						STAGE_USERINFO.ActivateSynergy(JobType.Mafia);
						Utile_Class.DebugLog_Value("Mafia 화약 2개로 충격탄 제작");
					}
					break;
				}
				else if (synergyHT != null && m_CanMakes[i].m_MatType == StageMaterialType.Bread) {
					if (STAGEINFO.m_Materials[(int)StageMaterialType.Food] < synergyHT || synergyHT < 1) {
						canmerge = false;
					}
					else {
						canmerge = true;
						STAGE_USERINFO.ActivateSynergy(JobType.Hunter);
						Utile_Class.DebugLog_Value("Hunter 음식 2개로 빵 제작");
					}
					break;
				}
				else if (synergyPM != null && m_CanMakes[i].m_MatType == StageMaterialType.MedBottle) {
					if (STAGEINFO.m_Materials[(int)StageMaterialType.Medicine] < synergyPM || synergyPM < 1) {
						canmerge = false;
					}
					else {
						canmerge = true;
						STAGE_USERINFO.ActivateSynergy(JobType.Pharmacist);
						Utile_Class.DebugLog_Value("Pharmacist 약품 2개로 약병 제작");
					}
					break;
				}
				else if (synergyAT != null && m_CanMakes[i].m_MatType == StageMaterialType.Candle) {
					if (STAGEINFO.m_Materials[(int)StageMaterialType.Herb] < synergyAT || synergyAT < 1) {
						canmerge = false;
					}
					else {
						canmerge = true;
						STAGE_USERINFO.ActivateSynergy(JobType.Artist);
						Utile_Class.DebugLog_Value("Artist 허브 2개로 양초 제작");
					}
					break;
				}
				else if (type > StageMaterialType.DefaultMat && STAGEINFO.m_Materials[(int)type] < need) {
					canmerge = false;
					break;
				}
				else if (type <= StageMaterialType.DefaultMat && STAGEINFO.m_Materials[(int)type] < need + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MoreMaterial))) {
					canmerge = false;
					break;
				}
				canmerge = true;
			}
			if (canmerge) {//머지 가능
				//*머지 단계에 따른 애니메이션 및 대사
				int step = TDATA.GetMakingGrade(m_CanMakes[i].m_MatType);
				m_SUI.Anim.SetTrigger(string.Format("Make_0{0}", step));
				switch (step) {
					case 1: 
						PlayEffSound(SND_IDX.SFX_0211);
						STAGE_USERINFO.CharSpeech(DialogueConditionType.MergeItem);
						break;
					case 2: PlayEffSound(SND_IDX.SFX_0212); break;
					case 3: 
						PlayEffSound(SND_IDX.SFX_0213);
						STAGE_USERINFO.CharSpeech(DialogueConditionType.HighMergeItem);
						break;
				}
				//재료와 카드 머지연출과 함께 제거
				Vector3 endpos = Vector3.zero;//합쳐진 위치
				List<GameObject> obj = new List<GameObject>();
				for (int j = 0; j < m_CanMakes[i].m_Materal.Count; j++) {
					StageMaterialType type = m_CanMakes[i].m_Materal[j].m_Type;
					List<Item_Stage_MakeCard> cards = m_Cards.FindAll(t => t.m_MatType == type);
					if(cards.Count > 0) endpos = cards[0].transform.position;
					int cnt = 0;
					if (synergyDT != null && m_CanMakes[i].m_MatType == StageMaterialType.Sniping)
						cnt = Mathf.RoundToInt((float)synergyDT); 
					else if (synergyMF != null && m_CanMakes[i].m_MatType == StageMaterialType.ShockBomb)
						cnt = Mathf.RoundToInt((float)synergyMF);
					else if (synergyHT != null && m_CanMakes[i].m_MatType == StageMaterialType.Bread)
						cnt = Mathf.RoundToInt((float)synergyHT);
					else if (synergyPM != null && m_CanMakes[i].m_MatType == StageMaterialType.MedBottle)
						cnt = Mathf.RoundToInt((float)synergyPM);
					else if (synergyAT != null && m_CanMakes[i].m_MatType == StageMaterialType.Candle)
						cnt = Mathf.RoundToInt((float)synergyAT);
					else {
						if (type <= StageMaterialType.DefaultMat) {
							cnt = m_CanMakes[i].m_Materal[j].m_Cnt + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MoreMaterial));
						}
						else {
							cnt = m_CanMakes[i].m_Materal[j].m_Cnt;
						}
					}

					for (int k = cnt - 1; k > -1 ; k--) {
						cards[k].Merge(MERGY_TIME);
						obj.Add(cards[k].gameObject);
						cards[k].MoveBucket(cards[k].transform.position, endpos - cards[k].transform.position, MERGY_TIME);
						m_Cards.Remove(cards[k]);
						STAGEINFO.m_Materials[(int)m_CanMakes[i].m_Materal[j].m_Type]--;
					}
				}
				yield return new WaitUntil(() => obj.Find((t) => t != null) == null);
				//yield return new WaitForSeconds(0.4f);

				//디버프에 MergeFailChance 있으면 N%확률로 제작 실패
				Item_Stage_MakeCard card = null;
				float rand = 0f;
				if (m_MergeFailChance.Count > 0) {
					rand = m_MergeFailChance[0];
					m_MergeFailChance.RemoveAt(0);
				}
				if (!STAGE_USERINFO.ISBuff(StageCardType.MergeFailChance) || 
					(STAGE_USERINFO.ISBuff(StageCardType.MergeFailChance) && rand >= STAGE_USERINFO.GetBuffValue(StageCardType.MergeFailChance))) {
					//최종 완제만 None이고 유틸이지만 재료일 경우도 있으니 재료 수 증가
					if (!IS_Last(m_CanMakes[i].m_MatType))
						STAGEINFO.m_Materials[(int)m_CanMakes[i].m_MatType]++;
					//유틸 카드 생성
					card = Utile_Class.Instantiate(m_SUI.CardPrefab, m_SUI.CardBucket).GetComponent<Item_Stage_MakeCard>();
					card.SetData(m_CanMakes[i].m_MatType, m_CanMakes[i], CardClickCB);
					m_Cards.Add(card);
					card.BucketOff();

					madecnt++;
				}
				else {
					//조합 실패 연출
					madecnt++;
				}
				//정렬 전 이동 시작 위치 지정
				for (int j = 0; j < m_Cards.Count; j++) {
					m_Cards[j].SetStartPos(m_Cards[j].transform.position);
				}
				//정렬
				SetSort();
				if(card != null) card.SetStartPos(endpos);//정렬 후 합쳐진 위치를 시작점으로 지정
				// Canvas 사용 제거
				//card.MergeMoveSort(0.4f);

				//정렬 및 연출
				SortApply();
				yield return new WaitForEndOfFrame();//솔팅해서 자리 세팅 한프레임 기다림

				// 이동 연출
				// 이동하는놈이 상위에 보여야하므로 위치 변경해주어야되므로 layoutgroup 잠시 꺼둔다.
				group.enabled = false;
				// 이동하는놈 상위로 보내기
				RectTransform rtfcard = null;
				int sibldx = 0;
				if (card != null) {
					rtfcard = (RectTransform)card.transform;
					sibldx = rtfcard.GetSiblingIndex();
					rtfcard.SetAsLastSibling();
				}

				// 이동 연출 시작
				MoveApply();

				////아무 제작이나 해도 체크
				//STAGE.m_Check.Check(StageCheckType.AnyMaking, m_CanMakes[i].m_CardIdx);
				//STAGE.CheckEnd();

				yield return new WaitForSeconds(NEXT_ACTION_DELAY);
				// 이동한놈 다시 자신의 자리로 보내기
				if (card != null) rtfcard.SetSiblingIndex(sibldx);
				// 이동이 끝났으므로 layoutgroup 다시 켜준다.
				group.enabled = true;

				//자원봉사자 시너지
				if (synergyVT != null && synergyVT > 0f) {
					int preval = STAGE_USERINFO.m_AP[0];
					STAGE_USERINFO.m_AP[0] = Mathf.Clamp(STAGE_USERINFO.m_AP[0] + Mathf.RoundToInt((float)synergyVT), 0, STAGE_USERINFO.m_AP[1]);
					//행동력 회복 스킬이나 시너지도 추가 될거임
					DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], preval, STAGE_USERINFO.m_AP[1]);
					STAGE_USERINFO.ActivateSynergy(JobType.Volunteer);
				}
				break;
			}
		}

		if (madecnt > 0) yield return MergeAction();
	}
	/// <summary> 획득과 동시에 정렬 </summary>
	void SetSort() {//정렬 코드 하나로 합치기
		for (int i = 0; i < m_Cards.Count; i++) {
			m_Cards[i].SetStartPos(m_Cards[i].transform.position);
		}
		//1.재료타입 순서로
		//2.완제 - 기본재료
		//3.완제는 인덱스 순서
		m_Cards.Sort((Item_Stage_MakeCard _a, Item_Stage_MakeCard _b) => {
			if (_a.m_TData != null && _b.m_TData != null) return _b.TDATA.GetMakingGrade(_b.m_MatType).CompareTo(_a.TDATA.GetMakingGrade(_a.m_MatType)); //기본 재료는 TStageMakingTable이 안붙음
			else if (_a.m_TData != null && _b.m_TData == null) return -1;
			else if (_a.m_TData == null && _b.m_TData != null) return 1;
			return _b.m_MatType.CompareTo(_a.m_MatType);
		});
	}
	void SortApply() {
		for (int i = 0; i < m_Cards.Count; i++) {
			m_Cards[i].transform.SetAsLastSibling();
		}
	}
	void MoveApply() {
		for (int i = 0; i < m_Cards.Count; i++) {
			m_Cards[i].MoveBucket(m_Cards[i].m_SPos, Vector3.zero, MERGY_MOVE_TIME);
		}
	}
	/// <summary> 카드 들어갈 자리 </summary>
	public Vector2 GetCardPos(StageMaterialType _type) {
		List<Item_Stage_MakeCard> cards = m_Cards.FindAll(t => t.m_MatType == _type);
		if (cards.Count > 0) {//기존것이 있으면 뒤에 붙이기
			return cards[cards.Count - 1].transform.position + m_SUI.InitPos;
		}
		else {//기존것이 없으면 인덱스 빠른 재료뒤에 대신 기본 재료는 유틸카드 재료보다는 뒤에있어야함
			for(int i = (int)_type - 1; i > -1 ; i--) {
				List<Item_Stage_MakeCard> othercard = m_Cards.FindAll(t => t.m_MatType == (StageMaterialType)i);
				if (othercard.Count > 0) {
					return othercard[othercard.Count - 1].transform.position + m_SUI.InitPos;
				}
			}
		}
		//완제만 있을경우 완제 뒤에
		if(m_Cards.Count> 0) return m_Cards[m_Cards.Count - 1].transform.position + m_SUI.InitPos;
		//아무것도 없다면 기본 위치
		return m_SUI.InitPos;
	}
	public void ClickRecipeInfo()
	{
		if (STAGE.IS_SelectAction_Pause()) return;
		if (STAGE.IS_SelectAction()) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageMakingList, 0)) return;

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_MakingInfo, (result, obj) => {
			//if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_MergeListOut)) TUTO.Next();
		} , m_CanMakes, STAGE_USERINFO.ISBuff(StageCardType.MoreMaterial));
		//if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_MergeList)) TUTO.Next();
	}

	public Item_Stage_MakeCard GetItem(StageMaterialType type)
	{
		return m_Cards.Find(o => o.m_MatType == type);
	}

	public GameObject GetMakingInfoPanel()
	{
		return m_SUI.Panels[0];
	}

	public int DiscardAll() {
		int cnt = m_Cards.Count;
		if (m_Cards.Count > 0) {
			for (int i = m_Cards.Count - 1; i > -1; i--) {
				Item_Stage_MakeCard card = m_Cards[i];
				m_Cards.Remove(card);
				if (!IS_Last(card.m_MatType)) STAGEINFO.m_Materials[(int)card.m_MatType]--;
				card.transform.SetParent(m_SUI.ReturnBucket);
				card.StartRemoveAction();
			}
			m_Cards.Clear();
		}
		return cnt;
	}

	public bool IS_Last(StageMaterialType _type) {
		switch (_type) {
			case StageMaterialType.AirStrike:
			case StageMaterialType.CureKit:
			case StageMaterialType.Steak:
			case StageMaterialType.Shampoo:
			case StageMaterialType.Drug:
			case StageMaterialType.Flare:
			case StageMaterialType.PowderBomb:
			case StageMaterialType.NapalmBomb:
				return true;
			default:return false;
		}
	}

	public void TutoMergeTabAction(Action _cb) {
		gameObject.SetActive(true);//50/234
		StartCoroutine(IE_TutoMergeTabStartAction(_cb));
	}
	IEnumerator IE_TutoMergeTabStartAction(Action _cb) {
		m_SUI.Anim.SetTrigger("FirstIn");
		m_SUI.Anim.SetTrigger("FirstIn_Gear");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		_cb?.Invoke();
	}
}
