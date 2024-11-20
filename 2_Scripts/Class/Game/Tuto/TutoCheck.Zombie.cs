using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Zombie
{
	/// <summary> 시작 시나리오 시작 토크 </summary>
	StartTalk = 1,
	/// <summary> 5020 대화 </summary>
	DLG_5020,
	/// <summary> PDA선택 </summary>
	Select_PDA,
	/// <summary> PDA 시작 연출 </summary>
	PDA_Action,
	/// <summary> 버튼 언락 연출 </summary>
	Btn_Unlock,
	/// <summary> 좀비 사육장 버튼 보여주기 </summary>
	Select_PDA_ZombieFram,
	/// <summary> 5022 대화 </summary>
	DLG_5022,
	/// <summary> 좀비 배치 페이지 유도 </summary>
	Select_Zombie_Room,
	/// <summary> 5024 대화 </summary>
	DLG_5024,
	/// <summary> 5025 대화 </summary>
	DLG_5025,
	/// <summary> 5028 대화 </summary>
	DLG_5028,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Zombie()
	{
		TutoType_Zombie type = GetTutoState<TutoType_Zombie>();
		return false;
	}
	void PlayTuto_Zombie(int no, object[] args)
	{
		TutoType_Zombie type = (TutoType_Zombie)no;
		TutoUI ui;
		PopupBase playui;
		GameObject obj;
		RectTransform rtf;
		Item_PDA_Menu pda;
		Item_PDA_ZombieFarm farm;
		Item_PDA_ZombieFarm_Main farmmain;
		Item_PDA_ZombieFarm_SetRoom farmsetroom;
		Item_ZombieFarm_Catched_Element roomitem;
		switch (type)
		{
		case TutoType_Zombie.StartTalk:
			POPUP.ShowTutoStartAction(1901, () => {
				Next();
			});
			break;
		case TutoType_Zombie.DLG_5020:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5020, () => { Next(); });
			break;
		case TutoType_Zombie.Select_PDA:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			obj = ((Main_Play)playui).GetMenuBtn(MainMenuType.PDA);
			ui.SetFocus(0, obj.transform, Vector3.zero, 0, 0, true);
			ui.StartDlg(-1);
			ui.UIClone();
			break;
		case TutoType_Zombie.PDA_Action:
			POPUP.RemoveTutoUI();
			break;
		case TutoType_Zombie.Btn_Unlock:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			pda.SetLockFX(ContentType.ZombieFarm);
			POPUP.RemoveTutoUI();
			POPUP.StartTutoTimer(() => { Next(); }, 2.2f);
			break;
		case TutoType_Zombie.Select_PDA_ZombieFram:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			obj = pda.GetMainMenu(Item_PDA_Menu.State.ZombieFarm);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Zombie.DLG_5022:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.SetFocus(0, Vector3.zero, 0, 0);
			ui.StartDlg(5022, () => { Next(); });
			break;
		case TutoType_Zombie.Select_Zombie_Room:
			ui = POPUP.ShowTutoUI();
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmmain = farm.GetItem(Item_PDA_ZombieFarm.State.Main).GetComponent<Item_PDA_ZombieFarm_Main>();
			farmmain.ScrollLock(true);
			var farmitem = farmmain.GetListItem(0);
			obj = farmitem.GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			ui.StartDlg(-1);
			break;
		case TutoType_Zombie.DLG_5024:

			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmmain = farm.GetItem(Item_PDA_ZombieFarm.State.Main).GetComponent<Item_PDA_ZombieFarm_Main>();
			farmmain.ScrollLock(false);

			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			farmsetroom = farm.GetItem(Item_PDA_ZombieFarm.State.SetRoom).GetComponent<Item_PDA_ZombieFarm_SetRoom>();
			farmsetroom.ScrollLock(true);
			obj = farmsetroom.GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			ui.StartDlg(5024, () => { Next(); });
			break;
		case TutoType_Zombie.DLG_5025:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmsetroom = farm.GetItem(Item_PDA_ZombieFarm.State.SetRoom).GetComponent<Item_PDA_ZombieFarm_SetRoom>();
			roomitem = farmsetroom.GetZombieListItem(0);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			obj = roomitem.GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			ui.StartDlg(5025, () => { Next(); });
			break;
		case TutoType_Zombie.DLG_5028:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmsetroom = farm.GetItem(Item_PDA_ZombieFarm.State.SetRoom).GetComponent<Item_PDA_ZombieFarm_SetRoom>();
			roomitem = farmsetroom.GetZombieListItem(0);
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();

			obj = roomitem.GetTutoObj(1);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
			ui.StartDlg(5028, () => { Next(); });
			break;
		case TutoType_Zombie.End:
			playui = POPUP.GetMainUI();
			pda = ((Main_Play)playui).GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
			farm = pda.GetMenuItem().GetComponent<Item_PDA_ZombieFarm>();
			farmsetroom = farm.GetItem(Item_PDA_ZombieFarm.State.SetRoom).GetComponent<Item_PDA_ZombieFarm_SetRoom>();
			farmsetroom.ScrollLock(false);

			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Zombie(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Zombie type = (TutoType_Zombie)m_TutoState[m_NowTuto];
		switch (type)
		{
		case TutoType_Zombie.Select_PDA:
			if (checktype != TutoTouchCheckType.Play_Menu) return true;
			return (MainMenuType)args[0] != MainMenuType.PDA;
		case TutoType_Zombie.Select_PDA_ZombieFram:
			if (checktype != TutoTouchCheckType.Play_PDA_MainMenu) return true;
			return (Item_PDA_Menu.State)args[0] != Item_PDA_Menu.State.ZombieFarm;
		case TutoType_Zombie.Select_Zombie_Room:
			if (checktype != TutoTouchCheckType.Item_Zp_Element_Btn) return true;
			if ((int)args[0] != 1) return true;
			return (int)args[1] != 0;
		}
		return true;
	}
}
