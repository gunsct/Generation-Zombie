using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public partial class Union_Main : PopupBase
{
	[Serializable]
	public struct SShopUI
	{
		public GameObject Active;

		public Union_Store Store;
	}

	void Set_Shop_UI()
	{
		m_SUI.Main.Active.SetActive(false);
		m_SUI.Shop.Active.SetActive(true);

		m_SUI.Shop.Store.ActiveCloseBtn(false);
		m_SUI.Shop.Store.SetUI();
	}
}
