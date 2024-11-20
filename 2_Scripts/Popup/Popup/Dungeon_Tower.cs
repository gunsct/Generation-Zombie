using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_Tower : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform LvPrefab;//Item_BankLvGroup, maxlv / 2
		public Transform LvBucket;
		public Transform LvBucketTop;
		public Transform LvBucketCenter;
		public GameObject[] LimitObjs;
		public ScrollRect Scroll;
		public Item_RewardList_Item[] Rewards;
		public GameObject GoBtn;
	}
	[SerializeField] SUI m_SUI;
	List<Item_DG_Tower_Stg> m_LvElements = new List<Item_DG_Tower_Stg>();
	int m_LimitLv;
	bool notlast;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		GetLv();
		m_LimitLv = Math.Min(m_Lv[1], BaseValue.TOWER_LIMIT);
		notlast = USERINFO.m_Stage[StageContentType.Tower].Idxs[0].Clear != m_LimitLv;
		if (!notlast) m_ChangeStart = false;
		SetCommUI();
		SetNotCommUI(true);
		SetButtUI();
		m_SUI.GoBtn.SetActive(notlast);
		//StartCoroutine(ScrollCenter(m_ClearLv));
		StartCoroutine(StartDelay());
	}
	IEnumerator StartDelay() {
		MainMenuType premenu = STAGEINFO.GetPreMenu();
		if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0187);
		m_SUI.Anim.SetTrigger(notlast ? (m_ChangeStart && STAGEINFO.m_Result == StageResult.Clear ? "Change" : "Start") : "Change_ComingSoon");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => !Utile_Class.IsAniPlay(m_SUI.Anim));

		if (m_Lv[0] < m_LimitLv && m_ChangeStart) {
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 75f/156f));
			SetNotCommUI();
		}
		else {
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
			yield return SetTowerUI(m_Stagetable);
		}
		m_CanClick = true;
	}
	protected override void SetLv(int _lv) {
		bool refresh = m_Lv[0] != _lv && m_ClearLv > _lv;//해금 안된거랑 자기자신 아닐때
		base.SetLv(_lv);
		if (refresh) {
			SetNotCommUI();
			//StartCoroutine(ScrollCenter(m_Lv[0]));
		}
		SetButtUI();
	}
	//바뀌지 않는 기본 유아이들
	protected override void SetCommUI() {
		//남은레벨
		UTILE.Load_Prefab_List(Mathf.Min(m_Lv[0] + 1, 6), m_SUI.LvBucketCenter, m_SUI.LvPrefab);
		for (int i = m_SUI.LvBucketCenter.childCount - 1, cnt = 0; i > -1 ; i--, cnt++) {
			Item_DG_Tower_Stg element = m_SUI.LvBucketCenter.GetChild(i).GetComponent<Item_DG_Tower_Stg>();
			element.SetData(Math.Min(m_Lv[0], m_LimitLv - 1) - cnt);
			m_LvElements.Add(element);
		}
		//현재+ 클리어한 레벨
		int remain = m_LimitLv - (m_Lv[0] + 1);
		if (remain > 0) {
			UTILE.Load_Prefab_List(Mathf.Min(remain, 5), m_SUI.LvBucketTop, m_SUI.LvPrefab);
			for (int i = 0; i < m_SUI.LvBucketTop.childCount; i++) {
				Item_DG_Tower_Stg element = m_SUI.LvBucketTop.GetChild(i).GetComponent<Item_DG_Tower_Stg>();
				element.SetData(i + m_Lv[0] + 1);
				m_LvElements.Add(element);
			}
		}
	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		TStageTable stgdata = m_Stagetable;
		if (_first && m_ChangeStart) {
			stgdata = TDATA.GetStageTable(TDATA.GetModeTable(m_Content, m_Day, m_Pos)[Math.Max(0, m_Lv[0] - 1)].m_StageIdx);
		}
		StartCoroutine(SetTowerUI(stgdata));
		if (_first) {
			float listypos = -560f + 140f * m_Lv[0];
			if (m_ChangeStart) {
				for (int i = 0; i < m_LvElements.Count; i++) {
					Item_DG_Tower_Stg.State state = Item_DG_Tower_Stg.State.Not;
					if (m_LvElements[i].m_Lv == m_Lv[0]) state = Item_DG_Tower_Stg.State.Change;
					else if (m_LvElements[i].m_Lv == m_Lv[0] - 1) state = Item_DG_Tower_Stg.State.Cleared;
					m_LvElements[i].SetAnim(state);
				}
				iTween.ValueTo(gameObject, iTween.Hash("from", listypos - 140f, "to", listypos, "onupdate", "TW_LvBucketMove", "delay", 2f * (77f / 120f), "time", 2f * (45f / 120f)));
			}
			else {
				for (int i = 0; i < m_LvElements.Count; i++) {
					Item_DG_Tower_Stg.State state = Item_DG_Tower_Stg.State.Not;
					if (m_LvElements[i].m_Lv == m_Lv[0]) state = Item_DG_Tower_Stg.State.Now;
					m_LvElements[i].SetAnim(state);
				}
				m_SUI.LvBucket.localPosition = new Vector3(0f, Mathf.Min(0f, listypos, 0f));
			}
		}
	}
	void TW_LvBucketMove(float _amount) {
		m_SUI.LvBucket.localPosition = new Vector3(0f, Mathf.Min(0f, _amount, 0f));
	}
	IEnumerator SetTowerUI(TStageTable _table) {
		m_SCUI.GoBtn.SetActive(m_Lv[0] < m_LimitLv);
		for(int i = 0; i < m_SUI.LimitObjs.Length; i++) {
			m_SUI.LimitObjs[i].SetActive(i == 0 ? !notlast : notlast);
		}
		List<RES_REWARD_BASE> rewards = GetRewards(_table, false);
		for(int i = 0; i < m_SUI.Rewards.Length; i++) {
			if (rewards.Count > i) {
				m_SUI.Rewards[i].SetData(rewards[i]);
				m_SUI.Rewards[i].gameObject.SetActive(true);
			}
			else m_SUI.Rewards[i].gameObject.SetActive(false);
		}
		yield break;
	}
	IEnumerator ScrollCenter(int _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.Scroll.enabled = false;
		iTween.StopByName(gameObject, "Scrolling");
		float to = 0f;
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.horizontalNormalizedPosition, "to", to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		yield return new WaitForSeconds(0.3f);

		m_SUI.Scroll.enabled = true;
	}
	void TW_Scrolling(float _amount) {
		m_SUI.Scroll.horizontalNormalizedPosition = _amount;
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(Result);
	}

	public override void ScrollLock(bool _lock) {
		m_SUI.Scroll.enabled = !_lock;
	}
}
