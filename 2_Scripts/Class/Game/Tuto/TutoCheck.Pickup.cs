using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Pickup
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	DLG_5101,
	Focus_ShopMenu,
	Delay_Scrolling,
	Focus_Gacha,
	DLG_5103,
	Delay_TouchPickupBtn,
	DLG_5104,
	DLG_5107,
	DLG_5110,
	/// <summary> </summary>
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Pickup()
	{
		TutoType_Pickup type = GetTutoState<TutoType_Pickup>();
		return false;
	}
	void PlayTuto_Pickup(int no, object[] args)
	{
		TutoType_Pickup type = (TutoType_Pickup)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;

		switch (type)
		{
			case TutoType_Pickup.StartTalk:
				POPUP.ShowTutoStartAction(1901, () => {
					Next();
				});
				break;
			case TutoType_Pickup.DLG_5101:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5101, () => { Next(); });
				break;
			case TutoType_Pickup.Focus_ShopMenu:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Shop);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);

				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_Pickup.Delay_Scrolling:
				POPUP.RemoveTutoUI();
				Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.StartPos(true, ShopGroup.Gacha, true, () => { TUTO.Next(); });
				shop.SetScrollState(false);
				break;
			case TutoType_Pickup.Focus_Gacha:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetPanel(4);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2);
				break;
			case TutoType_Pickup.DLG_5103:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetPickupBtn();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(5103, () => { Next(); });
				ui.UIClone();
				break;
			case TutoType_Pickup.Delay_TouchPickupBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_Pickup.DLG_5104:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = ((Gacha_Pickup)playui).GetFirstSlot();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(5104, () => { Next(); }, 0, 1);
				ui.UIClone();
				break;
			case TutoType_Pickup.DLG_5107:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetPopup();
				obj = ((Gacha_Pickup)playui).GetList();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(5107, () => { Next(); });
				ui.UIClone();
				break;
			case TutoType_Pickup.DLG_5110:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(5110, () => { POPUP.StartTutoTimer(() => { Next(); }, 3); });
				ui.UIClone();
				break;
			case TutoType_Pickup.End:
				shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.SetScrollState(true);
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_Pickup(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Pickup type = (TutoType_Pickup)m_TutoState[m_NowTuto];
		switch (type) {
			case TutoType_Pickup.Focus_ShopMenu:
				if (checktype == TutoTouchCheckType.Play_Menu && (MainMenuType)args[0] == MainMenuType.Shop) return false;
				if (checktype == TutoTouchCheckType.ShopMenu && (int)args[0] == 0) return false;
				return true;
			case TutoType_Pickup.Delay_TouchPickupBtn:
				return checktype != TutoTouchCheckType.PickupGacha_Btn;
		}
		return true;
	}
}
