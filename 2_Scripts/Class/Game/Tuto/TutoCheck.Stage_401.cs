using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_401
{
	StageStart_Loading = 1,
	DL_1171,
	Focus_SurvIcon_1,
	DL_1181,
	Focus_Line_0,
	DL_147,
	Select_Char_1002,
	Char_1002_SkillEnd,
	Delay_Merge,
	Focus_Merge_Bread,
	Delay_UseItem,
	Focus_SurvIcon_2,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_401()
	{
		TutoType_Stage_401 type = GetTutoState<TutoType_Stage_401>();
		//switch (type) {
		//	case TutoType_Stage_401.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_401(int no, object[] args)
	{
		TutoType_Stage_401 type = (TutoType_Stage_401)no;
		TutoUI ui;
		RectTransform rtf;
		Vector3 pos, temp;
		Main_Stage stageui;
		Item_Stage_Char charcard;
		Item_Stage card;

		switch (type)
		{
			case TutoType_Stage_401.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_401.DL_1171:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(1171, () => { Next(); });
				break;
			case TutoType_Stage_401.Focus_SurvIcon_1:
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				rtf = (RectTransform)stageui.GetStatObj(StatType.Sat).transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				stageui.TutoSrvStatOn(stageui.GetStatObj(StatType.Sat), () => { Next(); }, 2f);
				break;
			case TutoType_Stage_401.DL_1181:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.StartDlg(1181, () => { Next(); });
				break;
			case TutoType_Stage_401.Focus_Line_0:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, true);
				break;
			case TutoType_Stage_401.DL_147:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(147, () => { Next(); });
				break;
			case TutoType_Stage_401.Select_Char_1002:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				charcard = STAGE.m_Chars[0];
				pos = Utile_Class.GetCanvasPosition(charcard.transform.position - charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(charcard.transform.position + charcard.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(charcard.transform.position), (temp.x - pos.x) * 0.75f, temp.y - pos.y, true);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.GetAPGauge.TuToApGaugeFullAction(0.5f);
				Start_ClickDelay();
				break;
			case TutoType_Stage_401.Char_1002_SkillEnd:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_401.Delay_Merge:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_401.Focus_Merge_Bread:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetMakeCard(StageMaterialType.Bread).transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				break;
			case TutoType_Stage_401.Delay_UseItem:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_401.Focus_SurvIcon_2:
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				rtf = (RectTransform)stageui.GetStatObj(StatType.Sat).transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				stageui.GetAPGauge.TuToApGaugeFullAction(1f);
				break;
			case TutoType_Stage_401.End:
				POPUP.RemoveTutoUI();
				SetTutoEnd();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_401(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_401 type = GetTutoState<TutoType_Stage_401>();
		Item_Stage_Char charcard;
		Item_Stage stagecard;
		switch (type)
		{
			case TutoType_Stage_401.StageStart_Loading:
			//case TutoType_Stage_401.Delay_Clear:
				return false;
			case TutoType_Stage_401.Focus_Line_0:
				if (checktype != TutoTouchCheckType.StageCard) return true;
				stagecard = (Item_Stage)args[0];
				return stagecard.m_Line != 0;
			case TutoType_Stage_401.Select_Char_1002:
				if (checktype != TutoTouchCheckType.StageCard_Char) return true;
				charcard = (Item_Stage_Char)args[0];
				return charcard.m_Info != null && charcard.m_Info.m_Idx != 1002;
			case TutoType_Stage_401.Char_1002_SkillEnd:
				if (checktype != TutoTouchCheckType.StageCard && checktype != TutoTouchCheckType.PopupOnClose) return true;
				if (checktype == TutoTouchCheckType.StageCard) return ((Item_Stage)args[0]).m_Info.m_TEnemyData == null;
				return false;
			case TutoType_Stage_401.Focus_Merge_Bread:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.Bread;
			case TutoType_Stage_401.Delay_UseItem:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
			case TutoType_Stage_401.Focus_SurvIcon_2:
				if (checktype != TutoTouchCheckType.PopupOnClose) return true;
				return false;
		}
		return true;
	}
}
