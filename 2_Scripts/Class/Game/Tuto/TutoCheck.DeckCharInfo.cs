using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_DeckCharInfo
{
	Decksetting_Delay = 1,
	DL_5314, 
	Focus_FirstChar,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_DeckCharInfo()
	{
		TutoType_DeckCharInfo type = GetTutoState<TutoType_DeckCharInfo>();
		//switch (type) {
		//	case TutoType_Stage_405.FREE_Play:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_DeckCharInfo(int no, object[] args)
	{
		TutoType_DeckCharInfo type = (TutoType_DeckCharInfo)no;
		TutoUI ui;
		RectTransform rtf;
		DeckSetting decksetting;

		switch (type)
		{
			case TutoType_DeckCharInfo.Decksetting_Delay:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_DeckCharInfo.DL_5314:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(5314, () => { Next(); });
				break;
			case TutoType_DeckCharInfo.Focus_FirstChar:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetSlot(0).transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true, TutoUI.FocusAnim.Press);
				POPUP.StartTutoTimer(() => { Next(); }, 5.6f);
				break;
			case TutoType_DeckCharInfo.End:
				SetTutoEnd();
				break;
		}
	}

	public bool TouchCheckLock_DeckCharInfo(TutoTouchCheckType checktype, object[] args)
	{
		//TutoType_DeckCharInfo type = GetTutoState<TutoType_DeckCharInfo>();
		//Item_CharManageCard card;

		return true;
	}
}
