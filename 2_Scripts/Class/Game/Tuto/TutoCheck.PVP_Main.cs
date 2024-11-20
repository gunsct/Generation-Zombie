using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_PVP_Main
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 5050 대화 </summary>
	DLG_5050,
	/// <summary> 다운타운 버튼 클릭 </summary>
	Select_Dungeon_Menu,
	/// <summary> 다운타운 시작 연출 </summary>
	ViewDungeon,
	/// <summary> 5052 대화 </summary>
	DLG_5052,
	/// <summary> PVP 선택 </summary>
	Select_PVP,
	/// <summary> PVP 연출 </summary>
	PVP_Action,
	/// <summary> 5054 대화 </summary>
	DLG_5054,
	/// <summary> 5056 대화 </summary>
	DLG_5056,
	/// <summary> 5056 대화 </summary>
	DLG_5057,
	/// <summary> 덱셋팅버튼 선택</summary>
	Select_DeckSetBtn,
	/// <summary> 덱셋팅 팝업 연출 </summary>
	DeckSet_Action,
	/// <summary> 5057 대화 </summary>
	DLG_5058,
	DL_5301,
	DL_5303,
	DL_5305,
	/// <summary> 닫기 </summary>
	//DeckSet_Close,
	/// <summary> 닫기 </summary>dddddddddddddddddddddd
	//PVPMain_Close,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_PVP_Main()
	{
		TutoType_PVP_Main type = GetTutoState<TutoType_PVP_Main>();
		return false;
	}
	void PlayTuto_PVP_Main(int no, object[] args)
	{
		TutoType_PVP_Main type = (TutoType_PVP_Main)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;

		switch (type)
		{
		case TutoType_PVP_Main.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_PVP_Main.DLG_5050:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, false);
			ui.StartDlg(5050, () => { Next(); });
			ui.UIClone();
			break;
		case TutoType_PVP_Main.Select_Dungeon_Menu:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);
			break;
		case TutoType_PVP_Main.ViewDungeon:
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 1f);
			break;
		case TutoType_PVP_Main.DLG_5052:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.PvP);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, new Vector3(rtf.rect.width * 0.1f, rtf.rect.height * -0.1f, 0), rtf.rect.width * 0.8f, rtf.rect.height * 0.8f, false);
			ui.StartDlg(5052, () => { Next(); });
			break;
		case TutoType_PVP_Main.Select_PVP:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.PvP);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, new Vector3(rtf.rect.width * 0.1f, rtf.rect.height * -0.1f, 0), rtf.rect.width * 0.8f, rtf.rect.height * 0.8f, true);
			break;
		case TutoType_PVP_Main.PVP_Action:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_PVP_Main.DLG_5054:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((PVP_Main)POPUP.GetPopup()).GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5054, () => { Next(); });
			break;
		case TutoType_PVP_Main.DLG_5056:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((PVP_Main)POPUP.GetPopup()).GetTutoObj(2);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5056, () => { Next(); }, 0, 1);
			break;
		case TutoType_PVP_Main.DLG_5057:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((PVP_Main)POPUP.GetPopup()).GetTutoObj(3);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5057, () => { Next(); });
			break;
		case TutoType_PVP_Main.Select_DeckSetBtn:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((PVP_Main)POPUP.GetPopup()).GetTutoObj(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
			ui.StartDlg(-1);
			break;
		case TutoType_PVP_Main.DeckSet_Action:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_PVP_Main.DLG_5058:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			var popup = (PVP_DeckSetting)POPUP.GetPopup();
			popup.ScrollLock(true);
			obj = popup.GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5058, () => { Next(); });
			break;
		case TutoType_PVP_Main.DL_5301:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5301, () => { Next(); });
			break;
		case TutoType_PVP_Main.DL_5303:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((PVP_DeckSetting)POPUP.GetPopup()).GetTutoObj(2);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5303, () => { Next(); });
			break;
		case TutoType_PVP_Main.DL_5305:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((PVP_DeckSetting)POPUP.GetPopup()).GetTutoObj(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5305, () => { Next(); });
			break;
		//case TutoType_PVP_Main.DeckSet_Close:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetPopup();
		//	obj = ((PVP_DeckSetting)playui).GetTutoObj(3);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
		//	ui.StartDlg(-1);
		//	break;
		//case TutoType_PVP.PVPMain_Close:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetPopup();
		//	obj = ((PVP_Main)playui).GetTutoObj(4);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_PVP_Main.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_PVP_Main(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_PVP_Main type = (TutoType_PVP_Main)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_PVP_Main.Select_Dungeon_Menu:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.Dungeon;
		case TutoType_PVP_Main.Select_PVP:
			if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
			return (StageContentType)args[0] != StageContentType.PvP;
		case TutoType_PVP_Main.Select_DeckSetBtn:
			if (checktype != TutoTouchCheckType.PVP_Main) return true;
			return (int)args[0] != 3;
			case TutoType_PVP_Main.PVP_Action:
				if (checktype != TutoTouchCheckType.PVP_Main) return true;
				return (int)args[0] != 0;
				//case TutoType_PVP_Main.DeckSet_Close:
				//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				//	return (PopupName)args[1] != PopupName.PVP_DeckSetting;
				//case TutoType_PVP.PVPMain_Close:
				//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				//	return (PopupName)args[1] != PopupName.PVP_Main;
		}
		return true;
	}
}
