using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Stage_FailCause : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform CloneBucket;
		public Transform FX;
	}
	[SerializeField]
	SUI m_SUI;
	StageFailKind m_FailKind;
	GameObject[] CloneObj = new GameObject[2];//0:원본, 1:복제된거
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_FailKind = (StageFailKind)aobjValue[0];
		if (aobjValue.Length > 1) CloneObj[0] = (GameObject)aobjValue[1];


		StartCoroutine(ViewCause());
		//FireBase-Analytics
		STAGEINFO.StageStatisticsLog(m_FailKind);
		STAGEINFO.StageFailAnalytics(m_FailKind);
	}

	/// <summary> 실패 원인 보여주기</summary>
	IEnumerator ViewCause()
	{
		//상황에 맞는 유아이 복제해서 세팅하고 그 위치에 이펙트 옮기기
		switch (m_FailKind)
		{
		/// <summary> 스테이지 실패 조건 으로 패배 </summary>
		case StageFailKind.FailType: Close(); yield break;
		/// <summary> 턴 카운트 제한 </summary>
		case StageFailKind.Turn:
			if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training)
			{//훈련
			 //훈련은 훈련 팝업에서 직접 인자 전달함, 체크박스 가져옴
			}
			else
			{
				CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetClockObj;
			}
			break;
		/// <summary> HP 0 </summary>
		case StageFailKind.HP:
				if (POPUP.GetBattleUI() == null) {
					CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetHpObj;
					CloneObj[0].SetActive(true);
				}
				else CloneObj[0] = POPUP.GetBattleUI().GetComponent<BattleUI>().GetHpObj;
			break;
		/// <summary> 정신 0 </summary>
		case StageFailKind.Men:
			if (POPUP.GetBattleUI() == null)    CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetStatObj(StatType.Men);
			else                                CloneObj[0] = POPUP.GetBattleUI().GetComponent<BattleUI>().GetStatObj(StatType.Men);
			break;
		/// <summary> 위생 0 </summary>
		case StageFailKind.Hyg:
			if (POPUP.GetBattleUI() == null)    CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetStatObj(StatType.Hyg);
			else                                CloneObj[0] = POPUP.GetBattleUI().GetComponent<BattleUI>().GetStatObj(StatType.Hyg);
			break;
		/// <summary> 포만감 0 </summary>
		case StageFailKind.Sat:
			if (POPUP.GetBattleUI() == null)    CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetStatObj(StatType.Sat);
			else                                CloneObj[0] = POPUP.GetBattleUI().GetComponent<BattleUI>().GetStatObj(StatType.Sat);
			break;
		/// <summary> 훈련 시간제한 </summary>
		case StageFailKind.Time:
			if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training)
			{//훈련
			 //훈련은 훈련 팝업에서 직접 인자 전달함, 시계 가져옴
			}
			else
			{//일반 스테이지
				if (POPUP.GetBattleUI() == null)	CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetTimerObj;
				else								CloneObj[0] = POPUP.GetBattleUI().GetComponent<BattleUI>().GetTimerObj; 
			}
			break;
		//트레이닝이냐 아니냐로 불러올 유아이 달라질듯
		/// <summary> 전투 횟수 제한 </summary>
		case StageFailKind.TurmoilCount:
			CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetModeAlarmObj(PlayType.TurmoilCount);
			break;
			case StageFailKind.OtherMission:
				CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_Stage>().GetMissionGuideObj;
				break;
		}
		if (CloneObj[0] != null)
		{
			//트위너는 원래거를 제거하고 한프레임 넘겨야 에러 안남
			iTween.Stop(CloneObj[0]);
			yield return new WaitForEndOfFrame();
			CloneObj[1] = Utile_Class.Instantiate(CloneObj[0], m_SUI.CloneBucket);
			///트워너, 애니나 캔버스 그룹, 레이어엘리먼트 등 강제로 트랜스폼을 잡거나 하는것들 다 꺼버림
			if (CloneObj[1].GetComponent<Animator>()) CloneObj[1].GetComponent<Animator>().enabled = false;
			if (CloneObj[1].GetComponent<CanvasGroup>()) CloneObj[1].GetComponent<CanvasGroup>().enabled = false;
			if (CloneObj[1].GetComponent<LayoutElement>()) CloneObj[1].GetComponent<LayoutElement>().enabled = false;
			iTween.Stop(CloneObj[1]);
			CloneObj[1].transform.eulerAngles = CloneObj[0].transform.eulerAngles;
			CloneObj[1].transform.localScale = CloneObj[0].transform.localScale;
			CloneObj[1].transform.position = CloneObj[0].transform.position;
			//CloneObj[1].GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
			yield return new WaitForEndOfFrame();
			CloneObj[1].gameObject.SetActive(true);
		}
		if (CloneObj[1] != null) m_SUI.FX.position = CloneObj[1].transform.position;

		//유아이 연출중이라 갱신 안되는것들만 따로 추림
		switch (m_FailKind)
		{
		/// <summary> 스테이지 실패 조건 으로 패배 </summary>
		case StageFailKind.Turn: break;
		/// <summary> HP 0 </summary>
		case StageFailKind.HP:
			for(int i = 0;i<2;i++) CloneObj[i].GetComponent<Item_HPUI>().SetData(0f, 0f, STAGE_USERINFO.GetMaxStat(StatType.HP));
			break;
		/// <summary> 정신 0 </summary>
		case StageFailKind.Men:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat>().SetData(0, STAGE_USERINFO.GetMaxStat(StatType.Men));
			break;
		/// <summary> 위생 0 </summary>
		case StageFailKind.Hyg:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat>().SetData(0, STAGE_USERINFO.GetMaxStat(StatType.Hyg));
			break;
		/// <summary> 포만감 0 </summary>
		case StageFailKind.Sat:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat>().SetData(0, STAGE_USERINFO.GetMaxStat(StatType.Sat));
			break;
		/// <summary> 훈련 시간제한 </summary>
		case StageFailKind.Time: break;
		/// <summary> 전투 횟수 제한 </summary>
		case StageFailKind.TurmoilCount: break;
		}
		yield return new WaitForSecondsRealtime(2f);
		Close();
	}
}
