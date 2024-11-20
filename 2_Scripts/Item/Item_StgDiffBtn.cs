using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_StgDiffBtn : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image BtnImg;
	}
	[SerializeField]
	SUI m_SUI;
	public StageDifficultyType m_DiffType;
	Action<int> m_CB;
	public bool IS_Cango;
	public void SetData(Action<int> _cb) {
		m_CB = _cb;

		int nowdiff = PlayerPrefs.GetInt($"StageDifficulty_{USERINFO.m_UID}");
		if(nowdiff == (int)m_DiffType) {
			switch (m_DiffType) {
				case StageDifficultyType.Hard:
					m_SUI.BtnImg.sprite = UTILE.LoadImg("UI/UI_StgMain/Button_Mode_N_Back", "png");
					break;
				case StageDifficultyType.Nightmare:
					m_SUI.BtnImg.sprite = UTILE.LoadImg("UI/UI_StgMain/Button_Mode_A_Back", "png");
					break;
			}
		}
		else {
			switch (m_DiffType) {
				case StageDifficultyType.Hard:
					m_SUI.BtnImg.sprite = UTILE.LoadImg("UI/UI_StgMain/Button_Mode_N", "png");
					break;
				case StageDifficultyType.Nightmare:
					m_SUI.BtnImg.sprite = UTILE.LoadImg("UI/UI_StgMain/Button_Mode_A", "png");
					break;
			}
		}
	}

	public void ClickBtn() {
		IS_Cango = false;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageDifficulty, m_DiffType)) {
			return;
		}
		int checkidx = 0;
		int msg = 325;
		switch (m_DiffType) {
		case StageDifficultyType.Hard:
			checkidx = BaseValue.HARD_OPEN;
			break;
		case StageDifficultyType.Nightmare:
			checkidx = BaseValue.NIGHTMARE_OPEN;
			msg = 326;
			break;
		}
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < checkidx)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), checkidx / 100, checkidx % 100, TDATA.GetString(msg)));
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}
		int nowdiff = PlayerPrefs.GetInt($"StageDifficulty_{USERINFO.m_UID}");
		m_CB.Invoke(nowdiff == (int)m_DiffType ? 0 : (int)m_DiffType);
		IS_Cango = true;
	}
}
