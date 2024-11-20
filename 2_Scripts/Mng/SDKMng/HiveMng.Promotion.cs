using hive;
using System;
using System.Collections.Generic;
using UnityEngine;
using static hive.AuthV4;

public partial class HiveMng : ClassMng
{
	public void ReLoadPromotion()
	{
		Promotion.updatePromotionData();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="IsFocus"> true라면 ‘오늘 하루 다시보지 않기’ 버튼을 표시하지 않음. 혹 오늘 이미 유저가 다시 보지 않도록 설정했더라도 무시하고 새소식 페이지를 띄움</param>
	/// <param name="CB"></param>
	public void ShowPromotion(PromotionType type = PromotionType.NEWS, bool IsFocus = true, Action<ResultAPI, PromotionEventType> CB = null)
	{
#if !UNITY_EDITOR
		POPUP.LockConnecting(true);
		hive.Promotion.showPromotion(type, IsFocus, (ResultAPI result, PromotionEventType promotionEventType) => {
			Utile_Class.DebugLog($"Promotion.showPromotion !! result : {result.toString()}\npromotionEventType : {promotionEventType}");
			POPUP.LockConnecting(false);
			CB?.Invoke(result, promotionEventType);

			//if (result.isSuccess())
			//{
			//	// API 호출 성공
				
			//}
		});
#endif
	}

	public void Review()
	{
		hive.Promotion.showNativeReview();
	}
}

