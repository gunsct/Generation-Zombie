using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_PDA_Collection_Main : Item_PDA_Base
{
	public enum Page
	{
		Archieve_Main,
		Collection_Main,
	}
	[Serializable]
	public struct STabUI
	{
		public GameObject Alram;
		public Image BG;
		public Image Icon;
		public Animator Anim;
	}
	[Serializable]
	public struct SBtnUI
	{
		public Button Btn;
		public Image BG;
		public TextMeshProUGUI Text;

		public Material[] BtnMats;
		public Color[] FontColors;
	}

	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Info;
		public Image Icon;

		public Color[] TabColors;
		public Sprite[] TabBg;
		public STabUI[] Tab;
		public STabUI[] TopTab;

		public RectTransform Prefab;
		public ScrollReck_ViewItemController ScrollController;

		public SBtnUI AllBtn;

		public Animator Ani;
		public GameObject LvUpPrefab;
	}
	[SerializeField] SUI m_SUI;
	List<TCollectionTable> m_TDatas = new List<TCollectionTable>();

	CollectionType m_Now;
	IEnumerator m_Action;
	bool Is_Change;
	int m_TopTab;
	private void OnDisable() {
		Is_Change = false;
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		
		base.SetData(CloaseCB, args);

		m_SUI.ScrollController.SetData(0, m_SUI.Prefab, SetScrollItem);

		SetTab((CollectionType)args[0], true);
		m_SUI.ScrollController.InitPosition();
		TabAlram();
	}

	public void SetTab(CollectionType Tab, bool _init = false) {
		m_SUI.Tab[(int)m_Now].Anim.SetTrigger("Off");

		m_Now = Tab;

		m_SUI.Title.text = TDATA.GetCollectionTypeName(Tab);
		m_SUI.Info.text = TDATA.GetCollectionTypeInfo(Tab);

		for (int i = 0; i < m_SUI.Tab.Length; i++)
		{
			m_SUI.Tab[i].BG.color = m_SUI.TabColors[0];
			m_SUI.Tab[i].BG.sprite = m_SUI.TabBg[0];
		}
		m_SUI.Tab[(int)Tab].Anim.SetTrigger(_init ? "OnStart" : "On");

		// 리스트 조회
		for (int i = 0; i < m_SUI.TopTab.Length; i++) {
			m_SUI.TopTab[i].Anim.SetTrigger(i == 0 ? (_init ? "OnStart" : "On") : "Off");
		}
		SetList(0);
	}
	public void SetTopTab(int _pos) {
		for(int i = 0; i < m_SUI.TopTab.Length; i++) {
			m_SUI.TopTab[i].Anim.SetTrigger(i == _pos ? "On" : "Off");
		}
		SetList(_pos);
	}

	void SetList(int _pos)
	{
		m_TopTab = _pos;
		//pos값으로 진행중, 완료
		//진행중은 현재만 1개식, 완료는 모든 완료된 단계 전부

		if (_pos == 0) {
			m_TDatas = USERINFO.m_Collection.GetList(m_Now);
			m_TDatas.RemoveAll(o => o.m_LV == USERINFO.m_Collection.GetMaxLV(o.m_Idx));
		}
		else {
			m_TDatas = USERINFO.m_Collection.GetCompCollection(m_Now);
		}
		m_TDatas.Sort(USERINFO.m_Collection.Sort);

		m_SUI.ScrollController.InitPosition();
		m_SUI.ScrollController.SetData(m_TDatas.Count, m_SUI.Prefab, SetScrollItem);

		SetScrollItem(ScrollReck_ViewItemController.RefreshMode.Normal);

		// 보상 받을수있는내역이 있는지 확인
		m_SUI.AllBtn.Btn.interactable = false;
		for (int i = 0; i < m_TDatas.Count; i++)
		{
			if(USERINFO.m_Collection.IsSuccess(m_TDatas[i])) m_SUI.AllBtn.Btn.interactable = _pos == 0;
		}

		if (m_SUI.AllBtn.Btn.interactable)
		{
			m_SUI.AllBtn.BG.material = m_SUI.AllBtn.BtnMats[1];
			m_SUI.AllBtn.Text.color = m_SUI.AllBtn.FontColors[1];
		}
		else
		{
			m_SUI.AllBtn.BG.material = m_SUI.AllBtn.BtnMats[0];
			m_SUI.AllBtn.Text.color = m_SUI.AllBtn.FontColors[0];
		}
	}
	void SetScrollItem(ScrollReck_ViewItemController.RefreshMode mode) {
		var listcnt = m_SUI.ScrollController.GetViewCnt();
		int offset = m_SUI.ScrollController.GetViewLine() * m_SUI.ScrollController.GetOneLineItemCnt();
		for (int i = 0; i < listcnt; i++, offset++) {
			Item_PDA_Collection_Element item = m_SUI.ScrollController.GetItem<Item_PDA_Collection_Element>(i);
			if (item == null) break;
			if (offset > -1 && offset < m_TDatas.Count) {
				item.SetData(m_TDatas[offset], m_TopTab == 1, OnItemRewardClick);
				item.gameObject.SetActive(true);
			}
			else item.gameObject.SetActive(false);
		}
	}

	void TabAlram()
	{
		Dictionary<CollectionType, bool> ActiveAlram = new Dictionary<CollectionType, bool>();
		var list = USERINFO.m_Collection.GetSucList().Select(o => o.m_Type).Distinct().ToList();
		for (int i = 0; i < m_SUI.Tab.Length; i++)
		{
			m_SUI.Tab[i].Alram.SetActive(list.Any(o => o == (CollectionType)i));
		}
	}


	public void SelectTab(int Pos)
	{
		CollectionType Tab = (CollectionType)Pos;
		if (m_Now == Tab) return;
		//if (Is_Change) return;
		Is_Change = true;
		PlayEffSound(SND_IDX.SFX_0121);
		SetTab(Tab);
		m_SUI.ScrollController.InitPosition();
		StartCoroutine(ChangeAction());
	}
	IEnumerator ChangeAction() {
		m_SUI.Ani.SetTrigger("Change");
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Ani));
		Is_Change = false;
	}

	public void ClickExit()
	{
		PlayEffSound(SND_IDX.SFX_0121);
		Is_Change = false;
		OnClose();
	}
	public override void OnClose()
	{
		if (m_Action != null) StopCoroutine(m_Action);
		m_CloaseCB?.Invoke(Item_PDA_Achieve.State.Menu, null);
	}

	/// <summary> 선택한 업적 보상 받기 </summary>
	void OnItemRewardClick(Item_PDA_Collection_Element item, int value)
	{
		SendItemGet(new List<TCollectionTable>() { item.m_Info });
	}

	/// <summary> 전체 받기 </summary>
	public void OnAllRewardClick()
	{
		SendItemGet(USERINFO.m_Collection.GetSucList(m_Now));
	}

	void SendItemGet(List<TCollectionTable> list)
	{
		if (list.Count < 1) return;
#if NOT_USE_NET
		// 해당 리스트 완료 셋팅
		for (int i = list.Count - 1; i > -1; i--)
		{
			USERINFO.m_Collection.SetEnd(list[i]);
		}
		USERINFO.m_Collection.ResetCheckCollection();

		StartCoroutine(RewardAction(list));
#else
		WEB.SEND_REQ_COLLECTION_LVUP((res) =>
		{
			if (m_Action != null) StopCoroutine(m_Action);
			m_Action = RewardAction(list);
			StartCoroutine(m_Action);
		}, list.Select(o => o.m_Idx).ToList());
#endif
	}

	IEnumerator RewardAction(List<TCollectionTable> list)
	{
		SetTab(m_Now);
		TabAlram();
		for (int i = 0; i < list.Count; i++) {
			PlayEffSound(SND_IDX.SFX_0310);
			Item_PDA_Collection_LvUp lvup = Utile_Class.Instantiate(m_SUI.LvUpPrefab, transform).GetComponent<Item_PDA_Collection_LvUp>();
			TCollectionTable table = TDATA.GetCollectionTable(list[i].m_Idx, list[i].m_LV + 1);
			lvup.SetData(table, i + 1);
			yield return new WaitForSeconds(0.25f);
		}
		m_Action = null;
	}

	public void ViewStats()
	{
		m_CloaseCB?.Invoke(Item_PDA_Achieve.State.Collection_BuffList, new object[] { m_Now });
	}
}
