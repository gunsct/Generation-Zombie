using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Instance
	private static TowerMng m_Instance = null;
	public bool m_IS_GameAccel = false;//배속기능 사용 여부
	public static TowerMng Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static bool IsValid()
	{
		return m_Instance != null;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Process
	private void Awake()
	{
#if NOT_USE_NET && STAGE_TEST
		if (MAIN.IS_BackState(MainState.START))
		{
			Debug.LogError("StageTest 씬을 통해서 실행할 것!!!!!!");
			MAIN.Exit();
			return;
		}
#elif UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.ReStart();
			return;
		}
#endif
		m_Instance = this;
		m_Map = UTILE.LoadPrefab("Item/Tower/Item_TowerBG", true, m_MapPanel).GetComponent<Item_TowerBG>();//string.Format("Item/Tower/Item_TowerBG", STAGEINFO.m_TStage.GetStageBGName())
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start()
	{
		m_PlayAction = StartAction();
		StartCoroutine(m_PlayAction);
	}

	private void Update()
	{
		SelectCheck();
		SlideCheck();
	}



	public bool IS_SelectAction_Pause()
	{
		if (!MAIN.IS_State(MainState.TOWER)) return true;
		if (POPUP.IS_PopupUI()) return true;
		if (POPUP.IS_MsgUI()) return true;
		return false;
	}
	public bool IS_SelectAction()
	{
		return m_PlayAction != null;
	}

	void SelectCheck() {
		if (IS_SelectAction_Pause()) return;
		if (IS_SelectAction()) return;
		if (TouchCheck()) {
			if (m_MainUI.IsShowSynergyInfo() || m_MainUI.IsShowAlarmToolTip() || m_MainUI.IsShowDebuffCardInfo()) {
				m_MainUI.OffSynergyInfo();
				m_MainUI.OffAlarmToolTip();
				m_MainUI.OffAlarmDebuffCardToolTip();
			}
			else {
				Vector2 worldpos = Utile_Class.GetWorldPosition(Input.mousePosition);

				RaycastHit2D[] hit = Physics2D.RaycastAll(worldpos, Vector2.zero);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Tower_Element card = hitobj.GetComponent<Item_Tower_Element>();
					if (card != null) {
						if (card.m_Lock) continue;
						if (card.m_MapData.m_EventType == TowerEventType.Entrance) continue;
						// 스테이지 카드 선택
						m_Map.m_PreCard = m_Map.m_NowCard;
						m_Map.m_NowCard = card;
						m_PlayAction = SelectAction(card);
						StartCoroutine(m_PlayAction);
					}
					break;
				}
			}
		}
	}
	void SlideCheck() {
#if UNITY_EDITOR
		if (Input.GetMouseButton(0))
#else
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
		{
			if (IS_SelectAction_Pause()) return;
			if (IS_SelectAction()) return;
			Vector2 worldpos = Utile_Class.GetWorldPosition(Input.mousePosition);
			m_Map.MapDrag(worldpos.y * 2f);
		}
		else m_Map.InitLastY();
	}
}
