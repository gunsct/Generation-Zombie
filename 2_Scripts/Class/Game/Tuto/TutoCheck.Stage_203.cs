using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_203
{
	StageStart_Loading = 1,
	//Decksetting_Delay = 1,
	//DL_116,
	//Focus_GoStageBtn,
	//StageStart_Loading,
	//FREE_Play_1,
	//DL_117,
	//Select_Char_1024,
	//Focus_First_Line,
	//Char_1024_SkillEnd,
	//DL_120,
	DL_1731,
	Focus_AccBtn,
	DL_1736,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_203()
	{
		TutoType_Stage_203 type = GetTutoState<TutoType_Stage_203>();
		//switch (type) {
		//	case TutoType_Stage_203.Delay_Clear:
		//		return true;
		//}
		return true;
	}
	void PlayTuto_Stage_203(int no, object[] args)
	{
		TutoType_Stage_203 type = (TutoType_Stage_203)no;
		TutoUI ui;
		RectTransform rtf;
		//Item_Stage card;
		//Vector3 pos, temp;
		//Item_Stage_Char charcard;
		//DeckSetting decksetting;
		//Main_Stage stageui;

		switch (type) {

			case TutoType_Stage_203.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			//case TutoType_Stage_203.Decksetting_Delay:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_203.DL_116:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(116, () => { Next(); });
			//	break;
			//case TutoType_Stage_203.Focus_GoStageBtn:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
			//	rtf = (RectTransform)decksetting.GetStartBtn().transform;
			//	ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
			//	break;
			//case TutoType_Stage_203.StageStart_Loading:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_203.FREE_Play_1:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_203.DL_117:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(117, () => { Next(); });
			//	break;
			//case TutoType_Stage_203.Select_Char_1024:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	charcard = STAGE.m_Chars[0];
			//	pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetAPGauge.TuToApGaugeFullAction(0.5f);
			//	Start_ClickDelay();
			//	break;
			//case TutoType_Stage_203.Focus_First_Line:
			//	ui = POPUP.ShowTutoUI();
			//	ui.StartDlg(-1);
			//	card = STAGE.m_ViewCard[0][1];
			//	pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//	ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3f, temp.y - pos.y, true);
			//	break;
			//case TutoType_Stage_203.Char_1024_SkillEnd:
			//	POPUP.RemoveTutoUI();
			//	break;
			//case TutoType_Stage_203.DL_120:
			//	ui = POPUP.ShowTutoUI();
			//	ui.RemoveClone();
			//	ui.SetFocus(0, Vector3.zero, 0, 0);
			//	ui.StartDlg(120, () => { Next(); });
			//	stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
			//	stageui.GetAPGauge.TuToApGaugeFullAction(1f);
			//	charcard = STAGE.m_Chars[0];
			//	charcard.SkillColoTimeInit();
			//	break;
			case TutoType_Stage_203.DL_1731:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(1731, () => { Next(); });
				break;
			case TutoType_Stage_203.Focus_AccBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				rtf = (RectTransform)POPUP.GetMainUI().GetComponent<Main_Stage>().GetAccbtn().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_Stage_203.DL_1736:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(1736, () => { Next(); });
				break;
			//case TutoType_Stage_203.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_203.End:
				SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_Stage_203(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_203 type = GetTutoState<TutoType_Stage_203>();
		//Item_Stage_Char charcard;
		//Item_Stage stagecard;
		switch (type) {
			case TutoType_Stage_203.StageStart_Loading: return false;
			//case TutoType_Stage_203.Focus_GoStageBtn:
			//	if (checktype != TutoTouchCheckType.PopupOnClose) return true;
			//	return (int)args[0] == 0;
			//case TutoType_Stage_203.Decksetting_Delay:
			//case TutoType_Stage_203.StageStart_Loading:
			//case TutoType_Stage_203.FREE_Play_1:
			//	//case TutoType_Stage_103.Delay_Clear:
			//	//case TutoType_Stage_103.GetMat_Delay:
			//	return false;
			//case TutoType_Stage_203.Select_Char_1024:
			//	if (checktype != TutoTouchCheckType.StageCard_Char) return true;
			//	charcard = (Item_Stage_Char)args[0];
			//	return charcard.m_Info != null && charcard.m_Info.m_Idx != 1024;
			//case TutoType_Stage_203.Focus_First_Line:
			//	if (checktype != TutoTouchCheckType.StageCard) return true;
			//	stagecard = (Item_Stage)args[0];
			//	return stagecard.m_Line != 0;
			//case TutoType_Stage_203.Char_1024_SkillEnd:
			//	if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
			//	if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
			//	return false;
			case TutoType_Stage_203.Focus_AccBtn:
				if (checktype != TutoTouchCheckType.StageMenu) return true;
				return (int)args[0] != 1;
		}
		return true;
	}
}
