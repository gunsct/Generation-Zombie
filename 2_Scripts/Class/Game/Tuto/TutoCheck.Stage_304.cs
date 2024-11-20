using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_304
{
	StageStart_Loading = 1,
	DL_139,
	Focus_Line_0,
	Delay_NextLine,
	Focus_Line_1_2,
	Select_Char_1013,
	Char_1013_SkillEnd,
	Delay_Merge,
	Focus_Merge_ShockBomb,
	ShockBoom_Action_Start,
	SelectShockBombTarget,
	ShockBomb_Action_End,
	DL_145,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_304()
	{
		TutoType_Stage_304 type = GetTutoState<TutoType_Stage_304>();
		//switch (type) {
		//	case TutoType_Stage_304.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_304(int no, object[] args)
	{
		TutoType_Stage_304 type = (TutoType_Stage_304)no;
		TutoUI ui;
		RectTransform rtf;
		Vector3 pos, temp;
		Main_Stage stageui;
		Item_Stage_Char charcard;
		Item_Stage card;

		switch (type)
		{
			case TutoType_Stage_304.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.DL_139:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(139, () => { Next(); });
				break;
			case TutoType_Stage_304.Focus_Line_0:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_304.Delay_NextLine:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.Focus_Line_1_2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, false);
				ui.StartDlg(140, () => { Next(); }, 0f, 2);
				break;
			case TutoType_Stage_304.Select_Char_1013:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[1];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(0.5f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_304.Char_1013_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.Delay_Merge:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.Focus_Merge_ShockBomb:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetMakeCard(StageMaterialType.ShockBomb).transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				break;
			case TutoType_Stage_304.ShockBoom_Action_Start:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.SelectShockBombTarget:
				ui = POPUP.ShowTutoUI();
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), temp.x - pos.x, temp.y - pos.y, true);
				break;
			case TutoType_Stage_304.ShockBomb_Action_End:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_304.DL_145:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(145, () => { Next(); });
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				charcard = STAGE.m_Chars[1];
				charcard.SkillColoTimeInit();
				break;
			case TutoType_Stage_304.End:
				POPUP.RemoveTutoUI();
				SetTutoEnd();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_304(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_304 type = GetTutoState<TutoType_Stage_304>();
		Item_Stage_Char charcard;
		Item_Stage stagecard;
		switch (type)
		{
			case TutoType_Stage_304.StageStart_Loading:
			case TutoType_Stage_304.Delay_NextLine:
				return false;
			case TutoType_Stage_304.Focus_Line_0:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0 || stagecard.m_Pos != 1;
			case TutoType_Stage_304.Select_Char_1013:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1013;
			case TutoType_Stage_304.Char_1013_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				return false;
			case TutoType_Stage_304.Focus_Merge_ShockBomb:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.ShockBomb;
			case TutoType_Stage_304.SelectShockBombTarget:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 1 || stagecard.m_Pos != 2;
			case TutoType_Stage_304.ShockBomb_Action_End:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
		}
		return true;
	}
}
