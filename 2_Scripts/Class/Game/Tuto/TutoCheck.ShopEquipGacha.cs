using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_ShopEquipGacha
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	DL_5012,
	Focus_ShopMenu,
	Delay_Scrolling,
	DL_5015,
	Focus_EquipGacha,
	DL_5016,
	Select_10Gacha,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_ShopEquipGacha()
	{
		TutoType_ShopEquipGacha type = GetTutoState<TutoType_ShopEquipGacha>();
		return false;
	}
	void PlayTuto_ShopEquipGacha(int no, object[] args)
	{
		TutoType_ShopEquipGacha type = (TutoType_ShopEquipGacha)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
			case TutoType_ShopEquipGacha.StartTalk:
				POPUP.ShowTutoStartAction(1901, () => {
					Next();
				});
				break;
			case TutoType_ShopEquipGacha.DL_5012:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5012, () => { Next(); });
				break;
			case TutoType_ShopEquipGacha.Focus_ShopMenu:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Shop);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				ui.UIClone();
				break;
			case TutoType_ShopEquipGacha.Delay_Scrolling:
				POPUP.RemoveTutoUI();
				Shop shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.StartPos(true, ShopGroup.ItemGacha, true, () => { TUTO.Next(); });
				shop.SetScrollState(false);
				break;
			case TutoType_ShopEquipGacha.DL_5015:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5015, () => { Next(); });
				break;

			case TutoType_ShopEquipGacha.Focus_EquipGacha:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetPanel(9);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(-1);
				ui.UIClone();
				POPUP.StartTutoTimer(() => { Next(); }, 3);
				break;
			case TutoType_ShopEquipGacha.DL_5016:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5016, () => { Next(); });
				break;
			case TutoType_ShopEquipGacha.Select_10Gacha:
				ui = POPUP.ShowTutoUI();
				playui = POPUP.GetMainUI();
				shop = ((Main_Play)playui).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				obj = shop.GetEquipGacha10Btn();
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				ui.StartDlg(-1);
				break;
			case TutoType_ShopEquipGacha.End:
				shop = ((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
				shop.SetScrollState(true);
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_ShopEquipGacha(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_ShopEquipGacha type = (TutoType_ShopEquipGacha)m_TutoState[m_NowTuto];
		switch (type) {
			case TutoType_ShopEquipGacha.Focus_ShopMenu:
				if (checktype == TutoTouchCheckType.Play_Menu && (MainMenuType)args[0] == MainMenuType.Shop) return false;
				if (checktype == TutoTouchCheckType.ShopMenu && (int)args[0] == 0) return false;
				return true;
			case TutoType_ShopEquipGacha.Select_10Gacha:
				if (checktype != TutoTouchCheckType.ShopBuy_ItemGacha) return true;
				return (int)args[0] != 1;
		}
		return true;
	}
}
