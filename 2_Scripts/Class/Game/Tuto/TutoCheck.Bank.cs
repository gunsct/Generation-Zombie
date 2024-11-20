using UnityEngine;

public enum TutoType_Bank
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 1331 대화 </summary>
	//DLG_1331,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 1341 대화 </summary>
	//DLG_1341,
	///// <summary> 로지은행 선택 </summary>
	//Select_Bank,
	/// <summary> 로지은행 팝업 </summary>
	ViewBankPopup = 1,
	/// <summary> 1351 대화 </summary>
	DLG_1351,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Bank()
	{
		TutoType_Bank type = GetTutoState<TutoType_Bank>();
		return false;
	}

	void PlayTuto_Bank(int no, object[] args)
	{
		TutoType_Bank type = (TutoType_Bank)no;
		TutoUI ui;
		//PopupBase playui;
		//GameObject obj;
		//RectTransform rtf;
		switch (type)
		{
		//case TutoType_Bank.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//case TutoType_Bank.DLG_1331:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1331, () => { Next(); });
		//	break;
		//case TutoType_Bank.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_Bank.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_Bank.DLG_1341:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1341, () => { Next(); });
		//	break;
		//case TutoType_Bank.Select_Bank:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Bank);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_Bank.ViewBankPopup:
			POPUP.RemoveTutoUI();
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(true);
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_Bank.DLG_1351:
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(false);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1351, () => { Next(); });
			break;
		case TutoType_Bank.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Bank(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Bank type = (TutoType_Bank)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		////case TutoType_Bank.Select_Dungeon_Menu:
		////	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		////	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		////case TutoType_Bank.Select_Bank:
		////	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		////	return (StageContentType)args[0] != StageContentType.Bank;
		//}
		return true;
	}
}
