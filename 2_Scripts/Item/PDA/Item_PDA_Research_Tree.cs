using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Research_Tree : Item_PDA_Base
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Folder;
		public TextMeshProUGUI Name;
		public ScrollRect Scroll;
		public RectTransform List;
		public RectTransform Prefab;

		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;
	ResearchType m_Tree = ResearchType.End;
#pragma warning restore 0649

	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);
		if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.Select_ResearchTree)) TUTO.Next(this);
		SetTree((ResearchType)args[0]);
		StartCoroutine(StartAction());
	}
	IEnumerator StartAction()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.ViewResearchTree)) TUTO.Next(this);
	}

	public void SetTree(ResearchType tree)
	{
		if (m_Tree == tree)
		{
			SetListData();
			return;
		}
		m_Tree = tree;
		switch (m_Tree)
		{
		case ResearchType.Research:
			m_SUI.Icon.sprite = UTILE.LoadImg("UI/UI_Exploration/Icon_Research_3", "png");
			m_SUI.Name.text = TDATA.GetString(205);
			m_SUI.Folder.text = "C:/Directory/Document/Research/";
			break;
		case ResearchType.Training:
			m_SUI.Icon.sprite = UTILE.LoadImg("UI/UI_Exploration/Icon_Research_6", "png");
			m_SUI.Name.text = TDATA.GetString(207);
			m_SUI.Folder.text = "C:/Directory/Document/Training/";
			break;
		case ResearchType.Remodeling:
			m_SUI.Icon.sprite = UTILE.LoadImg("UI/UI_Exploration/Icon_Research_7", "png");
			m_SUI.Name.text = TDATA.GetString(208);
			m_SUI.Folder.text = "C:/Directory/Document/Remodeling/";
			break;
		}
		// 라인초기화

		// 전체 리스트 알아내기
		// 0번째 라인은 제목줄
		UTILE.Load_Prefab_List(TDATA.GetResearchTable_MaxLine(m_Tree), m_SUI.List, m_SUI.Prefab);

		
		int linepos = SetListData();
		// 사이즈가 정상적으로 셋팅되지않아 1프레임 쉬어줌
		//StartCoroutine(MovePos(linepos));
		m_SUI.Scroll.verticalNormalizedPosition = 1f;
	}

	public int SetListData()
	{
		List<int> idxs = TDATA.GetResearchTable_GroupIdxs(m_Tree);
		for (int i = m_SUI.List.childCount - 1; i > -1; i--) m_SUI.List.GetChild(i).GetComponent<Item_PDA_Research_Tree_Line>().Init((pos) => m_SUI.List.GetChild(pos).GetComponent<Item_PDA_Research_Tree_Line>());
		int linepos = 0;
		int? reserching = null;
		// 첫째 라인은 제목줄
		for (int i = 0; i < idxs.Count; i++)
		{
			ResearchInfo info = USERINFO.GetResearchInfo(m_Tree, idxs[i]);
			int line = info.m_TData.m_Pos.m_Line;
			Item_PDA_Research_Tree_Line item = m_SUI.List.GetChild(line).GetComponent<Item_PDA_Research_Tree_Line>();
			item.SetData(info, OnClickItem);

			if (info.m_State != TimeContentState.Idle) reserching = line;
			else if (info.Is_Open()) linepos = line;
		}

		return reserching != null ? reserching.Value : linepos;
	}

	IEnumerator MovePos(int Line)
	{
		m_SUI.Scroll.velocity = Vector2.zero;
		if (Line < 1)
		{
			m_SUI.Scroll.verticalNormalizedPosition = 1f;
			yield break;
		}
		yield return new WaitForEndOfFrame();
		//// 사이즈 및 위치 갱신 해준다. 적용이 잘안되어서 스킵
		//VerticalLayoutGroup congroup = m_SUI.List.GetComponent<VerticalLayoutGroup>();
		//congroup.SetLayoutVertical();
		//congroup = m_SUI.Scroll.content.GetComponent<VerticalLayoutGroup>();
		//congroup.SetLayoutVertical();
		// (pivot계산을 안하기위해 pivot을 0으로 셋팅)
		// y값은 음수로 셋팅됨, list panel 만큼 땡겨줌
		float y = ((RectTransform)m_SUI.List.GetChild(Line).transform).anchoredPosition.y + m_SUI.List.anchoredPosition.y;
		// y값이 음수기때문에 1에서 빼줘야 자신의 위치
		float pos = 1f + y / (m_SUI.Scroll.content.rect.height - ((RectTransform)m_SUI.Scroll.transform).rect.height);

		// 마지막 연구쪽 라인을 찾아서 넣어주어야함
		m_SUI.Scroll.verticalNormalizedPosition = pos;//pos < 0 ? 1f :
	}

	void OnClickItem(Item_PDA_Research_Element item, int state) {

		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_Research_Element, item)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		ResearchInfo info = item.m_Info;
		if (info.m_State == TimeContentState.Play) {
			StartComplePopup(item);
			return;
		}
		// 상세보기창으로 연결
		m_CloaseCB?.Invoke(Item_PDA_Research.State.Detail, new object[] { m_Tree, item.m_Info });
	}

	void StartComplePopup(Item_PDA_Research_Element item)
	{
		ResearchInfo info = item.m_Info;
		if (info.m_State != TimeContentState.Play) return;
		if (!info.IS_Complete())
		{
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(215), (result, obj) =>
			{
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						OnComplate(item);
					}
					else {
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
				}
			}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Research, item.m_Info.GetRemainTime()), false);
		}
		else OnComplate(item);
	}

	void OnComplate(Item_PDA_Research_Element item)
	{
		ResearchInfo info = item.m_Info;
		info.OnComplete((res) =>
		{
			if (!res.IsSuccess()) return;
			m_CloaseCB?.Invoke(Item_PDA_Research.State.Result, new object[] { m_Tree, item.m_Info });
		});
	}
	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 2)) return;
		m_CloaseCB?.Invoke(Item_PDA_Research.State.Menu, null);
		m_Tree = ResearchType.End;
	}

	public GameObject GetScroll()
	{
		return m_SUI.Scroll.gameObject;
	}
}
