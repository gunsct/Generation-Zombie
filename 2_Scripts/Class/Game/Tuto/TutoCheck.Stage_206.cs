using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_206
{
	StageStart_Loading = 1,
	Focus_Clock_1,
	Focus_Line_0_1_1,
	Focus_Clock_2,
	Focus_MissionGuide_1,
	Focus_Merge_Sniping,
	Sniping_Action_Start,
	SelectSnipingTarget,
	Sniping_Action_End,
	DL_136,
	Focus_Line_0_1_2,
	FREE_Play_1,
	Focus_MissionGuide_2,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_206()
	{
		TutoType_Stage_206 type = GetTutoState<TutoType_Stage_206>();
		//switch (type) {
		//	case TutoType_Stage_208.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_206(int no, object[] args)
	{
		TutoType_Stage_206 type = (TutoType_Stage_206)no;
		TutoUI ui;
		RectTransform rtf;
		Vector3 pos, temp;
		Main_Stage stageui;
		Item_Stage card;

		switch (type) {
			case TutoType_Stage_206.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_206.Focus_Clock_1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetClockObj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(128, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_206.Focus_Line_0_1_1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_206.Focus_Clock_2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetClockObj.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(131, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_206.Focus_MissionGuide_1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(134, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_206.Focus_Merge_Sniping:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetFirstMakeCard.transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				break;
			case TutoType_Stage_206.Sniping_Action_Start:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_206.SelectSnipingTarget:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[2][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_206.Sniping_Action_End:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_206.DL_136:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(136, () => { Next(); });
				break;
			case TutoType_Stage_206.Focus_Line_0_1_2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_206.FREE_Play_1:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_206.Focus_MissionGuide_2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(137, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_206.End:
				SetTutoEnd();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_206(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_206 type = GetTutoState<TutoType_Stage_206>();
		Item_Stage stagecard;
		switch (type) {
			case TutoType_Stage_206.StageStart_Loading:
			case TutoType_Stage_206.FREE_Play_1:
				return false;
			case TutoType_Stage_206.Focus_Line_0_1_1:
			case TutoType_Stage_206.Focus_Line_0_1_2:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Pos != 1;
			case TutoType_Stage_206.Focus_Merge_Sniping:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.Sniping;
			case TutoType_Stage_206.SelectSnipingTarget:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 2 || stagecard.m_Pos != 2;
			case TutoType_Stage_206.Sniping_Action_End:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
		}
		return true;
	}
}
