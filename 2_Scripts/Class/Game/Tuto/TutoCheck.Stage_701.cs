using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_701
{
	DeckSorting = 1,
	Focus_DeckStageInfo,
	DL_2501,
	Focus_CharListOnBtn,
	DL_2511,
	//Focus_GuardChars,
	Select_Char_1024,
	Select_Char_1036,
	Focus_Synergy,
	DL_2521,
	Delay_DeckScroll,
	StageStart_Loading,
	DL_2101,
	//FREE_Play_1,
	DL_2111,
	SelectStageCard_1,
	Delay_Merge,
	DL_2121,
	Focus_MergeLight,
	Light_Action_Start,
	SelectLightTarget,
	Light_Action_End,
	DL_2131,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_701()
	{
		TutoType_Stage_701 type = GetTutoState<TutoType_Stage_701>();

		//switch (type) {
		//	case TutoType_Stage_701.FREE_Play_1:
		//	//case TutoType_Stage_701.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_701(int no, object[] args)
	{
		TutoType_Stage_701 type = (TutoType_Stage_701)no;
		TutoUI ui;
		GameObject obj;
		RectTransform rtf;
		Item_Stage card;
		Vector3 pos, temp;
		DeckSetting decksetting;

		switch (type) {
			case TutoType_Stage_701.DeckSorting:
				POPUP.RemoveTutoUI();
				POPUP.StartTutoTimer(() => { Next(); }, 1f);
				break;
			case TutoType_Stage_701.Focus_DeckStageInfo:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetStageInfo.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_701.DL_2501:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2501, () => { Next(); });
				break;
			case TutoType_Stage_701.Focus_CharListOnBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetCharListOnBtn.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_701.DL_2511:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2511, () => { Next(); });
				break;
			//case TutoType_Stage_701.Focus_GuardChars:
			case TutoType_Stage_701.Select_Char_1024:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				obj = decksetting.GetCharCard(1024);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_701.Select_Char_1036:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				obj = decksetting.GetCharCard(1036);
				rtf = (RectTransform)obj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_701.Focus_Synergy:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetSynergy.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_701.DL_2521:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2521, () => { Next(); });
				break;
			case TutoType_Stage_701.Delay_DeckScroll:
				POPUP.RemoveTutoUI();
				POPUP.StartTutoTimer(() => { Next(); }, 0.2f);
				break;
			case TutoType_Stage_701.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_701.DL_2101:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2101, () => { Next(); });
				break;
			case TutoType_Stage_701.DL_2111:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2111, () => { Next(); });
				break;
			case TutoType_Stage_701.SelectStageCard_1:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, true);
				Start_ClickDelay();
				break;
			case TutoType_Stage_701.Delay_Merge:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_701.DL_2121:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2121, () => { Next(); });
				break;
			case TutoType_Stage_701.Focus_MergeLight:
				Main_Stage mainui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				rtf = (RectTransform)mainui.GetMaking.GetFirstMakeCard.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_701.Light_Action_Start:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_701.SelectLightTarget:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 5, temp.y - pos.y, true);
				break;
			case TutoType_Stage_701.Light_Action_End:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_701.DL_2131:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2131, () => { Next(); });
				break;
			//case TutoType_Stage_701.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_701.End:
				SetTutoEnd();
				Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_701(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_701 type = GetTutoState<TutoType_Stage_701>();
		Item_Stage stagecard;
		Item_CharManageCard deckcharcard;

		switch (type) {
			case TutoType_Stage_701.DeckSorting:
			//case TutoType_Stage_701.Delay_Clear: 
				return false;
			case TutoType_Stage_701.Focus_CharListOnBtn:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				return (int)args[0] != 0;
			case TutoType_Stage_701.Select_Char_1024:
			case TutoType_Stage_701.Select_Char_1036:
				if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
				if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
					&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
				deckcharcard = (Item_CharManageCard)args[1];
				return deckcharcard?.m_Info?.m_Idx != (type == TutoType_Stage_701.Select_Char_1024 ? 1024 : 1036);
			case TutoType_Stage_701.DL_2521:
			case TutoType_Stage_701.Delay_DeckScroll:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return false;
				return (int)args[0] == 2;
			case TutoType_Stage_701.StageStart_Loading:
				if (checktype == TutoTouchCheckType.PopupOnClose && (int)args[0] == 0 && (PopupName)args[1] == PopupName.DeckSetting) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(262));
					return true;
				}
				return false;
			case TutoType_Stage_701.SelectStageCard_1:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0;
			case TutoType_Stage_701.Focus_MergeLight:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.FlashLight;
			case TutoType_Stage_701.SelectLightTarget:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 1;
			case TutoType_Stage_701.Light_Action_End:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
		}
		return true;
	}
}
