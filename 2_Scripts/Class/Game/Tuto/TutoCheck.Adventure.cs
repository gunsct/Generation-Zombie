using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Adventure
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1541 대화 </summary>
	DLG_1541,
	/// <summary> PDA선택 </summary>
	Select_PDA,
	/// <summary> PDA 시작 연출 </summary>
	PDA_Action,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock,
	/// <summary> 1551 대화 </summary>
	DLG_1551,
	/// <summary> 탐험 버튼 선택 </summary>
	Select_Adventure,
	/// <summary> 제작 팝업 연출 대기 </summary>
	ViewAdventure,
	/// <summary> 1561 대화 </summary>
	DLG_1561,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Adventure()
	{
		TutoType_Adventure type = GetTutoState<TutoType_Adventure>();
		return false;
	}
	void PlayTuto_Adventure(int no, object[] args)
	{
		TutoType_Adventure type = (TutoType_Adventure)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		Item_PDA_Menu pda;
		switch (type)
		{
		case TutoType_Adventure.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Adventure.DLG_1541:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1541, () => { Next(); });
			break;
		case TutoType_Adventure.Select_PDA:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Adventure.PDA_Action:
		case TutoType_Adventure.ViewAdventure:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Adventure.Btn_Unlock:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			pda.SetLockFX(ContentType.Explorer);
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_Adventure.DLG_1551:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1551, () => { Next(); });
			break;
		case TutoType_Adventure.Select_Adventure:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			obj = pda.GetMainMenu(Item_PDA_Menu.State.Adventure);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			ui.StartDlg(-1);
			break;
		case TutoType_Adventure.DLG_1561:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1561, () => { Next(); });
			break;
		case TutoType_Adventure.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Adventure(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Adventure type = (TutoType_Adventure)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Adventure.Select_PDA:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.PDA;

		case TutoType_Adventure.Select_Adventure:
			if (checktype != TutoTouchCheckType.Play_PDA_MainMenu) return true;
			return (Item_PDA_Menu.State)args[0] != Item_PDA_Menu.State.Adventure;
		}
		return true;
	}
}
