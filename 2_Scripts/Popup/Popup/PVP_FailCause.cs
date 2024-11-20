using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PVP_FailCause : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Transform CloneBucket;
		public Transform FX;
	}
	[SerializeField] SUI m_SUI;
	PVPFailCause m_Cause;
	int m_Pos;
	GameObject[] CloneObj = new GameObject[2];//0:원본, 1:복제된거

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Cause = (PVPFailCause)aobjValue[0];
		m_Pos = (int)aobjValue[1];

		StartCoroutine(ViewCause());
		base.SetData(pos, popup, cb, aobjValue);
	}
	private void Update() {
		if (CloneObj[1] != null) {
			CloneObj[1].transform.position = CloneObj[0].transform.position;
		}
	}
	IEnumerator ViewCause() {
		m_SUI.Anim.SetTrigger(m_Pos == 0 ? "Defeat" : "Victory");

		switch (m_Cause) {
			case PVPFailCause.Men:
				CloneObj[0] = PVP.GetStatObj(m_Pos, StatType.Men);//Item_SurvStat_PVP
				break;
			case PVPFailCause.Sat:
				CloneObj[0] = PVP.GetStatObj(m_Pos, StatType.Sat);
				break;
			case PVPFailCause.Hyg:
				CloneObj[0] = PVP.GetStatObj(m_Pos, StatType.Hyg);
				break;
			case PVPFailCause.HP:
				CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_PVP>().GetHPBar(m_Pos);
				break;
			case PVPFailCause.Turn:
				CloneObj[0] = POPUP.GetMainUI().GetComponent<Main_PVP>().GetTurnGroup;
				break;
		}

		if (CloneObj[0] != null) {
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
			CloneObj[1].transform.localScale = CloneObj[0].transform.localScale * (Vector3.one / Canvas_Controller.SCALE).x;
			CloneObj[1].transform.position = CloneObj[0].transform.position;
			//CloneObj[1].GetComponent<RectTransform>().pivot = new Vector2(0f, 0f);
			yield return new WaitForEndOfFrame();
			CloneObj[1].gameObject.SetActive(true);
		}
		if (CloneObj[1] != null) m_SUI.FX.position = CloneObj[1].transform.position;

		switch (m_Cause) {
			case PVPFailCause.Men:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat_PVP>().SetData(0f, 0f, PVP.GetMaxStat(UserPos.My, StatType.Men));
				break;
			case PVPFailCause.Sat:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat_PVP>().SetData(0f, 0f, PVP.GetMaxStat(UserPos.My, StatType.Sat));
				break;
			case PVPFailCause.Hyg:
				for (int i = 0; i < 2; i++) CloneObj[i].GetComponent<Item_SurvStat_PVP>().SetData(0f, 0f, PVP.GetMaxStat(UserPos.My, StatType.Hyg));
				break;
			case PVPFailCause.HP:
				//CloneObj[1].transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
				break;
			case PVPFailCause.Turn:
				break;
		}
	}
}
