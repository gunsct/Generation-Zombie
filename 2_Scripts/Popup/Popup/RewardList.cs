using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class RewardList : PopupBase
{
	// 스크롤 아이템 0.25초부터 0.5초간격으로 Start해주기
	[System.Serializable]
	public struct SListUI
	{
		public GameObject Active;
		public ScrollRect Scroll;
		public RectTransform Prefab;
	}

	[System.Serializable]
	public struct SOneUI
	{
		public GameObject Active;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Name;
	}

	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public GameObject CloseBtn;
		public SOneUI OneItem;
		public SListUI List;
		public TextMeshProUGUI TitleTxt;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_PlayAction;
	bool m_ListAction;
	/// <summary>서버서 인벤토리 부족 체크</summary>
	bool m_IsInvenLowGoPost;
	float m_SkipDelay = 4f;
	Dictionary<Res_RewardType, RectTransform> m_RewardPos = new Dictionary<Res_RewardType, RectTransform>();
	RewardAssetAni RewAssetAni;
	private void Start()
	{
		m_PlayAction = StartAniCheck();
		StartCoroutine(m_PlayAction);
		Invoke("SkipOn", m_SkipDelay);
	}

	/// <summary> aobjValue 0 : 제작된 아이템 정보, 1 : 제작된 수량, 2 : 제작된 등급(소모품은 모두 일반등급이라 제작시 내부 등급 필요) </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		PlayEffSound(SND_IDX.SFX_0310);
		List<RES_REWARD_BASE> items = (List<RES_REWARD_BASE>)aobjValue[0];
		//지금은 없어진 뭔가.. 알 수 없는거라 주석걸어둠
		//if (aobjValue.Length > 1)
		//{
		//	bool isSuccess = (bool) aobjValue[1];
		//	if (isSuccess)
		//	{
		//		m_SUI.TitleTxt.text = TDATA.GetString(64);
		//		m_SUI.Anim.SetTrigger("Normal");
		//	}
		//	else
		//	{
		//		m_SUI.TitleTxt.text = TDATA.GetString(43);
		//		m_SUI.Anim.SetTrigger("Fail");
		//	}
		//}
		
		m_ListAction = false;
		if (items.Count == 0)
		{
			Close();
			return;
		}
		else if (items.Count < 2)
		{
			SetOneItem(items[0]);
		}
		else
		{
			SetListItem(items);
		}

		if (m_IsInvenLowGoPost) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));

		m_SUI.CloseBtn.SetActive(false);
	}

	void Load_RewardAssetAniPopup()
	{
		if (m_RewardPos.Count > 0) RewAssetAni = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardAssetAni, (result, obj) => { RewAssetAni = null; }, m_RewardPos).GetComponent<RewardAssetAni>();
	}

	void SetOneItem(RES_REWARD_BASE item)
	{
		m_SUI.OneItem.Active.SetActive(true);
		m_SUI.List.Active.SetActive(false);
		m_SUI.OneItem.Item.SetData(item);

		if(item.result_code == EResultCode.SUCCESS_POST) m_IsInvenLowGoPost = true;

		string strName = null;
		switch (item.Type)
		{
		case Res_RewardType.DNA:
		{
			TDnaTable tdata = TDATA.GetDnaTable(item.GetIdx());
			strName = tdata.GetName();
		}
		break;
		case Res_RewardType.Zombie:
		{
			TZombieTable tdata = TDATA.GetZombieTable(item.GetIdx());
			strName = tdata.GetName();
		}
		break;
		case Res_RewardType.Char:
		{
			TCharacterTable tdata = TDATA.GetCharacterTable(item.GetIdx());
			strName = tdata.GetCharName();
		}
		break;
		default:
		{
			switch (item.Type)
			{
				case Res_RewardType.Money: 
				case Res_RewardType.Exp: 
				case Res_RewardType.Cash:
					if (m_RewardPos.ContainsKey(item.Type)) break;
					m_RewardPos.Add(item.Type, (RectTransform)m_SUI.OneItem.Item.transform);
					break;
			}
			TItemTable tdata = TDATA.GetItemTable(item.GetIdx());
			strName = tdata.GetName();
		}
		break;
		}

		m_SUI.OneItem.Name.text = strName;
		Load_RewardAssetAniPopup();

		if (RewAssetAni != null)
		{
			RewAssetAni.StartAction(m_SUI.OneItem.Item.m_RewardType);
			RewAssetAni.Dealay_Close(2f);
		}
	}

	void SetListItem(List<RES_REWARD_BASE> items)
	{
		m_SUI.OneItem.Active.SetActive(false);
		m_SUI.List.Active.SetActive(true);
		RectTransform panel = m_SUI.List.Scroll.content;
		UTILE.Load_Prefab_List(items.Count, panel, m_SUI.List.Prefab);
		for(int i = items.Count - 1; i > -1; i--) {
			Item_RewardList_Item item = panel.GetChild(i).GetComponent<Item_RewardList_Item>();
			item.gameObject.SetActive(false);
			item.transform.localScale = Vector3.one * 0.8f;
			item.SetData(items[i]);
			switch (items[i].Type)
			{
				case Res_RewardType.Money: 
				case Res_RewardType.Exp: 
				case Res_RewardType.Cash:
					if (m_RewardPos.ContainsKey(items[i].Type)) break;
					m_RewardPos.Add(items[i].Type, (RectTransform)item.transform);
					break;
			}
			if (items[i].result_code == EResultCode.SUCCESS_POST) m_IsInvenLowGoPost = true;
		}
		Load_RewardAssetAniPopup();
		m_PlayAction = ListActionStart();
		StartCoroutine(m_PlayAction);
	}

	IEnumerator ListActionStart()
	{
		m_ListAction = true;
		RectTransform panel = m_SUI.List.Scroll.content;
		// 스크롤 아이템 0.25초부터 0.5초간격으로 Start해주기
		yield return new WaitForSeconds(0.25f);
		for (int i = 0; i < panel.childCount; i++) {
			PlayEffSound(SND_IDX.SFX_0313);
			Item_RewardList_Item item = panel.GetChild(i).GetComponent<Item_RewardList_Item>();
			item.gameObject.SetActive(true);
			yield return new WaitForEndOfFrame();
			m_SUI.List.Scroll.verticalNormalizedPosition = 0f;
			if (RewAssetAni != null) RewAssetAni.StartAction(item.m_RewardType);
			yield return new WaitForSeconds(0.25f);
		}
		if (RewAssetAni != null) RewAssetAni.Dealay_Close(1.75f);
		m_ListAction = false;
	}

	IEnumerator StartAniCheck()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		yield return new WaitWhile(() => m_ListAction);
		m_SUI.CloseBtn.SetActive(true);
		m_PlayAction = null;
	}

	public void SkipOn()
	{
		m_ListAction = false;
	}

	public void ClickExit()
	{
		if (RewAssetAni != null) return;
		Close(0);
	}

	public override void Close(int Result = 0)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_PlayAction != null) return;
		m_SUI.CloseBtn.SetActive(false);
		m_PlayAction = CloseAni(Result);
		StartCoroutine(m_PlayAction);
	}

	public IEnumerator CloseAni(int Result) {
		if (RewAssetAni != null) RewAssetAni.SetOffTopAsset();
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_PlayAction = null;
		base.Close(Result);
	}
}
