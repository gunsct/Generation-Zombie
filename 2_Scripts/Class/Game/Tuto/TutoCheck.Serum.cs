using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Serum
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1661 대화 </summary>
	DLG_1661,
	/// <summary> 생존자 버튼 클릭 </summary>
	Select_CharInfo_Menu,
	/// <summary> 1024 캐릭터 선택 </summary>
	Select_Char_1024,
	/// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	ViewCharInfo,
	/// <summary> 혈청 버튼 선택 </summary>
	Select_SerumBtn,
	/// <summary> 혈청 연출 </summary>
	ViewSerum,
	/// <summary> 1661 대화 </summary>
	DLG_1671,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Serum()
	{
		TutoType_Serum type = GetTutoState<TutoType_Serum>();
		return false;
	}
	void PlayTuto_Serum(int no, object[] args)
	{
		TutoType_Serum type = (TutoType_Serum)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type)
		{
		case TutoType_Serum.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Serum.DLG_1661:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1661, () => { Next(); });
			break;
		case TutoType_Serum.Select_CharInfo_Menu:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Character);
			rtf = (RectTransform)obj.transform;
			//ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Serum.Select_Char_1024:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			Item_CharManagement charlist = (Item_CharManagement)args[0];
			obj = charlist.GetCharCard(1024).gameObject;
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Serum.ViewCharInfo:
		case TutoType_Serum.ViewSerum:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Serum.Select_SerumBtn:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Serum.DLG_1671:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1671, () => { Next(); });
			break;
		case TutoType_Serum.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Serum(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Serum type = (TutoType_Serum)m_TutoState[m_NowTuto];
		Item_CharManageCard card;
		switch (type)
		{
		case TutoType_Serum.Select_CharInfo_Menu:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.Character;
		case TutoType_Serum.Select_Char_1024:
			if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
			if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
				&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
			card = (Item_CharManageCard)args[1];
			return card?.m_Info?.m_Idx != 1024;
		case TutoType_Serum.Select_SerumBtn:
			if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
			return (int)args[0] != 8;
		}
		return true;
	}
}
