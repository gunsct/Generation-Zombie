using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Item_Tower_Element : ObjMng
{
	public enum State {
		Active,
		Deactive,
		Now
	}

	[Serializable]
	public struct Paths
	{
		public GameObject[] Path;
	}																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																									
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Paths[] Path;
		public GameObject[] EnemyGrade;
		public SpriteRenderer Icon;
		public TextMeshPro Name;
		public ParticleSystem FX;
	}
	[SerializeField]
	SUI m_SUI;
	[HideInInspector] public StageCardInfo m_Info;
	[HideInInspector] public TTowerMapTable m_MapData;
	GameObject[] m_Paths = new GameObject[3];
	public bool m_Lock;

	public void SetData(TStageCardTable table, TTowerMapTable _map) {
		Init();

		bool change = m_Info != null;

		m_Info = new StageCardInfo(table.m_Idx);
		m_MapData = _map;

		m_SUI.Name.text = m_Info.GetName();

		switch (m_MapData.m_EventType) {
			case TowerEventType.Entrance:
				m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/SW_Door", "png");
				break;
			case TowerEventType.NormalEnemy:
			case TowerEventType.EliteEnemy: 
			case TowerEventType.Boss:
				TEnemyTable enemy = TDATA.GetEnemyTable(Mathf.RoundToInt(table.m_Value1));
				m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("Card/Tower/TowerIcon_{0}", (int)enemy.m_Type), "png");

				for (int i = 0; i < (int)enemy.m_Grade; i++) {
					m_SUI.EnemyGrade[i].SetActive(true);
				}
				switch ((int)enemy.m_Grade) {
					case 1:
						m_SUI.EnemyGrade[0].transform.localPosition = new Vector3(0f, -0.2609997f, 0f);
						break;
					case 2:
						m_SUI.EnemyGrade[0].transform.localPosition = new Vector3(-0.3f, -0.2609997f, 0f);
						m_SUI.EnemyGrade[1].transform.localPosition = new Vector3(0.3f, -0.2609997f, 0f);
						break;
					case 3:
						m_SUI.EnemyGrade[0].transform.localPosition = new Vector3(-0.548f, -0.2609997f, 0f);
						m_SUI.EnemyGrade[1].transform.localPosition = new Vector3(0f, -0.1159998f, 0f);
						m_SUI.EnemyGrade[2].transform.localPosition = new Vector3(0.482f, -0.285f, 0f);
						break;
				}
				break;
			case TowerEventType.OpenEvent:
			case TowerEventType.SecrectEvent:
				//선택 전이고 카드테이블에서 오픈, 시크립 타입일때는 랜덤 이미지
				if (!change && (table.m_Type == StageCardType.Tower_OpenEvent || table.m_Type == StageCardType.Tower_SecrectEvent))
					m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_16", "png");
				else {
					switch (table.m_Type) {
						case StageCardType.Enemy:
							enemy = TDATA.GetEnemyTable(Mathf.RoundToInt(table.m_Value1));
							m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("Card/Tower/TowerIcon_{0}", (int)enemy.m_Type), "png");
							break;
						case StageCardType.Tower_Refugee: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_13", "png"); break;
						case StageCardType.Tower_Rest: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_12", "png"); break;
						case StageCardType.Tower_Status: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_15", "png"); break;
						case StageCardType.Tower_SupplyBox:
							switch (table.m_Value2) {
								case 0: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_11", "png"); break;
								case 2: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_10", "png"); break;
								default: m_SUI.Icon.sprite = UTILE.LoadImg("Card/Tower/TowerIcon_9", "png"); break;
							}
							break;
					}
				}
				break;
		}

		SetLock(null);
		SetPath();
	}
	public void Init() {
		if (m_SUI.EnemyGrade.Length == 3)
			for (int i = 0; i < 3; i++) {
				m_SUI.EnemyGrade[i].SetActive(false);
		}
		for(int i = 0; i < m_SUI.Path.Length; i++) {
			for(int j = 0; j < m_SUI.Path[i].Path.Length; j++) {
				m_SUI.Path[i].Path[j].SetActive(false);
			}
			m_Paths[i] = null;
		}
	}
	public void PlayAni(State _state, float waittime, Action EndCB = null) {
		StartCoroutine(Ani_EndCheck(_state, waittime, EndCB));
	}

	IEnumerator Ani_EndCheck(State _state, float waittime, Action EndCB) {
		yield return new WaitForSeconds(waittime);
		if (m_SUI.Anim != null) m_SUI.Anim.SetTrigger(_state.ToString());
		yield return new WaitForEndOfFrame();
		if (m_SUI.Anim != null) yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		EndCB?.Invoke();
	}
	/// <summary> 현재 카드의 잠금 상태 체크 </summary>
	public void SetLock(TTowerMapTable _table = null) {
		bool isnext = false;
		if (_table == null) m_Lock = true;
		else {
			for (int i = 0; i < _table.m_Ways.Length; i++) {
				if (_table.m_Ways[i] == m_MapData.m_Idx) isnext = true;
			}
			m_Lock = (m_MapData.m_Row != _table.m_Row + 1 || !isnext);
		}
		if (m_SUI.Anim != null) m_SUI.Anim.SetTrigger(m_Lock ? "Deactive" : "Active");
	}
	/// <summary> 최초 길 생성 </summary>
	void SetPath() {
		List<List<int>> posgroup = new List<List<int>>() { new List<int>() { 0, 1, 2, 3 }, new List<int>() { 0, 1 }, new List<int>() { 0 } };
		for (int i = 0 ; i < m_MapData.m_Ways.Length; i++) {
			TTowerMapTable table = TDATA.GetTowerMapTable(m_MapData.m_Ways[i]);
			if (table == null) continue;
			int dist = table.m_Column - m_MapData.m_Column;//-왼쪽 +오른쪽
			int gid = Mathf.Abs(dist);
			int pos = posgroup[Mathf.Abs(dist)][UTILE.Get_Random(0, posgroup[gid].Count)];
			posgroup[gid].Remove(pos);
			GameObject path = m_SUI.Path[gid].Path[pos];
			path.transform.localEulerAngles = new Vector3(0f, dist <= 0 ? 0f : 180f, 0f);
			path.SetActive(true);
			m_Paths[table.m_Column] = path;
			path.transform.GetChild(0).GetComponent<Animator>().SetTrigger(m_MapData.m_EventType != TowerEventType.Entrance ?  "NotYet" : "Active");
		}
	}
	/// <summary> 선택한 카드의 다음으로 가는 길 표시 갱신</summary>
	public void PathNext(Item_Tower_Element _now, Item_Tower_Element[] _nexts) {
		for (int i = 0; i < m_Paths.Length; i++) {
			if (m_Paths[i] != null) m_Paths[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger(_nexts[i].m_Lock || _now != this ? "Deactive" : "Active");
		}
	}
	/// <summary> 선택한 카드 끝난 후 이전 길 표시 갱신 </summary>
	public void PathSelectEnd(Item_Tower_Element _pre, Item_Tower_Element _now) {
		for (int i = 0; i < m_Paths.Length; i++) {
			if (m_Paths[i] == null) continue;
			GetComponent<SortingGroup>().sortingOrder = i == _now.m_MapData.m_Column && this == _pre ? 3 : 2;
			m_Paths[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger(i == _now.m_MapData.m_Column && this == _pre ? "NotYet" : "ActToDeact");
		}
	}
	public void SetFX(bool _on) {
		if (_on) m_SUI.FX.Play();
		else m_SUI.FX.Stop();
	}
}
