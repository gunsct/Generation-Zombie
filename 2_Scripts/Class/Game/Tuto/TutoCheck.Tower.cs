using UnityEngine;

public enum TutoType_Tower
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 1391 대화 </summary>
	//DLG_1391,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 1401 대화 </summary>
	//DLG_1401,
	///// <summary> 로지은행 선택 </summary>
	//Select_Tower,
	/// <summary> 로지은행 팝업 </summary>
	ViewTowerPopup = 1,
	/// <summary> 1411 대화 </summary>
	DLG_1411,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Tower()
	{
		TutoType_Tower type = GetTutoState<TutoType_Tower>();
		return false;
	}

	void PlayTuto_Tower(int no, object[] args)
	{
		TutoType_Tower type = (TutoType_Tower)no;
		TutoUI ui;
		//PopupBase playui;
		//GameObject obj;
		//RectTransform rtf;
		switch (type)
		{
		//case TutoType_Tower.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_Tower.DLG_1391:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1391, () => { Next(); });
		//	break;
		//case TutoType_Tower.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_Tower.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_Tower.DLG_1401:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1401, () => { Next(); });
		//	break;
		//case TutoType_Tower.Select_Tower:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Tower);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_Tower.ViewTowerPopup:
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_Tower.DLG_1411:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1411, () => { Next(); });
			break;
		case TutoType_Tower.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Tower(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Tower type = (TutoType_Tower)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_Tower.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_Tower.Select_Tower:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.Tower;
		//}
		return true;
	}
}
