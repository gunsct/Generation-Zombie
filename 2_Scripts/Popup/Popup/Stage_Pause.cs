using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Stage_Pause : PopupBase
{
	public enum EndType
	{
		Resume = 0,
		ReTry,
		GoMain
	}
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Goal;
		public TextMeshProUGUI[] Fail;
	}
	[SerializeField]
	SUI m_SUI;
	Coroutine m_Cor;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		//목표
		m_SUI.Goal.text = STAGEINFO.m_TStage.GetInfo();
		//실패 조건
		List<string> fail = STAGEINFO.m_TStage.GetFailInfo();
		for (int i = 0; i < fail.Count; i++) {
			m_SUI.Fail[i].text = fail[i];
		}
		Time.timeScale = 0f;
	}

	IEnumerator SetResume(EndType _type) {
		if (_type != EndType.ReTry) {
			yield return IE_DelayClose(null);
			//m_SUI.Anim.SetTrigger("End");
			//yield return new WaitForEndOfFrame();
			//yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		}

		switch (_type)
		{
			case EndType.Resume:
				Close(1);
				break;
			case EndType.ReTry:
				STAGEINFO.StageFailAnalytics(StageFailKind.None, 7);
				STAGEINFO.StageReset((result) => {
					if (result == EResultCode.SUCCESS) {
						if(TUTO.IsTutoPlay()) TUTO.SetReset();
						m_Cor = StartCoroutine(IE_DelayClose(()=> { Close(1); }));
						//Close(1);
					}
				});
				break;
			case EndType.GoMain:
				//FireBase-Analytics
				STAGEINFO.StageStatisticsLog(StageFailKind.None, 8);
				STAGEINFO.StageFailAnalytics(StageFailKind.None, 8);
				if (TUTO.IsTutoPlay()) TUTO.SetReset();

				Close(1);
				if (STAGEINFO.m_StageModeType == StageModeType.Training) {
					POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
					PLAY.SetBGSND();
				}
				else
					MAIN.StateChange(MainState.PLAY);
				break;
		}
	}
	/// <summary> 재개 </summary>
	public void ClickResume() {
		if (m_Cor != null) return;
		m_Cor = StartCoroutine(SetResume(EndType.Resume));
	}
	/// <summary> 재도전 </summary>
	public void ClickReTry() {
		if (m_Cor != null) return;
		//튜토중이면 재시작 불가
		if (TUTO.IsTutoPlay() && STAGEINFO.m_StageModeType == StageModeType.Stage) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(262));
			return;
		}
		List<Animator> anims = new List<Animator>();
		GameObject obh = POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(STAGEINFO.m_TStage.m_Energy > 0 ? 190 : 786), (result, obj) => {
			if (result == 1) {
				if (STAGEINFO.m_TStage.m_Energy > 0 && USERINFO.m_Energy.Cnt < STAGEINFO.m_TStage.m_Energy) {
					Time.timeScale = STAGEINFO.m_StageModeType == StageModeType.Stage ? BaseValue.STAGE_STEP1_TIMESCALE : 1f;//0이면 코루틴이 안돔
					POPUP.StartLackPop(BaseValue.ENERGY_IDX, false, (res) => {
						if (res == EResultCode.SUCCESS) {
							m_Cor = StartCoroutine(SetResume(EndType.ReTry));
						}
					}, TDATA.GetString(782));
					return;
				}
				m_Cor = StartCoroutine(SetResume(EndType.ReTry));
			}
			else
				m_Cor = StartCoroutine(SetResume(EndType.Resume));
		}, PriceType.Energy, BaseValue.ENERGY_IDX, STAGEINFO.m_TStage.m_Energy, false).gameObject;
		Time.timeScale = 0.05f;//0이면 코루틴이 안돔

		anims.AddRange(obh.GetComponents<Animator>());
		anims.AddRange(obh.GetComponentsInChildren<Animator>());
		for(int i = 0;i< anims.Count; i++) anims[i].updateMode = AnimatorUpdateMode.UnscaledTime;
	}
	/// <summary> 메인으로 </summary>
	public void ClickGoMain() {
		if (m_Cor != null) return;
		//스테이지에서 튜토중이었다면 리셋
		//if (TUTO.IsTutoPlay() && STAGEINFO.m_StageModeType == StageModeType.Stage) TUTO.SetReset();
		m_Cor = StartCoroutine(SetResume(EndType.GoMain));
	}
	IEnumerator IE_DelayClose(Action _cb) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		_cb?.Invoke();
	}
	public override void Close(int Result = 0)
	{
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (Result == 0)
			m_Cor = StartCoroutine(SetResume(EndType.Resume));
		else
			base.Close(Result);
	}
}
