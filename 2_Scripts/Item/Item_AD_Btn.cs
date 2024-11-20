using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_AD_Btn : ObjMng
{
    public enum AnimType
    {
        Empty,
        Set,
        NowSelect,
        Ready
    }

    [Serializable]
    public struct SUI
    {
        public GameObject ADIcon;
		public GameObject TimeIcon;
        public TextMeshProUGUI Label;
		public TextMeshProUGUI Desc;
		public Button Btn;
	}

    [SerializeField] private SUI m_SUI;
	[SerializeField, ReName("광고중", "VIP")]
	private int[] m_Descs;
	string m_Label;
	string m_LabelAdd;
	string m_Desc;
	string m_DescAdd;
	bool m_ISMsg;

	void Awake()
	{
		if (MainMng.IsValid())
		{
			DLGTINFO.f_RFVIPBtn += RefreshUI;
		}
	}
	void OnDestroy()
	{
		if (MainMng.IsValid() && DLGTINFO != null)
		{
			DLGTINFO.f_RFVIPBtn -= RefreshUI;
		}
	}

	private void OnEnable()
	{
		RefreshUI();
	}

	public void Interactable(bool on)
	{
		m_SUI.Btn.interactable = on;
	}

	public void SetLabel(string Label, bool ISMsg = false, string _labeladd = null, bool _istimer = false)
	{
		m_Label = Label;
		m_LabelAdd = _labeladd;
		m_ISMsg = ISMsg;
		if(m_SUI.TimeIcon != null) m_SUI.TimeIcon.SetActive(_istimer);
		RefreshUI();
	}

	public void SetDesc(string Label, string _adddesc)
	{
		m_Desc = Label;
		m_DescAdd = _adddesc;
		SetDescUI();
	}

	void SetDescUI()
	{
		if (m_SUI.Desc != null)
		{
			if (!string.IsNullOrEmpty(m_Desc)) m_SUI.Desc.text = m_Desc;
			else if (m_Descs.Length > 0)
			{
				if (USERINFO.m_ShopInfo.IsPassBuy()) m_SUI.Desc.text = TDATA.GetString(m_Descs[1]) + m_DescAdd;
				else m_SUI.Desc.text = TDATA.GetString(m_Descs[0]) + m_DescAdd;
			}
		}
	}

	public void RefreshUI()
	{
		SetDescUI();
		HorizontalLayoutGroup group = m_SUI.Label.transform.parent.GetComponent<HorizontalLayoutGroup>();
		if(m_ISMsg)
		{
			if (group != null) group.padding.left = group.padding.right = 0;
			m_SUI.Label.text = m_Label;
			m_SUI.ADIcon.SetActive(false);
			return;
		}
		if (USERINFO.m_ShopInfo.IsPassBuy())
		{
			if(group != null) group.padding.left = group.padding.right = 0;
			m_SUI.Label.text = string.IsNullOrEmpty(m_LabelAdd) ? "FREE" : string.Format("FREE {0}", m_LabelAdd);
			m_SUI.ADIcon.SetActive(false);
			return;
		}

		if (group != null)
		{
			group.padding.left = 20;
			group.padding.right = 10;
		}

		m_SUI.Label.text = string.IsNullOrEmpty(m_Label) ? TDATA.GetString(5107) : m_Label;
		m_SUI.ADIcon.SetActive(true);
	}
}