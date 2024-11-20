using UnityEngine;

public enum TutoType_Academy
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 1301 대화 </summary>
	//DLG_1301,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	/// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 1311 대화 </summary>
	//DLG_1311,
	///// <summary> 아카데미 선택 </summary>
	//Select_Academy,
	/// <summary> 아카데미 팝업 </summary>
	ViewAcademyPopup = 1,
	/// <summary> 1321 대화 </summary>
	DLG_1321,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Academy()
	{
		TutoType_Academy type = GetTutoState<TutoType_Academy>();
		return false;
	}
	void PlayTuto_Academy(int no, object[] args)
	{
		TutoType_Academy type = (TutoType_Academy)no;
		TutoUI ui;
		//PopupBase playui;
		//GameObject obj;
		//RectTransform rtf;
		switch (type)
		{
		//case TutoType_Academy.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_Academy.DLG_1301:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1301, () => { Next(); });
		//	break;
		//case TutoType_Academy.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	//ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
		//	ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_Academy.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_Academy.DLG_1311:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1311, () => { Next(); });
		//	break;
		//case TutoType_Academy.Select_Academy:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Academy);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_Academy.ViewAcademyPopup:
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_Academy.DLG_1321:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1321, () => { Next(); });
			break;
		case TutoType_Academy.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Academy(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Academy type = (TutoType_Academy)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_Academy.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_Academy.Select_Academy:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.Academy;
		//}
		return true;
	}
}
