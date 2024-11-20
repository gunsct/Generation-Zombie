using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Tower_Single : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public SpriteRenderer Card;
		public TextMeshPro Name;
		public SpriteRenderer StatIcon;
	}
	[SerializeField] SUI m_SUI;

	void RefreshResolutin() {
		transform.position = Vector3.zero;
		Vector3 scale = Vector3.one / Canvas_Controller.SCALE;
		scale.z = 1;
		transform.localScale = scale;
	}
	private void Update() {
		RefreshResolutin();
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		TStageCardTable tdata = (TStageCardTable)aobjValue[0];

		Vector3 panelpos = Utile_Class.GetWorldPosition(Vector2.zero, -10f);
		panelpos.z = 0;
		transform.position = panelpos;
		//스테이지가 메인일떄 사이즈랑 위치
		float diff = 0;
		float h = 2560 - diff;
		if (MAIN.IS_State(MainState.TOWER) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			diff = 230 + 50;
			h = 2560 - diff;
		}

		m_SUI.Card.sprite = tdata.GetImg();
		m_SUI.Name.text = tdata.GetName();

		switch (tdata.m_Type) {
			case StageCardType.RecoveryHp:
				m_SUI.StatIcon.sprite = UTILE.LoadImg("UI/Icon/Icon_FugiStat_04", "png");
				break;
			case StageCardType.RecoveryMen:
			case StageCardType.PerRecoveryMen:
				m_SUI.StatIcon.sprite = UTILE.LoadImg("UI/Icon/Icon_FugiStat_01", "png");
				break;
			case StageCardType.RecoveryHyg:
			case StageCardType.PerRecoveryHyg:
				m_SUI.StatIcon.sprite = UTILE.LoadImg("UI/Icon/Icon_FugiStat_02", "png");
				break;
			case StageCardType.RecoverySat:
			case StageCardType.PerRecoverySat:
				m_SUI.StatIcon.sprite = UTILE.LoadImg("UI/Icon/Icon_FugiStat_03", "png");
				break;
		}

		StartCoroutine(AutoEnd());
	}

	IEnumerator AutoEnd() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close(0);
	}
}
