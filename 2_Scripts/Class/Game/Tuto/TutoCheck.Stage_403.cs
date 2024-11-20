using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_403
{
	StageStart_Loading = 1, 
	Focus_MissionGuide,
	Focus_Line_1,
	FREE_Play,
	Focus_SurvIcon,
	Focus_Merge_Bread,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_403()
	{
		TutoType_Stage_403 type = GetTutoState<TutoType_Stage_403>();
		//switch (type) {
		//	case TutoType_Stage_403.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_403(int no, object[] args)
	{
		TutoType_Stage_403 type = (TutoType_Stage_403)no;
		TutoUI ui;
		RectTransform rtf;
		Vector3 pos, temp;
		Main_Stage stageui;
		Item_Stage card;

		switch (type)
		{
			case TutoType_Stage_403.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_403.Focus_MissionGuide:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMissionGuideObj.transform;
				ui.SetFocus(2, rtf.position + new Vector3(-rtf.rect.width * 0.5f * rtf.lossyScale.x, -rtf.rect.height * 0.5f * rtf.lossyScale.y, 0f), rtf.rect.width, rtf.rect.height, false);
				ui.StartDlg(149, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_403.Focus_Line_1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 5, temp.y - pos.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_403.FREE_Play:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_403.Focus_SurvIcon:
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				ui = POPUP.ShowTutoUI();
				rtf = (RectTransform)stageui.GetStatObj(StatType.Sat).transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				ui.StartDlg(152, () => { Next(); }, 0f, 1);
				break;
			case TutoType_Stage_403.Focus_Merge_Bread:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				rtf = (RectTransform)stageui.GetMaking.GetMakeCard(StageMaterialType.Bread).transform;
				ui.SetFocus(2, rtf.position, rtf.rect.width, rtf.rect.height, true);
				break;
			case TutoType_Stage_403.End:
				POPUP.RemoveTutoUI();
				SetTutoEnd();
				stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_403(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_403 type = GetTutoState<TutoType_Stage_403>();
		switch (type)
		{
			case TutoType_Stage_403.StageStart_Loading:
			case TutoType_Stage_403.FREE_Play:
				return false;
			case TutoType_Stage_403.Focus_Merge_Bread:
				if (checktype != TutoTouchCheckType.StageMaking) return true;
				return ((Item_Stage_MakeCard)args[0]).m_MatType != StageMaterialType.Bread;
		}
		return true;
	}
}
