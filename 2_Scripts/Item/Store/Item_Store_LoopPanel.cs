using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_Store_LoopPanel : ObjMng
{
#pragma warning disable 0649
	[Serializable]
	public struct SUI
	{
		public Animator Ani;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Time;
		public TextMeshProUGUI PriceLabel;
		public TextMeshProUGUI Price;
		public Image Profile;
		public Image Nation;
		public Text Name;
		public GameObject[] Decos;
		public GameObject MyGetInfo;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649

	AuctionItem m_Now;
	AuctionItem m_Befor;
	Action m_ReLoadCB;

	bool IsChange { get { return m_Now != null && !m_Now.IsChangeAction && m_Befor != null && (m_Now.UserNo != m_Befor.UserNo || m_Now.GetPrice() != m_Befor.GetPrice()); } }


	private void Update()
	{
		if (m_Now == null) return;
		SetTimeUI();
	}

	public void SetData(AuctionItem Now, AuctionItem Befor, Action ReLoadCB)
	{
		m_Now = Now;
		m_Befor = Befor;
		m_ReLoadCB = ReLoadCB;
		int pos = USERINFO.m_Auction.Items.IndexOf(Now) % 3;
		switch(pos)
		{
		case 0:
			m_sUI.Decos[0].SetActive(true);
			m_sUI.Decos[1].SetActive(false);
			m_sUI.Decos[2].SetActive(false);
			m_sUI.Decos[3].SetActive(false);
			break;
		case 1:
			m_sUI.Decos[0].SetActive(false);
			m_sUI.Decos[1].SetActive(true);
			m_sUI.Decos[2].SetActive(true);
			m_sUI.Decos[3].SetActive(false);
			break;
		default:
			m_sUI.Decos[0].SetActive(false);
			m_sUI.Decos[1].SetActive(false);
			m_sUI.Decos[2].SetActive(false);
			m_sUI.Decos[3].SetActive(true);
			break;
		}
		// 변경 연출 전(노출된적이 없음) && 이전 데이터가 있고 유저가 다을때
		SetUI(IsChange);
	}

	void SetUI(bool UseBefor)
	{
		if(m_Now == null)
		{
			m_sUI.Item.gameObject.SetActive(false);
			m_sUI.Profile.sprite = TDATA.GetUserProfileImage(0);
			m_sUI.Name.text = "";
			m_sUI.Price.text = "0";
			m_sUI.MyGetInfo.SetActive(false);
		}
		else
		{
			AuctionItem item = UseBefor && m_Befor != null ? m_Befor : m_Now;
			m_sUI.Item.gameObject.SetActive(true);
			m_sUI.Item.SetData(item.GetReward(), (obj) => { }, false);
			m_sUI.Price.text = Utile_Class.CommaValue(item.GetPrice());
			if (item.UserNo == 0)
			{
				m_sUI.PriceLabel.text = TDATA.GetString(1114);
				m_sUI.Profile.sprite = TDATA.GetUserProfileImage(0);
				m_sUI.Name.text = "-";
				m_sUI.Nation.gameObject.SetActive(false);
			}
			else
			{
				m_sUI.PriceLabel.text = TDATA.GetString(702);
				m_sUI.Profile.sprite = TDATA.GetUserProfileImage(item.Profile);
				m_sUI.Name.text = item.m_Name;
				m_sUI.Nation.gameObject.SetActive(true);
				m_sUI.Nation.sprite = BaseValue.GetNationIcon(item.Nation);
			}
			m_sUI.MyGetInfo.SetActive(item.UserNo == USERINFO.m_UID);
		}
		SetTimeUI();
	}

	void SetTimeUI()
	{
		if (m_Now == null)
		{
			m_sUI.Time.text = TDATA.GetString(704);
			return;
		}
		long time = m_Now.Times[1] - (long)UTILE.Get_ServerTime_Milli();
		if(time < 1)
		{
			// 마감됨
			m_sUI.Time.text = TDATA.GetString(700);
		}
		else
		{
			m_sUI.Time.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, (double)time * 0.001D);
		}
	}

	public void StartChangeAction()
	{
		if (!IsChange) return;
		m_Now.IsChangeAction = true;
		SetUI(false);
		m_sUI.Ani.SetTrigger("Change");
	}

	public void OnDetailInfo()
	{
		if (m_Now == null) return;
		if (m_Now.kind == RewardKind.Item) POPUP.ViewItemInfo((result, obj) => { }, new object[] { m_Now, PopupName.NONE, null }).OnlyInfo();
		else POPUP.ViewItemToolTip(m_Now.GetReward(), (RectTransform)m_sUI.Item.transform);
	}


	public void OnReLoad()
	{
		m_ReLoadCB?.Invoke();
	}
}
