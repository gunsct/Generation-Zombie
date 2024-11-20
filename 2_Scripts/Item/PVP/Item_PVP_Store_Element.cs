using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Item_PVP_Store_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI TypeName;
		public TextMeshProUGUI BuyCnt;
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Price;
		public Image PriceIcon;
		public Transform ChildObj;
		public GameObject SeasonMark;
	}
	[SerializeField] SUI m_SUI;
	Action<Item_PVP_Store_Element> m_CB;
	public TShopTable m_TData;
	RES_SHOP_USER_BUY_INFO m_BuyInfo;
	string[] m_AnimStrs = new string[2];

	private void OnEnable() {
		for(int i = 0; i < 2; i++) {
			if(!string.IsNullOrEmpty(m_AnimStrs[i])) m_SUI.Anim.SetTrigger(m_AnimStrs[i]);
		}
	}
	public void SetData(TShopTable _tdata, Action<Item_PVP_Store_Element> _cb) {
		m_TData = _tdata;
		m_CB = _cb;

		double inittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{_tdata.m_ResetType}_{USERINFO.m_UID}", "0"));
		m_BuyInfo =  USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TData.m_Idx);
		int buycnt = m_BuyInfo == null ? 0 : m_BuyInfo.Cnt;

		m_AnimStrs[0] = m_TData.m_ResetType == ShopResetType.ZeroTime ? "Daily" : "Limited";
		m_AnimStrs[1] = buycnt < m_TData.m_LimitCnt ? "Normal" : "SoldOut";
		if (gameObject.activeSelf) {
			m_SUI.Anim.SetTrigger(m_AnimStrs[0]);
			m_SUI.Anim.SetTrigger(m_AnimStrs[1]);
		}

		m_SUI.TypeName.text = m_TData.m_ResetType == ShopResetType.ZeroTime ? TDATA.GetString(7011) : TDATA.GetString(7012);
		m_SUI.BuyCnt.text = string.Format("{0} / {1}", buycnt, m_TData.m_LimitCnt);
		m_SUI.Card.SetData(MAIN.GetRewardBase(_tdata, RewardKind.Item)[0]);
		m_SUI.Price.text = Utile_Class.CommaValue(m_TData.GetPrice());
		m_SUI.Price.color = m_SUI.PriceIcon.color = USERINFO.IS_CanBuy(m_TData) ? Utile_Class.GetCodeColor("#333333") : Utile_Class.GetCodeColor("#A62525");

		switch (m_TData.m_Level) {
			case 1:
				transform.localEulerAngles = new Vector3(0f, 0f, 1f);
				m_SUI.ChildObj.localPosition = new Vector3(0f, 287.7349f, 0f);
				break;
			case 2:
				transform.localEulerAngles = Vector3.zero;
				m_SUI.ChildObj.localPosition = new Vector3(0f, 281.98f, 0f);
				break;
			case 3:
				transform.localEulerAngles = new Vector3(0f, 0f, -0.7f);
				m_SUI.ChildObj.localPosition = new Vector3(0f, 287.7349f, 0f);
				break;
			case 4:
				transform.localEulerAngles = Vector3.zero;
				m_SUI.ChildObj.localPosition = new Vector3(0f, 274.9f, 0f);
				break;
			case 5:
				transform.localEulerAngles = new Vector3(0f, 0f, -1f);
				m_SUI.ChildObj.localPosition = new Vector3(0f, 287.7349f, 0f);
				break;
			case 6:
				transform.localEulerAngles = Vector3.zero;
				m_SUI.ChildObj.localPosition = new Vector3(0f, 278.2f, 0f);
				break;
			default:
				transform.localEulerAngles = Vector3.zero;
				m_SUI.ChildObj.localPosition = new Vector3(0f, 287.7349f, 0f);
				break;
		}
		m_SUI.SeasonMark.SetActive(m_TData.m_TagType == TagType.SEASON);
	}
	public bool IS_CanBuy() {
		int buycnt = m_BuyInfo == null ? 0 : m_BuyInfo.Cnt;
		return buycnt < m_TData.m_LimitCnt;
	}
	public void ClickBuy() {
		m_CB?.Invoke(this);
	}
}
