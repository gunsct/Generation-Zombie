using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Training : PopupBase
{
#pragma warning disable 0649
	[Serializable]
	public struct SUI
	{
		public RectTransform Panel;
		public Animator Ani;
		public Image PauseBtnIcon;
		public Sprite[] PauseSprite;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Lv;
		public Animator BigLife;
		public Animator[] Lifes;
		public Animator[] SuccChecks;
		public TextMeshProUGUI[] Timer;
		public Animator TimerAnim;
		public HorizontalLayoutGroup LifeLayout;
		public GameObject LifePanel;
		public GameObject TimerPanel;
	}
	[SerializeField] SUI m_SUI;

	Item_MainMenu_Stg_Bg m_BG;
	Item_Training m_Training;
	int m_Idx;
	int m_MaxCnt, m_ClearCnt;
	int[] m_CheckCnt = new int[2];//0:성공, 1:실패
	bool m_IsStageMode;
	bool Is_Pause;
	string m_TimerTrig;
	TTrainingTable m_TData { get { return TDATA.GetTrainingTable(m_Idx); } }
	IEnumerator m_IsAction;
	IEnumerator m_LifeAction;

	public Item_Training GetTraining { get { return m_Training; } }
	public GameObject GetTimerObj { get { return m_SUI.TimerPanel; } }
	public GameObject GetLifeObj { get { return m_SUI.LifePanel; } }
#pragma warning restore 0649

	// aobjValue => [0] : 스테이지모드로 시작 여부, [1] : 트레이닝 인덱스, [2] : 성공 횟수, [3] : 최대 시도횟수
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_IsStageMode = (bool)aobjValue[0];
		m_Idx = (int)aobjValue[1];
		m_ClearCnt = (int)aobjValue[2];
		m_MaxCnt = (int)aobjValue[3];

		m_SUI.Name.text = m_TData.GetName();
		m_SUI.Lv.text = string.Format("Lv.{0}", m_TData.m_LV);

		for (int i = m_SUI.SuccChecks.Length - 1; i > -1; i--) {
			m_SUI.SuccChecks[i].gameObject.SetActive(m_SUI.SuccChecks.Length - 1 - i < m_ClearCnt);
			m_SUI.SuccChecks[i].SetTrigger("Off");
		}
		m_CheckCnt[1] = m_MaxCnt - m_ClearCnt;
		m_SUI.BigLife.gameObject.SetActive(m_CheckCnt[1] > 5); 
		m_SUI.BigLife.SetTrigger("On");
		if( m_CheckCnt[1] <= 5) {
			for (int i = 0; i < m_SUI.Lifes.Length; i++) {
				m_SUI.Lifes[i].gameObject.SetActive(i < m_CheckCnt[1]);
				m_SUI.Lifes[i].SetTrigger("On");
			}
		}
		else {
			for (int i = 0; i < m_SUI.Lifes.Length; i++) {
				m_SUI.Lifes[i].gameObject.SetActive(i < m_CheckCnt[1] - 5);
				m_SUI.Lifes[i].SetTrigger("On");
			}
		}
		SetTimer(m_TData.m_Time);
		SetTimerAnim("Start");

		// 트레이닝 모드 로드
		m_Training = UTILE.LoadPrefab(string.Format("Item/Training/Item_TR_{0}", m_TData.m_Type.ToString()), true, m_SUI.Panel).GetComponent<Item_Training>();
		m_Training.SetData(m_Idx, m_ClearCnt, m_MaxCnt, m_TData.m_Time, SetFX, SetLife, SetSuccCheck, SetTimer, SetTimerAnim);
		if (m_IsStageMode) StartStageMode();

		PlayStageBGSound();
	}

	private void Update()
	{
		STAGEINFO.SetRunTime(Time.deltaTime);
	}

	void SetLife() {
		if (m_CheckCnt[1] == 6) StartCoroutine(m_LifeAction = Life5Action());
		else {
			if(m_LifeAction != null) {
				StopCoroutine(m_LifeAction);
				iTween.StopByName(gameObject, "LifeSpacing");
				iTween.StopByName(gameObject, "LifeAlpha");
				m_SUI.BigLife.gameObject.SetActive(false);
				for (int i = 0; i < m_CheckCnt[1]; i++) {
					m_SUI.Lifes[i].gameObject.SetActive(true); 
					m_SUI.Lifes[i].SetTrigger("On");
				}
				TW_LifeLayout(-25);
				TW_LifeAlpha(1);
			}
			m_SUI.Lifes[(m_CheckCnt[1] - 1) % 5].SetTrigger("Hit");
			//m_SUI.Lifes[(m_CheckCnt[1] - 1) % 5].SetTrigger("Hit");
		}
		m_CheckCnt[1]--;
	}
	IEnumerator Life5Action() {
		m_SUI.Lifes[0].SetTrigger("Hit");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Lifes[0]));

		TW_LifeLayout(-100f);
		TW_LifeAlpha(0f);

		m_SUI.BigLife.gameObject.SetActive(false);

		for (int i = 0; i < 5; i++) {
			m_SUI.Lifes[i].gameObject.SetActive(true);
			m_SUI.Lifes[i].SetTrigger("On");//i < m_CheckCnt[1] ? "On" : "Off"
		}

		iTween.ValueTo(gameObject, iTween.Hash("from", -100f, "to", -25f, "onupdate", "TW_LifeLayout", "time", 0.4f, "name", "LifeSpacing"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "onupdate", "TW_LifeAlpha", "time", 0.4f, "name", "LifeAlpha"));
	}
	void TW_LifeLayout(float _amount) {
		m_SUI.LifeLayout.spacing = _amount;
	}
	void TW_LifeAlpha(float _amount) {
		for (int i = 0; i < m_SUI.Lifes.Length; i++) m_SUI.Lifes[i].GetComponent<CanvasGroup>().alpha = _amount;
	}
	void SetSuccCheck() {
		m_SUI.SuccChecks[m_SUI.SuccChecks.Length - 1 - m_CheckCnt[0]].SetTrigger("Hit");
		m_CheckCnt[0]++;
	}
	void SetTimer(float _amount) {//6.4321  06:43 21
		string pretrig = m_TimerTrig;
		m_TimerTrig = _amount > 3 ? "Normal" : "Danger";
		if(pretrig != m_TimerTrig)SetTimerAnim(m_TimerTrig);
		m_SUI.Timer[0].text = Mathf.FloorToInt(_amount).ToString();
		int dotnum = Mathf.CeilToInt(_amount * 100 % 100);
		m_SUI.Timer[1].text = string.Format(".{0}", dotnum > 0 ? dotnum.ToString("00") : "00");
	}
	void SetTimerAnim(string _trig) {
		m_SUI.TimerAnim.SetTrigger(_trig);
	}

	void StartStageMode()
	{
		m_BG = UTILE.LoadPrefab(STAGEINFO.m_TStage.GetBGName(), true, transform).GetComponent<Item_MainMenu_Stg_Bg>();
		m_BG.transform.SetAsFirstSibling();
		m_BG.MaskOff();

		// 스테이지 시작 알림 내용 출력
		m_IsAction = StageProc();
		StartCoroutine(m_IsAction);
	}
	IEnumerator StageProc()
	{
		TUTO.Start(TutoStartPos.Training);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Training.GetComponent<Animator>()));
		GameObject obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Mission, null, STAGEINFO.m_Idx).gameObject;
		yield return new WaitUntil(() => obj == null);
		
		yield return m_Training.Play();

		m_SUI.Ani.SetTrigger("Close");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));

		switch (m_Training.m_OutResult)
		{
		case Item_Training.Result.Success:
			StageClear();
			break;
		case Item_Training.Result.TimeOver:
			StageFail(StageFailKind.Time);
			break;
		case Item_Training.Result.Fail:
			StageFail(StageFailKind.Turn);
			break;
		default:
			Close();
			break;
		}

		// 결과창 보여주기
		m_IsAction = null;
	}

	public void StageClear()
	{
#if NOT_USE_NET
		OnClear();
#else
		STAGEINFO.StageClear((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					StageClear();
				});
				return;
			}
			OnClear(res);
		});
#endif
	}

#if NOT_USE_NET
	void OnClear()
#else
	void OnClear(LS_Web.RES_STAGE_CLEAR res)
#endif
	{
		STAGEINFO.m_Result = StageResult.Clear;
		POPUP.GetWorldUIPanel().gameObject.SetActive(true);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Result, (result, obj) => {
			switch (STAGEINFO.m_PlayType)
			{
			case StagePlayType.Stage:
				TStageTable tdata = STAGEINFO.m_TStage;
				if (tdata != null) {
					var stage = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
					//stage.Clear = tdata.m_Idx;
					if(USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear == 103) {
						PlayerPrefs.SetInt($"View_Intro_{USERINFO.m_UID}", 1);
						PlayerPrefs.Save();
					}
					if (tdata.m_NextIdx != 0) stage.Idx = tdata.m_NextIdx;
#if NOT_USE_NET
					stage.PlayCount = 1;
					stage.ChapterReward = STAGEINFO.IS_ReplayStg ? 0 : TDATA.GetChapterTable(USERINFO.GetDifficulty(), stage.Clear) == null ? 0 : stage.Clear;
					TStageTable stagetable = TDATA.GetStageTable(stage.Clear, USERINFO.GetDifficulty());
					if (stagetable.m_ClearEvent) USERINFO.m_AddEvent = TDATA.GetRandEventTable().m_Idx;
#endif
						stage.Selects.Clear();
					USERINFO.SetMainMenuAlarmVal(MainMenuType.Stage);
				}
				break;
			default:
				USERINFO.OutGameClear();
				break;
			}
			Close();
		}
#if NOT_USE_NET
		);
#else
		, res);
#endif
	}


	public void StageFail(StageFailKind failkind) {
		STAGEINFO.m_Result = StageResult.Fail;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_FailCause, (result, obj) => {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StageFailed, (result, obj) => {
				Close();
			}, failkind);
		}, failkind, m_Training.m_OutResult == Item_Training.Result.TimeOver ? GetTimerObj : GetLifeObj);
	}
	public void SetFX(bool _succ) {
		m_SUI.Ani.SetTrigger(_succ ? "Succ" : "Fail");
	}
	public void Click_Guide() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Guide_Training, (res, obj) => { Time.timeScale = 1f; });
		Time.timeScale = 0f;//팝업 켤떄 타임스케일 1이라 켜진 뒤에 0
	}
	public void Click_Pause() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Pause);
		//Is_Pause = !Is_Pause;
		//Time.timeScale = Is_Pause ? 1 : 0;
		//m_SUI.PauseBtnIcon.sprite = m_SUI.PauseSprite[Is_Pause ? 1 : 0];
	}
}
