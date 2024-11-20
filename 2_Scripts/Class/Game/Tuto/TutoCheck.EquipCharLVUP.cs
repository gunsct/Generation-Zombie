using UnityEngine;

public enum TutoType_EquipCharLVUP
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock,
	/// <summary> 1641 대화 </summary>
	DLG_1641,
	/// <summary> 생존자 버튼 클릭 </summary>
	Select_CharInfo_Menu,
	/// <summary> 1024 캐릭터 선택 </summary>
	Select_Char_1021,
	/// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	ViewCharInfo,
	/// <summary> 장비 장착 영역 보기 </summary>
	ViewEquipArea,
	/// <summary> 1651 대화 </summary>
	DLG_1651,
	/// <summary> 장비 자동 장착 </summary>
	Focus_AutoEquip,
	/// <summary> 1921 대화 </summary>
	DLG_1921,
	/// <summary> 무기칸 유도 </summary>
	Focus_Weapon,
	/// <summary> 무기강화 유도 </summary>
	Focus_EquipUpgrade,
	/// <summary> 1931 대화 </summary>
	DLG_1931,
	/// <summary> 강화 버튼 그룹 유도 </summary>
	Focus_UpgradeGroup,
	/// <summary> 1941 대화 </summary>
	DLG_1941,
	/// <summary> 강화 팝업 나가기 유도 </summary>
	Focus_EquipUpgradeExit,
	/// <summary> 1654 대화 </summary>
	DLG_1654,
	/// <summary> 캐릭터 레벨업 버튼 그룹 유도 </summary>
	Focus_CharLVUPGroup,
	/// <summary> 1655 대화 </summary>
	DLG_1655,
	/// <summary> 캐릭터 레벨 유도 </summary>
	Focus_CharLV,
	/// <summary> 1656 대화 </summary>
	DLG_1656,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_EquipCharLVUP()
	{
		TutoType_EquipCharLVUP type = GetTutoState<TutoType_EquipCharLVUP>();
		return false;
	}
	void PlayTuto_EquipCharLVUP(int no, object[] args)
	{
		TutoType_EquipCharLVUP type = (TutoType_EquipCharLVUP)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type) {
		case TutoType_EquipCharLVUP.StartTalk:
			USERINFO.m_Chars.Find(o => o.m_Idx == 1021).TutoUnEquipAll();
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_EquipCharLVUP.Btn_Unlock:
			POPUP.RemoveTutoUI();
			playui = POPUP.GetMainUI();
			((Main_Play)playui).GetMenuGroup.SetLockFX(MainMenuType.Character);
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_EquipCharLVUP.DLG_1641:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1641, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Select_CharInfo_Menu:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Character);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_EquipCharLVUP.Select_Char_1021:
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
		case TutoType_EquipCharLVUP.ViewCharInfo:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_EquipCharLVUP.ViewEquipArea:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			POPUP.StartTutoTimer(() => { Next(); }, 2);
			break;
		case TutoType_EquipCharLVUP.DLG_1651:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1651, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Focus_AutoEquip:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(4);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			break;
		case TutoType_EquipCharLVUP.DLG_1921:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1921, () => { Next(); }, 0, 2);
			break;
		case TutoType_EquipCharLVUP.Focus_Weapon:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(5);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			break;
		case TutoType_EquipCharLVUP.Focus_EquipUpgrade:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Item_Equip)playui).GetBtn(Info_Item_Equip.BtnName.LVUp);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			break;
		case TutoType_EquipCharLVUP.DLG_1931:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1931, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Focus_UpgradeGroup:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((EquipLevelUp)playui).GetTutoFocus(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.rect.width * rtf.localScale.x, rtf.rect.height * rtf.localScale.y, false);
			POPUP.StartTutoTimer(() => { Next(); }, 3f);
			break;
		case TutoType_EquipCharLVUP.DLG_1941:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.StartDlg(1941, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Focus_EquipUpgradeExit:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			playui = POPUP.GetPopup();
			obj = ((EquipLevelUp)playui).GetTutoFocus(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			break;
		case TutoType_EquipCharLVUP.DLG_1654:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1654, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Focus_CharLVUPGroup:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(7);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			POPUP.StartTutoTimer(() => { Next(); }, 2f);
			break;
		case TutoType_EquipCharLVUP.DLG_1655:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1655, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.Focus_CharLV:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(6);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			POPUP.StartTutoTimer(() => { Next(); }, 2f);
			break;
		case TutoType_EquipCharLVUP.DLG_1656:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1656, () => { Next(); });
			break;
		case TutoType_EquipCharLVUP.End:
			SetTutoEnd();
			// 스크롤이 꺼졌으므로 다시 켜줌
			((Info_Character)POPUP.GetPopup()).SetUI();
			break;
		}
	}

	public bool TouchCheckLock_EquipCharLVUP(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_EquipCharLVUP type = (TutoType_EquipCharLVUP)m_TutoState[m_NowTuto];
		Item_CharManageCard card;
		switch (type)
		{
		case TutoType_EquipCharLVUP.Select_CharInfo_Menu:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.Character;
		case TutoType_EquipCharLVUP.Select_Char_1021:
			if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
			if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
				&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
			card = (Item_CharManageCard)args[1];
			return card?.m_Info?.m_Idx != 1021;
		case TutoType_EquipCharLVUP.Focus_AutoEquip:
			if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
			return (int)args[0] != 5;
		case TutoType_EquipCharLVUP.Focus_Weapon:
			if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
			if((int)args[0] == 4 && (int)args[1] == 0) return false;
			return true;
		case TutoType_EquipCharLVUP.Focus_EquipUpgrade:
			if (checktype != TutoTouchCheckType.Info_Item_Equip) return true;
			return (int)args[0] != 0;
		case TutoType_EquipCharLVUP.Focus_EquipUpgradeExit:
			if (checktype != TutoTouchCheckType.PopupOnClose) return true;
			return (int)args[0] != 0;
		}

		return true;
	}
}
