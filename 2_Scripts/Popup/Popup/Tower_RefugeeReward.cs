using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tower_RefugeeReward : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_Reward_Card[] RewardCards;
		public SpriteRenderer[] RefugeeIcon;
		public GameObject[] DownMark;
	}
	[SerializeField]
	SUI m_SUI;
	public TStageCardTable m_SelectCardTable;
	bool m_CanTouch;
	List<TStageCardTable> m_CardTables = new List<TStageCardTable>();
	string m_CloseAni;
	IEnumerator m_Action; //end ani check
	void RefreshResolutin() {
		transform.position = Vector3.zero;
		Vector3 scale = Vector3.one / Canvas_Controller.SCALE;
		scale.z = 1;
		transform.localScale = scale;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		Vector3 panelpos = Utile_Class.GetWorldPosition(Vector2.zero, -10f);
		panelpos.z = 0;
		transform.position = panelpos;
		//스테이지가 메인일떄 사이즈랑 위치
		float diff = 0;
		float h = 2560 - diff;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			diff = 230 + 50;
			h = 2560 - diff;
		}

		m_CardTables = (List<TStageCardTable>)aobjValue[0];
		for(int i = 0;i< m_SUI.RewardCards.Length; i++) {
			m_SUI.RewardCards[i].SetData(new StageCardInfo(m_CardTables[i].m_Idx), StageRewardState.Normal);
			TEnemyTable enemytable = TDATA.GetEnemyTable(Mathf.RoundToInt(m_CardTables[i].m_Value1));
			m_SUI.DownMark[i].SetActive(false);//enemytable.IS_BadRefugee()
			m_SUI.RefugeeIcon[i].sprite = BaseValue.GetStatMark(enemytable.m_Type, enemytable.m_RewardGID);
		}
		StartCoroutine(StartAnim());
	}

	private void Update() {
		RefreshResolutin();
		if (m_Action != null) return;
#if UNITY_EDITOR
		if (Input.GetMouseButtonUp(0))
#else
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
#endif
		if (m_CanTouch) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hit = Physics.RaycastAll(ray, Camera.main.farClipPlane);
			for (int i = 0; i < hit.Length; i++) {
				GameObject hitobj = hit[i].transform.gameObject;
				if (!hitobj.activeSelf) continue;
				Item_Reward_Card card = hitobj.GetComponent<Item_Reward_Card>();
				if (card == null) continue;
				for (int j = 0; j < m_SUI.RewardCards.Length; j++) {
					if (m_SUI.RewardCards[j] == card) {
						m_SelectCardTable = m_CardTables[j];
						m_CanTouch = false;
						m_CloseAni = string.Format("Select_{0}", j + 1);
						Close(1);
						break;
					}
				}
				break;
			}
		}
	}
	IEnumerator StartAnim() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_CanTouch = true;
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger(m_CloseAni);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}
}
