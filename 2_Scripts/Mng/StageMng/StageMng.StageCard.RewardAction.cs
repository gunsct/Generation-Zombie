using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Reward Process Action
	IEnumerator RewardAction_Proc(int RewardIdx, int _line = -1, int _pos = -1, Vector3 _spos = default, Vector3 _sscale = default)
	{
		int xpos = _pos == -1 ? m_ActionMovePos : _pos;
		int ypos = _line == -1 ? 0 : _line;
		m_IS_KillFirstLine = true;//첫라인 죽였을 경우
		List<Item_Stage> fadeCards = new List<Item_Stage>();
		List<Item_Stage> actioncards = new List<Item_Stage>();
		float movetime = 0.3f;//0.5->0.3
		float interverX = BaseValue.STAGE_INTERVER.x;
		float interverY = BaseValue.STAGE_INTERVER.y;
		float x = -interverX + interverX * xpos;
		float y = interverY * ypos;
		Item_Stage card = CreateStageCard(ypos, xpos, new Vector3(x, y, 0), RewardIdx, m_Stage.ActionPanel.transform);
		card.SetFrame(1);

		m_SelectVirture = card;
		card.GetComponent<SortingGroup>().sortingOrder = 4;

		if (_spos != default) {
			card.transform.localPosition = _spos;
			card.transform.localScale = _sscale;
		}
		else {
			float Scale = BaseValue.STAGE_SELECT_LINE_SCALE.x;
			Vector3 v3SPos = card.transform.localPosition;
			Vector3 v3EPos = new Vector3(v3SPos.x * Scale, v3SPos.y - (BaseValue.STAGE_INTERVER.y * Scale * (m_IS_KillFirstLine ? 2 : 1) - BaseValue.STAGE_INTERVER.y), 0f);
			//Vector3 v3GPos = v3EPos - v3SPos;
			//Vector3 v3SScale = card.transform.localScale;
			//Vector3 v3GScale = BaseValue.STAGE_SELECT_LINE_SCALE - v3SScale;

			card.transform.localScale = BaseValue.STAGE_SELECT_LINE_SCALE;
			float MoveX = 0;
			switch (xpos) {
				case 0: MoveX = BaseValue.STAGE_INTERVER.x; break;
				case 2: MoveX = -BaseValue.STAGE_INTERVER.x; break;
			}
			v3EPos.x -= v3EPos.x + MoveX;
			card.transform.localPosition = v3EPos;
		}
		Vector3 lopo = card.transform.localPosition;
		Vector3 losc = card.transform.localScale;

		card.ActiveDark(false);
		yield return new WaitForSeconds(0.75f);
		actioncards.Add(card);
		card.Action(EItem_Stage_Card_Action.FadeIn, 0f, (obj) =>
		{
			actioncards.Remove(obj);
		}, movetime);
		// 첫줄에서 죽이고 보상으로 유틸을 쓴다면 첫줄도 포함한다
		// 중앙에 있는 카드들중 중앙 카드들만 제외하고 꺼준다.
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage temp = m_ViewCard[0][i];
			if (temp == null || !temp.IS_FadeIn) continue;
			if (!temp.IS_NoAction) continue;
			if (!m_IS_KillFirstLine) {
				fadeCards.Add(temp);
				actioncards.Add(temp);
				temp.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
					actioncards.Remove(obj);
				}, movetime);
			}
			else {
				actioncards.Add(temp);
				temp.Action(EItem_Stage_Card_Action.TargetOff, 0, (obj) => {
					actioncards.Remove(obj);
				}, movetime);
			}
		}
		yield return new WaitWhile(() => actioncards.Count > 0);

		//if (card.m_Info.m_NowTData.IS_UtileCard()) {
		//	card.Action(EItem_Stage_Card_Action.FadeOut);
		//}

		bool isEndAction = true;
		if (card.m_Info.m_NowTData.m_Type != StageCardType.Material && !card.m_Info.m_NowTData.IS_BuffCard()) {
			isEndAction = false;
			card.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
				isEndAction = true;
			}, m_CenterChar.transform.position);//m_Chars[2 -> 0]
		}

		yield return new WaitWhile(() => !isEndAction);

		if (card.m_Info.m_NowTData.IS_UtileCard()) {
			float actiontime = 0.2f;//0.3->0.2
			card = CreateStageCard(-1, 1, new Vector3(0, -8.72f, 0), RewardIdx, m_Stage.ActionPanel.transform);
			m_SelectVirture = card;
			card.transform.localScale = BaseValue.STAGE_SELECT_LINE_SCALE;
			//**범위가 예외인것들이 있음 여기서만 따로 예외처리 해줘야함
			if (card.m_Info.m_NowTData.m_Type == StageCardType.C4) card.m_Line = 0;
			card.GetComponent<SortingGroup>().sortingOrder = 4;
			card.ActiveDark(false);
			card.Action(EItem_Stage_Card_Action.FadeIn, 0, null, actiontime);
			//card = vircard;
		}

		yield return SelectAction_StageCardProc(card);

		yield return new WaitWhile(() => IS_SelectAction_Pause());

		//if (card != null && card.m_Info.m_NowTData.IS_UtileCard()) {
		//	card.Action(EItem_Stage_Card_Action.FadeIn);
		//	yield return new WaitWhile(() => !card.IS_NoAction);
		//}

		// 중간 대전이 
		//생성된 유틸카드 제거
		//RemoveStage(card);
		//actioncards.Remove(card);
		//사용 불가였던 생성된 카드 제거
		//**card가 0라인 타겟때문에 Make 함수를 끌어다 써서 MakingActionCardRemove에서 destroy될 수 있으니 있는 경우만 진행
		if (card != null) {
			actioncards.Add(card);
			card.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
				//RemoveStage(card);
				actioncards.Remove(obj);
				Destroy(card.gameObject);
			}, movetime);
		}
		yield return new WaitWhile(() => actioncards.Count > 0);
		// 중앙에 있는 카드들중 중앙 카드들만 제외하고 꺼준다.
		for (int i = 0; i < fadeCards.Count; i++)
		{
			Item_Stage temp = fadeCards[i];
			actioncards.Add(temp);
			temp.Action(EItem_Stage_Card_Action.FadeIn, 0, (obj) =>
			{
				actioncards.Remove(obj);
			}, movetime);
		}

		yield return new WaitWhile(() => actioncards.Count > 0);
		m_IS_KillFirstLine = false;
	}
}
