using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class PVP_StartReward : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator[] Anim;
		public GameObject EnemyPanel;
		public Item_Reward_Card[] Cards;
		public RectTransform Block;
		public TextMeshPro EnemyCardDesc;
	}
	[SerializeField] SUI m_SUI;
	List<Dictionary<Item_Reward_Card, TSelectDropGroupTable>> m_Rewards = new List<Dictionary<Item_Reward_Card, TSelectDropGroupTable>>();
	bool Is_CanSelect;
	bool Is_UserSelect;

	private void Awake() {
		m_SUI.EnemyPanel.SetActive(false);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		Vector3 panelpos = Utile_Class.GetWorldPosition(Vector2.zero, -10f);
		panelpos.z = 0;
		transform.position = panelpos;
		m_SUI.Block.localPosition = -transform.localPosition;
		//스테이지가 메인일떄 사이즈랑 위치
		float diff = 0;
		float h = 2560 - diff;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			diff = 230 + 50;
			h = 2560 - diff;
		}
		m_SUI.Block.sizeDelta = new Vector2(m_SUI.Block.sizeDelta.x, h);
		m_SUI.Block.localPosition += new Vector3(0f, diff * 0.5f, 0f);

		RefreshResolutin();

		int rewardgid = (int)aobjValue[0];

		StartCoroutine(IE_SetCard(rewardgid));

		PlayEffSound(SND_IDX.SFX_0320);
	}

	void RefreshResolutin() {
		transform.position = Vector3.zero;
		Vector3 scale = Vector3.one / Canvas_Controller.SCALE;
		scale.z = 1;
		transform.localScale = scale;
	}
	private void Update() {
		RefreshResolutin();

		bool cantouch = false;
		if (MAIN.IS_State(MainState.PVP) && POPUP.GetMainUI().m_Popup == PopupName.PVP) cantouch = PVP.TouchCheck();

		if (cantouch && Is_CanSelect) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hit = Physics.RaycastAll(ray, Camera.main.farClipPlane);
			for (int i = 0; i < hit.Length; i++) {
				GameObject hitobj = hit[i].transform.gameObject;
				if (!hitobj.activeSelf) continue;
				Item_Reward_Card hitcard = hitobj.GetComponent<Item_Reward_Card>();
				if (hitcard != null) {
					Is_CanSelect = false;
					StartCoroutine(SelectReward(hitcard, 0));
				}
			}
		}
	}
	IEnumerator IE_SetCard(int _gid) {
		for (int i = 0; i < 2; i++) {
			List<TSelectDropGroupTable> picktables = new List<TSelectDropGroupTable>();
			m_Rewards.Add(new Dictionary<Item_Reward_Card, TSelectDropGroupTable>());
			for (int j = 0; j < 3; j++) {
				TSelectDropGroupTable table = TDATA.GetRandSelectDropGroupTable(_gid, picktables);
				m_SUI.Cards[i * 3 + j].SetData(new StageCardInfo(table.m_Val), StageRewardState.Normal);
				m_Rewards[i].Add(m_SUI.Cards[i * 3 + j], table);
				picktables.Add(table);
			}
		}

		PlayEffSound(SND_IDX.SFX_0210);

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim[0]));

		Is_CanSelect = true;

		yield return new WaitWhile(() => Is_UserSelect == false);

		m_SUI.EnemyPanel.SetActive(true);

		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim[1]));

		StartCoroutine(SelectReward(m_SUI.Cards[3 + UTILE.Get_Random(0, 3)], 1));
	}
	IEnumerator SelectReward(Item_Reward_Card _card, int _pos) {
		TSelectDropGroupTable droptable = m_Rewards[_pos][_card];
		TStageCardTable cardtable = TDATA.GetStageCardTable(droptable.m_Val);

		PlayEffSound(SND_IDX.SFX_0321);
		m_SUI.EnemyCardDesc.text = _card.m_Info.m_TData.GetInfo();

		int pos = 0;
		for(int i = 0; i < m_Rewards[_pos].Count; i++) {
			if(m_Rewards[_pos].ElementAt(i).Key == _card) {
				pos = i;
				break;
			}
		}
		switch (pos) {
			case 0: m_SUI.Anim[_pos].SetTrigger("End_L"); break;
			case 1: m_SUI.Anim[_pos].SetTrigger("End_C"); break;
			case 2: m_SUI.Anim[_pos].SetTrigger("End_R"); break;
		}

		PVP.SetBuff((UserPos)_pos, cardtable.m_Idx);

		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim[_pos], _pos == 0 ? 60f/100f : 1f));

		if (_pos == 0) Is_UserSelect = true;
		else Close(0);
	}
}
