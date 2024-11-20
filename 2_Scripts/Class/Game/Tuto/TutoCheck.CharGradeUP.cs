using UnityEngine;

public enum TutoType_CharGradeUP
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 4001 대화 </summary>
	DLG_4601,
	/// <summary> 생존자 버튼 클릭 </summary>
	Select_CharInfo_Menu,
	/// <summary> 1024 캐릭터 선택 </summary>
	Select_Char_1021,
	/// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	ViewCharInfo,
	Focus_GradeUPMenu,
	DLG_4611,
	Focus_GradeUPBtn,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_CharGradeUP()
	{
		TutoType_CharGradeUP type = GetTutoState<TutoType_CharGradeUP>();
		return false;
	}
	void PlayTuto_CharGradeUP(int no, object[] args)
	{
		TutoType_CharGradeUP type = (TutoType_CharGradeUP)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type)
		{
		case TutoType_CharGradeUP.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_CharGradeUP.DLG_4601:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(4601, () => { Next(); });
			break;
		case TutoType_CharGradeUP.Select_CharInfo_Menu:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Character);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_CharGradeUP.Select_Char_1021:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			Item_CharManagement charlist = (Item_CharManagement)args[0];
			obj = charlist.GetCharCard(1021).gameObject;
			charlist.CharScrolling(obj.transform);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_CharGradeUP.ViewCharInfo:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_CharGradeUP.Focus_GradeUPMenu:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(8);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			break;
		case TutoType_CharGradeUP.DLG_4611:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(4611, () => { Next(); });
			break;
		case TutoType_CharGradeUP.Focus_GradeUPBtn:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((CharacterRankUP)playui).GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			POPUP.StartTutoTimer(() => { Next(); }, 2);
			break;
		case TutoType_CharGradeUP.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_CharGradeUP(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_CharGradeUP type = (TutoType_CharGradeUP)m_TutoState[m_NowTuto];
		Item_CharManageCard card;
		switch (type)
		{
		case TutoType_CharGradeUP.Select_CharInfo_Menu:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.Character;
		case TutoType_CharGradeUP.Select_Char_1021:
			if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
			if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
				&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
			card = (Item_CharManageCard)args[1];
			return card?.m_Info?.m_Idx != 1021;
		case TutoType_CharGradeUP.Focus_GradeUPMenu:
			if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
			return (int)args[0] != 3;



		//case TutoType_CharGradeUP.Focus_AutoEquip:
		//	if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
		//	return (int)args[0] != 5;
		//case TutoType_CharGradeUP.Focus_Weapon:
		//	if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
		//	if((int)args[0] == 4 && (int)args[1] == 0) return false;
		//	return true;
		//case TutoType_CharGradeUP.Focus_EquipUpgrade:
		//	if (checktype != TutoTouchCheckType.Info_Item_Equip) return true;
		//	return (int)args[0] != 0;
		//case TutoType_CharGradeUP.Focus_EquipUpgradeExit:
		//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
		//	return (int)args[0] != 0;
		}

		return true;
	}
}
