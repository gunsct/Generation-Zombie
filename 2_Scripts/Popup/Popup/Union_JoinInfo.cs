using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public class Union_JoinInfo : PopupBase
{
	public enum ListState
	{
		/// <summary> 추천 리스트 </summary>
		Recommend = 0,
		/// <summary> 길드 찾기 </summary>
		Find
	}

	[Serializable]
	public struct SInfoUI
	{
		public Image Mark;
		public Image Nation;
		public Text Name;
		public Text Master;
		public Text Intro;
		public GameObject IntroEmpty;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Cnt;
	}

	[Serializable]
	public struct SJoinInfoUI
	{
		public TextMeshProUGUI JoinLV;
		public TextMeshProUGUI JoinType;

		public Item_Button JoinBtn;
		public TextMeshProUGUI JoinBtnLabel;
	}
	[Serializable]
	public struct SRES_ListUI
	{
		public GameObject None;
		public ScrollRect Scroll;
		public RectTransform Prefab;
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;

		public SInfoUI Info;
		public SJoinInfoUI Join;
		public SRES_ListUI Res;
	}
	[SerializeField] SUI m_SUI;
	bool IsStart;
	ListState m_ListState;
	RES_GUILDINFO Info;
	public RES_GUILDINFO_SIMPLE MyJoin;

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		Info = (RES_GUILDINFO)aobjValue[0];
		MyJoin = (RES_GUILDINFO_SIMPLE)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);
		StartCoroutine(Timecheck());
	}

	public override void SetUI() {
		base.SetUI();

		// 정보 UI

		m_SUI.Info.Mark.sprite = Info.GetGuilMark();
		m_SUI.Info.Nation.sprite = BaseValue.GetNationIcon(Info.Nation);
		int LV = 1;
		long Exp = 0;
		Info.Calc_Exp(out LV, out Exp);
		m_SUI.Info.LV.text = string.Format("Lv.{0}", LV.ToString());
		m_SUI.Info.Name.text = Info.Name;
		var master = Info.Users.Find(o => o.Grade == GuildGrade.Master);
		m_SUI.Info.Master.text = master.Name;
		m_SUI.Info.Intro.text = Info.Intro;
		if(Info.Intro.Length < 1)
		{
			m_SUI.Info.Intro.gameObject.SetActive(false);
			m_SUI.Info.IntroEmpty.SetActive(true);
		}
		else
		{
			m_SUI.Info.Intro.gameObject.SetActive(true);
			m_SUI.Info.IntroEmpty.SetActive(false);
		}
		m_SUI.Info.Cnt.text = string.Format("{0}/{1}", Info.UserCnt, Info.MaxUserCnt);


		m_SUI.Join.JoinLV.text = string.Format("Lv.{0}", Info.JoinLV.ToString());
		m_SUI.Join.JoinType.text = BaseValue.GetGuildJoinType(Info.JoinType);


		SetResList();
	}

	void SetResList()
	{
		if(Info.EndRes.Count < 1)
		{
			m_SUI.Res.None.SetActive(true);
			m_SUI.Res.Scroll.gameObject.SetActive(false);
			return;
		}

		m_SUI.Res.None.SetActive(false);
		m_SUI.Res.Scroll.gameObject.SetActive(true);
		var list = new List<TResearchTable.Effect>();
		for(int i = Info.EndRes.Count - 1; i > -1; i-- )
		{
			var tdata = TDATA.GetGuildRes(Info.EndRes[i]);
			var data = list.Find(o => o.m_Eff == tdata.m_Eff.m_Eff);
			if(data == null)
			{
				data = new TResearchTable.Effect();
				data.m_Eff = tdata.m_Eff.m_Eff;
				data.m_Value = 0;
				list.Add(data);
			}
			data.m_Value += tdata.m_Eff.m_Value;
		}

		list.Sort((befor, after) =>
		{
			return befor.m_Eff.CompareTo(after.m_Eff);
		});

		int Max = list.Count;
		UTILE.Load_Prefab_List(Max, m_SUI.Res.Scroll.content, m_SUI.Res.Prefab);

		for (int i = 0; i < Max; i++)
		{
			Item_Union_Research_EffectList_Element element = m_SUI.Res.Scroll.content.GetChild(i).GetComponent<Item_Union_Research_EffectList_Element>();
			element.SetData(list[i]);
		}
	}

	IEnumerator Timecheck()
	{
		UIMng.BtnBG btnbg = UIMng.BtnBG.Green;
		int idx = Info.JoinType == GuildJoinType.Auto ? 6017 : 6020;
		if (MyJoin.UID == Info.UID)
		{
			m_SUI.Join.JoinBtn.SetBG(UIMng.BtnBG.Red);
			m_SUI.Join.JoinBtnLabel.text = TDATA.GetString(6064);
			yield break;
		}

		if (Info.JoinLV > USERINFO.m_LV) btnbg = UIMng.BtnBG.Not;
		else if (Info.JoinType == GuildJoinType.Auto && Info.UserCnt >= Info.MaxUserCnt) btnbg = UIMng.BtnBG.Not;

		string label = TDATA.GetString(idx);

		var etime = Math.Max(0, USERINFO.m_GRTime + 24 * 60 * 60 * 1000L);
		var gaptime = Math.Max(0f, (etime - UTILE.Get_ServerTime_Milli()) * 0.001d);

		while (gaptime > 0)
		{
			m_SUI.Join.JoinBtn.SetBG(UIMng.BtnBG.Not);
			m_SUI.Join.JoinBtnLabel.text = string.Format("{0}\n{1}", label, string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.hh_mm, Math.Max(0f, gaptime))));
			if (gaptime < 0.1f) break;
			yield return new WaitForSeconds(1f - (float)(UTILE.Get_ServerTime() % 1d));
			gaptime = Math.Max(0f, (etime - UTILE.Get_ServerTime_Milli()) * 0.001d);
		}

		m_SUI.Join.JoinBtn.SetBG(btnbg);
		m_SUI.Join.JoinBtnLabel.text = label;
	}

	void Send_Cancel_Join(Action CB)
	{
		WEB.SEND_REQ_CANCEL_GUILD_JOIN((res) => {
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_GUILD_JOIN:
					// 이미 가입된 길드가 있음
					Close((int)Union_JoinList.CloseResult.LoadGuild);
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6154));
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
				return;
			}
			MyJoin = new RES_GUILDINFO_SIMPLE();
			CB?.Invoke();
		});
	}

	#region Btn
	public void Click_Join()
	{
		if (!IsStart) return;
		if(Info.JoinLV > USERINFO.m_LV)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6024));
			return;
		}
		if(Info.JoinType == GuildJoinType.Auto && Info.UserCnt >= Info.MaxUserCnt)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
			return;
		}

		var grtime = USERINFO.m_GRTime + 24 * 60 * 60 * 1000L;
		if (grtime > UTILE.Get_ServerTime_Milli())
		{
			POPUP.Set_MsgBox(PopupName.Msg_Timer, string.Empty, string.Empty, (result, obj) => { }, 6118, grtime, Utile_Class.TimeStyle.hh_mm);
			return;
		}

		// 가입 신청 중인 연합 확인
		if (MyJoin != null && MyJoin.UID != 0)
		{
			if(MyJoin.UID == Info.UID)
			{
				Send_Cancel_Join(() =>
				{
					StartCoroutine(Timecheck());
				});
				return;
			}
			POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, string.Format(TDATA.GetString(6063), MyJoin.Name), (result, obj) => {
				if (result == 1)
				{
					Send_Cancel_Join(() => {});
				}
			}, TDATA.GetString(288), TDATA.GetString(6064));
			return;
		}

		WEB.SEND_REQ_GUILD_JOIN((res) => {
			if (!res.IsSuccess())
			{
				switch(res.result_code)
				{
				case EResultCode.ERROR_GUILD_MANY_REQ:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6043));
					break;
				case EResultCode.ERROR_GUILD_REQ_LV:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6024));
					break;
				case EResultCode.ERROR_GUILD_JOIN:
					Close((int)Union_JoinList.CloseResult.LoadGuild);
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6154));
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
				return;
			}
			if (res.UID != 0) {
				PlayEffSound(SND_IDX.SFX_1501);
				Close((int)Union_JoinList.CloseResult.Success);
				return;
			}
			Close((int)Union_JoinList.CloseResult.None);
			MyJoin.Copy(Info);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6023));
		}, Info.UID);
	}


	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
	#endregion
}
