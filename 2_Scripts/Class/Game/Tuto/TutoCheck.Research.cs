using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Research
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 1571 대화 </summary>
	DLG_1571,
	/// <summary> PDA선택 </summary>
	Select_PDA,
	/// <summary> PDA 시작 연출 </summary>
	PDA_Action,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock,
	/// <summary> 1581 대화 </summary>
	DLG_1581,
	/// <summary> 연구 버튼 선택 </summary>
	Select_PDA_ResearchBtn,
	/// <summary> 연구 팝업 연출 대기 </summary>
	ViewResearch,
	/// <summary> 연구 트리 선택 </summary>
	Select_ResearchTree,
	/// <summary> 연구 트리 연출 대기 </summary>
	ViewResearchTree,
	/// <summary> 1591 대화 </summary>
	DLG_1591,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Research()
	{
		TutoType_Research type = GetTutoState<TutoType_Research>();
		return false;
	}
	void PlayTuto_Research(int no, object[] args)
	{
		TutoType_Research type = (TutoType_Research)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		Item_PDA_Menu pda;
		switch (type)
		{
		case TutoType_Research.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Research.DLG_1571:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1571, () => { Next(); });
			break;
		case TutoType_Research.Select_PDA:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Research.PDA_Action:
		case TutoType_Research.ViewResearch:
		case TutoType_Research.ViewResearchTree:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Research.Btn_Unlock:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			pda.SetLockFX(ContentType.Research);
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_Research.DLG_1581:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1581, () => { Next(); });
			break;
		case TutoType_Research.Select_PDA_ResearchBtn:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			obj = pda.GetMainMenu(Item_PDA_Menu.State.Research);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Research.Select_ResearchTree:
			ui = POPUP.ShowTutoUI();
			obj = ((Item_PDA_Research_Menu)args[0]).GetBtn(ResearchType.Research);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Research.DLG_1591:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1591, () => { Next(); });
			break;
		case TutoType_Research.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Research(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Research type = (TutoType_Research)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Research.Select_PDA:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.PDA;

		case TutoType_Research.Select_PDA_ResearchBtn:
			if (checktype != TutoTouchCheckType.Play_PDA_MainMenu) return true;
			return (Item_PDA_Menu.State)args[0] != Item_PDA_Menu.State.Research;

		case TutoType_Research.Select_ResearchTree:
			if (checktype != TutoTouchCheckType.Play_PDA_Research_Menu) return true;
			return (ResearchType)args[0] != ResearchType.Research;
		}
		return true;
	}
}
