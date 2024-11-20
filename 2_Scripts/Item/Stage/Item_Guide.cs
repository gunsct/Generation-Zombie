using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Guide : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Transform CardParent;
		public Transform InfoParent;
		public GameObject GoalCardPrefab;
		public GameObject GoalInfoPrefab;
		public ContentSizeFitter Fitter;
	}
	[SerializeField]
	SUI m_SUI;
	List<StageClearType> m_Type = new List<StageClearType>();
	Dictionary<StageClearType, List<int>> m_RefreshType = new Dictionary<StageClearType, List<int>>();
	Dictionary<StageClearType, List<int>> m_ClearType = new Dictionary<StageClearType, List<int>>();
	List<Item_Guide_GoalCard> m_GoalCards = new List<Item_Guide_GoalCard>();
	List<Item_Guide_GoalInfo> m_GoalInfos = new List<Item_Guide_GoalInfo>();
	bool m_IsReadyRefresh = false;
	bool m_IsReadyClear = false;

	public List<Item_Guide_GoalInfo> GetInfos { get { return m_GoalInfos; } }

	public void SetData() {
		TStageTable stage = STAGEINFO.m_TStage;
		List<TStageCondition<StageClearType>> _Clear = stage.m_Clear;
		bool is_continuity = STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.Continuity;
		for (int i = 0; i < _Clear.Count; i++) {
			m_Type.Add(_Clear[i].m_Type);

			Item_Guide_GoalCard card = Utile_Class.Instantiate(m_SUI.GoalCardPrefab, m_SUI.CardParent).GetComponent<Item_Guide_GoalCard>();
			card.SetData(_Clear[i],i);
			card.transform.localPosition = Vector3.zero;
			card.transform.localEulerAngles = Vector3.zero;
			card.transform.localScale = Vector3.one * 0.35f;
			card.transform.SetAsFirstSibling();
			m_GoalCards.Add(card);

			Item_Guide_GoalInfo info = Utile_Class.Instantiate(m_SUI.GoalInfoPrefab, m_SUI.InfoParent).GetComponent<Item_Guide_GoalInfo>();
			info.SetData(_Clear[i].m_Type, i, !is_continuity || (is_continuity && i == 0), _Clear.Count);
			m_GoalInfos.Add(info);
			//info.gameObject.SetActive(false);
		}
	}

	public void GuideTransRefresh() {
		switch (STAGEINFO.m_TStage.m_ClearMethod) {
			case ClearMethodType.None:
				//m_GoalInfos[0].gameObject.SetActive(true);
				iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(30f, 0f, 0f), "time", 0.5f, "islocal", true));
				iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -7f), "time", 0.5f, "islocal", true));
				iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.38f, 0.38f, 1f), "time", 0.5f, "islocal", true));
				break;
			case ClearMethodType.Continuity:
				//m_GoalInfos[0].gameObject.SetActive(true);
				if (m_GoalCards.Count < 2) {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(30f, 0f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -7f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.38f, 0.38f, 1f), "time", 0.5f, "islocal", true));
					m_GoalCards[0].SetAnim(Item_Guide_GoalCard.State.BlindOff, null);
				}
				else if (m_GoalCards.Count < 3) {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(12.1f, 3.1f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 2f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.35f, 0.35f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[1].gameObject, iTween.Hash("position", new Vector3(48f, -5.9f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[1].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -2.84f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[1].gameObject, iTween.Hash("scale", new Vector3(0.35f, 0.35f, 1f), "time", 0.5f, "islocal", true));

					m_GoalCards[0].SetAnim(m_GoalCards[1].m_State == Item_Guide_GoalCard.State.Blind ? Item_Guide_GoalCard.State.BlindOff : Item_Guide_GoalCard.State.Normal, null);
					m_GoalCards[1].SetAnim(Item_Guide_GoalCard.State.Blind, null);
				}
				else {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(-0.8f, 14.8f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 1.755f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[1].gameObject, iTween.Hash("position", new Vector3(25.5f, 2.3f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[1].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -2.693f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[1].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[2].gameObject, iTween.Hash("position", new Vector3(43f, -12.8f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[2].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -6.26f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[2].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));

					m_GoalCards[1].SetAnim(Item_Guide_GoalCard.State.Blind, null);
					m_GoalCards[2].SetAnim(Item_Guide_GoalCard.State.Blind, null);
				}
				break;
			case ClearMethodType.SameTime:
				//for(int i = 0;i< m_GoalCards.Count; i++) {
				//	m_GoalInfos[i].gameObject.SetActive(true);
				//}
				if (m_GoalCards.Count < 2) {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(30f, 0f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -7f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.38f, 0.38f, 1f), "time", 0.5f, "islocal", true));
				}
				else if (m_GoalCards.Count < 3) {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(-64f, -10f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 8f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.35f, 0.35f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[1].gameObject, iTween.Hash("position", new Vector3(69f, -16f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[1].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -13f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[1].gameObject, iTween.Hash("scale", new Vector3(0.35f, 0.35f, 1f), "time", 0.5f, "islocal", true));
				}
				else {
					iTween.MoveTo(m_GoalCards[0].gameObject, iTween.Hash("position", new Vector3(-128f, -20f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[0].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, 14.45f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[0].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[1].gameObject, iTween.Hash("position", new Vector3(-26.5f, 3.6f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[1].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -2f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[1].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));

					iTween.MoveTo(m_GoalCards[2].gameObject, iTween.Hash("position", new Vector3(85f, -29f, 0f), "time", 0.5f, "islocal", true));
					iTween.RotateTo(m_GoalCards[2].gameObject, iTween.Hash("rotation", new Vector3(0f, 0f, -21.5f), "time", 0.5f, "islocal", true));
					iTween.ScaleTo(m_GoalCards[2].gameObject, iTween.Hash("scale", new Vector3(0.33f, 0.33f, 1f), "time", 0.5f, "islocal", true));
				}
				break;
		}
	}

	private void Update() {
		if (m_IsReadyRefresh)
			if(IS_CanRefresh()) 
				Refresh();

		if (!m_IsReadyRefresh && m_IsReadyClear)
			if (IS_CanRefresh())
				Clear();
	}
	/// <summary> 목표 유아이 갱신이 가능 상태인지 여부, 현재는 상위 캔버스 그룹 알파로 하는데 위치로 바꿔.. </summary>
	bool IS_CanRefresh() {
		if (!transform.parent.GetComponent<CanvasGroup>()) return true;
		if (transform.parent.GetComponent<CanvasGroup>().alpha != 1f) return false;
		if (POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			if (STAGEINFO.m_Result == StageResult.Fail) return false;
			Main_Stage stage = POPUP.GetMainUI().GetComponent<Main_Stage>();
			if (!stage.IS_Anim(Main_Stage.AniName.Ready) && !stage.IS_Anim(Main_Stage.AniName.Out)) return true;
		}
		return false;
	}
	/// <summary> 가이드 체크시 호출 </summary>
	public void SetRefresh(StageClearType _type, int _pos) {
		m_IsReadyRefresh = true;
		if (!m_RefreshType.ContainsKey(_type)) m_RefreshType.Add(_type, new List<int>());
		if (!m_RefreshType[_type].Contains(_pos)) m_RefreshType[_type].Add(_pos);
	}
	void Refresh() {
		m_IsReadyRefresh = false;
		for(int i = m_RefreshType.Count - 1; i >= 0; i--) {
			for(int j = m_RefreshType.ElementAt(i).Value.Count - 1;j >= 0; j--) {
				Item_Guide_GoalCard card = m_GoalCards.Find((t) => t.m_Type == m_RefreshType.ElementAt(i).Key && t.m_Pos == m_RefreshType.ElementAt(i).Value[j]);
				if(card != null) card.SetAnim(Item_Guide_GoalCard.State.Change, null);
				Item_Guide_GoalInfo info = m_GoalInfos.Find((t) => t.m_Type == m_RefreshType.ElementAt(i).Key && t.m_Pos == m_RefreshType.ElementAt(i).Value[j]);
				if(info != null) info.Refresh();
			}
			m_RefreshType.Remove(m_RefreshType.ElementAt(i).Key);
		}
		if(STAGEINFO.m_PlayType == StagePlayType.Event) {
			MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => BaseValue.EVENT_LIST.Contains(o.Type));
			if (evt != null) {
				switch (evt.Prefab) {
					case "Event_10": PlayEffSound(SND_IDX.SFX_3060); break;
					default: PlayEffSound(SND_IDX.SFX_0209); break;
				}
			}
		}
		else PlayEffSound(SND_IDX.SFX_0209);
	}
	public void SetClear(StageClearType _type, int _pos) {
		m_IsReadyClear = true;
		if (!m_ClearType.ContainsKey(_type)) m_ClearType.Add(_type, new List<int>());
		if (!m_ClearType[_type].Contains(_pos)) m_ClearType[_type].Add(_pos);
	}
	void Clear() {
		m_IsReadyClear = false;
		for (int i = m_ClearType.Count - 1; i >= 0; i--) {
			for (int j = m_ClearType.ElementAt(i).Value.Count - 1; j >= 0; j--) {
				if (gameObject.activeInHierarchy)
					StartCoroutine(IE_SetClear(m_ClearType.ElementAt(i).Key, m_ClearType.ElementAt(i).Value[j]));
			}
			m_ClearType.Remove(m_ClearType.ElementAt(i).Key);
		}
	}
	IEnumerator IE_SetClear(StageClearType _type, int _pos) {
		List<GameObject> actives = new List<GameObject>();
		//특정 카드랑 인포에 애니메이션 전달하고 콜백으로 다 되면 리스트에서 삭제하고 트랜스리프레시 함
		Item_Guide_GoalCard card = m_GoalCards.Find((t) => t.m_Type == _type && t.m_Pos == _pos);
		if (card != null) {
			actives.Add(card.gameObject);
			card.SetAnim(Item_Guide_GoalCard.State.Complete, (obj) => { 
				actives.Remove(obj);
				if (m_GoalCards.Count > 1) {
					m_GoalCards.Remove(card);
					Destroy(card.gameObject);
				}
			});
		}
		Item_Guide_GoalInfo info = m_GoalInfos.Find((t) => t.m_Type == _type && t.m_Pos == _pos);
		if (info != null) info.SetAnim(Item_Guide_GoalInfo.State.Complete, (obj) => { 
			actives.Remove(obj);
			if (m_GoalInfos.Count > 1) {
				//destroy는 안하고 뭔가 다른 애니 발동할듯
				m_GoalInfos.Remove(info);
				Destroy(info.gameObject);
				m_GoalInfos[0].Refresh();
			}
		});
		m_SUI.Fitter.enabled = false;
		yield return new WaitUntil(() => actives.Count < 1);

		GuideTransRefresh();

		yield return new WaitForEndOfFrame();
		m_SUI.Fitter.enabled = true;
	}

	public void SetCardLoop(bool _loop) {
		if(gameObject.activeInHierarchy)
		StartCoroutine(CardLoopAction(_loop));
	}
	IEnumerator CardLoopAction(bool _loop) {
		for (int i = 0; i < m_GoalCards.Count; i++) {
			yield return new WaitWhile(() => !m_GoalCards[i].gameObject.activeSelf);
			m_GoalCards[i].SetAnim(_loop ? "Loop" : "Not");
			if (_loop) yield return new WaitForSeconds(0.5f);
		}
	}
}
