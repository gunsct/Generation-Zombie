using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class Union_JoinList : PopupBase
{
	public enum CloseResult
	{
		/// <summary> 등급 변경됨 마스터 등급만 가능한 UI는 닫아주기 </summary>
		Grade = -3,
		/// <summary> 길드 해산 </summary>
		Destroy = -2,
		/// <summary> 다른 길드 가입되거나 추방된 상태이므로 다시 로드 </summary>
		LoadGuild = -1,
		None = 0,
		/// <summary> 길드 가입된 상태이므로 다시 로드 </summary>
		Success,
		/// <summary> UI만 갱싱 </summary>
		UIReset,
	}

	public enum ListState
	{
		/// <summary> 추천 리스트 </summary>
		Recommend = 0,
		/// <summary> 길드 찾기 </summary>
		Find
	}

	[Serializable]
	public struct SNoneListUI
	{
		public GameObject Active;

		public TextMeshProUGUI Label;
		public GameObject Btn;
	}
	[Serializable]
	public struct SListUI
	{
		public GameObject Active;

		public ScrollRect Scroll;
		public RectTransform Prefab;
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;


		public InputField Input;

		public SNoneListUI NoneList;
		public SListUI List;

		public GameObject[] TutoObj;//0:길드 리스트, 1:Create 버튼
	}
	[SerializeField] SUI m_SUI;
	bool IsStart;
	ListState m_ListState;
	RES_GUILD_RECOMMEND m_Data;
	SND_IDX m_PreBGSND;

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
		PlayEffSound(SND_IDX.SFX_1500);
		if (TUTO.IsTuto(TutoKind.Guild, (int)TutoType_Guild.View_GuildList)) TUTO.Next();
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PreBGSND = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_0020);

		if (TUTO.IsTuto(TutoKind.Guild, (int)TutoType_Guild.Select_Guild)) TUTO.Next();
		IsStart = false;
		m_SUI.Input.characterLimit = BaseValue.NICKNAME_LENGTH;
		var data = (RES_GUILD_RECOMMEND)aobjValue[0];
		m_Data = data;
		m_ListState = ListState.Recommend;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		SetList();
	}

	void SetList()
	{
		m_Data.Guilds.Sort((befor, after) =>
		{
			int bNo = befor.UID == m_Data.MyJoin.UID ? 0 : 1;
			int aNo = after.UID == m_Data.MyJoin.UID ? 0 : 1;
			return bNo.CompareTo(aNo);
		});
		int Max = m_Data.Guilds.Count;
		bool IsRec = m_ListState == ListState.Recommend;
		if (IsRec) m_SUI.Input.text = "";
		if (Max < 1)
		{
			m_SUI.NoneList.Active.SetActive(true);
			m_SUI.List.Active.SetActive(false);

			m_SUI.NoneList.Label.text = TDATA.GetString(IsRec ? 6025 : 6003);
			m_SUI.NoneList.Btn.SetActive(!IsRec);
		}
		else
		{
			m_SUI.NoneList.Active.SetActive(false);
			m_SUI.List.Active.SetActive(true);
			UTILE.Load_Prefab_List(Max, m_SUI.List.Scroll.content, m_SUI.List.Prefab);
			for (int i = 0; i < Max; i++)
			{
				var info = m_Data.Guilds[i];
				Item_Union_JoinList_Element element = m_SUI.List.Scroll.content.GetChild(i).GetComponent<Item_Union_JoinList_Element>();
				element.SetData(info, Click_GuildDetailInfo, info.UID == m_Data.MyJoin.UID);
			}
		}
	}
#region Btn

	public void Click_ResetList()
	{
		if (!IsStart) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 5)) return;
		WEB.SEND_REQ_GUILD_RECOMMEND((res) => {
			if (res.IsSuccess())
			{
				// 자신의 길드정보 갱신해주기
				m_Data = res;
				m_ListState = ListState.Recommend;

				SetUI();
			}
		}, new List<long>());
	}

	public void Click_GuildDetailInfo(RES_GUILDINFO_SIMPLE Guild)
	{
		if (!IsStart) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 4, Guild)) return;
		WEB.SEND_REQ_GUILD((res) => {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_JoinInfo, (result, obj) => {
				switch (result)
				{
				case -1:
				case 1:
					Close(result);
					break;
				case 0:
					// 신청상태 체크
					m_Data.MyJoin = obj.GetComponent<Union_JoinInfo>().MyJoin;
					break;
				}
			}, res, m_Data.MyJoin);
			
		}, (int)GuildInfoMode.All, Guild.UID);
	}

	public void Click_Search()
	{
		if (!IsStart) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 3)) return;
		if (m_SUI.Input == null || m_SUI.Input.text.Length < 1) return;
		WEB.SEND_REQ_GUILD_FIND((res) => {
			if (res.IsSuccess())
			{
				// 자신의 길드정보 갱신해주기
				m_Data.Guilds = res.Guilds;
				m_ListState = ListState.Find;

				SetUI();
			}
		}, m_SUI.Input.text);
	}

	public void Click_Shop()
	{
		if (!IsStart) return;
		GoShop();
	}
	public void GoShop() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 2)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Store, (result, obj) => { }, true);
	}

	public void Click_CreateGuild()
	{
		if (!IsStart) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 1)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_NewUnion, (result, obj) => {
			switch(result)
			{
			case -1:
			case 1:
				// 연출 무시하고 UI갱신을 위해 바로 호출
				base.Close(result);
				break;
			case 0:
				// 신청상태 체크
				m_Data.MyJoin = obj.GetComponent<Union_NewUnion>().MyJoin;
				break;
			}

		}, m_Data.MyJoin);
	}


	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Union_JoinList, 0)) return;
		// 연출 사용 안함
		PlayBGSound(m_PreBGSND);
		base.Close(Result);
		//StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		PlayBGSound(m_PreBGSND);
		base.Close(_result);
	}
	#endregion


	///////튜토용
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}

	public virtual void ScrollLock(bool _lock)
	{
		m_SUI.List.Scroll.enabled = !_lock;
	}

	public virtual void InputLock(bool _lock)
	{
		m_SUI.Input.enabled = !_lock;
	}
}
