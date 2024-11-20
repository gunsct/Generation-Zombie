using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	/// <summary> 카드를 선택해서 공격시 선공</summary>
	bool m_SelectAtk;
	int m_CardLastLine = -1;
	public bool ISEndLine(int line) {
		if (m_CardLastLine < 0) return true;
		return m_CardLastLine > line;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Card
	public void LoadStage() {
		SetActiveStage(true);
		for (int i = 0; i < m_ViewCard.Length; i++) CreateStageLine(i, i, null, true);
	}

	void SetActiveStage(bool Active) {
		m_Stage.ActionPanel.gameObject.SetActive(Active);
		m_LowEffPanel.gameObject.SetActive(Active);
		m_BGEffPanel[0].gameObject.SetActive(Active);
		m_BGEffPanel[1].gameObject.SetActive(Active);
		if (!Active) for (int i = 0; i < m_Damage.Panel.childCount; i++) Destroy(m_Damage.Panel.GetChild(0).gameObject);

		for (int i = 0; i < m_Chars.Length; i++) m_Chars[i].gameObject.SetActive(Active);
	}

	void RemoveStage(Item_Stage card) {
		RemoveTargetLight(card);
		// 카드 pool 이동
		if (card != null) {
			RemoveTarget(card);

			// AI 정지 대상으로 셋팅되어있는지 확인
			for (int i = m_AIStopInfos.Count - 1; i > -1; i--) {
				if (m_AIStopInfos[i].m_Target == card) RemoveAIStopInfo(m_AIStopInfos[i]);
			}
			// AI 원거리 블록 대상으로 셋팅되어있는지 확인
			for (int i = m_AiBlockRangeAtkInfos.Count - 1; i > -1; i--) {
				if (m_AiBlockRangeAtkInfos[i].m_Target == card) RemoveAiBlockRangeAtkInfo(m_AiBlockRangeAtkInfos[i]);
			}
			// 화상 대상으로 셋팅되어있는지 확인
			for (int i = m_BurnInfos.Count - 1; i > -1; i--) {
				if (m_BurnInfos[i].m_Target == card) RemoveBurnInfo(m_BurnInfos[i]);
			}
			// 가로등 대상으로 셋팅되어있는지 확인
			for (int i = m_StreetLightInfos.Count - 1; i > -1; i--) {
				if (m_StreetLightInfos[i].m_Target == card) RemoveStreetLightInfo(m_StreetLightInfos[i]);
			}
			if (card.transform.parent == m_Stage.Panel[1])//리워드 카드는 생성시 ActionPanel에 넣고 패널1에 안넣기 떄문에 예외처리
				card.transform.SetParent(m_Stage.Panel[0]);
		}
	}
	public void RemoveTarget(Item_Stage card) {
		for (int i = m_Stage.Panel[1].childCount - 1; i > -1; i--) {
			Item_Stage stage = m_Stage.Panel[1].GetChild(i).GetComponent<Item_Stage>();
			if (stage.m_Target == card) stage.m_Target = null;
		}
	}
	Item_Stage GetPoolCard() {
		if (m_Stage.Panel[0].childCount < 1) Utile_Class.Instantiate(m_Stage.Prefab, m_Stage.Panel[0]);
		return m_Stage.Panel[0].GetChild(m_Stage.Panel[0].childCount - 1).GetComponent<Item_Stage>();
	}

	TStageCardTable CharSkill_CreateStageCardIdx() {
		int TotProb = 0;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(CharSkill_CheckUseStageCard);
		for (int i = list.Count - 1; i > -1; i--) {
			if (list[i].m_IsEndType) list.RemoveAt(i);
		}

		// 등장조건에 

		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].GetHighProb();
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--) {
			if (Rand < list[i].GetHighProb()) return list[i];
			Rand -= list[i].GetHighProb();

		}
		TStageCardTable card = list.Count > 0 ? list[0] : null;
		if (card == null) {
			List<TStageCardTable> datas = STAGEINFO.GetMaterialCardIdxs();
			card = datas[UTILE.Get_Random(0, datas.Count)];
		}
		return card;
	}
	TStageCardTable CharSkill_CreateMatStageCardIdx(StageMaterialType _type) {
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(CharSkill_CheckUseStageCardMat);

		return list.Find((t) => t.m_Value1 == (int)_type);
	}
	StageCardInfo CreateStageCardData(List<StageCardType> _ignoretype = null) {
		int TotProb = 0;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(CheckUseStageCard);
		if (_ignoretype != null && _ignoretype.Count > 0) list.RemoveAll((t) => _ignoretype.Contains(t.m_Type));
		for (int i = list.Count - 1; i > -1; i--) {
			if (list[i].m_IsEndType) list.RemoveAt(i);
		}

		// 등장조건에 

		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].GetProb();
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--) {
			if (Rand < list[i].GetProb()) return DarkCardCheck(new StageCardInfo(list[i].m_Idx), _ignoretype);
			Rand -= list[i].GetProb();

		}
		return null;
	}


	// 어둠카드의 실제 카드 생성
	public StageCardInfo DarkCardCheck(StageCardInfo info, List<StageCardType> _ignoretype = null) {
		if (!info.IsDarkCard) return info;
		int TotProb = 0;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(CheckDarkRealStageCard);
		if (_ignoretype != null && _ignoretype.Count > 0) list.RemoveAll((t) => _ignoretype.Contains(t.m_Type));
		for (int i = list.Count - 1; i > -1; i--) {
			if (list[i].m_IsEndType) list.RemoveAt(i);
		}

		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].m_DarkProb;
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--) {
			if (Rand < list[i].m_DarkProb) {
				info.SetRealIdx(list[i].m_Idx);
				break;
			}
			Rand -= list[i].m_DarkProb;

		}
		return info;
	}

	int CreateStageLastCardIdx() {
		int TotProb = 0;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(t => t.GetProb() > 0 && m_Check.IS_AppearCard(t.m_Idx) && t.m_IsEndType);

		if (list.Count < 0) return 0;

		// 등장조건에 

		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].GetProb();
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--) {
			if (Rand < list[i].GetProb()) return list[i].m_Idx;
			Rand -= list[i].GetProb();

		}
		return 0;
	}

	int CreateStageLineCardIdx(int turn) {
		int TotProb = 0;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup().FindAll(CheckUseStageLineCard);

		if (list.Count < 0) return 0;

		// 턴수체크
		list.RemoveAll((info) => info.m_LimitCount == 0 || turn % info.m_LimitCount > 0 || info.m_IsEndType);
		// 등장조건에
		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].GetProb();
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--) {
			if (Rand < list[i].GetProb()) return list[i].m_Idx;
			Rand -= list[i].GetProb();
		}
		return 0;
	}

	bool CharSkill_CheckUseStageCard(TStageCardTable Card) {
		if (Card.IS_LineCard()) return false;
		if (!m_Check.IS_CreateCard(Card.m_Idx)) return false;//IS_AppearCard
		if (Card.GetHighProb() < 1) return false;
		switch (Card.m_Type) {
			case StageCardType.Enemy:
			case StageCardType.Synergy:
			case StageCardType.Roadblock:
			case StageCardType.AllRoadblock:
			case StageCardType.Dark:
				return false;
		}
		// 스테이지 제한 체크
		if (Card.m_LimitCount > 0) {
			// 생성 가능한 갯수
			int Cnt = 0;
			for (int i = m_ViewCard.Length - 1; i > -1; i--) {
				if (m_ViewCard[i] == null) continue;
				for (int j = m_ViewCard[i].Length - 1; j > -1; j--) {
					if (m_ViewCard[i][j] == null) continue;
					if (m_ViewCard[i][j].m_Info.m_Idx == Card.m_Idx) {
						Cnt++;
						if (Cnt >= Card.m_LimitCount) return false;
					}
				}
			}
		}
		return !Card.m_IsEndType;
	}

	/// <summary> 스킬로 변환될 재료카드 체크, 테이블에는 확률0이 있을 수 있다 </summary>
	bool CharSkill_CheckUseStageCardMat(TStageCardTable Card) {
		if (Card.IS_LineCard()) return false;
		if (!m_Check.IS_AppearCard(Card.m_Idx)) return false;
		if (Card.m_Type != StageCardType.Material) return false;

		// 스테이지 제한 체크
		if (Card.m_LimitCount > 0) {
			// 생성 가능한 갯수
			int Cnt = 0;
			for (int i = m_ViewCard.Length - 1; i > -1; i--) {
				if (m_ViewCard[i] == null) continue;
				for (int j = m_ViewCard[i].Length - 1; j > -1; j--) {
					if (m_ViewCard[i][j] == null) continue;
					if (m_ViewCard[i][j].m_Info.m_Idx == Card.m_Idx) {
						Cnt++;
						if (Cnt >= Card.m_LimitCount) return false;
					}
				}
			}
		}
		return !Card.m_IsEndType;
	}

	bool CheckUseStageCard(TStageCardTable Card) {
		if (Card.IS_LineCard()) return false;
		if (!m_Check.IS_AppearCard(Card.m_Idx)) return false;
		if (Card.GetProb() < 1) return false;
		switch (Card.m_Type) {
			case StageCardType.Synergy:
				if (!m_User.IS_CreateSynergy()) return false;
				break;
		}
		// 스테이지 제한 체크
		if (Card.m_LimitCount > 0) {
			// 생성 가능한 갯수
			int Cnt = 0;
			for (int i = m_ViewCard.Length - 1; i > -1; i--) {
				if (m_ViewCard[i] == null) continue;
				for (int j = m_ViewCard[i].Length - 1; j > -1; j--) {
					if (m_ViewCard[i][j] == null) continue;
					if (m_ViewCard[i][j].m_Info.m_Idx == Card.m_Idx) {
						Cnt++;
						if (Cnt >= Card.m_LimitCount) return false;
					}
				}
			}
		}
		return !Card.m_IsEndType;
	}

	bool CheckUseStageLineCard(TStageCardTable Card) {
		if (Card.GetProb() < 1) return false;
		if (!m_Check.IS_AppearCard(Card.m_Idx)) return false;
		return Card.IS_LineCard();
	}

	bool CheckDarkRealStageCard(TStageCardTable Card) {
		if (Card.m_Type == StageCardType.Dark) return false;
		if (!m_Check.IS_AppearCard(Card.m_Idx)) return false;
		if (Card.m_DarkProb < 1) return false;
		switch (Card.m_Type) {
			case StageCardType.Synergy:
				if (!m_User.IS_CreateSynergy()) return false;
				break;
		}
#if CREATE_DARK_CARD_ENEMY// 몬스터 다크카드 테스트용
		if (Card.m_Type != StageCardType.Enemy) return false;
#endif
		// 스테이지 제한 체크
		if (Card.m_LimitCount > 0) {
			// 생성 가능한 갯수
			int Cnt = 0;
			for (int i = m_ViewCard.Length - 1; i > -1; i--) {
				if (m_ViewCard[i] == null) continue;
				for (int j = m_ViewCard[i].Length - 1; j > -1; j--) {
					if (m_ViewCard[i][j] == null) continue;
					if (m_ViewCard[i][j].m_Info.m_Idx == Card.m_Idx || m_ViewCard[i][j].m_Info.m_RealIdx == Card.m_Idx) {
						Cnt++;
						if (Cnt >= Card.m_LimitCount) return false;
					}
				}
			}
		}
		return !Card.m_IsEndType;
	}

	Item_Stage CreateStageCard(int line, int pos, Vector3 SPos, int directidx, Transform _parent = null, List<StageCardType> _ignoretype = null) {
		Item_Stage item = GetPoolCard();
		item.SetPos(line, pos);
		if (directidx > 0) 
			item.SetData(new StageCardInfo(directidx));
		else 
			item.SetData(CreateStageCardData(_ignoretype));
		item.transform.SetParent(_parent == null ? m_Stage.Panel[1] : _parent);
		item.transform.localPosition = SPos;
		item.transform.localRotation = Quaternion.Euler(Vector3.zero);
		item.transform.localScale = Vector3.one;

		return item;
	}
	Item_Stage CreateStageCard(int line, int pos, Vector3 SPos, float waittime = 0f, Action<Item_Stage> EndCB = null, int directidx = 0, bool StartAction = false, List<StageCardType> _ignoretype = null) {
		Item_Stage item = CreateStageCard(line, pos, SPos, directidx, null, _ignoretype);
		m_ViewCard[line][pos] = item;
		if (StartAction) EndCB?.Invoke(item);
		else item.Action(EItem_Stage_Card_Action.Move, waittime, EndCB);
		return item;
	}

	public void CreateStageLine(int turn, int Line, Action<int> EndCB = null, bool StartLoad = false) {
		if (!ISEndLine(Line)) return;
		for (int i = 0; i < m_ViewCard[Line].Length; i++) {
			RemoveStage(m_ViewCard[Line][i]);
			m_ViewCard[Line][i] = null;
		}

		float interver = BaseValue.STAGE_INTERVER.x;
		int cnt = 3 + Line * 2;
		float x = (cnt - 1) * interver * -0.5f;
		float fWaitTime = 0.5f;

		float CreateY = BaseValue.STAGE_INTERVER.y;
		if (StartLoad) CreateY *= Line;
		else CreateY *= (float)(BaseValue.STAGE_LINE + 6);


		// 마지막 라인으로 채울 카드가 조건을 만족했는지 확인
		m_Check.Check(StageCheckType.CreateCardLine, 0);
		TStageTable tdata = STAGEINFO.m_TStage;
		List<int> loadcardidxs = new List<int>();
		if (turn > 0) {
			float rate = 1f;

			m_User.m_AddLV += tdata.m_AddEnemyLV;
			m_User.m_AddZombieLV += tdata.m_AddEnemyLV * rate;
		}
		// 튜토리얼 전용 몬스터 강제 셋팅
		if (StartLoad && TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 101, 101, 101 }; break;
				case 1: loadcardidxs = new List<int>() { 101, 101, 101, 101, 102 }; break;
				case 2: loadcardidxs = new List<int>() { 101, 101, 101, 101, 101, 101, 101}; break;
				case 3: loadcardidxs = new List<int>() { 101, 101, 101, 101, 101, 101, 101, 101, 101 }; break;
				case 4: loadcardidxs = new List<int>() { 101, 101, 101, 101, 101, 101, 101, 101, 101, 101, 101 }; break;
				case 5: loadcardidxs = new List<int>() { 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102, 102 }; break;
				case 6: loadcardidxs = new List<int>() { 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105, 105 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 201, 201, 201 }; break;
				case 1: loadcardidxs = new List<int>() { 201, 201, 201, 201, 201 }; break;
				case 2: loadcardidxs = new List<int>() { 202, 202, 202, 202, 202, 202, 202 }; break;
				case 3: loadcardidxs = new List<int>() { 202, 202, 202, 202, 202, 202, 202, 202, 202 }; break;
				case 4: loadcardidxs = new List<int>() { 203, 203, 203, 203, 203, 203, 203, 203, 203, 203, 203 }; break;
				case 5: loadcardidxs = new List<int>() { 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201 }; break;
				case 6: loadcardidxs = new List<int>() { 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201, 201 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 302, 302, 302 }; break;
				case 1: loadcardidxs = new List<int>() { 303, 303, 303, 303, 303 }; break;
				case 2: loadcardidxs = new List<int>() { 301, 301, 301, 304, 301, 301, 301 }; break;
				case 3: loadcardidxs = new List<int>() { 304, 304, 304, 304, 304, 304, 304, 304, 304 }; break;
				case 4: loadcardidxs = new List<int>() { 301, 301, 301, 301, 304, 301, 304, 301, 301, 301, 301 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 401, 401, 401 }; break;
				case 1: loadcardidxs = new List<int>() { 401, 401, 402, 401, 401 }; break;
				case 2: loadcardidxs = new List<int>() { 401, 401, 402, 403, 402, 401, 401 }; break;
				case 3: loadcardidxs = new List<int>() { 401, 401, 401, 401, 402, 401, 401, 401, 401 }; break;
				case 4: loadcardidxs = new List<int>() { 404, 404, 404, 404, 404, 402, 404, 404, 404, 404, 404 }; break;
				case 5: loadcardidxs = new List<int>() { 401, 401, 401, 401, 401, 402, 403, 402, 401, 401, 401, 401, 401 }; break;
				case 6: loadcardidxs = new List<int>() { 401, 401, 401, 401, 401, 401, 401, 402, 401, 401, 401, 401, 401, 401, 401 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 301, 301, 301 }; break;
				case 1: loadcardidxs = new List<int>() { 301, 301, 301, 301, 301 }; break;
				case 2: loadcardidxs = new List<int>() { 303, 303, 303, 303, 303, 303, 303 }; break;
				case 3: loadcardidxs = new List<int>() { 303, 303, 303, 303, 303, 303, 303, 303, 303 }; break;
				case 4: loadcardidxs = new List<int>() { 301, 302, 301, 302, 301, 302, 301, 302, 301, 302, 301 }; break;
				case 5: loadcardidxs = new List<int>() { 301, 302, 301, 302, 301, 302, 301, 302, 301, 302, 301, 302, 301 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 504, 504, 504 }; break;
				case 1: loadcardidxs = new List<int>() { 502, 502, 502, 502, 502 }; break;
				case 2: loadcardidxs = new List<int>() { 501, 501, 501, 501, 501, 501, 501}; break;
				case 3: loadcardidxs = new List<int>() { 505, 505, 505, 505, 505, 505, 505, 505, 505 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 801, 805, 801 }; break;
				case 1: loadcardidxs = new List<int>() { 801, 801, 802, 801, 801 }; break;
				case 2: loadcardidxs = new List<int>() { 805, 805, 805, 805, 805, 805, 805 }; break;
				case 3: loadcardidxs = new List<int>() { 801, 801, 801, 802, 801, 801, 801, 801, 801}; break;
				case 4: loadcardidxs = new List<int>() { 801, 801, 801, 801, 801, 801, 801, 801, 801, 801, 801 }; break;
				case 5: loadcardidxs = new List<int>() { 802, 802, 802, 802, 802, 802, 802, 802, 802, 802, 802, 802, 802 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_301, (int)TutoType_Stage_301.StageStart_Loading)) {
			switch (Line) {
				case 1: loadcardidxs = new List<int>() { 25 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 401, 404, 401 }; break;
				case 1: loadcardidxs = new List<int>() { 401, 401, 401, 401, 401 }; break;
				case 2: loadcardidxs = new List<int>() { 401, 401, 401, 402, 401, 401, 401 }; break;
				case 3: loadcardidxs = new List<int>() { 404, 404, 404, 404, 404, 404, 404, 404, 404 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 1001 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 104, 104, 104 }; break;
				case 1: loadcardidxs = new List<int>() { 101, 101, 101, 101, 101 }; break;
				case 2: loadcardidxs = new List<int>() { 103, 103, 103, 102, 103, 103, 103 }; break;
				case 3: loadcardidxs = new List<int>() { 101, 101, 101, 101, 101, 101, 101, 101, 101 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_403, (int)TutoType_Stage_403.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 305, 305, 305 }; break;
				case 1: loadcardidxs = new List<int>() { 306, 306, 306, 306, 306 }; break;
				case 2: loadcardidxs = new List<int>() { 305, 305, 305, 305, 305, 305, 305 }; break;
				case 3: loadcardidxs = new List<int>() { 305, 306, 305, 306, 305, 306, 305, 306, 305 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_501, (int)TutoType_Stage_501.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 202 }; break;
				case 1: loadcardidxs = new List<int>() { 41 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_601, (int)TutoType_Stage_601.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 214 }; break;
			}
		}
		else if (StartLoad && TUTO.IsTuto(TutoKind.Stage_801, (int)TutoType_Stage_801.StageStart_Loading)) {
			switch (Line) {
				case 0: loadcardidxs = new List<int>() { 230 }; break;
			}
		}
		else if (turn > 0) {
			if (m_CardLastLine < 0) {
				loadcardidxs = new List<int>() { CreateStageLastCardIdx() };
				if (loadcardidxs[0] > 0) m_CardLastLine = Line;
			}
			if (loadcardidxs.Count > 0 && loadcardidxs[0] == 0 && turn > 0) loadcardidxs[0] = CreateStageLineCardIdx(turn);
		}

		for (int Pos = 0, CreateCnt = 0; Pos < cnt; Pos++, x += interver, CreateCnt++) {
			List<StageCardType> ignores = new List<StageCardType>();
			//Roadlock 3개 연달아 못나오게 체크
			if (Pos > 1 && m_ViewCard[Line][Pos - 2].m_Info.IS_RoadBlock && m_ViewCard[Line][Pos - 1].m_Info.IS_RoadBlock) {
				ignores.Add(StageCardType.Roadblock);
				ignores.Add(StageCardType.AllRoadblock);
			}

			CreateStageCard(Line, Pos, new Vector3(x, CreateY, 0), fWaitTime, (obj) => {
				CreateCnt--;
				if (CreateCnt == 0) EndCB?.Invoke(Line);
			}, loadcardidxs.Count < 1 ? 0 : (loadcardidxs.Count > 1 ? loadcardidxs[Pos] : loadcardidxs[0]), StartLoad, ignores.Count > 0 ? ignores : null);
		}
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Card Select

	public void SetDamage(Item_Stage target, int Damage, bool isCri = false) {
		Item_Damage eff = Utile_Class.Instantiate(m_Damage.Prefab, m_Damage.Panel).GetComponent<Item_Damage>();
		eff.SetData(Damage, isCri);
		eff.transform.position = target.transform.position + new Vector3(UTILE.Get_Random(-0.2f, 0.2f), UTILE.Get_Random(-0.2f, 0.2f), 0f);
		eff.GetComponent<SortingGroup>().sortingOrder = target.m_Line == 0 ? 3 : target.m_Line * -1;
	}

	IEnumerator SetSelectCard(Item_Stage card) {
		FirstLineBump(false);
		m_MainUI.SetPathLine();
		m_Area.Clear();
		m_SelectLine = card.m_Line;
		m_SelectPos = card.m_Pos;
		m_PreSelectPos = m_SelectPos;
		m_ActionMovePos = 1;

		///////////////////////////////////////////////////////
		/// 선택한 카드가 적인 경우 습격보다 우선으로 전투 진행
		/// 


		bool enemy = false;
		if (m_SelectStage.m_Info.IS_EnemyCard) {
			enemy = true;
			m_SelectAtk = true;

			m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_RealTData.m_Type);
			if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.Focus_Line_0_1_2) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
			if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_Line_1_0_2) && STAGE_USERINFO.m_TurnCnt == 1) POPUP.RemoveTutoUI();
			yield return SelectAction_StageCardProc(m_SelectStage);

			m_SelectAtk = false;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 0단계 : 습격 체크 및 실행
		yield return SelectAction_Attack(m_SelectStage);
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 1단계 : 첫라인 선택되지 않은 카드 제거 및 중앙이동
		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_Line_0_0_1) && STAGE_USERINFO.m_TurnCnt == 0) {
			List<Item_Stage> activecards = new List<Item_Stage>();
			Item_Stage movecard = m_ViewCard[1][4];
			Item_Stage targetcard = m_ViewCard[1][3];
			m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = movecard;
			m_ViewCard[movecard.m_Line][movecard.m_Pos] = targetcard;
			activecards.Add(targetcard);
			targetcard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
				activecards.Remove(obj);
			}, movecard);

			for(int i = 0; i < 2; i++) {//3,0 5,2 3,8 5,10 // 4,1 6,3 4,9 6,11
				for(int j = 0; j < 9; j++) {
					movecard = m_ViewCard[5 + i][2 + i + j];
					targetcard = m_ViewCard[3 + i][i + j];
					m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = movecard;
					m_ViewCard[movecard.m_Line][movecard.m_Pos] = targetcard;
					activecards.Add(targetcard);
					targetcard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
						activecards.Remove(obj);
					}, movecard);
				}
			}
			yield return new WaitWhile(() => activecards.Count > 0);
		}

		yield return SelectAction_Start(m_SelectStage);
		

		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_Line_0_0_1) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_Line_1_0_2) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Focus_Line_2_0_1) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Focus_Line_0_0_1) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Focus_Line_0) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_105, (int)TutoType_Stage_105.Focus_Line_0) && STAGE_USERINFO.m_TurnCnt == 4) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_105, (int)TutoType_Stage_105.Select_Exit) && STAGE_USERINFO.m_TurnCnt == 5) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.Focus_Line_0_1_1) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Focus_Line_0) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Focus_Line_0) && STAGE_USERINFO.m_TurnCnt == 0) TUTO.Next();
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		/////////////////////////////////////////////////////////
		///선택한 카드가 적이 아닌경우는 습격 끝나고 진행
		if (!enemy && !m_IS_Jumping) {
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			// 2단계 : 카드 진행
			STAGE_USERINFO.CharSpeech(DialogueConditionType.OtherSelect);

			int cnt = 1;
			if(card.m_Info.m_RealTData.m_Type == StageCardType.Material) {//재료에 제한된 선택일 경우 수량 체크
				int matcountdown = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown));
				int matcnt = Mathf.Max(Mathf.RoundToInt(card.m_Info.m_RealTData.m_Value2) - matcountdown, 1);
				cnt = matcnt + card.m_Info.m_PlusCnt;
			}
			m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_RealTData.m_Type, cnt);
			yield return SelectAction_StageCardProc(m_SelectStage);
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		if (m_SelectStage.m_Info.m_TData.IS_LineCard() && STAGEINFO.m_Result == StageResult.None) {
			StageFail(StageFailKind.OtherMission);
			yield break;
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 3단계 : 카드 AI 발동
		yield return SelectAction_StageCardAI();

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 4단계 : 카드 제거및 라인 위치로 이동
		yield return SelectAction_End(m_SelectStage);

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		// 피난민등 자동으로 내려오게 하기위해 체크
		yield return Check_NullCardAction();
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		bool ISGuard = m_User.GetSynergeValue(JobType.Guard, 0) != null;
		if (ISGuard && STAGEINFO.m_TStage.GetDarkLv > 0) {
			STAGE_USERINFO.ActivateSynergy(JobType.Guard);
			Utile_Class.DebugLog_Value("Guard 시너지 발동 한줄 더 밝힘");
		}

		m_Area.Clear();
		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_1) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_2) && STAGE_USERINFO.m_TurnCnt == 2) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_3) && STAGE_USERINFO.m_TurnCnt == 3) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_3) && STAGE_USERINFO.m_TurnCnt == 4) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_2) && STAGE_USERINFO.m_TurnCnt == 2) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.FREE_Play_3) && STAGE_USERINFO.m_TurnCnt == 3) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.FREE_Play) && STAGE_USERINFO.m_TurnCnt == 2) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_105, (int)TutoType_Stage_105.FREE_Play_1) && STAGE_USERINFO.m_TurnCnt == 4) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_105, (int)TutoType_Stage_105.Machingun_Action_End) && STAGE_USERINFO.m_TurnCnt == 5) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.FREE_Play_1) && STAGE_USERINFO.m_TurnCnt == 2) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.FREE_Play_1) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.FREE_Play_1) && STAGE_USERINFO.m_TurnCnt == 2) TUTO.Next();
		m_IS_KillFirstLine = false;

		//N턴마다 줄 넘김
		yield return SetSkipTurn();

		m_IS_Jumping = false;
		// 피난민등 자동으로 내려오게 하기위해 체크
		yield return Check_NullCardAction();

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		//첫줄 선택시 방향 표시
		m_MainUI.SetPathLine(m_ViewCard[0][0], m_ViewCard[0][1], m_ViewCard[0][2]);

		//첫줄 카드 락
		yield return StageCardLock();

		//턴당 킬 수 초기화
		m_OneTurnKillCnt = 0;

		m_StageAction = null;

		//N턴 마다 0번라인이 자동으로 선택됨
		if (STAGE_USERINFO.ISBuff(StageCardType.ConRandomChoice) && m_User.m_TurnCnt % Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConRandomChoice)) == 0) {
			List<int> idx = new List<int>() { 0, 1, 2 };
			for (int i = 0; i < 3; i++) {
				int row = idx[UTILE.Get_Random(0, idx.Count)];
				idx.Remove(row);
				if (m_ViewCard[0][row] == null) continue;
				if (m_ViewCard[0][row].IS_Die()) continue;
				if (m_ViewCard[0][row].m_Info.IS_RoadBlock) continue;
				if (m_ViewCard[0][row].IS_Lock) continue;
				m_StageAction = SetSelectCard(m_ViewCard[0][row]);
				yield return m_StageAction;
				m_StageAction = null;
				yield break;
			}
		}

		FirstLineBump(true);
		if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Delay_NextLine) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_403, (int)TutoType_Stage_403.FREE_Play) && STAGE_USERINFO.m_TurnCnt == 1) TUTO.Next();
	}

	IEnumerator BattleStartAction() {
		float movetime = 0.3f;
		GameObject panel = m_Stage.ScalePanel.gameObject;
		iTween.ScaleTo(panel, iTween.Hash("scale", Vector3.zero, "time", movetime, "easetype", "easeOutQuad"));
		iTween.ValueTo(panel, iTween.Hash("from", panel.GetComponent<RenderAlpha_Controller>().Alpha, "to", 0f, "time", movetime, "easetype", "easeInQuart", "onupdate", "SetAlpha"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(panel));
	}
	IEnumerator BattleEndAction() {
		float movetime = 0.3f;
		GameObject panel = m_Stage.ScalePanel.gameObject;
		iTween.ScaleTo(panel, iTween.Hash("scale", Vector3.one, "time", movetime, "easetype", "easeOutQuad"));
		iTween.ValueTo(panel, iTween.Hash("from", panel.GetComponent<RenderAlpha_Controller>().Alpha, "to", 1f, "time", movetime, "easetype", "easeInQuart", "onupdate", "SetAlpha"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(panel));
	}

	/// <summary> 0단계 : 습격 </summary>
	IEnumerator SelectAction_Attack(Item_Stage card, List<int> _pos = null) {
		//if (STAGEINFO.m_Mode == ModeType.ZHorde) yield break;
		if (m_AutoPlay) yield break;
		//if (TUTO.IsTutoPlay()) yield break;
		if (m_IS_Jumping) yield break;
		if (STAGE_USERINFO.GetBuffValue(StageCardType.Hide) > 0f) yield break;

			Item_Stage enemy = null;
		List<Item_Stage> AutoBattleEnemy = new List<Item_Stage>();
		List<int> prepos = new List<int>();//적일때 위에 카드 내려와도 습격 안당하게 처리
		if (_pos != null) prepos = _pos;
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage temp = m_ViewCard[0][i];
			if (temp == null) continue;
			if (temp == card) continue;
			if (temp.IS_Die()) continue;
			if (!temp.m_Info.IS_EnemyCard) continue;
			if (temp.m_Info.ISRefugee) continue;
			if (prepos.Contains(i)) continue;//적일때 위에 카드 내려와도 습격 안당하게 처리
			AutoBattleEnemy.Add(temp);
		}
		if (AutoBattleEnemy.Count > 0) {
			AutoBattleEnemy.Sort((Item_Stage _a, Item_Stage _b) => {
				if (_a.m_Info.m_TEnemyData.GetStat(EEnemyStat.SPD) > _b.m_Info.m_TEnemyData.GetStat(EEnemyStat.SPD)) return -1;
				else if (_a.m_Info.m_TEnemyData.GetStat(EEnemyStat.SPD) < _b.m_Info.m_TEnemyData.GetStat(EEnemyStat.SPD)) return 1;
				return 0;
			});
			enemy = AutoBattleEnemy[0];
			prepos.Add(enemy.m_Pos);//적 위치 다음 적전투로 전달
		}
		if (enemy == null) yield break;

		if (enemy.m_Info.m_TData.m_Type == StageCardType.Hive) {
			StageCardInfo info = enemy.m_Info;
			bool Action = true;
			enemy.SetCardChange(Mathf.RoundToInt(info.m_TData.m_Value2));
			enemy.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => {
				Action = false;
			});
			yield return new WaitWhile(() => Action);
		}
		yield return StartBattle(EBattleMode.EnemyAtk, enemy);
		// 전투 종료까지 대기

		yield return new WaitWhile(() => IS_SelectAction_Pause());
#if STAGE_NO_BATTLE
		card.IS_KilledDuel = true;
		BATTLEINFO.m_Result = EBattleResult.WIN;
#endif
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
		if (BATTLEINFO.m_Result == EBattleResult.WIN) {
			enemy.Action(EItem_Stage_Card_Action.Die);
			int lv = enemy.m_Info.m_TEnemyData.m_RewardLV;
			int rewardgid = enemy.m_Info.m_TEnemyData.m_RewardGID;
			bool allgroup = enemy.m_Info.m_TEnemyData.m_AllGroup;
			bool cancanble = enemy.m_Info.m_TEnemyData.m_RewardCancle;
			yield return new WaitUntil(() => enemy.IS_NoAction);

			if (enemy.m_Info.m_TEnemyData.m_Grade == EEnemyGrade.Elite)
				yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup, true, -1, enemy.transform.position, true);
			else
				yield return Action_BattleReward_Rand(enemy, rewardgid, lv, allgroup);

			//yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup);
			yield return Check_DieCardAction();
			yield return SelectAction_Attack(card, prepos);
		}

		yield return new WaitWhile(() => IS_SelectAction_Pause());
	}

	/// <summary> 1단계 : 첫라인 선택되지 않은 카드 제거 및 중앙이동 </summary>
	IEnumerator SelectAction_Start(Item_Stage card) {
		m_ActionMovePos = m_SelectPos;
		float MoveX = 0;
		switch (m_SelectPos) {
			case 0: MoveX = BaseValue.STAGE_INTERVER.x; break;
			case 2: MoveX = -BaseValue.STAGE_INTERVER.x; break;
		}
		GameObject Activepanel = m_Stage.Panel[1].gameObject;
		iTween.MoveTo(Activepanel, iTween.Hash("x", MoveX, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "linear", "islocal", true));

		// 0번째라인 가운데로 맞추기 및 선택되지 않은 다른카드 제거
		float x = m_ViewCard[0][card.m_Pos].transform.localPosition.x + MoveX;
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage item = m_ViewCard[0][i];
			if (item == null) continue;
			if (!item.IS_FadeIn) continue;
			//if(card.m_Info.m_NowTData.m_Type != StageCardType.Item_RewardBox)
				iTween.MoveTo(item.gameObject, iTween.Hash("x", item.transform.localPosition.x - x, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "linear", "islocal", true));
			if (item != card) item.Action(EItem_Stage_Card_Action.FadeOut);
		}

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Activepanel));
	}

	/// <summary> 4단계 : 카드 제거및 라인 위치로 이동 </summary>
	IEnumerator SelectAction_End(Item_Stage card) {

		if (m_CardLastLine > -1) m_CardLastLine--;
		// 카드 pool 이동
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			RemoveStage(m_ViewCard[0][i]);
			m_ViewCard[0][i] = null;
		}

		float moveX = m_Stage.Panel[1].localPosition.x;
		m_Stage.Panel[1].localPosition = Vector3.zero;
		List<Item_Stage> removecards = new List<Item_Stage>();
		for (int j = 1, Start = card.m_Pos, End = Start + 3; j < m_ViewCard.Length; j++, End += 2) {
			for (int i = 0, Offset = 0; i < m_ViewCard[j].Length; i++) {
				Item_Stage TempCard = m_ViewCard[j][i];
				if (TempCard == null) continue;
				Vector3 pos = TempCard.transform.localPosition;
				pos.x += moveX;
				TempCard.transform.localPosition = pos;
				if (i < Start || i >= End) {
					removecards.Add(TempCard);
					TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
						// 카드 pool 이동
						RemoveStage(obj);
						removecards.Remove(obj);
					});
				}
				else {
					int line = j - 1;
					TempCard.SetPos(line, Offset);
					m_ViewCard[line][Offset] = TempCard;
					Offset++;
				}
				m_ViewCard[j][i] = null;
			}
		};

		SetMoveAddY(BaseValue.STAGE_INTERVER.y);
		CheckAIStopInfoTurn(m_SelectPos);
		CheckAiBlockRangeAtkInfoTurn(m_SelectPos);
		CheckBurnInfoTurn(m_SelectPos);
		CheckStreetLightInfoTurn(m_SelectPos);
		yield return CheckLightTurn(m_SelectPos);
		yield return new WaitWhile(() => removecards.Count > 0);
		int select = m_SelectPos;
		// 라인 생성의 갭을 맞추기위해 선택 카드 변경
		m_ActionMovePos = m_SelectPos = 1;

		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 새로운 라인 추가
		for (int j = 0, jmax = m_ViewCard.Length - 1; j < jmax; j++) {
			for (int i = 0; i < m_ViewCard[j].Length; i++) {
				Item_Stage TempCard = m_ViewCard[j][i];
				if (TempCard == null) continue;
				//if (TempCard.gameObject.activeInHierarchy) continue;
				actioncards.Add(TempCard);
				//yield return new WaitUntil(() => TempCard.gameObject.activeInHierarchy);
				TempCard.Action(EItem_Stage_Card_Action.Move, 0f, (obj) => {
					actioncards.Remove(obj);
				});

			}
		}
		iTween.ValueTo(gameObject, iTween.Hash("from", m_MoveAddY, "to", 0, "time", BaseValue.STAGE_MOVE_TIME, "easetype", "easeInQuart", "onupdate", "SetMoveAddY"));
		PlayEffSound(SND_IDX.SFX_0208);
		CreateStageLine(m_User.m_TurnCnt + 1 + (m_ViewCard.Length - 1), m_ViewCard.Length - 1);
		yield return new WaitWhile(() => actioncards.Count > 0);
		SetMoveAddY(0);
		yield return AddTime();
	}

	void GetRefugee(Item_Stage card) {
		StageCardInfo info = card.m_Info;
		TStageCardTable tdata = info.m_TData;
		float value = 0f, times;
		if (!CheckEnd()) {
			m_Check.Check(StageCheckType.Rescue, info.m_EnemyIdx);
			if(info.m_TEnemyData.IS_BadRefugee())
				m_Check.Check(StageCheckType.Rescue_Infectee, info.m_EnemyIdx);
			else
				m_Check.Check(StageCheckType.Rescue_Refugee, info.m_EnemyIdx);
		}
		//SND.StopAllVoice();
		PlayVoiceSnd(info.m_TEnemyData.m_RescueVoice);
		times = 1f;
		switch (info.m_TEnemyData.m_Type) {
			case EEnemyType.SatRefugee:
				value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.SatRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(value), StatType.None, 0 , card.m_Info));
				break;
			case EEnemyType.MenRefugee:
				value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.MenRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
				break;
			case EEnemyType.HygRefugee:
				value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.HygRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
				break;
			case EEnemyType.HpRefugee:
				value = m_User.GetStat(StatType.Heal) * TDATA.GetConfig_Float(ConfigType.HpRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
				break;
			case EEnemyType.RandomRefugee:
				List<StatType> stat = new List<StatType>() { StatType.Men, StatType.Hyg, StatType.Sat, StatType.HP };
				for (int i = 0; i < 2; i++) {
					StatType randstat = stat[UTILE.Get_Random(0, stat.Count)];
					stat.Remove(randstat);
					switch (randstat) {
						case StatType.Men:
							value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.RandomRefugeeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
							break;
						case StatType.Hyg:
							value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.RandomRefugeeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
							break;
						case StatType.Sat:
							value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.RandomRefugeeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
							break;
						case StatType.HP:
							value = m_User.GetStat(StatType.Heal) * TDATA.GetConfig_Float(ConfigType.RandomRefugeeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(value), StatType.None, 0, card.m_Info));
							break;
					}
				}
				break;
			case EEnemyType.AllRefugee:
				value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.AllRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(value)));

				value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.AllRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(value)));

				value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.AllRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(value)));

				value = m_User.GetStat(StatType.Heal) * TDATA.GetConfig_Float(ConfigType.AllRefugeeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(value)));
				break;
			case EEnemyType.SatInfectee:
				value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.SatInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(-value)));
				break;
			case EEnemyType.MenInfectee:
				value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.MenInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(-value)));
				break;
			case EEnemyType.HygInfectee:
				value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.HygInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(-value)));
				break;
			case EEnemyType.HpInfectee:
				value = m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.HpInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(-value)));
				break;
			case EEnemyType.RandomInfectee:
				stat = new List<StatType>() { StatType.Men, StatType.Hyg, StatType.Sat, StatType.HP };
				for (int i = 0; i < 2; i++) {
					StatType randstat = stat[UTILE.Get_Random(0, stat.Count)];
					stat.Remove(randstat);
					switch (randstat) {
						case StatType.Men:
							value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.RandomInfecteeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(-value)));
							break;
						case StatType.Hyg:
							value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.RandomInfecteeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(-value)));
							break;
						case StatType.Sat:
							value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.RandomInfecteeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(-value)));
							break;
						case StatType.HP:
							value = m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.RandomInfecteeCharge) * times;
							StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(-value)));
							break;
					}
				}
				break;
			case EEnemyType.Allinfectee:
				value = m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.AllInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(-value)));

				value = m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.AllInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(-value)));

				value = m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.AllInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(-value)));

				value = m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.AllInfecteeCharge) * times;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(-value)));
				break;
			case EEnemyType.MaterialRefugee:
				float rand = UTILE.Get_Random(0f, 1f);
				int cnt = 0;
				if (rand < 0.5f) cnt = 1;
				else if (rand < 0.8f) cnt = 2;
				else if (rand < 1f) cnt = 3;
				AddMaterial((StageMaterialType)info.m_TEnemyData.m_RewardGID, cnt);
				break;
		}
		//대사
		STAGE_USERINFO.CharSpeech(DialogueConditionType.RescueRefugee);
	}

	Dictionary<Transform, EF_BuffCenterAlarm> m_AddStatAlarm = new Dictionary<Transform, EF_BuffCenterAlarm>();

	public void Call_AddStat_Action(Transform pos, StatType type1, int value1, StatType type2 = StatType.None, int value2 = 0) {
		StartCoroutine(AddStat_Action(pos, type1, value1, type2, value2));
	}
	public IEnumerator AddStat_Action(Transform pos, StatType type1, int value1, StatType type2 = StatType.None, int value2 = 0, StageCardInfo _info = null, string _name = null) {
		//if (!STAGE_USERINFO.Is_UseStat(type1)) yield break;//안쓰는 스텟은 동작 안하게
		if(type1 == StatType.HP && value1 > 0) {
			//청결도에 따른 디버프
			if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusHpRecovery)) {
				value1 -= Mathf.RoundToInt(value1 * STAGE_USERINFO.m_DebuffValues[DebuffType.MinusHpRecovery]);
			}
		}
		value1 = m_User.CalcDropStatValue(type1, value1);
		if (value1 == 0) yield break;

		if (STAGE_USERINFO.Is_UseStat(type1) || (type2 != StatType.None && STAGE_USERINFO.Is_UseStat(type2))) {
			if (m_AddStatAlarm.ContainsKey(pos) && m_AddStatAlarm[pos] == null) m_AddStatAlarm.Remove(pos);
			if (!m_AddStatAlarm.ContainsKey(pos)) {
				m_AddStatAlarm.Add(pos, UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>());
			}
		}

		if (STAGE_USERINFO.Is_UseStat(type1)) {
			value1 = AddStat(type1, value1, value1 > 0);
			m_AddStatAlarm[pos].SetData(pos, type1, value1, _info, _name); 
		}

		if (type2 != StatType.None && STAGE_USERINFO.Is_UseStat(type2))
		{
			if (type2 == StatType.HP && value2 > 0) {
				//청결도에 따른 디버프
				if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusHpRecovery)) {
					value2 -= Mathf.RoundToInt(value2 * STAGE_USERINFO.m_DebuffValues[DebuffType.MinusHpRecovery]);
				}
			}
			value2 = m_User.CalcDropStatValue(type2, value2);
			AddStat(type2, value2, value2 > 0);
			m_AddStatAlarm[pos].SetData(pos, type2, value2, _info, _name);
		}
	}

	/// <summary> 카드 세팅이 다 끝난 후 cardlock 모드면 0라인 3장중 N장 선택불가 </summary>
	IEnumerator Debuff_CardLock(int _cnt = 1) {
		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<int> pos = new List<int>();
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			if (!m_ViewCard[0][i].IS_Lock && !m_ViewCard[0][i].m_Info.IS_RoadBlock)
				pos.Add(i);
		}

		if (pos.Count < 2) yield break;

		for (int i = 0; i < _cnt; i++) {
			if (pos.Count < 2) yield break;
			int randpos = pos[UTILE.Get_Random(0, pos.Count)];
			pos.Remove(randpos);
			Item_Stage card = m_ViewCard[0][randpos];
			if (card == null) continue;
			actioncards.Add(card);
			card.Action(EItem_Stage_Card_Action.Lock, 0, (obj) => {
				actioncards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => actioncards.Count > 0);
	}

	IEnumerator StageCardLock() {
		//카드 세팅이 다 끝난 후 cardlock 모드면 0라인 3장중 N장 선택불가
		if (STAGEINFO.m_TStage.GetMode(PlayType.EasyCardLock) != null)
			yield return Mode_EasyCardLock();
		//카드 세팅이 다 끝난 후 cardlock 모드면 0라인 3장중 N장 선택불가
		if (STAGEINFO.m_TStage.GetMode(PlayType.CardLock) != null)
			yield return Mode_CardLock();
		//카드 세팅이 다 끝난 후 멘탈디버프 cardlock 이면 1장 남기고 unlock 카드중 1장 선택 불가
		if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.CardLock) && UTILE.Get_Random(0f, 1f) < STAGE_USERINFO.m_DebuffValues[DebuffType.CardLock])
			yield return Debuff_CardLock();
		//매턴 버프에 stagecardlock 있으면 첫줄 카드중 최소 1장 남기고 선택 불가
		if (STAGE_USERINFO.ISBuff(StageCardType.ConStageCardLock)) {
			int lockcnt = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConStageCardLock));
			yield return Debuff_CardLock(lockcnt);
		}
	}
	public void AllRefreshCardImgName() {
		m_StageAction = Debuff_AllRefreshCardImgName();
		StartCoroutine(m_StageAction);
	}
	IEnumerator Debuff_AllRefreshCardImgName() {
		List<Item_Stage> actioncards = new List<Item_Stage>();

		for (int i = 0; i < STAGE.m_ViewCard.Length; i++) {
			for (int j = 0; j < STAGE.m_ViewCard[i].Length; j++) {
				Item_Stage card = STAGE.m_ViewCard[i][j];
				if (card == null) continue;
				if (card.IS_Die()) continue;
				if (card.m_Info.IsDark) continue;
				if (card.m_Info.m_TData.m_Type == StageCardType.Material) {
					actioncards.Add(card);
					card.Action(EItem_Stage_Card_Action.RefreshImgName, 0f, (t)=> { actioncards.Remove(t); });
				}
			}
		}
		yield return new WaitWhile(() => actioncards.Count > 0);
		m_StageAction = null;
	}

	IEnumerator SetSkipTurn() {
		//N턴 마다 첫줄을 그냥 넘김
		if (STAGE_USERINFO.ISBuff(StageCardType.ConSkipTurn) && m_User.m_TurnCnt % Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConSkipTurn)) == 0) {
			if (m_CardLastLine == 0) yield break;

			List<Item_Stage> removecards = new List<Item_Stage>();
			for (int i = 0; i < m_ViewCard[0].Length; i++) {
				Item_Stage TempCard = m_ViewCard[0][i];
				removecards.Add(TempCard);
				TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
					// 카드 pool 이동
					RemoveStage(obj);
					removecards.Remove(obj);
				});
				m_ViewCard[0][i] = null;
			}

			for (int j = 1, Start = 1, End = Start + 3; j < m_ViewCard.Length; j++, End += 2) {
				for (int i = 0, Offset = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					if (i < Start || i >= End) {
						removecards.Add(TempCard);
						TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
							// 카드 pool 이동
							RemoveStage(obj);
							removecards.Remove(obj);
						});
					}
					else {
						int line = j - 1;
						TempCard.SetPos(line, Offset);
						m_ViewCard[line][Offset] = TempCard;
						Offset++;
					}
					m_ViewCard[j][i] = null;
				}
			}

			yield return new WaitWhile(() => removecards.Count > 0);

			List<Item_Stage> actioncards = new List<Item_Stage>();
			// 새로운 라인 추가
			for (int j = 0, jmax = m_ViewCard.Length - 1; j < jmax; j++) {
				for (int i = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					actioncards.Add(TempCard);
					TempCard.Action(EItem_Stage_Card_Action.Move, 0f, (obj) => {
						actioncards.Remove(obj);
					});
				}
			}

			CreateStageLine(0, m_ViewCard.Length - 1);
			yield return new WaitWhile(() => actioncards.Count > 0);
		}
	}

	/// <summary>
	/// 모든 에너미 변경
	/// </summary>
	/// <param name="_idx"></param>
	/// <returns></returns>
	public IEnumerator SetAllEnemyChange(int _idx) {
		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = 0; i < m_ViewCard.Length; i++) {
			for(int j = 0; j < m_ViewCard[i].Length; j++) {
				Item_Stage card = m_ViewCard[i][j];
				if (card == null) continue;
				if (!card.m_Info.IS_EnemyCard) continue;
				if (card.m_Info.ISRefugee) continue;
				if (card.m_Info.m_EnemyIdx == _idx) continue;
				if (card.IS_Die()) continue;

				card.SetEnemyChange(_idx);
				offcards.Add(card);
				card.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => {
					offcards.Remove(obj);
				});
			}
		}
		yield return new WaitWhile(() => offcards.Count > 0);
	}
}
