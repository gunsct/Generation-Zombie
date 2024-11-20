using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Union_Research_List : PopupBase
{
	[Serializable]
	public struct SStepUI
	{
		public SimpleScrollSnap Scroll;
		public RectTransform Prefab;
		public Action<int> AniChange;

		[ReName("활성 단계", "비활성 단계")]
		public SLockUI[] SetpName;
	}

	[Serializable]
	public struct SLockUI
	{
		public GameObject Active;
		public TextMeshProUGUI Label;
	}

	[Serializable]
	public struct SResUI
	{
		public ScrollRect Scroll;
		public RectTransform Prefab;

		public SLockUI Lock;
		[HideInInspector] public List<Item_Union_Research_Element> Items;
	}

	[Serializable]
    public struct SUI
	{
		public Animator Anim;

		public TextMeshProUGUI GuildStep;

		public SStepUI Step;
		public SResUI Res;
	}

	[SerializeField] SUI m_SUI;

	bool IsStart;
	int MyGuildStep;
	int Select;
	int LV;
	long Exp;
	List<TGuild_ResearchTable> TDataList = new List<TGuild_ResearchTable>();

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		MyGuildStep = USERINFO.m_Guild.ResStep;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		USERINFO.m_Guild.Calc_Exp(out LV, out Exp);
		LoadStepList();


		m_SUI.GuildStep.text = string.Format(TDATA.GetString(6077), MyGuildStep);
		m_SUI.Step.Scroll.GoToPanel(MyGuildStep - 1);
	}

	void LoadStepList()
	{
		int Max = TDATA.GetGuild_ResMaxStep();

		int i;
		int nChildCnt = m_SUI.Step.Scroll.NumberOfPanels;
		int nGap = Max - nChildCnt;
		if (nGap < 0)
		{
			// 차이 개수 만큼 차감
			// 처음 위치부터 바로 셋팅해야하는데 바로 삭제가 되지않아 셋팅이 잘못되는경우가 있어 뒤에서부터 삭제한다.
			for (i = nChildCnt - 1; i >= nChildCnt + nGap; i--) m_SUI.Step.Scroll.RemoveFromBack();
		}
		else
		{
			// 차이 개수만큼 생성
			for (i = 0; i < nGap; i++) m_SUI.Step.Scroll.Add(m_SUI.Step.Prefab.gameObject, i);
		}
		m_SUI.Step.AniChange = null;
		for (i = 0; i < Max; i++)
		{
			var item = m_SUI.Step.Scroll.Content.GetChild(i).GetComponent<Item_Union_Research_Step>();
			item.SetData(i + 1);
			m_SUI.Step.AniChange += item.StartAni;
		}
	}

	public void SelectStep(int Step)
	{
		Select = Step;

		m_SUI.Res.Lock.Label.text = string.Format(TDATA.GetString(6176), Step - 1);
		m_SUI.Res.Lock.Active.SetActive(MyGuildStep < Step);

		if(MyGuildStep < Step)
		{
			m_SUI.Step.SetpName[0].Active.SetActive(false);
			m_SUI.Step.SetpName[1].Active.SetActive(true);
			m_SUI.Step.SetpName[1].Label.text = string.Format(TDATA.GetString(6077), Step);
		}
		else
		{
			m_SUI.Step.SetpName[0].Active.SetActive(true);
			m_SUI.Step.SetpName[1].Active.SetActive(false);
			m_SUI.Step.SetpName[0].Label.text = string.Format(TDATA.GetString(6077), Step);
		}

		LoadStepResList();
	}

	int GetResSortValue(TGuild_ResearchTable tdata)
	{
		if (tdata.m_Idx == USERINFO.m_Guild.ResIdx) return 0;
		if (USERINFO.m_Guild.EndRes.Contains(tdata.m_Idx)) return 3;

		if (LV < tdata.m_Unlock.m_LV) return 2;
		if (tdata.m_Unlock.m_ResIdx > 0 && !USERINFO.m_Guild.EndRes.Contains(tdata.m_Unlock.m_ResIdx)) return 2;
		return 1;
	}

	public void LoadStepResList()
	{
		int LV;
		long Exp;
		USERINFO.m_Guild.Calc_Exp(out LV, out Exp);
		int step = USERINFO.m_Guild.ResStep;

		TDataList.Clear();
		TDataList.AddRange(TDATA.GetGuildResGroupList(Select));
		TDataList.Sort((befor, after) =>
		{
			int bsv = GetResSortValue(befor);
			int asv = GetResSortValue(after);
			if (bsv != asv) return bsv.CompareTo(asv);
			return befor.m_Idx.CompareTo(after.m_Idx);
		});

		int Max = TDataList.Count;

		int i;
		int nChildCnt = m_SUI.Res.Items.Count;
		int nGap = Max - nChildCnt;
		if (nGap < 0)
		{
			// 차이 개수 만큼 차감
			// 처음 위치부터 바로 셋팅해야하는데 바로 삭제가 되지않아 셋팅이 잘못되는경우가 있어 뒤에서부터 삭제한다.
			for (i = nChildCnt - 1; i >= nChildCnt + nGap; i--)
			{
				var item = m_SUI.Res.Items[i];
				m_SUI.Res.Items.Remove(item);
				Destroy(item.gameObject, 0f);
			}
		}
		else
		{
			// 차이 개수만큼 생성
			for (i = 0; i < nGap; i++)
			{
				var item = Instantiate(m_SUI.Res.Prefab.gameObject, m_SUI.Res.Scroll.content);
				m_SUI.Res.Items.Add(item.GetComponent<Item_Union_Research_Element>());
			}
		}

		for (i = 0; i < Max; i++)
		{
			Item_Union_Research_Element element = m_SUI.Res.Items[i];
			element.SetData(TDataList[i].m_Idx, LV, step, OnClick);
		}
	}

	bool m_IsChanging;
	public void OnStepChanging()
	{
		var item = m_SUI.Step.Scroll.Content.GetChild(m_SUI.Step.Scroll.CurrentPanel).GetComponent<Item_Union_Research_Step>();
		m_SUI.Step.AniChange?.Invoke(item.m_Num);
	}

	public void OnStepChange()
	{
		var item = m_SUI.Step.Scroll.Content.GetChild(m_SUI.Step.Scroll.CurrentPanel).GetComponent<Item_Union_Research_Step>();
		m_SUI.Step.AniChange?.Invoke(item.m_Num);
		SelectStep(item.m_Num);
	}

	void ReLoadGuild()
	{
		USERINFO.m_Guild.LoadGuild(() =>
		{
			if (USERINFO.m_Guild.UID == 0)
			{
				USERINFO.m_Guild.Set_AlramOff();
				Close((int)Union_JoinList.CloseResult.LoadGuild);
			}
			else SetUI();
		}, 0, true, true, true);
	}

	#region Btn
	void OnClick(Item_Union_Research_Element.Mode Mode, int Idx)
	{
		switch(Mode)
		{
		case Item_Union_Research_Element.Mode.Start: ResStart(Idx); break;
		case Item_Union_Research_Element.Mode.Stop: ResStop(Idx); break;
		}
	}

	void ResStart(int Idx)
	{
		var tdata = TDATA.GetGuildRes(Idx);
		string Msg = "";
		if(USERINFO.m_Guild.ResIdx != 0) Msg = string.Format(TDATA.GetString(6186), tdata.GetName());
		else Msg = string.Format(TDATA.GetString(6112), tdata.GetName(), tdata.ValueToString());

		POPUP.Set_MsgBox(PopupName.Msg_YN_BtnControl, TDATA.GetString(6109), Msg, (result, obj) => {
			if (result == 1) Send_Start(Idx);
		}
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(288), BG = UIMng.BtnBG.Brown }
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = TDATA.GetString(6113), BG = UIMng.BtnBG.Green });
	}

	void Send_Start(int Idx)
	{
		WEB.SEND_REQ_GUILD_RES_START((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_NOT_FOUND_GUILD:
				case EResultCode.ERROR_GUILD_NOT_MEMBER:    // 강퇴당함(마스터 이전후 강퇴됨)
					Close((int)Union_JoinList.CloseResult.LoadGuild);
					break;
				case EResultCode.ERROR_GUILD_GRADE:         // 마스터 아님
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
					ReLoadGuild();
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					ReLoadGuild();
					break;
				}
				return;
			}
			PlayEffSound(SND_IDX.SFX_1520);
			SetUI();
		}, Idx);
	}

	void ResStop(int Idx)
	{
		POPUP.Set_MsgBox(PopupName.Msg_YN_BtnControl, TDATA.GetString(6156), TDATA.GetString(6157), (result, obj) => {
			if (result == 1) Send_Stop(Idx);
		}
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(288), BG = UIMng.BtnBG.Brown }
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = TDATA.GetString(6156), BG = UIMng.BtnBG.Green });

	}


	void Send_Stop(int Idx)
	{
		WEB.SEND_REQ_GUILD_RES_STOP((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_NOT_FOUND_USER:
				case EResultCode.ERROR_NOT_FOUND_GUILD:
				case EResultCode.ERROR_GUILD_NOT_MEMBER:    // 강퇴당함(마스터 이전후 강퇴됨)
					Close((int)Union_JoinList.CloseResult.LoadGuild);
					break;
				case EResultCode.ERROR_GUILD_GRADE:         // 마스터 아님
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
					ReLoadGuild();
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					ReLoadGuild();
					break;
				}
				return;
			}
			PlayEffSound(SND_IDX.SFX_0471);
			SetUI();
		}, Idx);
	}

	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	} 
#endregion
}
