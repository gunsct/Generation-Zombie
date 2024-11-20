using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_TowerBG : ObjMng
{
	public enum AniName {
		Start = 0,
		End,
		None
	}
	[Serializable]
	public struct SUI
	{
		public Animator Ani;
		public SpriteRenderer BG;
		public GameObject StartCardPrefab;
		public GameObject CardPrefab;
		public Transform CardsParent;
		public SpriteRenderer[] BGGrads;
	}
	[SerializeField]
	SUI m_SUI;

	Item_Tower_Element[,] m_Cards;
	public List<Item_Tower_Element> m_EnemyCard = new List<Item_Tower_Element>();
	public Item_Tower_Element m_PreCard;
	public Item_Tower_Element m_NowCard;
	public int m_LastRow;		//마지막 행
	IEnumerator m_Play;
	float m_LastDragY;

	Vector2 m_Interval = new Vector2(3.5f, 6.5f);
	/// <summary> 타워 스테이지 세팅</summary>
	public void SetData() {
		Init();

		Color bgcolor = Utile_Class.GetCodeColor("#79B39B");
		switch (STAGEINFO.m_LV / 10) {
			case 0 : bgcolor = Utile_Class.GetCodeColor("#79B39B"); break;
		}
		m_SUI.BG.sprite = STAGEINFO.m_TStage.GetStageBG();
		TOWER.BG.color = m_SUI.BG.color = bgcolor;
		
		List<TTowerMapTable> mapdatas = TDATA.GetTowerMapGroupTable(STAGEINFO.m_TStage.m_DifficultyType, STAGEINFO.m_Idx);//타워만 예외로 레벨이 아닌 stagetable의 인덱스로 호출함
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup();

		TTowerMapTable last = mapdatas.Find(t => t.m_EventType == TowerEventType.Boss);
		m_LastRow = last.m_Row;

		m_Cards = new Item_Tower_Element[m_LastRow + 1, 3];
		for (int i = 0;i< mapdatas.Count; i++) {
			TStageCardTable cardtable = null;
			cardtable = TDATA.GetStageCardTable(mapdatas[i].m_EventVal);
			Item_Tower_Element card = Utile_Class.Instantiate(cardtable.m_Type == StageCardType.Tower_Entrance ? m_SUI.StartCardPrefab : m_SUI.CardPrefab, m_SUI.CardsParent).GetComponent<Item_Tower_Element>();
			card.SetData(cardtable, mapdatas[i]);
			if (card.m_Info.IS_EnemyCard && !card.m_Info.ISRefugee && !card.m_Info.IS_Boss) m_EnemyCard.Add(card);
			m_Cards[mapdatas[i].m_Row, mapdatas[i].m_Column] = card;
			card.transform.localPosition = new Vector3(m_Interval.x * (mapdatas[i].m_Column - 1), -4f + m_Interval.y * mapdatas[i].m_Row, 0f);

			if (mapdatas[i].m_EventType == TowerEventType.Entrance) m_NowCard = card;
		}
	}
	void Init() {
		m_LastDragY = 0;
		m_SUI.CardsParent.localPosition = Vector3.zero;

		if (m_Cards != null) {
			for (int i = 0; i < m_Cards.GetLength(0); i++) {
				for (int j = 0; j < m_Cards.GetLength(1); j++) {
					if (m_Cards[i, j] != null) Destroy(m_Cards[i, j].gameObject);
				}
			}
		}

		m_EnemyCard.Clear();
		m_PreCard = null;
		m_NowCard = null;
		m_LastRow = 0;
	}
	/// <summary> 선택이 끝난 후 이전 다음 현재 카드 및 길 갱신</summary>
	public void SelectEnd() {
		int prerow = m_PreCard.m_MapData.m_Row;
		int nowrow = m_NowCard.m_MapData.m_Row;
		//이전거 락, 패스 갱신
		for (int i = 0; i < 3; i++) {
			if (m_Cards[prerow, i] == null) continue;
			m_Cards[prerow, i].PathSelectEnd(m_PreCard, m_NowCard);
			m_Cards[prerow, i].SetLock(null);
		}
		//다음거 언락
		if (m_NowCard.m_MapData.m_Row < m_LastRow) {
			for (int i = 0; i < 3; i++) {
				if (m_Cards[nowrow + 1, i] == null) continue;
				if (m_Cards[nowrow + 1, i] != null) m_Cards[nowrow + 1, i].SetLock(m_NowCard.m_MapData);
			}
		}
		//선택한거 락, 패스 갱신
		for (int i = 0; i < 3; i++) {
			if (m_Cards[nowrow, i] == null) continue;
			m_Cards[nowrow, i].SetLock(null);
			if (m_NowCard.m_MapData.m_Row != m_LastRow)
				m_Cards[nowrow, i].PathNext(m_NowCard, new Item_Tower_Element[3] { m_Cards[nowrow + 1, 0], m_Cards[nowrow + 1, 1], m_Cards[nowrow + 1, 2] });
		}
		if (m_NowCard.m_MapData.m_Row < m_LastRow)
			iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.CardsParent.localPosition.y, "to", - m_Interval.y * nowrow , "time", 0.5f, "onupdate", "TW_AutoMapDrag"));
	}
	public void PlayAni(AniName ani, Action EndCB = null, bool isCreateAction = true)
	{
		if(m_Play != null)
		{
			StopCoroutine(m_Play);
			m_Play = null;
		}
		switch(ani)
		{
		case AniName.None: m_Play = Ani_EndCheck(EndCB); break;
		case AniName.Start: 
				m_Play = Ani_Start(EndCB, isCreateAction);
				break;
		case AniName.End:
				m_Play = Ani_EndCheck(EndCB); 
				break;
		}
		m_SUI.Ani.SetTrigger(ani.ToString());
		StartCoroutine(m_Play);
	}
	IEnumerator Ani_EndCheck(Action EndCB)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));

		if (m_Cards != null) {
			for (int i = 0; i < m_Cards.GetLength(0); i++) {
				for (int j = 0; j < m_Cards.GetLength(1); j++) {
					if (m_Cards[i, j] != null) m_Cards[i, j].SetFX(false);
				}
			}
		}
		m_SUI.BGGrads[0].color = m_SUI.BGGrads[1].color = new Color(0f, 0f, 0f, 0f);
		EndCB?.Invoke();
		m_Play = null;
	}
	IEnumerator Ani_Start(Action EndCB, bool isCreateAction = true)
	{
		m_SUI.BGGrads[0].color = m_SUI.BGGrads[1].color = new Color(0f, 0f, 0f, 1f);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));

		if (m_Cards != null) {
			for (int i = 0; i < m_Cards.GetLength(0); i++) {
				for (int j = 0; j < m_Cards.GetLength(1); j++) {
					if (m_Cards[i, j] != null) m_Cards[i, j].SetFX(true);
				}
			}
		}

		for (int i = 0; i < 3; i++) {
			if (m_Cards[1, i] != null) m_Cards[1, i].SetLock(m_NowCard.m_MapData);
		}

		EndCB?.Invoke();
		m_Play = null;
	}
	public void MapDrag(float _ypos) {
		float movey = 0f;
		if (m_LastDragY != 0f) movey = _ypos - m_LastDragY;
		m_LastDragY = _ypos;
		m_SUI.CardsParent.localPosition = new Vector3(0f, Mathf.Clamp(m_SUI.CardsParent.localPosition.y + movey, 5f - m_Interval.y * (m_LastRow - 1), 0f), 0f);
	}
	void TW_AutoMapDrag(float _amount) {
		m_SUI.CardsParent.localPosition = new Vector3(0f, Mathf.Clamp(_amount, 5f - m_Interval.y * (m_LastRow - 1), 0f), 0f);
	}
	public void InitLastY() {
		m_LastDragY = 0f;
	}
}
