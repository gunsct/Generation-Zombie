using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_Stage_501
{
	StageStart_Loading = 1,
	DL_2311,
	Focus_Line1,
	DL_2314,
	//Delay_Clear,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_Stage_501()
	{
		TutoType_Stage_501 type = GetTutoState<TutoType_Stage_501>();
		//switch (type) {
		//	case TutoType_Stage_501.Delay_Clear:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_Stage_501(int no, object[] args)
	{
		TutoType_Stage_501 type = (TutoType_Stage_501)no;
		TutoUI ui;
		Vector3 pos, temp;
		Item_Stage card;

		switch (type)
		{
			case TutoType_Stage_501.StageStart_Loading:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_Stage_501.DL_2311:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2311, () => { Next(); });
				break;
			case TutoType_Stage_501.Focus_Line1:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				card = STAGE.m_ViewCard[0][1];
				pos = Utile_Class.GetCanvasPosition(card.transform.position - card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(card.transform.position + card.ImgSize * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(card.transform.position), (temp.x - pos.x) * 3, temp.y - pos.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_Stage_501.DL_2314:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2314, () => { Next(); });
				break;
			//case TutoType_Stage_501.Delay_Clear:
			//	ui = POPUP.ShowTutoUI();
			//	POPUP.RemoveTutoUI();
			//	break;
			case TutoType_Stage_501.End:
				POPUP.RemoveTutoUI();
				SetTutoEnd();
				Main_Stage stageui = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stageui.AccToggleCheck();
				break;
		}
	}

	public bool TouchCheckLock_Stage_501(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_Stage_501 type = GetTutoState<TutoType_Stage_501>();
		switch (type)
		{
			case TutoType_Stage_501.StageStart_Loading:
			//case TutoType_Stage_501.Delay_Clear:
				return false;
		}
		return true;
	}
}
