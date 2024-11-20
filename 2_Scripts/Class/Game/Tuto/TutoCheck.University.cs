using UnityEngine;

public enum TutoType_University
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 1451 대화 </summary>
	//DLG_1451,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 1461 대화 </summary>
	//DLG_1461,
	///// <summary> 댄 공장 선택 </summary>
	//Select_University,
	/// <summary> 댄 공장 팝업 </summary>
	ViewUniversityPopup = 1,
	/// <summary> 1471 대화 </summary>
	DLG_1471,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_University()
	{
		TutoType_University type = GetTutoState<TutoType_University>();
		return false;
	}
	void PlayTuto_University(int no, object[] args)
	{
		TutoType_University type = (TutoType_University)no;
		TutoUI ui;
		//PopupBase playui;
		//GameObject obj;
		//RectTransform rtf;
		switch (type)
		{
		//case TutoType_University.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_University.DLG_1451:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1451, () => { Next(); });
		//	break;
		//case TutoType_University.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_University.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_University.DLG_1461:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1461, () => { Next(); });
		//	break;
		//case TutoType_University.Select_University:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.University);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_University.ViewUniversityPopup:
			POPUP.RemoveTutoUI();
			POPUP.GetPopup().GetComponent<Dungeon_University>().ScrollLock(true);
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_University.DLG_1471:
			POPUP.GetPopup().GetComponent<Dungeon_University>().ScrollLock(false);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1471, () => { Next(); });
			break;
		case TutoType_University.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_University(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_University type = (TutoType_University)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_University.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_University.Select_University:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.University;
		//}
		return true;
	}
}
