using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	public void Restart()
	{
		StopAllCoroutines();
		m_PlayAction = StartAction();
		StartCoroutine(m_PlayAction);
	}

	IEnumerator StartAction() {
		m_Map.SetData();
		// 스테이지 진입만큼 소모한후 완료되면 실행할것
		STAGEINFO.PlayInit();
		TStageTable tdata = STAGEINFO.m_TStage;
		m_User.m_Turn = tdata.m_LimitTurn;
		m_User.m_Time = tdata.m_StartTime;

		PlayStageBGSound();
		bool IsAniPlay = true;
		m_Map.PlayAni(Item_TowerBG.AniName.Start, () =>
		{
			IsAniPlay = false;
		});
		yield return new WaitWhile(() => IsAniPlay);
		bool PopupCloase = false;
		Action<int, GameObject> StartPopupEndCB = (result, obj) => {
			PopupCloase = true;
		};
		m_MainUI = POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Stage, null, StartPopupEndCB).GetComponent<Main_Stage>();
		yield return new WaitWhile(() => !PopupCloase);
		m_PlayAction = null;
	}
}
