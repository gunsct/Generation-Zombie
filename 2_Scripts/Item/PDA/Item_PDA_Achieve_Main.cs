using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_PDA_Achieve_Main : Item_PDA_Base
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

		public ScrollRect Scroll;
		public RectTransform Prefab;

		public SBtnUI AllBtn;

		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;
	TAchievementTable.Tab m_Now;
	IEnumerator m_Action;
	bool Is_Change;

	private void OnDisable() {
		Is_Change = false;
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		SetTab(TAchievementTable.Tab.Stage, true);
		InitScrollPosition();
		TabAlram();
	}

	void InitScrollPosition()
	{
		m_SUI.Scroll.verticalNormalizedPosition = 1;
		m_SUI.Scroll.velocity = Vector2.zero;
		m_SUI.Scroll.StopMovement();
	}

	public void SetTab(TAchievementTable.Tab Tab, bool _init = false) {
		m_SUI.Tab[(int)m_Now].Anim.SetTrigger("Off");

		m_Now = Tab;
		m_SUI.Title.text = TDATA.GetAchieveTabName(Tab);
		m_SUI.Info.text = TDATA.GetAchieveTabInfo(Tab);

		for (int i = 0; i < m_SUI.Tab.Length; i++)
		{
			m_SUI.Tab[i].BG.color = m_SUI.TabColors[0];
			m_SUI.Tab[i].BG.sprite = m_SUI.TabBg[0];
		}

		m_SUI.Tab[(int)Tab].Anim.SetTrigger(_init ? "On" : "OnStart");
		//m_SUI.Tab[(int)Tab].BG.color = m_SUI.TabColors[1];
		//m_SUI.Tab[(int)Tab].BG.sprite = m_SUI.TabBg[1];
		//m_SUI.Icon.sprite = m_SUI.Tab[(int)Tab].Icon.sprite;

		// 리스트 조회
		SetList();
	}

	void SetList()
	{
		List<TAchievementTable> list = USERINFO.m_Achieve.GetAchieveList(m_Now);
		RectTransform content = m_SUI.Scroll.content;
		UTILE.Load_Prefab_List(list.Count, content, m_SUI.Prefab);

		list.Sort(USERINFO.m_Achieve.Sort);

		// 보상 받을수있는내역이 있는지 확인
		m_SUI.AllBtn.Btn.interactable = false;
		for (int i = 0, iMax = list.Count; i < iMax; i++)
		{
			if (content.GetChild(i).GetComponent<Item_PDA_Achieve_Element>().SetData(list[i], OnItemRewardClick)) m_SUI.AllBtn.Btn.interactable = true;
		}

		if(m_SUI.AllBtn.Btn.interactable)
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

	void TabAlram()
	{
		Dictionary< TAchievementTable.Tab, bool> ActiveAlram = new Dictionary<TAchievementTable.Tab, bool>();
		var list = USERINFO.m_Achieve.GetSucAchieveList().Select(o => o.m_Tab).Distinct().ToList();
		for(int i = 0; i < m_SUI.Tab.Length; i++)
		{
			m_SUI.Tab[i].Alram.SetActive(list.Any(o => o == (TAchievementTable.Tab)i));
		}
	}


	public void SelectTab(int Pos)
	{
		TAchievementTable.Tab Tab = (TAchievementTable.Tab)Pos;
		if (m_Now == Tab) return;
		if (Is_Change) return;
		Is_Change = true;
		PlayEffSound(SND_IDX.SFX_0121);
		SetTab(Tab);
		InitScrollPosition();
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
		if (m_Action != null) return;
		m_CloaseCB?.Invoke(Item_PDA_Achieve.State.Menu, null);
	}

	/// <summary> 선택한 업적 보상 받기 </summary>
	void OnItemRewardClick(Item_PDA_Achieve_Element item, int value)
	{
		SendItemGet(new List<TAchievementTable>() { item.m_Info });
	}

	/// <summary> 전체 받기 </summary>
	public void OnAllRewardClick()
	{
		SendItemGet(USERINFO.m_Achieve.GetSucAchieveList(m_Now));
	}

	void SendItemGet(List<TAchievementTable> list)
	{
		if (list.Count < 1) return;
#if !NOT_USE_NET
		WEB.SEND_REQ_ACHIEVE_REWARD((res) => {
			if (res.IsSuccess()) {
				SetTab(m_Now);
				if (res.Rewards == null) return;
				m_Action = RewardAction(res.GetRewards());
				StartCoroutine(m_Action);
			}
		}, list.Select(o => new REQ_ACHIEVE_REWARD() { Idx = o.m_Idx, LV = o.m_LV }).ToList());
#endif
	}

	IEnumerator RewardAction(List<RES_REWARD_BASE> Rewards)
	{
		yield return MAIN.IE_RewardList(new object[] { Rewards }, null);
		SetTab(m_Now);
		TabAlram();
		m_Action = null;
	}
}
