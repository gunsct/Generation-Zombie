using UnityEngine;

public enum TutoType_Cemetery
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 1421 대화 </summary>
	//DLG_1421,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 1431 대화 </summary>
	//DLG_1431,
	///// <summary> 댄 공장 선택 </summary>
	//Select_Cemetery,
	/// <summary> 댄 공장 팝업 </summary>
	ViewCemeteryPopup = 1,
	/// <summary> 1441 대화 </summary>
	DLG_1441,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Cemetery()
	{
		TutoType_Cemetery type = GetTutoState<TutoType_Cemetery>();
		return false;
	}
	void PlayTuto_Cemetery(int no, object[] args)
	{
		TutoType_Cemetery type = (TutoType_Cemetery)no;
		TutoUI ui;
		//PopupBase playui;
		//GameObject obj;
		//RectTransform rtf;
		switch (type)
		{
		//case TutoType_Cemetery.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_Cemetery.DLG_1421:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1421, () => { Next(); });
		//	break;
		//case TutoType_Cemetery.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_Cemetery.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_Cemetery.DLG_1431:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1431, () => { Next(); });
		//	break;
		//case TutoType_Cemetery.Select_Cemetery:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Cemetery);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_Cemetery.ViewCemeteryPopup:
			POPUP.RemoveTutoUI();
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(true);
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_Cemetery.DLG_1441:
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(false);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1441, () => { Next(); });
			break;
		case TutoType_Cemetery.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Cemetery(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Cemetery type = (TutoType_Cemetery)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_Cemetery.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_Cemetery.Select_Cemetery:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.Cemetery;
		//}
		return true;
	}
}
