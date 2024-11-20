using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Subway
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 5040 대화 </summary>
	//DLG_5040,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 다운타운 시작 연출 </summary>
	//ViewDungeon,
	///// <summary> 5042 대화 </summary>
	//DLG_5042,
	///// <summary> 지하철 선택 </summary>
	//Select_Subway,
	/// <summary> 지하철 연출 </summary>
	Subway_Action = 1,
	/// <summary> 5044 대화 </summary>
	DLG_5044,
	/// <summary> 5045 대화 </summary>
	DLG_5045,
	/// <summary> 5047 대화 </summary>
	DLG_5047,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Subway()
	{
		TutoType_Subway type = GetTutoState<TutoType_Subway>();
		return false;
	}
	void PlayTuto_Subway(int no, object[] args)
	{
		TutoType_Subway type = (TutoType_Subway)no;
		TutoUI ui;
		//PopupBase playui;
		GameObject obj;
		RectTransform rtf;

		switch (type)
		{
		//case TutoType_Subway.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_Subway.DLG_5040:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();

		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, false);

		//	ui.StartDlg(5040, () => { Next(); });
		//	ui.UIClone();
		//	break;
		//case TutoType_Subway.Select_Dungeon_Menu:

		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);

		//	break;
		//case TutoType_Subway.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	POPUP.StartTutoTimer(() => { Next(); }, 1f);
		//	break;
		//case TutoType_Subway.DLG_5042:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Subway);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);

		//	ui.StartDlg(5042, () => { Next(); });
		//	break;
		//case TutoType_Subway.Select_Subway:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Subway);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	break;
		case TutoType_Subway.Subway_Action:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Subway.DLG_5044:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5044, () => { Next(); });
			break;
		case TutoType_Subway.DLG_5045:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = ((Dungeon_Subway)POPUP.GetPopup()).GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);
			ui.StartDlg(5045, () => { Next(); }, 0, 1);
			break;
		case TutoType_Subway.DLG_5047:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			obj = ((Dungeon_Subway)POPUP.GetPopup()).GetTutoObj(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, false);

			ui.StartDlg(5047, () => { Next(); }, 0, 1);
			break;
		case TutoType_Subway.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Subway(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Subway type = (TutoType_Subway)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_Subway.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_Subway.Select_Subway:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.Subway;
		//}
		return true;
	}
}
