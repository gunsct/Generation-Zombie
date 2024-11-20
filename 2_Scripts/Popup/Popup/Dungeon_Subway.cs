using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_Subway : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Timer;
		public GameObject[] Decos;
		public GameObject LvGroupPrefab;//Item_BankLvGroup, maxlv / 2
		public Transform LvGroupBucket;
		public ScrollRect Scroll;
		public TextMeshProUGUI Name;
		public Transform RewardPrefab;//Item_Subway_RewardElement
		public Transform RewardBucket;


		public GameObject[] TutoObj;//0:보상 리스트, 1:입장버튼
	}
	[SerializeField] SUI m_SUI;
	public List<List<int>> m_DecoIdxs = new List<List<int>>();

	private void Awake() {
		m_DecoIdxs.Add(new List<int>() {0,14,11 });//3
		m_DecoIdxs.Add(new List<int>() {1,15,12 });//3
		m_DecoIdxs.Add(new List<int>() {2,16 });//2
		m_DecoIdxs.Add(new List<int>() {3,10,13 });//3
		m_DecoIdxs.Add(new List<int>() {4,9 });//2
		m_DecoIdxs.Add(new List<int>() {5,8 });//2
		m_DecoIdxs.Add(new List<int>() {6,7,14 });//3
		m_DecoIdxs.Add(new List<int>() {7,6 });//2
		m_DecoIdxs.Add(new List<int>() {8,5,15 });//3
		m_DecoIdxs.Add(new List<int>() {9,4,16 });//3
		m_DecoIdxs.Add(new List<int>() {10, 3,0 });//3
		m_DecoIdxs.Add(new List<int>() {11, 2 });//2
		m_DecoIdxs.Add(new List<int>() {12, 1 });//2
		m_DecoIdxs.Add(new List<int>() {13, 0, 1 });//3
	}
	private void Update() {
		for(int i = 0;i<m_SUI.Timer.Length;i++)
			m_SUI.Timer[i].text = string.Format("{0} {1}", UTILE.GetSecToTimeStr(0, 86400 - UTILE.Get_ServerTime() % 86400), TDATA.GetString(728));
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		GetLv();
		SetCommUI();
		SetNotCommUI(true);
		SetButtUI();
		//StartCoroutine(ScrollCenter(m_ClearLv));
		StartCoroutine(StartDelay());
	}
	IEnumerator StartDelay() {
		MainMenuType premenu = STAGEINFO.GetPreMenu();
		if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0182);
		yield return SetSubwayUI();
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		if (TUTO.IsTuto(TutoKind.Subway, (int)TutoType_Subway.Subway_Action)) TUTO.Next();
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
		m_SUI.Name.text = m_Stagetable.GetName();
		for(int i = 0; i < m_SUI.Decos.Length; i++) {
			m_SUI.Decos[i].SetActive(false);
		}
		List<int> idxs = m_DecoIdxs[m_Modetable.m_OpenDay * 2 + m_Modetable.m_Pos];
		for(int i = 0; i < idxs.Count; i++) {
			m_SUI.Decos[idxs[i]].SetActive(true);
		}
	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		if (!Utile_Class.IsAniPlay(m_SUI.Anim)) StartCoroutine(SetSubwayUI());

	}
	IEnumerator SetSubwayUI() {
		List<RES_REWARD_BASE> rewards = GetRewards(m_Stagetable);
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.RewardBucket, m_SUI.RewardPrefab);
		for(int i = 0; i < rewards.Count; i++) {
			m_SUI.RewardBucket.GetChild(i).GetComponent<Item_Subway_RewardElement>().SetData(rewards[i], null, false);
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
	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Subway, 0)) return;
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
	///////튜토용
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}
}
