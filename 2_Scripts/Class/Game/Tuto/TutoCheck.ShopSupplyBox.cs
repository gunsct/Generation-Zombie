using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_ShopSupplyBox
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock,
	DL_2201,
	Focus_ShopMenu,
	Delay_Scrolling,
	DL_2203,
	Focus_SupplyBox,
	Focus_ShopMenu2,
	DL_4501,
	Focus_CharGacha,
	DL_4503,
	Select_10Gacha,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_ShopSupplyBox()
	{
		TutoType_ShopSupplyBox type = GetTutoState<TutoType_ShopSupplyBox>();
		return false;
	}
	void PlayTuto_ShopSupplyBox(int no, object[] args)
	{
		TutoType_ShopSupplyBox type = (TutoType_ShopSupplyBox)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
			case TutoType_ShopSupplyBox.StartTalk:
				POPUP.ShowTutoStartAction(1901, () => {
					Next();
				});
				break;
			case TutoType_ShopSupplyBox.Btn_Unlock:
				POPUP.RemoveTutoUI();
				playui = POPUP.GetMainUI();
				((Main_Play)playui).GetMenuGroup.SetLockFX(MainMenuType.Shop);
				POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
				break;
			case TutoType_ShopSupplyBox.DL_2201:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2201, () => { Next(); });
				break;
			case TutoType_ShopSupplyBox.Focus_ShopMenu:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Shop);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_ShopSupplyBox.Delay_Scrolling:
				POPUP.RemoveTutoUI();
				Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.StartPos(true, ShopGroup.SupplyBox, true, () => { TUTO.Next(); });
				shop.SetScrollState(false);
				break;
			case TutoType_ShopSupplyBox.Focus_SupplyBox:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetPanel(6);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 3);
				break;
			case TutoType_ShopSupplyBox.DL_2203:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2203, () => { Next(); });
				break;
			case TutoType_ShopSupplyBox.Focus_ShopMenu2:
				POPUP.RemoveTutoUI();
				shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.StartPos(false, ShopGroup.Gacha, true, () => { Next(); });
				break;
			case TutoType_ShopSupplyBox.DL_4501:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(4501, () => { Next(); });
				break;
			case TutoType_ShopSupplyBox.Focus_CharGacha:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetPanel(4);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 3);
				break;
			case TutoType_ShopSupplyBox.DL_4503:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(4503, () => { Next(); });
				break;
			case TutoType_ShopSupplyBox.Select_10Gacha:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetGacha10Btn();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				break;
			case TutoType_ShopSupplyBox.End:
				shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.SetScrollState(true);
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_ShopSupplyBox(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_ShopSupplyBox type = (TutoType_ShopSupplyBox)m_TutoState[m_NowTuto];
		switch (type) {
			case TutoType_ShopSupplyBox.Focus_ShopMenu:
				if (checktype == TutoTouchCheckType.Play_Menu && (MainMenuType)args[0] == MainMenuType.Shop) return false;
				if (checktype == TutoTouchCheckType.ShopMenu && (int)args[0] == 0) return false;
				return true;
				//if (checktype != TutoTouchCheckType.Play_Menu) return true;
				//return (MainMenuType)args[0] != MainMenuType.Shop;
			case TutoType_ShopSupplyBox.Select_10Gacha:
				if (checktype != TutoTouchCheckType.ShopBuy_Gacha) return true;
				return (int)args[0] != 0;
		}
		return true;
	}
}
