using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutoType_UserReview
{
	/// <summary> 유저 평가 팝업 </summary>
	MSG_POPUP = 1,
	End
}

public partial class TutoCheck : ClassMng
{
	public bool IsCameraMove_UserReview()
	{
		TutoType_UserReview type = GetTutoState<TutoType_UserReview>();
		return false;
	}
	void PlayTuto_UserReview(int no, object[] args)
	{
		TutoType_UserReview type = (TutoType_UserReview)no;
		switch (type)
		{
		case TutoType_UserReview.MSG_POPUP:
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.UserReview, (result, obj) => {
				Next();
			});
			break;
		case TutoType_UserReview.End:
			SetTutoEnd();
			break;
		}
	}

	public bool TouchCheckLock_UserReview(TutoTouchCheckType checktype, object[] args)
	{
		return false;
	}
}
