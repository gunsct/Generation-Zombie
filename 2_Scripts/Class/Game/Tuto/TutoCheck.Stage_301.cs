using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_301
{
	StageStart_Loading = 1,
	DL_4301,
	Focus_Line2,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_301()
	{
		TutoType_Stage_301 type = GetTutoState<TutoType_Stage_301>();
		//switch (type) {
		//	case TutoType_Stage_301.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_301(int no, object[] args)
	{
		TutoType_Stage_301 type = (TutoType_Stage_301)no;
		TutoUI ui;
		Vector3 pos, temp;
		Item_Stage card;

		switch (type)
		{
			case TutoType_Stage_301.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_301.DL_4301:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(4301, () => { Next(); });
				break;
			case TutoType_Stage_301.Focus_Line2:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[1][2];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 5, temp.y - pos.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			//case TutoType_Stage_301.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_301.End:
				POPUP.RemoveTutoUI();
				SetTutoEnd();
				Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_301(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_301 type = GetTutoState<TutoType_Stage_301>();
		switch (type)
		{
			case TutoType_Stage_301.StageStart_Loading:
			//case TutoType_Stage_301.Delay_Clear:
				return false;
		}
		return true;
	}
}
