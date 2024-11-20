using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_Cemetery : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public ScrollRect RewardScroll;
		public Transform LvPrefab;//Item_DG_University_Stg
		public Transform LvBucket;
		public Transform RewardPrefab;
		public Transform RewardBucket;
		public TextMeshProUGUI RewardMsg;
		public Transform[] MoveDeco;
		public float[] MoveDecoXS;
		public float[] MoveDecoXE;
	}
	[SerializeField] SUI m_SUI;
	List<Item_DG_Cemetery_Stg> m_LvElements = new List<Item_DG_Cemetery_Stg>();

	private void Update() {
		float ratio = 1f - m_SUI.Scroll.horizontalNormalizedPosition;
		for(int i = 0; i < m_SUI.MoveDeco.Length; i++) {
			m_SUI.MoveDeco[i].localPosition = new Vector3(m_SUI.MoveDecoXS[i] + (m_SUI.MoveDecoXE[i] - m_SUI.MoveDecoXS[i]) * ratio, m_SUI.MoveDeco[i].localPosition.y, m_SUI.MoveDeco[i].localPosition.z);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		//m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>().padding.right = Screen.width;

		GetLv();
		SetCommUI();
		SetNotCommUI(true);
		SetButtUI();
		int initlv = m_ClearLv == m_Lv[2] ? m_ClearLv : Mathf.Max(0, m_ClearLv - 1);
		StartCoroutine(ScrollCenter(m_Lv[0]));
		StartCoroutine(StartDelay());
	}
	IEnumerator StartDelay() {
		MainMenuType premenu = STAGEINFO.GetPreMenu();
		if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0184);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Cemetery_Stg.State.NotSelect);
		yield return SetCemeteryUI();
		yield return new WaitForSeconds(0.6f);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Cemetery_Stg.State.Select);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_CanClick = true;
	}
	protected override void SetLv(int _lv) {
		bool refresh = m_Lv[0] != _lv && m_ClearLv >= _lv;//해금 안된거랑 자기자신 아닐때
		base.SetLv(_lv);
		if (refresh) {
			SetNotCommUI();
			StartCoroutine(ScrollCenter(m_Lv[0]));
		}
		SetButtUI();
	}
	//바뀌지 않는 기본 유아이들
	protected override void SetCommUI() {
		UTILE.Load_Prefab_List(m_Lv[1], m_SUI.LvBucket, m_SUI.LvPrefab);
		float[] rots = new float[] { 3, 0, -3.13f, 4.72f };
		for (int i = 0 ,cnt = 0; i < m_SUI.LvBucket.childCount; i++, cnt++) {
			Item_DG_Cemetery_Stg element = m_SUI.LvBucket.GetChild(i).GetComponent<Item_DG_Cemetery_Stg>();
			element.SetData(i, m_Modetables[i].m_StageLimit, m_Modetables[i].m_DiffType, SetLv);//m_LimitStg
			element.transform.GetChild(0).transform.localPosition = new Vector3(0f, i%2 == 0 ? -206.55f : 206.55f, 0f);
			if (cnt == rots.Length) cnt = 0;
			element.transform.GetChild(0).transform.localEulerAngles = new Vector3(0f, 0f, rots[cnt]);
			m_LvElements.Add(element);
		}
	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		if (!Utile_Class.IsAniPlay(m_SUI.Anim)) StartCoroutine(SetCemeteryUI());
		for (int i = 0; i < m_LvElements.Count; i++) {
			Item_DG_Cemetery_Stg.State state = Item_DG_Cemetery_Stg.State.NotOpen;
			if (i <= m_ClearLv && i < m_Lv[2]) {
				state = Item_DG_Cemetery_Stg.State.NotSelect;
			}
			else if (m_Modetables[i].m_StageLimit > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[i].m_DiffType].Idx) {
				state = Item_DG_Cemetery_Stg.State.Lock;
			}
			else state = Item_DG_Cemetery_Stg.State.NotOpen;
			if (i == m_Lv[0]) state = Item_DG_Cemetery_Stg.State.Select;
			m_LvElements[i].SetAnim(state);
		}
	}
	IEnumerator SetCemeteryUI() {
		List<RES_REWARD_BASE> rewards = GetRewards(m_Stagetable, true);
		rewards.RemoveAll( o=>o.Type == Res_RewardType.Zombie);
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.RewardBucket, m_SUI.RewardPrefab);
		for(int i = 0; i < m_SUI.RewardBucket.childCount; i++) {
			Item_RewardList_Item reward = m_SUI.RewardBucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			reward.SetData(rewards[i], null, false);
			reward.transform.localScale = Vector3.one * 0.75f;
		}
		int[] cnt = new int[2];
		for (int i = 0; i < m_Stagetable.m_ClearReward.Count; i++) {//박스형이라 좀비랑 혈청 구분해야함
			TItemTable tdata = TDATA.GetItemTable(m_Stagetable.m_ClearReward[i].m_Idx);
			List <RES_REWARD_BASE> items = TDATA.GetGachaItemList(tdata);

			if (items.Find(o=>o.Type == Res_RewardType.Zombie) != null)
				cnt[1] += m_Stagetable.m_ClearReward[i].m_Count;
			else
				cnt[0] += m_Stagetable.m_ClearReward[i].m_Count;
		}
		m_SUI.RewardMsg.text = string.Format(TDATA.GetString(742), cnt[0], cnt[1]);
		yield break;
	}
	IEnumerator ScrollCenter(int _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.Scroll.enabled = false;
		iTween.StopByName(gameObject, "Scrolling");
		float padding = m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>().padding.left;
		float to = m_LvElements[_pos].transform.localPosition.x - padding - 100f;// / m_SUI.Scroll.content.rect.width;
		to = Mathf.Clamp(to, 0f, m_SUI.Scroll.content.rect.width - Screen.width - padding);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.content.localPosition.x, "to", -to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		yield return new WaitForSeconds(0.3f);

		m_SUI.Scroll.enabled = true;
	}
	void TW_Scrolling(float _amount) {
		m_SUI.Scroll.content.localPosition = new Vector3(_amount, 0f, 0f);
		//m_SUI.Scroll.horizontalNormalizedPosition = _amount;
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
