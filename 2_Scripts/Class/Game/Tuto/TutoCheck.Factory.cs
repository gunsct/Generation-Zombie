using UnityEngine;

public enum TutoType_Factory
{
	///// <summary> 시작 시나리오 시작 토크 </summary>
	//StartTalk = 1,
	///// <summary> 버튼 언락 연출 </summary>
	//Btn_Unlock,
	///// <summary> 1361 대화 </summary>
	//DLG_1361,
	///// <summary> 다운타운 버튼 클릭 </summary>
	//Select_Dungeon_Menu,
	///// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	//ViewDungeon,
	///// <summary> 댄 공장 선택 </summary>
	//Select_Factory,
	/// <summary> 댄 공장 팝업 </summary>
	ViewFactoryPopup = 1,
	/// <summary> 1381 대화 </summary>
	DLG_1381,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Factory()
	{
		TutoType_Factory type = GetTutoState<TutoType_Factory>();
		return false;
	}
	void PlayTuto_Factory(int no, object[] args)
	{
		TutoType_Factory type = (TutoType_Factory)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
		//case TutoType_Factory.StartTalk:
		//	POPUP.ShowTutoStartAction(1901, () => {
		//		Next();
		//	});
		//	break;
		//	case TutoType_Factory.Btn_Unlock:
		//		POPUP.RemoveTutoUI();
		//		playui = POPUP.GetMainUI();
		//		((Main_Play)playui).GetMenuGroup.SetLockFX(MainMenuType.Dungeon);
		//		POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
		//		break;
		//	case TutoType_Factory.DLG_1361:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	ui.SetFocus(0, Vector3.zero, 0, 0);
		//	ui.StartDlg(1361, () => { Next(); });
		//	break;
		//case TutoType_Factory.Select_Dungeon_Menu:
		//	ui = POPUP.ShowTutoUI();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Dungeon);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, obj.transform, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	ui.UIClone();
		//	break;
		//case TutoType_Factory.ViewDungeon:
		//	POPUP.RemoveTutoUI();
		//	Next();
		//	break;
		//case TutoType_Factory.Select_Factory:
		//	ui = POPUP.ShowTutoUI();
		//	ui.RemoveClone();
		//	playui = POPUP.GetMainUI();
		//	obj = ((Main_Play)playui).GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().GetBtn(StageContentType.Factory);
		//	rtf = (RectTransform)obj.transform;
		//	ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width, rtf.rect.height, true);
		//	ui.StartDlg(-1);
		//	break;
		case TutoType_Factory.ViewFactoryPopup:
			POPUP.RemoveTutoUI();
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(true);
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_Factory.DLG_1381:
			POPUP.GetPopup().GetComponent<Dungeon_Info>().ScrollLock(false);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1381, () => { Next(); });
			break;
		case TutoType_Factory.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Factory(TutoTouchCheckType checktype, object[] args)
	{
		//TutoType_Factory type = (TutoType_Factory)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		//case TutoType_Factory.Select_Dungeon_Menu:
		//	if (checktype != TutoTouchCheckType.Play_Menu) return true;
		//	return (MainMenuType)args[0] != MainMenuType.Dungeon;
		//case TutoType_Factory.Select_Factory:
		//	if (checktype != TutoTouchCheckType.Dungeon_Menu) return true;
		//	return (StageContentType)args[0] != StageContentType.Factory;
		//}
		return true;
	}
}
