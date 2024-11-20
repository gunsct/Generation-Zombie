using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;
using static LS_Web;
using System.Text;
using UnityEngine.UI;

public class Dungeon_University_Detail : Dungeon_Info
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public ScrollRect RewardScroll;

		public Image Icon;
		public Sprite[] Icons;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI[] Days;
		public Image[] DaysBG;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI Deco;
		public Item_RewardList_Item[] RewardCard;
		public Item_University_Reward_Piece[] RewardPieces;
		public TextMeshProUGUI RewardDesc;
		public Transform LvPrefab;//Item_DG_University_Stg
		public Transform LvBucket;
	}
	[SerializeField] SUI m_SUI;
	Action m_CloseCB;
	List<RES_REWARD_BASE> m_PieceRewards = new List<RES_REWARD_BASE>();
	List<Item_DG_University_Stg> m_LvElements = new List<Item_DG_University_Stg>();
	private void Awake() {
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		//m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>().padding.right = Screen.width;

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
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_University_Stg.State.NotSelect);
		yield return SetUniversityUI();
		yield return new WaitForSeconds(0.4f);
		m_LvElements[m_Lv[0]].SetAnim(Item_DG_University_Stg.State.Select);
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
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.University];
		UserInfo.StageIdx stlv = stage.Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos);
		int stgidx = TDATA.GetModeTable(StageContentType.University, stlv.Idx, m_Day, m_Pos).m_StageIdx;
		TStageTable tdata = TDATA.GetStageTable(stgidx);

		m_SUI.Icon.sprite = m_SUI.Icons[((int)m_Day - 1) * 2 + m_Pos];
		m_SUI.Title.text = tdata.GetName();
		switch (m_Day) {
			case DayOfWeek.Monday:
				m_SUI.Title.color = Utile_Class.GetCodeColor("#3C191B");
				m_SUI.DaysBG[0].color = m_SUI.DaysBG[1].color = Utile_Class.GetCodeColor("#602727");
				break;
			case DayOfWeek.Tuesday:
				m_SUI.Title.color = Utile_Class.GetCodeColor("#443825");
				m_SUI.DaysBG[0].color = m_SUI.DaysBG[1].color = Utile_Class.GetCodeColor("#5B4320");
				break;
			case DayOfWeek.Wednesday:
				m_SUI.Title.color = Utile_Class.GetCodeColor("#1E351E");
				m_SUI.DaysBG[0].color = m_SUI.DaysBG[1].color = Utile_Class.GetCodeColor("#1F4423");
				break;
			case DayOfWeek.Thursday:
				m_SUI.Title.color = Utile_Class.GetCodeColor("#191C3C");
				m_SUI.DaysBG[0].color = m_SUI.DaysBG[1].color = Utile_Class.GetCodeColor("#273860");
				break;
			case DayOfWeek.Friday:
				m_SUI.Title.color = Utile_Class.GetCodeColor("#3B3044");
				m_SUI.DaysBG[0].color = m_SUI.DaysBG[1].color = Utile_Class.GetCodeColor("#41215B");
				break;
		}
		m_SUI.Desc.text = tdata.GetInfo();
		m_SUI.Days[0].text = TDATA.GetString(141 + (int)m_Day);
		switch (m_Day) {
			case DayOfWeek.Monday:
			case DayOfWeek.Tuesday:
			case DayOfWeek.Wednesday:
				m_SUI.Days[1].text = TDATA.GetString(147);
				break;
			case DayOfWeek.Thursday:
			case DayOfWeek.Friday:
				m_SUI.Days[1].text = TDATA.GetString(141);
				break;
		}
		m_SUI.Deco.text = string.Format("<size=200>{0}</size> Class", m_Pos == 0 ? "A" : "B");

		
		UTILE.Load_Prefab_List(m_Lv[1], m_SUI.LvBucket, m_SUI.LvPrefab);
		for (int i = 0; i < m_SUI.LvBucket.childCount; i++) {
			Item_DG_University_Stg element = m_SUI.LvBucket.GetChild(i).GetComponent<Item_DG_University_Stg>();
			element.SetData(i, m_Modetables[i].m_StageLimit, m_Modetables[i].m_DiffType, SetLv);
			element.transform.GetChild(0).transform.localPosition = new Vector3(0f, i%2 == 0 ? 104.43f : -22f, 0f);
			m_LvElements.Add(element);
		}

	}
	//레벨에 따라 바뀌는 유아이들
	protected override void SetNotCommUI(bool _first = false) {
		if (!Utile_Class.IsAniPlay(m_SUI.Anim)) StartCoroutine(SetUniversityUI());
		for (int i = 0; i < m_LvElements.Count; i++) {
			Item_DG_University_Stg.State state = Item_DG_University_Stg.State.NotOpen;
			if (i <= m_ClearLv && i < m_Lv[2]) {
				state = Item_DG_University_Stg.State.NotSelect;
			}
			//else {
			//	state = i == m_Lv[2] - 1 && m_Lv[2] > 0 && m_LimitStg > USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx ? Item_DG_University_Stg.State.Lock : Item_DG_University_Stg.State.NotOpen;
			//}
			else if (m_Modetables[i].m_StageLimit > USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)m_Modetables[i].m_DiffType].Idx) {
				state = Item_DG_University_Stg.State.Lock;
			}
			else state = Item_DG_University_Stg.State.NotOpen;
			if (i == m_Lv[0]) state = Item_DG_University_Stg.State.Select;
			m_LvElements[i].SetAnim(state);
		}
	}
	IEnumerator SetUniversityUI() {
		List<RES_REWARD_BASE> rewards = GetRewards(m_Stagetable);
		List<RES_REWARD_BASE> mats = rewards.FindAll(o => TDATA.GetItemTable(o.GetIdx()).m_Type != ItemType.CharaterPiece);
		m_PieceRewards = rewards.FindAll(o => TDATA.GetItemTable(o.GetIdx()).m_Type == ItemType.CharaterPiece);
		for(int i = 0; i < m_SUI.RewardCard.Length; i++) {
			m_SUI.RewardCard[i].transform.parent.gameObject.SetActive(i < mats.Count);
			if (i < mats.Count) {
				m_SUI.RewardCard[i].SetData(mats[i], null, false);
			}
		}
		int piececnt = 0;
		for (int i = 0; i < m_SUI.RewardPieces.Length; i++) {
			m_SUI.RewardPieces[i].gameObject.SetActive(i < m_PieceRewards.Count);
			if(i < m_PieceRewards.Count) {
				TCharacterTable cdata = TDATA.GetCharacterTableToPiece(m_PieceRewards[i].GetIdx());
				m_SUI.RewardPieces[i].SetData(cdata);
				piececnt += ((RES_REWARD_ITEM)m_PieceRewards[i]).Cnt;
			}
		}
		m_SUI.RewardDesc.text = string.Format("{0}\n({1})", TDATA.GetString(113), TDATA.GetString(730));//랜덤박스들은 수량 예측이 안되니.. 그냥 보상횟수 입력

		yield break;
	}
	/// <summary> 보상목록 보기 </summary>
	public void Click_RewardList() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_University_CharpieceList, null, m_Stagetable, m_Lv[0], m_PieceRewards);
	}
	IEnumerator ScrollCenter(int _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.Scroll.enabled = false;
		iTween.StopByName(gameObject, "Scrolling");
		float to = m_LvElements[_pos].transform.localPosition.x - m_SUI.Scroll.content.GetComponent<HorizontalLayoutGroup>().padding.left - 100f;// / m_SUI.Scroll.content.rect.width;
		to = Mathf.Clamp(to, 0f, m_SUI.Scroll.content.rect.width - Screen.width);
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Scroll.content.localPosition.x, "to", -to, "onupdate", "TW_Scrolling", "time", 0.3f, "name", "Scrolling"));

		yield return new WaitForSeconds(0.3f);

		m_SUI.Scroll.enabled = true;
	}
	void TW_Scrolling(float _amount) {
		m_SUI.Scroll.content.localPosition = new Vector3(_amount, 0f, 0f);
		//m_SUI.Scroll.horizontalNormalizedPosition = _amount;
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
		base.Close(Result);
	}

	public override void ScrollLock(bool _lock) {
		m_SUI.Scroll.enabled = !_lock;
	}
}
