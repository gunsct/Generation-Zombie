using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_Bank : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform[] DecoPos;//x-300~200 y-300~190
		public Transform[] DecoRot;//5~10도
		public TextMeshProUGUI Dollar;
		public GameObject LvGroupPrefab;//Item_BankLvGroup, maxlv / 2
		public Transform LvGroupBucket;
		public ScrollRect Scroll;
	}
	[SerializeField] SUI m_SUI;
	List<Item_DG_Bank_Stg> m_LvElements = new List<Item_DG_Bank_Stg>();//레벨 엘리먼트들 순서대로 받음

	private void Awake() {
		for (int i = 0; i < m_SUI.DecoPos.Length; i++) {
			m_SUI.DecoPos[i].gameObject.SetActive(false);
		}
		TW_DollarCounting(0);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

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
		if (premenu != MainMenuType.Dungeon) SND.PlayEffSound(SND_IDX.SFX_0186);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Bank_Stg.State.NotSelect);
		yield return SetDollarUI();
		yield return new WaitForSeconds(0.3f);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Bank_Stg.State.Select);
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
		for (int i = 0, cnt = 0; cnt < m_Lv[1]; i++) {
			Item_BankLvGroup lvgroup = Utile_Class.Instantiate(m_SUI.LvGroupPrefab, m_SUI.LvGroupBucket).GetComponent<Item_BankLvGroup>();
			cnt += lvgroup.SetData(i == 0, (i - 1) % 5, cnt, m_Lv[1]);
			m_LvElements.AddRange(lvgroup.GetElements());
		}
		for (int i = 0; i < m_LvElements.Count; i++) {
			if (i >= m_Lv[1]) m_LvElements[i].gameObject.SetActive(false);
			else m_LvElements[i].SetData(i, m_Modetables[i].m_StageLimit, m_Modetables[i].m_DiffType, SetLv);
		}
	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		if (!Utile_Class.IsAniPlay(m_SUI.Anim)) StartCoroutine(SetDollarUI());
		for (int i = 0; i < m_LvElements.Count; i++) {
			Item_DG_Bank_Stg.State state = Item_DG_Bank_Stg.State.NotOpen;
			if (i <= m_ClearLv && i < m_Lv[2]) {
				state = Item_DG_Bank_Stg.State.NotSelect;
			}
			//else {
			//	state = i == m_Lv[2] - 1 && m_Lv[2] > 0 && m_LimitStg > USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx ? Item_DG_Bank_Stg.State.Lock : Item_DG_Bank_Stg.State.NotOpen;
			//}
			else if (m_Modetables[i].m_StageLimit > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[i].m_DiffType].Idx) {
				state = Item_DG_Bank_Stg.State.Lock;
			}
			else state = Item_DG_Bank_Stg.State.NotOpen;
			if (i == m_Lv[0]) state = Item_DG_Bank_Stg.State.Select;
			m_LvElements[i].SetAnim(state);
		}
	}
	//달러 유아이 세팅
	IEnumerator SetDollarUI() {
		for (int i = 0; i < m_SUI.DecoPos.Length; i++) {
			m_SUI.DecoPos[i].gameObject.SetActive(false);
		}
		TW_DollarCounting(0);
		//달러 보상 총합 및 카운팅
		int dollar = GetRewards(m_Stagetable).FindAll(o => o.Type == Res_RewardType.Money).Sum(o => ((RES_REWARD_MONEY)o).Add);
		iTween.StopByName(gameObject, "DollarCounting");
		iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", dollar, "onupdate", "TW_DollarCounting", "time", 1f, "name", "DollarCounting"));
		yield return new WaitForSeconds(0.5f);
		//데코 돈뭉치
		List<int> nums = new List<int>();
		for (int i = 0; i < 10; i++) nums.Add(i);
		int step = Mathf.Clamp(m_Lv[0] / 10, 2, 10);
		for (int i = 0; i < step; i++) {
			int randpos = UTILE.Get_Random(0, nums.Count);
			int pos = nums[randpos];
			nums.Remove(pos);

			m_SUI.DecoRot[pos].eulerAngles = new Vector3(0f, 0f, UTILE.Get_Random(5f, 10f));
			//m_SUI.DecoPos[pos].position = new Vector3(UTILE.Get_Random(-300, 200), UTILE.Get_Random(-300, 200), m_SUI.DecoPos[i].position.z);
			m_SUI.DecoPos[pos].SetAsFirstSibling();
			m_SUI.DecoPos[pos].gameObject.SetActive(true);
			yield return new WaitForSeconds(0.1f);
		}
	}
	void TW_DollarCounting(float _amount) {
		int dollar = Mathf.RoundToInt(_amount);
		int nonzero = 5;
		StringBuilder zerotxt = new StringBuilder();
		while (dollar / 10 > 0) {
			dollar /= 10;
			nonzero--;
		}
		for (int i = 0; i < nonzero; i++) zerotxt.Append("0");
		m_SUI.Dollar.text = string.Format("$ <alpha=\"50%\">{0}</color>{1}", zerotxt.ToString(), Utile_Class.CommaValue(_amount));
	}
	IEnumerator ScrollCenter(int _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.Scroll.enabled = false;
		iTween.StopByName(gameObject, "Scrolling");

		float to = m_LvElements[_pos].transform.parent.localPosition.x - m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>().padding.left - 100f;// / m_SUI.Scroll.content.rect.width;
		to = Mathf.Clamp(to, 0f, m_SUI.Scroll.content.rect.width - Screen.width);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.content.localPosition.x, "to", -to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		//float to = Mathf.Max(0f, (((RectTransform)m_LvElements[_pos].transform.parent).localPosition.x - ((RectTransform)m_LvElements[_pos].transform.parent).rect.width * 2f) / m_SUI.Scroll.content.rect.width);
		//iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.horizontalNormalizedPosition, "to", to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

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
