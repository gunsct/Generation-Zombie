using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Making
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock1,
	/// <summary> 1481 대화 </summary>
	DLG_1481,
	/// <summary> PDA선택 </summary>
	Select_PDA,
	/// <summary> PDA 시작 연출 </summary>
	PDA_Action,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock2,
	/// <summary> 1491 대화 </summary>
	DLG_1491,
	/// <summary> 제작 버튼 선택 </summary>
	Select_Making,
	/// <summary> 제작 팝업 연출 대기 </summary>
	ViewMaking,
	/// <summary> 1501 대화 </summary>
	DLG_1501,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Making()
	{
		TutoType_Making type = GetTutoState<TutoType_Making>();
		return false;
	}
	void PlayTuto_Making(int no, object[] args)
	{
		TutoType_Making type = (TutoType_Making)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		Item_PDA_Menu pda;
		switch (type) {
		case TutoType_Making.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Making.Btn_Unlock1:
			POPUP.RemoveTutoUI();
			playui = POPUP.GetMainUI();
			((Main_Play)playui).GetMenuGroup.SetLockFX(MainMenuType.PDA);
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_Making.DLG_1481:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1481, () => { Next(); });
			break;
		case TutoType_Making.Select_PDA:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Making.PDA_Action:
		case TutoType_Making.ViewMaking:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Making.Btn_Unlock2:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			pda.SetLockFX(ContentType.Making);
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_Making.DLG_1491:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1491, () => { Next(); });
			break;
		case TutoType_Making.Select_Making:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			obj = pda.GetMainMenu(Item_PDA_Menu.State.Making);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Making.DLG_1501:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(1501, () => { Next(); });
			break;
		case TutoType_Making.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Making(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Making type = (TutoType_Making)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Making.Select_PDA:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.PDA;

		case TutoType_Making.Select_Making:
			if (checktype != TutoTouchCheckType.Play_PDA_MainMenu) return true;
			return (Item_PDA_Menu.State)args[0] != Item_PDA_Menu.State.Making;
		}
		return true;
	}
}
