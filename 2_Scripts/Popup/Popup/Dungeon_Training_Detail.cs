using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_Training_Detail : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public ScrollRect RewardScroll;
		public TextMeshProUGUI Title;
		public Image BG;
		public Sprite[] BGs;
		public Transform LvPrefab;//Item_DG_Training_Stg
		public Transform LvBucket;
		public GameObject[] FXs;
		public TextMeshProUGUI Energy;
		public GameObject EnergyGroup;
	}
	[SerializeField] SUI m_SUI;
	List<Item_DG_Training_Stg> m_LvElements = new List<Item_DG_Training_Stg>();
	Action m_CloseCB;
	TStageTable m_TData { get { return TDATA.GetStageTable(TDATA.GetModeTable(m_Content, m_Day, m_Pos)[m_Lv[0]].m_StageIdx); } }

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFShellUI += SetEnergy;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFShellUI -= SetEnergy;
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		PLAY.PlayEffSound(SND_IDX.SFX_1902);

		m_CloseCB = (Action)aobjValue[3];

		GetLv();
		SetCommUI();
		SetNotCommUI(true);
		SetButtUI();
		int initlv = m_ClearLv == m_Lv[2] ? m_ClearLv : Mathf.Max(0, m_ClearLv - 1);
		StartCoroutine(ScrollCenter(m_Lv[0]));
		StartCoroutine(StartDelay());
	}
	IEnumerator StartDelay() {
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Training_Stg.State.NotSelect);
		yield return SetTrainingUI();
		yield return new WaitForSeconds(0.2f);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_Training_Stg.State.Select);
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
		for(int i = m_LvElements.Count - 1; i > -1; i--) {
			Destroy(m_LvElements[i]);
			m_LvElements.RemoveAt(i);
		}
		for(int i = 0; i < m_Lv[1]; i++) {
			Item_DG_Training_Stg element = Utile_Class.Instantiate(m_SUI.LvPrefab.gameObject, m_SUI.LvBucket).GetComponent<Item_DG_Training_Stg>();
			element.SetData(i, m_Modetables[i].m_StageLimit, m_Modetables[i].m_DiffType, m_Pos, GetReward(i), SetLv);
			m_LvElements.Add(element);
		}
		m_SUI.BG.sprite = m_SUI.BGs[m_Pos];
		switch (m_Pos) {//노초빨파
			case 0: 
				m_SUI.Title.text = TDATA.GetString(233); 
				break;
			case 1: 
				m_SUI.Title.text = TDATA.GetString(234); 
				break;
			case 2: 
				m_SUI.Title.text = TDATA.GetString(235); 
				break;
			case 3: 
				m_SUI.Title.text = TDATA.GetString(236); 
				break;
		}
	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		if (!Utile_Class.IsAniPlay(m_SUI.Anim)) StartCoroutine(SetTrainingUI());
		for (int i = 0; i < m_LvElements.Count; i++) {
			Item_DG_Training_Stg.State state = Item_DG_Training_Stg.State.NotOpen;
			if (i <= m_ClearLv && i < m_Lv[2]) { 
				state = Item_DG_Training_Stg.State.NotSelect;
			}
			//else {
			//	state = i == m_Lv[2] - 1 && m_Lv[2] > 0 && m_LimitStg > USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx ? Item_DG_Training_Stg.State.Lock : Item_DG_Training_Stg.State.NotOpen;
			//}
			else if (m_Modetables[i].m_StageLimit > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[i].m_DiffType].Idx) {
				state = Item_DG_Training_Stg.State.Lock;
			}
			else state = Item_DG_Training_Stg.State.NotOpen;
			if (i == m_Lv[0]) state = Item_DG_Training_Stg.State.Select;
			m_LvElements[i].SetAnim(state);
		}

		m_SUI.Energy.text = string.Format("<size=140%>-</size> {0}", m_TData.m_Energy);
		SetEnergy(USERINFO.m_Energy.Cnt);
	}
	protected override void SetButtUI() {
		base.SetButtUI();
		m_SUI.EnergyGroup.SetActive(m_TData.m_Energy > 0 && USERINFO.m_Stage[m_Content].IS_CanGoStage());
	}
	void SetEnergy(long _cnt) {
		m_SUI.Energy.color = BaseValue.GetUpDownStrColor(_cnt, m_TData.m_Energy, "#D2533C", "#FFFFFF");
	}
	List<RES_REWARD_BASE> GetReward(int _lv) {
		TModeTable modetable =  TDATA.GetModeTable(m_Content, m_Day, m_Pos)[_lv];
		TStageTable stagetable =  TDATA.GetStageTable(modetable.m_StageIdx);
		return GetRewards(stagetable);
	}
	IEnumerator SetTrainingUI() {
		yield break;
	}
	IEnumerator ScrollCenter(int _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.Scroll.enabled = false;
		iTween.StopByName(gameObject, "Scrolling");

		float to = Mathf.Abs(m_LvElements[_pos].transform.localPosition.y + ((RectTransform)m_LvElements[_pos].transform).rect.height * 3f);// / m_SUI.Scroll.content.rect.width;
		to = Mathf.Clamp(to, 0f, m_SUI.Scroll.content.rect.height - Screen.height);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.content.localPosition.y, "to", to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		//float to = 1f - Mathf.Abs(m_LvElements[_pos].transform.localPosition.y) / m_SUI.Scroll.content.rect.height;
		//iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.verticalNormalizedPosition, "to", to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		yield return new WaitForSeconds(0.3f);

		m_SUI.Scroll.enabled = true;
	}
	void TW_Scrolling(float _amount) {
		m_SUI.Scroll.content.localPosition = new Vector3(0, _amount, 0f);
		//m_SUI.Scroll.verticalNormalizedPosition = _amount;
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int Result) {
		m_CloseCB?.Invoke();
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		for (int i = 0; i < m_SUI.FXs.Length; i++) {
			m_SUI.FXs[i].SetActive(false);
		}
		base.Close(Result);
	}

	public override void ScrollLock(bool _lock) {
		m_SUI.Scroll.enabled = !_lock;
	}
}
