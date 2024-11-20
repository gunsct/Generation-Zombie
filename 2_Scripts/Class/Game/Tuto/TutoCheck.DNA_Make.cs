using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_DNA_Make
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 5030 대화 </summary>
	DLG_5030,
	/// <summary> PDA선택 </summary>
	Select_PDA,
	/// <summary> PDA 시작 연출 </summary>
	PDA_Action,
	/// <summary> 좀비 사육장 버튼 보여주기 </summary>
	Select_PDA_ZombieFram,
	/// <summary> 5032 대화 </summary>
	DLG_5032,
	/// <summary> 종비 생성 버튼 선택 </summary>
	Select_DNA_Making,
	/// <summary> 종비 생성 UI 보여주기 </summary>
	View_DNA_Making,
	/// <summary> 5034 대화 </summary>
	DLG_5034,
	/// <summary> 5035 대화 </summary>
	DLG_5035,
	/// <summary> 5037 대화 </summary>
	DLG_5037,
	/// <summary> 5039 대화 </summary>
	DLG_5039,
	/// <summary> 3초 대기 </summary>
	Delay_01,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_DNA_Make()
	{
		TutoType_DNA_Make type = GetTutoState<TutoType_DNA_Make>();
		return false;
	}
	void PlayTuto_DNA_Make(int no, object[] args)
	{
		TutoType_DNA_Make type = (TutoType_DNA_Make)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;

		Item_PDA_Menu pda;
		Item_PDA_ZombieFarm farm;
		Item_PDA_ZombieFarm_Main farmmain;
		DNAMaking making;
		switch (type)
		{
		case TutoType_DNA_Make.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_DNA_Make.DLG_5030:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, false);

			ui.StartDlg(5030, () => { Next(); });
			ui.UIClone();
			break;
		case TutoType_DNA_Make.Select_PDA:

			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);

			break;
		case TutoType_DNA_Make.PDA_Action:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_DNA_Make.Select_PDA_ZombieFram:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			obj = pda.GetMainMenu(Item_PDA_Menu.State.ZombieFarm);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_DNA_Make.DLG_5032:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmmain = farm.GetItem(Item_PDA_ZombieFarm.State.Main).GetComponent<Item_PDA_ZombieFarm_Main>();
			farmmain.ScrollLock(true);
			// 생성 버튼 강조
			rtf = (RectTransform)farmmain.GetTutoObj(0).transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);

			ui.StartDlg(5032, () => { Next(); });
			break;
		case TutoType_DNA_Make.Select_DNA_Making:
			ui = POPUP.ShowTutoUI();

			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmmain = farm.GetItem(Item_PDA_ZombieFarm.State.Main).GetComponent<Item_PDA_ZombieFarm_Main>();
			rtf = (RectTransform)farmmain.GetTutoObj(0).transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);

			break;
		case TutoType_DNA_Make.View_DNA_Making:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmmain = farm.GetItem(Item_PDA_ZombieFarm.State.Main).GetComponent<Item_PDA_ZombieFarm_Main>();
			farmmain.ScrollLock(false);// 스크롤 막아둔거 풀어두기
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 0.6f);
			break;
		case TutoType_DNA_Make.DLG_5034:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5034, () => { Next(); }, 0, 1);
			break;
		case TutoType_DNA_Make.DLG_5035:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			making = (DNAMaking)POPUP.GetPopup();
			rtf = (RectTransform)making.GetTutoObj(0).transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);

			ui.StartDlg(5035, () => { Next(); }, 0, 1);
			break;
		case TutoType_DNA_Make.DLG_5037:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			making = (DNAMaking)POPUP.GetPopup();
			rtf = (RectTransform)making.GetTutoObj(1).transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);

			ui.StartDlg(5037, () => { Next(); });
			break;
		case TutoType_DNA_Make.DLG_5039:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			making = (DNAMaking)POPUP.GetPopup();
			rtf = (RectTransform)making.GetTutoObj(2).transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);

			ui.StartDlg(5039, () => { Next(); });
			break;
		case TutoType_DNA_Make.Delay_01:
			POPUP.StartTutoTimer(() => { Next(); }, 3f);
			break;
		case TutoType_DNA_Make.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_DNA_Make(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_DNA_Make type = (TutoType_DNA_Make)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_DNA_Make.Select_PDA:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.PDA;
		case TutoType_DNA_Make.Select_PDA_ZombieFram:
			if (checktype != TutoTouchCheckType.Play_PDA_MainMenu) return true;
			return (Item_PDA_Menu.State)args[0] != Item_PDA_Menu.State.ZombieFarm;
		case TutoType_DNA_Make.Select_DNA_Making:
			if (checktype != TutoTouchCheckType.Item_PDA_ZombieFarm_Main) return true;
			return (int)args[0] != 0;
		}
		return true;
	}
}
