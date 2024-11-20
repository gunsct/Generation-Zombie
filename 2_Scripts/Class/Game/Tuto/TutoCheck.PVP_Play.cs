using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_PVP_Play
{
	DL_5309 = 1,
	DL_5310,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_PVP_Play()
	{
		TutoType_PVP_Play type = GetTutoState<TutoType_PVP_Play>();
		return false;
	}
	void PlayTuto_PVP_Play(int no, object[] args)
	{
		TutoType_PVP_Play type = (TutoType_PVP_Play)no;
		TutoUI ui;
		Main_PVP pvp = POPUP.GetMainUI().GetComponent<Main_PVP>();
		Vector3 pos, temp;
		GameObject obj;
		RectTransform rtf;

		switch (type)
		{
		case TutoType_PVP_Play.DL_5309:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			obj = pvp.GetTutoObj(0);
			rtf = (RectTransform)obj.transform;
			ui.SetFocus(2, rtf, Vector3.zero, rtf.sizeDelta.x, rtf.sizeDelta.y * 0.5f, false);
			ui.StartDlg(5309, () => { Next(); }, 0f, 1);
			break;
		case TutoType_PVP_Play.DL_5310:
			ui = POPUP.ShowTutoUI();
			ui.RemoveClone();
			ui.StartDlg(5310, () => { Next(); }, 0f, 1);
			obj = PVP.GetStatObj(0, StatType.Hyg);
			pos = Utile_Class.GetCanvasPosition(obj.transform.position - obj.transform.lossyScale * 0.5f) / Canvas_Controller.SCALE;
			temp = Utile_Class.GetCanvasPosition(obj.transform.position + obj.transform.lossyScale * 0.5f) / Canvas_Controller.SCALE;
			ui.SetFocus(2, Utile_Class.GetCanvasPosition(obj.transform.position), (temp.x - pos.x) * 4f, (temp.y - pos.y) * 0.45f, false);

			POPUP.StartTutoTimer(() => {
				obj = PVP.GetStatObj(1, StatType.Hyg);
				pos = Utile_Class.GetCanvasPosition(obj.transform.position - obj.transform.lossyScale * 0.5f) / Canvas_Controller.SCALE;
				temp = Utile_Class.GetCanvasPosition(obj.transform.position + obj.transform.lossyScale * 0.5f) / Canvas_Controller.SCALE;
				ui.SetFocus(2, Utile_Class.GetCanvasPosition(obj.transform.position), (temp.x - pos.x) * 4f, (temp.y - pos.y) * 0.55f, false);
			}, 2f);
			break;
		case TutoType_PVP_Play.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_PVP_Play(TutoTouchCheckType checktype, object[] args)
	{
		TutoType_PVP_Play type = (TutoType_PVP_Play)m_TutoState[m_NowTuto];
		//switch (type)
		//{
		//}
		return true;
	}
}
