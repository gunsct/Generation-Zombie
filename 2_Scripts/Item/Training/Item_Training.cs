using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public class Item_Training : ObjMng
{
	public enum State
	{
		Idle = 0,
		Play,
		End
	}

	public enum Result
	{
		None = 0,
		Fail,
		TimeOver,
		Success
	}
#pragma warning disable 0649
	[SerializeField] Transform m_BarPanel;
	// [0] : 기본, [1] : Focus
	[SerializeField] Color[] m_NormalCols = { new Color(0f, 0f, 0f, 160f / 255f), new Color(251f / 255f, 158f / 255f, 49f / 255f, 1f) };
	// [0] : 기본, [1] : Focus
	[SerializeField] Color[] m_AreaCols = { new Color(164f / 255f, 164f / 255f, 164f / 255f, 160f / 255f), new Color(1f, 216f / 255f, 25f / 255f, 1f) };
	Image[] m_Bars;
	bool[] m_Area;
	int[] m_TargetArea;
	int m_Idx, m_NowCnt = -1, m_MaxCnt, m_ClearCnt, m_SuccCnt, m_Focus;
	float m_TimeLimit;
	int m_BarSize;
	List<string> m_PickArea = new List<string>();
	Action<bool> m_FXCB;
	IEnumerator m_MoveCor;
	IEnumerator m_OnePlayCor;
	Result m_OnePlayResult;
	GraphicRaycaster m_GR;

	Action m_LifeCB;
	Action m_SuccCB;
	Action<float> m_TimerCB;
	Action<string> m_TimerAnimCB;
	bool IS_AllTimeLimit;
	bool IS_BarReset;
	bool IS_FocusMove;
	TTrainingTable m_TData { get { return TDATA.GetTrainingTable(m_Idx); } }

#pragma warning restore 0649
	public virtual void SetData(int idx, int clearcnt, int maxcnt, float time, Action<bool> _fxcb, Action _lifecb, Action _succcb, Action<float> _timercb, Action<string> _timeranimcb)
	{
		List<Image> child = new List<Image>(m_BarPanel.GetComponentsInChildren<Image>());
		// m_BarPanel에도 이미지가 있어 가져와짐 제거
		child.Remove(m_BarPanel.GetComponent<Image>());
		m_Bars = child.ToArray();

		m_Idx = idx;
		m_MaxCnt = maxcnt;
		m_ClearCnt = clearcnt;
		m_TimeLimit = time;
		m_FXCB = _fxcb;
		m_LifeCB = _lifecb;
		m_SuccCB = _succcb;
		m_TimerCB = _timercb;
		m_TimerAnimCB = _timeranimcb;

		IS_AllTimeLimit = m_TData.m_AllTimeLimit;
		IS_BarReset = m_TData.m_PointReset;

		m_GR = POPUP.GetComponent<GraphicRaycaster>();//캔버스 레이케스트용

		//시작은 전부 비활성화
		for (int i = m_Bars.Length - 1; i > -1; i--) m_Bars[i].gameObject.SetActive(false);
	}

	void TW_SetTime(float _Time) {
		m_TimerCB.Invoke(_Time);
	}

	void OnTimeOver() {
		if (m_OutResult != Result.None) return;

		PlayEffSound(SND_IDX.SFX_1200);

		if (IS_AllTimeLimit) {
			SetResult(Result.TimeOver);
		}
		else {
			m_LifeCB.Invoke();
			m_FXCB?.Invoke(false);

			if (m_ClearCnt > m_SuccCnt) {
				m_OnePlayResult = Result.Fail;
				FocusEnd();
				if (m_OnePlayCor != null) {
					StopCoroutine(m_OnePlayCor);
					m_OnePlayCor = null;
				}
			}
			else {
				SetResult(Result.TimeOver);
			}
		}
	}
	//게이지 단계별 색 변화 및 인덱스 체크
	void SetFocus(float _amount)
	{
		if(m_Focus > -1) m_Bars[m_Focus].color = GetBarColor(m_Focus, false);

		m_Focus = Mathf.CeilToInt(_amount);
		if(m_Focus > - 1) m_Bars[m_Focus].color = GetBarColor(m_Focus, true);
	}

	public Result m_OutResult;
	public IEnumerator Play()
	{
		m_OutResult = Result.None;
		m_SuccCnt = 0;

		for (int i = 0; i < m_MaxCnt; i++)
		{
			if (m_OutResult != Result.None) yield break;

			if((m_OnePlayResult == Result.Fail || m_OnePlayResult == Result.TimeOver) && m_TData.m_FailTileReset)  SetArea();
			else if(m_OnePlayResult == Result.Success || m_OnePlayResult == Result.None) SetArea();

			// 1회 플레이 시간이
			m_OnePlayResult = Result.None;
			// 0.5초정도 뒤에 시작한다
			if (m_TData.m_PointType == PointType.Normal) yield return new WaitForSeconds(m_TData.m_PointDelay);

			if (i > 0) {
				if (!IS_AllTimeLimit) m_TimerAnimCB.Invoke("Change");
				m_TimerAnimCB.Invoke(m_TimeLimit > 3 ? "Normal" : "Danger");
			}
			if (IS_AllTimeLimit && i == 0 || !IS_AllTimeLimit) {
				iTween.ValueTo(gameObject, iTween.Hash("from", m_TimeLimit, "to", 0
					, "time", m_TimeLimit
					, "delay", 0f
					, "easetype", "linear"
					, "onupdate", "TW_SetTime"
					, "oncomplete", "OnTimeOver", "name", "Timer"));
			}
			m_OnePlayCor = OnePlay((result) => {
				m_OnePlayResult = result;
			});
			StartCoroutine(m_OnePlayCor);
			if (IS_BarReset || (!IS_BarReset && i == 0)) {
				if (m_MoveCor == null) {
					m_MoveCor = FocusMoveStart();
					StartCoroutine(m_MoveCor);
				}
			}
			else iTween.ResumeByName(gameObject, "Focus");

			yield return new WaitWhile(() => m_OnePlayCor != null);
			// 한프레임 쉬어줌
			yield return new WaitForEndOfFrame();

			if (m_OutResult != Result.None) yield break;

			switch (m_OnePlayResult)
			{
				case Result.Success:
					m_SuccCnt++; 
					PlayEffSound(SND_IDX.SFX_1201);
					m_SuccCB.Invoke();
					break;
				case Result.Fail:
					break;
			}

			// 진행할 필요가 없음 전부 성공해도 실패임
			if (m_MaxCnt - 2 - i + m_SuccCnt < m_ClearCnt) break;
			// 성공
			if (m_ClearCnt <= m_SuccCnt)
			{
				SetResult(Result.Success);
				yield break;
			}
			if (m_OutResult != Result.None) yield break;

			m_NowCnt = i;
		}
		FocusEnd();
		SetResult(Result.Fail);
	}

	bool IsPress()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Training_Note)) return false;
		if (POPUP.IS_PopupUI()) return false;
		if (m_Focus < 0) return false;
		//타임오버면 자동으로 실패뜨게 처리
		if (m_OutResult == Result.TimeOver) return true;

		bool touch = false;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#else
		if (Input.GetMouseButtonDown(0))
#endif
		{
			List<RaycastResult> results = new List<RaycastResult>();
			PointerEventData ped = new PointerEventData(null);
			ped.position = Input.mousePosition;
			m_GR.Raycast(ped, results);
			touch = true;

			for (int i = 0; i < results.Count; i++) {
				if(results[i].gameObject.GetComponent<Button>() != null) {
					touch = false;
					break;
				}
			}
		}
		return touch;
	}
	/// <summary> 노트하나의 결과 </summary>
	void SetResult(Result result)
	{
		m_OutResult = result;
		iTween.StopByName(gameObject, "Timer");
	}

	bool m_FocusAction = true;
	void FocusEnd()
	{
		m_FocusAction = false;
		if (IS_BarReset) {
			iTween.StopByName(gameObject, "Focus");
			if (m_MoveCor != null) {
				StopCoroutine(m_MoveCor);
				m_MoveCor = null;
			}
		}
		else {
			iTween.PauseByName(gameObject, "Focus");
		}
	}
	void TW_FocusMoveEnd() {
		IS_FocusMove = false;
	}

	IEnumerator FocusMoveStart()
	{
		switch (m_TData.m_PointType) {
			//기존 러닝과 풀업은 다른 진행방식인데 4개 같게함
			case PointType.Normal:
				// 아래서 위로
				IS_FocusMove = true;
				iTween.ValueTo(gameObject, iTween.Hash("from", m_BarSize - 1, "to", -1f, "time", m_TData.m_Speed, "onupdate", "SetFocus", "oncomplete", "TW_FocusMoveEnd", "easetype", "linear", "name", "Focus"));

				yield return new WaitWhile(() => IS_FocusMove);
				//yield return new WaitForSeconds(m_TData.m_Speed + m_TData.m_Speed / (m_BarSize - 1));

				SetFocus(-1);
				yield return new WaitForSeconds(m_TData.m_PointDelay);

				m_MoveCor = FocusMoveStart();
				StartCoroutine(m_MoveCor);
				break;
			case PointType.Continue:
				IS_FocusMove = true;
				iTween.ValueTo(gameObject, iTween.Hash("from", m_BarSize - 1, "to", -1, "time", m_TData.m_Speed, "onupdate", "SetFocus", "oncomplete", "TW_FocusMoveEnd", "easetype", "linear", "name", "Focus"));
				yield return new WaitWhile(() => IS_FocusMove);
				//yield return new WaitForSeconds(m_TData.m_Speed + m_TData.m_Speed / (m_BarSize - 1));
				IS_FocusMove = true;
				iTween.ValueTo(gameObject, iTween.Hash("from", -1, "to", m_BarSize - 1, "time", m_TData.m_Speed, "onupdate", "SetFocus", "oncomplete", "TW_FocusMoveEnd", "easetype", "linear", "name", "Focus"));
				yield return new WaitWhile(() => IS_FocusMove);
				//yield return new WaitForSeconds(m_TData.m_Speed - m_TData.m_Speed / (m_BarSize - 1));

				m_MoveCor = FocusMoveStart();
				StartCoroutine(m_MoveCor);
				break;
		}
	}

	protected virtual IEnumerator OnePlay(Action<Result> result)
	{
		SetFocus(-1);
		m_FocusAction = true;
		while (m_OutResult == Result.None && m_FocusAction)
		{
			yield return new WaitWhile(() => !IsPress() && m_FocusAction);
			if (!m_FocusAction) break;
			// 현재 누른 상태일때의 위치 알아낸다.
			if (m_Area[m_Focus]) {
				m_FXCB?.Invoke(true);
				switch (m_TData.m_Type) {
					case TrainingType.Running: PlayEffSound(SND_IDX.SFX_1210); break;
					case TrainingType.PullUp: PlayEffSound(SND_IDX.SFX_1212); break;
					case TrainingType.ClimbingStairs: PlayEffSound(SND_IDX.SFX_1211); break;
					case TrainingType.Batting: PlayEffSound(SND_IDX.SFX_1213); break;
				}

				m_Area[m_Focus] = false;

				bool IsEnd = true;
				for (int i = 0; i < m_Area.Length; i++) {
					if (m_Area[i]) IsEnd = false;
				}
				if (IsEnd) {
					FocusEnd();
					if (!IS_AllTimeLimit || (IS_AllTimeLimit && m_ClearCnt == m_SuccCnt)) iTween.StopByName(gameObject, "Timer");
					m_OnePlayCor = null;
					result?.Invoke(Result.Success);
					yield break;
				}
				m_Bars[m_Focus].color = GetBarColor(m_Focus, true);
				// 아래서 위로
				SetFocus(-1);
			}
			else {
				m_FXCB?.Invoke(false);
				m_LifeCB.Invoke();
				switch (m_TData.m_Type) {
					case TrainingType.Running: PlayEffSound(SND_IDX.SFX_1200); break;
					case TrainingType.PullUp: PlayEffSound(SND_IDX.SFX_1200); break;
					case TrainingType.ClimbingStairs: PlayEffSound(SND_IDX.SFX_1200); break;
					case TrainingType.Batting: PlayEffSound(SND_IDX.SFX_1200); break;
				}
				FocusEnd();
				if (!IS_AllTimeLimit|| (IS_AllTimeLimit && m_MaxCnt == m_NowCnt + 1)) iTween.StopByName(gameObject, "Timer");
				if(m_TData.m_PointType == PointType.Normal) yield return new WaitForSeconds(0.5f);
				m_OnePlayCor = null;
				result?.Invoke(Result.Fail);
				yield break;
			}
		}
		FocusEnd();
		m_OnePlayCor = null;
		result?.Invoke(Result.Fail);
	}

	void SetArea()
	{
		if(m_PickArea.Count == 0) {
			m_PickArea.AddRange(m_TData.m_Areas);
		}
		int rand = UTILE.Get_Random(0, m_PickArea.Count);
		string area = m_PickArea[rand];
		m_PickArea.RemoveAt(rand);

		char[] bars = area.ToCharArray();
		m_BarSize = bars.Length;
		m_TargetArea = bars.Select(o => o - 48).ToArray();

		m_Area = new bool[m_BarSize];
		for (int i = m_Bars.Length - 1; i > -1; i--) m_Bars[i].gameObject.SetActive(i < m_BarSize);
		for (int i = m_Area.Length - 1; i > -1; i--) if(m_TargetArea[m_Area.Length - 1 - i] == 1) m_Area[i] = true;
		for (int i = m_Area.Length - 1; i > -1; i--) {
			if (IS_BarReset || (!IS_BarReset && m_NowCnt == -1)) {
				m_Bars[i].color = GetBarColor(i);
			}
			else if(m_Bars[i].color != m_NormalCols[1]) {
				m_Bars[i].color = GetBarColor(i);
				//m_Bars[i].gameObject.SetActive(m_Bars[i].gameObject.activeSelf);
			}
		}
	}

	Color GetBarColor(int Pos, bool IsFocus = false)
	{
		if(IsFocus)
		{
			if (m_Area[Pos]) return m_AreaCols[1];
			else return m_NormalCols[1];
		}
		else
		{
			if (m_Area[Pos]) return m_AreaCols[0];
			else return m_NormalCols[0];
		}
	}

	public GameObject GetFirstHitZon() {
		for (int i = m_Area.Length - 1; i > -1; i--) {
			if (m_Area[i] == true) {
				return m_Bars[i].gameObject;
			}
		}
		return null;
	}
	public GameObject GetHitZon() {
		for (int i = m_Area.Length - 1; i > -1; i--) {
			if (m_Area[i] == true) {
				return m_Bars[i].gameObject;
			}
		}
		return null;
	}
	public void TuToHitZonPause(Action _cb) {
		StartCoroutine(IE_TutoHitZonAction(_cb));
	}
	IEnumerator IE_TutoHitZonAction(Action _cb) {
		iTween.Resume(gameObject);

		yield return new WaitWhile(() => m_Bars[Mathf.Clamp(m_Focus, 0, m_Area.Length - 1)].gameObject != GetHitZon());
		yield return new WaitForEndOfFrame();

		iTween.Pause(gameObject);
		_cb?.Invoke();
	}
	public void TuToActionPause() {
		iTween.Pause(gameObject);
	}
	public void TuToActionResume() {
		iTween.Resume(gameObject);
	}
}
