using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Research_Detail : Item_PDA_Base
{
	public enum TabName
	{
		Preced = 0,
		Material,
		End
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SReserchInfoUI
	{
		public Image Icon;
		public GameObject Lock;
		public GameObject LV_Panel;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Info;
	}
	[System.Serializable]
	struct SUPLVUI
	{
		public GameObject Lock;
		public TextMeshProUGUI Now;
		public TextMeshProUGUI Next;
	}

	[System.Serializable]
	struct SBtnUI
	{
		public Button Btn;
		public TextMeshProUGUI Label;
	}

	[System.Serializable]
	struct STabUI
	{
		public Color[] BgCol;
		public Color[] LabelCol;
		public Sprite[] CheckIcon;

		public GameObject Active;
		public SBtnUI[] Btns;

		public SPrecedUI Preced;
		public SMaterialUI Material;

		public SStgLockUI StgLock;
	}

	[System.Serializable]
	struct SPrecedUI
	{
		public GameObject Active;
		public SPrecedInfoUI[] Infos;
	}
	[Serializable]
	struct SStgLockUI
	{
		public GameObject Active;
		public TextMeshProUGUI Desc;
	}
	[System.Serializable]
	struct SPrecedInfoUI
	{
		public GameObject Active;
		public Image Check;
		public TextMeshProUGUI Info;

		public Button Btn;
		public TextMeshProUGUI BtnLabel;
	}

	[System.Serializable]
	struct SMaterialUI
	{
		public GameObject Active;
		public GameObject Lock;
		public SMaterialInfoUI[] Infos;
	}

	[System.Serializable]
	struct SMaterialInfoUI
	{
		public GameObject Active;
		public Image Check;
		public Item_RewardItem_Card Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
	}

	[System.Serializable]
	struct SUI
	{
		public Animator Ani;

		public TextMeshProUGUI Time;

		public SUPLVUI UPInfo;

		public SReserchInfoUI Info;

		public GameObject MaxLVInfo;
		public GameObject TimeInfo;
		public GameObject StartBtn;
		public STabUI Tab;

		public Button ResearchBtn;
	}
#pragma warning restore 0649

	[SerializeField] SUI m_SUI;
	ResearchType m_Tree;
	ResearchInfo m_Info;
	bool m_IsFirstOpen, m_IsLock, m_IsUpgrade;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);
		m_Tree = (ResearchType)args[0];
		m_Info = (ResearchInfo)args[1];
		SetUI();
	}

	public void SetUI()
	{
		TResearchTable tdata = m_Info.m_TData;
		int MaxLV = m_Info.m_MaxLV;
		bool IsMaxLV = m_Info.m_GetLv == MaxLV;
		m_IsFirstOpen = m_Info.Is_Open();
		// 연구 정보
		m_SUI.Info.Icon.sprite = tdata.GetIcon();
		m_SUI.Info.Lock.SetActive(!m_IsFirstOpen);
		m_SUI.Info.LV_Panel.SetActive(m_IsFirstOpen);
		m_SUI.Info.LV.text = IsMaxLV ? "MAX" : string.Format("Lv. {0} / {1}", m_Info.m_GetLv, MaxLV);
		m_SUI.Info.Name.text = tdata.GetName();
		m_SUI.Info.Info.text = tdata.GetInfo(2, m_Info.m_GetLv != 0);

		m_SUI.Time.text = string.Format(TDATA.GetString(226), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, tdata.GetTime() * 0.001d));

		if(IsMaxLV)
		{
			m_SUI.MaxLVInfo.SetActive(true);
			m_SUI.TimeInfo.SetActive(false);
			m_SUI.StartBtn.SetActive(false);
			m_SUI.Tab.Active.SetActive(false);
			m_SUI.ResearchBtn.interactable = false;
			m_IsLock = true;
			m_IsUpgrade = false;
		}
		else
		{
			m_SUI.MaxLVInfo.SetActive(false);
			m_SUI.TimeInfo.SetActive(true);
			m_SUI.StartBtn.SetActive(true);
			m_SUI.Tab.Active.SetActive(true);

			// 레벨 표시
			m_SUI.UPInfo.Lock.SetActive(!m_IsFirstOpen);
			m_SUI.UPInfo.Now.transform.parent.gameObject.SetActive(m_IsFirstOpen);
			m_SUI.UPInfo.Now.text = string.Format("Lv. {0}", m_Info.m_GetLv);

			m_SUI.UPInfo.Next.text = m_Info.m_GetLv + 1 == MaxLV ? "MAX" : string.Format("Lv. {0}", m_Info.m_GetLv + 1);

			m_IsLock = false;

			// 오픈 상태인지 확인
			m_SUI.Tab.StgLock.Active.SetActive(!m_Info.Is_StgUnLock());
			m_SUI.Tab.StgLock.Desc.text = string.Format("{0}-{1} {2}", m_Info.m_TData.m_UnLockVal / 100, m_Info.m_TData.m_UnLockVal % 100, TDATA.GetString(594));

			for (int i = 0; i < 3; i++)
			{
				TResearchTable.Preced preced = tdata.m_Preced[i];
				if (preced.m_Idx == 0)
				{
					m_SUI.Tab.Preced.Infos[i].Active.SetActive(false);
					continue;
				}

				m_SUI.Tab.Preced.Infos[i].Active.SetActive(true);
				ResearchInfo Info = USERINFO.GetResearchInfo(m_Tree, preced.m_Idx);
				m_SUI.Tab.Preced.Infos[i].Info.text = string.Format(TDATA.GetString(209), Info.m_TData.GetName(), preced.m_LV);
				if (Info.m_GetLv < preced.m_LV)
				{
					m_SUI.Tab.Preced.Infos[i].Btn.gameObject.SetActive(true);
					if (Info.m_State == TimeContentState.Idle)
					{
						m_SUI.Tab.Preced.Infos[i].Btn.interactable = true;
						m_SUI.Tab.Preced.Infos[i].BtnLabel.text = TDATA.GetString(241);
					}
					else
					{
						m_SUI.Tab.Preced.Infos[i].Btn.interactable = false;
						m_SUI.Tab.Preced.Infos[i].BtnLabel.text = TDATA.GetString(258);
					}
					m_SUI.Tab.Preced.Infos[i].Check.sprite = m_SUI.Tab.CheckIcon[0];
					m_SUI.Tab.Preced.Infos[i].Info.color = Utile_Class.GetCodeColor("#D95F59");
					m_IsLock = true;
				}
				else
				{
					m_SUI.Tab.Preced.Infos[i].Btn.gameObject.SetActive(false);
					m_SUI.Tab.Preced.Infos[i].Check.sprite = m_SUI.Tab.CheckIcon[1];
					m_SUI.Tab.Preced.Infos[i].Info.color = Utile_Class.GetCodeColor("#A7B2A0");
				}
			}

			// 재료 개수 체크
			m_IsUpgrade = true;
			for (int i = 0; i < 3; i++)
			{
				if (i >= tdata.m_Mat.Count)
				{
					m_SUI.Tab.Material.Infos[i].Active.SetActive(false);
					continue;
				}

				m_SUI.Tab.Material.Infos[i].Active.SetActive(true);
				TResearchTable.Material mat = tdata.m_Mat[i];
				int Cnt = USERINFO.GetItemCount(mat.m_Idx);
				TItemTable titem = TDATA.GetItemTable(mat.m_Idx);
				m_SUI.Tab.Material.Infos[i].Cnt.text = string.Format("{0} / {1}", Cnt, mat.m_Count);
				m_SUI.Tab.Material.Infos[i].Name.text = titem.GetName();
				m_SUI.Tab.Material.Infos[i].Icon.SetData(mat.m_Idx);
				if (Cnt < mat.m_Count)
				{
					m_SUI.Tab.Material.Infos[i].Check.sprite = m_SUI.Tab.CheckIcon[0];
					m_SUI.Tab.Material.Infos[i].Cnt.color = Utile_Class.GetCodeColor("#D95F59");
					m_IsUpgrade = false;
				}
				else
				{
					m_SUI.Tab.Material.Infos[i].Check.sprite = m_SUI.Tab.CheckIcon[1];
					m_SUI.Tab.Material.Infos[i].Cnt.color = Utile_Class.GetCodeColor("#A7B2A0");
				}
			}
			//스테이지 락
			if (!m_Info.Is_StgUnLock()) {
				m_IsLock = true;
			}

			m_SUI.Tab.Material.Lock.SetActive(m_IsLock);
			m_SUI.ResearchBtn.interactable = !m_IsLock;
			SetTab((int)(m_IsLock ? TabName.Preced : TabName.Material));

		}

	}

	public void SetTab(int tabpos)
	{
		TabName tab = (TabName)tabpos;
		for(TabName i = TabName.Preced; i < TabName.End; i++)
		{
			bool IsSelectTab = tab == i;
			int pos = IsSelectTab ? 1 : 0;
			m_SUI.Tab.Btns[(int)i].Btn.image.color = m_SUI.Tab.BgCol[pos];
			m_SUI.Tab.Btns[(int)i].Btn.interactable = !IsSelectTab;
			m_SUI.Tab.Btns[(int)i].Label.color = m_SUI.Tab.LabelCol[pos];
		}

		m_SUI.Tab.Preced.Active.SetActive(tab == TabName.Preced);
		m_SUI.Tab.Material.Active.SetActive(tab == TabName.Material);
	}

	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	public override void OnClose()
	{
		m_CloaseCB?.Invoke(Item_PDA_Research.State.Tree, new object[] { m_Tree });
	}

	public void OnResearch()
	{
		List<ResearchType> types = new List<ResearchType>() { ResearchType.Research, ResearchType.Training, ResearchType.Remodeling };
		ResearchInfo playresearch = null;
		for (int i = 0; i < types.Count; i++) {
			if (playresearch == null) playresearch = USERINFO.IsResearching(types[i]);
			else break;
		}
		if (playresearch != null) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, string.Format(TDATA.GetString(846), playresearch.m_TData.GetName()), (result, obj) =>
			{
				if (result == 1)
				{
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						playresearch.OnComplete((res) => {
							if (res.IsSuccess()) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(847), playresearch.m_TData.GetName()));
						});
					}
					else {
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
				}
			}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Research, playresearch.GetRemainTime()), false);
			//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(255));
			return;
		}
		if (!m_IsUpgrade) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, Utile_Class.StringFormat(TDATA.GetString(202), TDATA.GetString(114)));
			return;
		}
		PlayEffSound(SND_IDX.SFX_0122);
#if NOT_USE_NET
		TResearchTable tdata = m_Info.m_TData;
		for (int i = tdata.m_Mat.Count - 1; i > -1; i--)
		{
			TResearchTable.Material mat = tdata.m_Mat[i];
			USERINFO.DeleteItem(mat.m_Idx, mat.m_Count);
		}

		m_Info.m_Times[0] = (long)UTILE.Get_ServerTime_Milli();
		m_Info.m_Times[1] = m_Info.m_Times[0] + tdata.GetTime();
		m_Info.m_State = TimeContentState.Play;
		MAIN.Save_UserInfo();
		OnClose();
#else
		WEB.SEND_REQ_RESEARCH_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			OnClose();
		}, m_Info);
#endif
	}

	public void OnChnageReserch(int Pos) {
		PlayEffSound(SND_IDX.SFX_0121);
		TResearchTable tdata = m_Info.m_TData;
		TResearchTable.Preced preced = tdata.m_Preced[Pos];
		SetData(m_CloaseCB, new object[] { m_Tree, USERINFO.GetResearchInfo(m_Tree, preced.m_Idx) });
		m_SUI.Ani.SetTrigger("SideMove");
	}
	public void Click_GoStage() {
		PlayEffSound(SND_IDX.SFX_0121);
		POPUP.GetMainUI().GetComponent<Main_Play>().MenuChange((int)MainMenuType.Stage, false);
	}
	/// <summary> 재료 수급처 팝업 </summary>
	public void ClickMatGuide(int _pos) {
		TResearchTable.Material mats = m_Info.m_TData.m_Mat[_pos];
		POPUP.ViewItemInfo((result, obj) => { SetUI(); }, new object[] { new ItemInfo(mats.m_Idx), PopupName.NONE, null, mats.m_Count });
	}
}
