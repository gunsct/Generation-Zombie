using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_DeckSynergy
{
	Decksetting_Delay = 1,
	DL_2001, 
	Focus_CharListOnBtn,
	Delay_ListOn,
	Select_FirstChar1013,
	Focus_CharListOffBtn,
	DL_2004,
	Focus_DeckSynergy,
	Delay1,
	DL_2006,
	Focus_SynergyList,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_DeckSynergy()
	{
		TutoType_DeckSynergy type = GetTutoState<TutoType_DeckSynergy>();
		//switch (type) {
		//	case TutoType_Stage_405.FREE_Play:
		//		return true;
		//}
		return false;
	}
	void PlayTuto_DeckSynergy(int no, object[] args)
	{
		TutoType_DeckSynergy type = (TutoType_DeckSynergy)no;
		TutoUI ui;
		RectTransform rtf;
		DeckSetting decksetting;
		SynergyDeck synergy;

		switch (type)
		{
			case TutoType_DeckSynergy.Decksetting_Delay:
				POPUP.RemoveTutoUI();
				break;
			case TutoType_DeckSynergy.DL_2001:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2001, () => { Next(); });
				break;
			case TutoType_DeckSynergy.Focus_CharListOnBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetCharListOnBtn.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_DeckSynergy.Delay_ListOn:
				POPUP.RemoveTutoUI();
				//ui = POPUP.ShowTutoUI();
				//ui.RemoveClone();
				//ui.SetFocus(0, Vector3.zero, 0, 0);
				break;
			case TutoType_DeckSynergy.Select_FirstChar1013:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetChar1013.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_DeckSynergy.Focus_CharListOffBtn:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetCharListOffBtn.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_DeckSynergy.DL_2004:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2004, () => { Next(); });
				break;
			case TutoType_DeckSynergy.Focus_DeckSynergy:
				ui = POPUP.ShowTutoUI();
				ui.StartDlg(-1);
				decksetting = POPUP.GetPopup().GetComponent<DeckSetting>();
				rtf = (RectTransform)decksetting.GetSynergy.transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, true);
				break;
			case TutoType_DeckSynergy.Delay1:
				POPUP.RemoveTutoUI();
				//ui = POPUP.ShowTutoUI();
				//ui.RemoveClone();
				//ui.SetFocus(0, Vector3.zero, 0, 0);
				POPUP.StartTutoTimer(() => { Next(); }, 3f);
				break;
			case TutoType_DeckSynergy.DL_2006:
				ui = POPUP.ShowTutoUI();
				ui.RemoveClone();
				ui.SetFocus(0, Vector3.zero, 0, 0);
				ui.StartDlg(2006, () => { Next(); }, 0, 1);
				break;
			case TutoType_DeckSynergy.Focus_SynergyList:
				ui = POPUP.ShowTutoUI();
				synergy = POPUP.GetPopup().GetComponent<SynergyDeck>();
				rtf = (RectTransform)synergy.GetAllSynergy().transform;
				ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y, false);
				POPUP.StartTutoTimer(() => { Next(); }, 2f);
				break;
			case TutoType_DeckSynergy.End:
				SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_DeckSynergy(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_DeckSynergy type = GetTutoState<TutoType_DeckSynergy>();
		Item_CharManageCard card;

		switch (type) {
			case TutoType_DeckSynergy.Decksetting_Delay:
				return false;
			case TutoType_DeckSynergy.Focus_CharListOnBtn:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				return (int)args[0] != 0;
			case TutoType_DeckSynergy.Select_FirstChar1013:
				if (checktype != TutoTouchCheckType.Item_CharManageCard_Select) return true;
				if ((Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Click
					&& (Item_CharManageCard.State)args[0] != Item_CharManageCard.State.Hold) return true;
				card = (Item_CharManageCard)args[1];
				return card?.m_Info?.m_Idx != 1013;
			case TutoType_DeckSynergy.Focus_CharListOffBtn:
				if (checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				return (int)args[0] != 1;
			case TutoType_DeckSynergy.Focus_DeckSynergy://레이캐스트 코드로 강제 버튼 활성화 시키는 코드 있어서 이렇게 처리함
				if (checktype != TutoTouchCheckType.DeckSetting && checktype != TutoTouchCheckType.DeckSetting_ListPage) return true;
				if (checktype == TutoTouchCheckType.DeckSetting )return(int)args[0] != 0;
				if (checktype == TutoTouchCheckType.DeckSetting_ListPage) return (int)args[0] != 2;
				return true;
		}
		return true;
	}
}
