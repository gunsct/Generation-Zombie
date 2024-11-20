using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using static LS_Web;

public class PostList : PopupBase
{
	[Serializable]
	public struct SRotPosition
	{
		public Vector2 Pos;
		public Vector3 Rot;
	}

	[Serializable]
	public struct SUI
	{
		public GameObject Prefab;
		public ScrollRect Scroll;
		public GameObject NoneText;

		public Item_Button AllBtn;
		public SRotPosition[] RotData;
	}
	[SerializeField] SUI m_SUI;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	bool m_IsStart = true;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		PLAY.PlayEffSound(SND_IDX.SFX_0200);
		m_SUI.NoneText.SetActive(true);
		m_SUI.Scroll.gameObject.SetActive(false);
		m_SUI.AllBtn.SetActive(false);
		m_IsStart = true;
		// 이벤트나 챌린지등 보상은 우편으로 들어오므로 우편함이 열리면 리스트를 받는다.
#if NOT_USE_NET
#else
		WEB.SEND_REQ_POSTINFO((res) => {
			ViewList();
		});
#endif
	}
	public void ViewList()
	{
		if (USERINFO.m_Posts.Count < 1)
		{
			m_SUI.NoneText.SetActive(true);
			m_SUI.Scroll.gameObject.SetActive(false);
			m_SUI.AllBtn.SetActive(false);
		}
		else
		{
			m_SUI.NoneText.SetActive(false);
			m_SUI.Scroll.gameObject.SetActive(true);
			m_SUI.AllBtn.SetActive(true);
			SetList();
		}
	}

	/// <summary> 제작 가능 리스트업 </summary>
	void SetList() {

		m_SUI.NoneText.SetActive(false);
		m_SUI.Scroll.gameObject.SetActive(true);

		UTILE.Load_Prefab_List(USERINFO.m_Posts.Count, m_SUI.Scroll.content, m_SUI.Prefab.transform);

		for(int i = USERINFO.m_Posts.Count -1; i > -1; i--)
		{
			Item_PostList item = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_PostList>();
			int RotPos = i % 4;
			item.SetData(USERINFO.m_Posts[i], ItemClick, m_SUI.RotData[RotPos].Pos, m_SUI.RotData[RotPos].Rot);
		}

		// 박스 아이템이나 케릭터가 아닌 아이템들 or Zombie or DNA 가 있을때만 활성화
		bool IsAllActive = USERINFO.m_Posts.FindAll(o => o.Rewards.Find(r =>
		{
			bool isSelect = false;
			if (r.Kind == RewardKind.Zombie || r.Kind == RewardKind.DNA)
			{
				isSelect = true;
			}
			else if (r.Kind == RewardKind.Item)
			{
				isSelect = TDATA.GetItemTable(r.Idx).m_Type != ItemType.AllBox;
			}

			return isSelect;
		}) != null).Count > 0;
		
		m_SUI.AllBtn.SetActive(IsAllActive, false, IsAllActive ? UIMng.BtnBG.Green : UIMng.BtnBG.Not);

		if (m_IsStart) StartCoroutine(ListAction());
		m_IsStart = false;
	}

	IEnumerator ListAction()
	{
		for (int i = 0, iMax = USERINFO.m_Posts.Count; i < iMax; i++)
		{
			// 최초놈이 Empty애니 셋팅이 있어 딜레이를 앞에줌
			yield return new WaitForSeconds(0.2f);
			if (m_SUI.Scroll.content.childCount - 1 < i) continue;
			Item_PostList item = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_PostList>();
			item.StartAni(Item_PostList.AniName.Start);
		}
	}

	void ItemClick(Item_PostList item)
	{
		var rewards = item.m_Info.GetRewards();
		if (rewards.Count > 0 && rewards[0].Type == Res_RewardType.Item && TDATA.GetItemTable(rewards[0].GetIdx()).m_Type == ItemType.Select) {
			Action<int, List<int>> cb = (idx, rewards) => {
				SEND_REQ_POST_REWARD(new List<PostInfo>() { item.m_Info }, rewards);
			};
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Select, null, new object[] { rewards[0].GetIdx(), Store_Select.State.Post, cb });
		}
		else {
			// 보상 받기
			SEND_REQ_POST_REWARD(new List<PostInfo>() { item.m_Info });
		}
	}

	void SEND_REQ_POST_REWARD(List<PostInfo> Posts, List<int> _pickupidx = null)
	{
		m_Rewards.Clear();
#if NOT_USE_NET
#else
		WEB.SEND_REQ_POST_REWARD((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			ViewList();
			if (res.Rewards == null) return;
			m_Rewards.AddRange(res.GetRewards());
			// 보상에 따라 맞춰 연출해주기
			if (m_Rewards.Count > 0)
			{
				MAIN.SetRewardList(new object[] { m_Rewards }, () => { m_Rewards.Clear(); });
				//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => {
				//	m_Rewards.Clear();
				//}, m_Rewards);
			}
		}, Posts.Select(o => o.UID).ToList(), _pickupidx);
#endif
	}

	public void GetAll()
	{
		List<PostInfo> Posts = USERINFO.m_Posts.FindAll(o => o.Rewards.Find(r =>
		{
			bool isSelect = false;
			if (r.Kind == RewardKind.Zombie || r.Kind == RewardKind.DNA)
			{
				isSelect = true;
			}
			else if (r.Kind == RewardKind.Item)
			{
				isSelect = TDATA.GetItemTable(r.Idx).m_Type != ItemType.AllBox && TDATA.GetItemTable(r.Idx).m_Type != ItemType.RandomBox && TDATA.GetItemTable(r.Idx).m_Type != ItemType.Select;
			}

			return isSelect;
		}) != null).ToList();

		if (Posts.Count < 1) return;

		SEND_REQ_POST_REWARD(Posts);
	}

	public override void Close(int Result = 0)
	{
		// 유저정보 최신화
#if !NOT_USE_NET && UNITY_EDITOR
		WEB.SEND_REQ_ALL_INFO((res) => {});
#endif
		base.Close(Result);

	}
}
