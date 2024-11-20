using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_DNA
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1681 대화 </summary>
	DLG_1681,
	/// <summary> 생존자 버튼 클릭 </summary>
	Select_CharInfo_Menu,
	/// <summary> 1024 캐릭터 선택 </summary>
	Select_Char_1024,
	/// <summary> 캐릭터 정보 시작 연출동안 대기 </summary>
	ViewCharInfo,
	/// <summary> DNA 버튼 선택 </summary>
	Select_DNABtn,
	/// <summary> DNA 연출 </summary>
	ViewDNA,
	/// <summary> DNA 장착 영역 보기 </summary>
	ViewDNAArea,
	/// <summary> 1691 대화 </summary>
	DLG_1691,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_DNA()
	{
		TutoType_DNA type = GetTutoState<TutoType_DNA>();
		return false;
	}
	void PlayTuto_DNA(int no, object[] args)
	{
		TutoType_DNA type = (TutoType_DNA)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		switch (type)
		{
		case TutoType_DNA.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_DNA.DLG_1681:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1681, () => { Next(); });
			break;
		case TutoType_DNA.Select_CharInfo_Menu:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.Character);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_DNA.Select_Char_1024:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			Item_CharManagement charlist = (Item_CharManagement)args[0];
			obj = charlist.GetCharCard(1024).gameObject;
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_DNA.ViewCharInfo:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_DNA.Select_DNABtn:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(2);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_DNA.ViewDNA:
			POPUP.RemoveTutoUI();
			// 연출시간 35프레임 멈춤
			POPUP.StartTutoTimer(() => { Next(); }, 0.58f);
			break;
		case TutoType_DNA.ViewDNAArea:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			playui = POPUP.GetPopup();
			obj = ((Info_Character)playui).GetTutoPanel(3);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			POPUP.StartTutoTimer(() => { Next(); });
			break;
		case TutoType_DNA.DLG_1691:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1691, () => { Next(); });
			break;
		case TutoType_DNA.End:
			SetTutoEnd();
			// 스크롤이 꺼졌으므로 다시 켜줌
			POPUP.GetPopup().GetComponent<Info_Character>().SetUI();
			break;
		}
	}

	public bool TouchCheckLock_DNA(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_DNA type = (TutoType_DNA)m_TutoState[m_NowTuto];
		Item_CharManageCard card;
		switch (type)
		{
		case TutoType_DNA.Select_CharInfo_Menu:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.Character;
		case TutoType_DNA.Select_Char_1024:
			if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
			if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
				&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
			card = (Item_CharManageCard)args[1];
			return card?.m_Info?.m_Idx != 1024;
		case TutoType_DNA.Select_DNABtn:
			if (checktype != TutoTouchCheckType.Info_Char_Btn) return true;
			return (int)args[0] != 6;
		}
		return true;
	}
}
